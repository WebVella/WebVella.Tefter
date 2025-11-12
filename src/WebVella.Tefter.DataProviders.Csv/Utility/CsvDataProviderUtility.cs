using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using WebVella.Tefter.Utility;

namespace WebVella.Tefter.DataProviders.Csv;

public static class CsvDataProviderUtility
{
    public static TfDataProviderSourceSchemaInfo GetSchemaInfo(this MemoryStream stream,
        CsvDataProvider provider,
        CultureInfo? culture = null, string? delimiter = null,
        Dictionary<string, string>? columnImportParseFormat = null)
    {
        stream.Position = 0;
        int maxSampleSize = 200;
        var result = new TfDataProviderSourceSchemaInfo();
        Dictionary<string, List<TfDatabaseColumnType>> suggestedColumnTypes = new();
        columnImportParseFormat ??= new Dictionary<string, string>();

        if (culture is null)
        {
            culture = Thread.CurrentThread.CurrentCulture;
        }

        //Check if it has header
        var headerColumns = stream.GetHeaderAsync(culture);
        if (headerColumns.Count == 0)
            throw new Exception("A header row is required for the file");

        var config = new CsvConfiguration(culture)
        {
            PrepareHeaderForMatch = args => args.Header.ToLower(),
            Encoding = Encoding.UTF8,
            IgnoreBlankLines = true,
            BadDataFound = null,
            TrimOptions = TrimOptions.Trim,
            MissingFieldFound = null,
            HasHeaderRecord = true
        };
        if (!String.IsNullOrWhiteSpace(delimiter))
            config.Delimiter = delimiter;


        #region << Process >>

        {
            HashSet<int> rowIndexToReadHs = stream.GenerateSampleRowIndexHashset(maxSampleSize, skipCount: 1);
            if (rowIndexToReadHs.Count <= 0) return result;
            stream.Position = 0;
            using var reader = new StreamReader(stream, leaveOpen: true);
            using var csvReader = new CsvReader(reader, config);
            csvReader.Read();
            csvReader.ReadHeader();
            if (csvReader.HeaderRecord is not null)
            {
                foreach (var item in csvReader.HeaderRecord)
                {
                    var columnName = item.ToSourceColumnName();
                    if (result.SourceColumnDefaultDbType.ContainsKey(columnName))
                        throw new Exception(
                            $"Column with the name '{columnName}' is found multiple times in the source");
                    result.SourceColumnDefaultDbType[columnName] = TfDatabaseColumnType.Text;
                }
            }
            else
            {
                throw new Exception(
                    $"Header row not found");
            }

            var counter = 0;
            while (csvReader.Read())
            {
                if (!rowIndexToReadHs.Contains(counter))
                {
                    counter++;
                    continue;
                }

                foreach (var columnNameUnprocessed in csvReader.HeaderRecord)
                {
                    var fieldValue = csvReader.GetField(columnNameUnprocessed);
                    var columnName = columnNameUnprocessed.ToSourceColumnName();
                    if (!result.SourceColumnDefaultValue.ContainsKey(columnName)
                        && !String.IsNullOrWhiteSpace(fieldValue))
                        result.SourceColumnDefaultValue[columnName] = fieldValue;

                    string? importFormat = null;
                    if (columnImportParseFormat.ContainsKey(columnName))
                    {
                        importFormat = columnImportParseFormat[columnName];
                    }

                    TfDatabaseColumnType type = TfDatabaseColumnType.Text;
                    if (fieldValue is not null)
                        type = SourceToColumnTypeConverter.GetDataTypeFromString(fieldValue, culture,
                            importFormat);

                    if (!suggestedColumnTypes.ContainsKey(columnName))
                        suggestedColumnTypes[columnName] = new();
                    suggestedColumnTypes[columnName].Add(type);
                }

                counter++;
            }

            foreach (var key in result.SourceColumnDefaultDbType.Keys)
            {
                var columnType = TfDatabaseColumnType.Text;
                if (suggestedColumnTypes.ContainsKey(key))
                    columnType = suggestedColumnTypes[key].GetTypeFromOptions();

                result.SourceColumnDefaultDbType[key] = columnType;
            }

            var preferredSourceTypeForDbType = new Dictionary<TfDatabaseColumnType, string>();
            foreach (var providerDataType in provider.GetSupportedSourceDataTypes())
            {
                var supportedDbList = provider.GetDatabaseColumnTypesForSourceDataType(providerDataType);
                //var supportedDbType = supportedDBList.Count > 0 ? supportedDBList.First() : TfDatabaseColumnType.Text;
                result.SourceTypeSupportedDbTypes[providerDataType] = supportedDbList.ToList();
                if (supportedDbList.Count > 0)
                {
                    preferredSourceTypeForDbType[supportedDbList.First()] = providerDataType;
                }
            }

            foreach (var columnName in result.SourceColumnDefaultDbType.Keys)
            {
                var dbType = result.SourceColumnDefaultDbType[columnName];
                if (preferredSourceTypeForDbType.ContainsKey(dbType))
                    result.SourceColumnDefaultSourceType[columnName] = preferredSourceTypeForDbType[dbType];
                else
                    result.SourceColumnDefaultSourceType[columnName] = provider.GetSupportedSourceDataTypes().First();
            }
        }

        #endregion

        stream.Position = 0;
        return result;
    }

    public static HashSet<int> GenerateSampleRowIndexHashset(this MemoryStream stream,
        int maxSampleSize = 200, int skipCount = 0)
    {
        stream.Position = 0;
        using var reader = new StreamReader(stream, leaveOpen: true);
        //for dealing with auto=generation indexes
        //we need to split the sample between the first and the last records
        int totalRecords = 0;
        while (reader.ReadLine() != null)
        {
            ++totalRecords;
        }

        return totalRecords.GenerateSampleIndexesForList(maxSampleSize, skipCount: skipCount);
    }

    public static async Task<(bool, string, TfDataProviderSourceSchemaInfo?)> CheckCsvFile(this MemoryStream? stream,
        CsvDataProvider provider,
        string filepath,
        CultureInfo? culture = null)
    {
        TfDataProviderSourceSchemaInfo? schema = null;
        if (stream is null)
            return (false, "File content is empty", schema);
        if (stream.Length == 0)
            return (false, "File content is empty", schema);

        if (Path.GetExtension(filepath).ToLower() != ".csv")
            return (false, "Can process only files with '.csv' extension", schema);

        culture ??= Thread.CurrentThread.CurrentCulture;

        #region << Check file contents for non printable Chars >>

        {
            stream.Position = 0;

            byte[] buffer = new byte[1024]; // 1KB buffer
            int bytesRead;
            using var reader = new StreamReader(stream, leaveOpen: true);
            while ((bytesRead = await reader.BaseStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                try
                {
                    string chunk = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    if (!chunk.ContainsOnlyPrintableAndValidChars())
                        return (false, "File contents contain non printable or invalid characters", schema);
                }
                catch (Exception)
                {
                    return (false, "File contents are not text", schema);
                }
            }
            stream.Position = 0;
        }

        #endregion

        #region << Check for Schema >>

        try
        {
            schema = stream.GetSchemaInfo(
                provider: provider,
                culture: culture,
                delimiter: null,
                columnImportParseFormat: null);
            stream.Position = 0;
        }
        catch (Exception ex)
        {
            return (false, $"Cannot evaluate the file schema. Error: {ex.Message}", schema);
        }

        #endregion

        #region << Check CSV Content >>

        {
            stream.Position = 0;
            var config = new CsvConfiguration(culture)
            {
                PrepareHeaderForMatch = args => args.Header.ToLower(),
                Encoding = Encoding.UTF8,
                IgnoreBlankLines = true,
                BadDataFound = null,
                TrimOptions = TrimOptions.Trim,
                MissingFieldFound = null,
                DetectDelimiter = true
            };
            using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
            using var csvReader = new CsvReader(reader, config);

            try
            {
                var delimiter = csvReader.Context.Configuration.Delimiter;
                if (String.IsNullOrWhiteSpace(delimiter))
                    return (false, "CSV delimiter cannot be determined", schema);

                try
                {
                    await csvReader.ReadAsync();
                }
                catch (Exception)
                {
                    return (false, "CSV file cannot be red", schema);
                }
            }
            catch (Exception ex)
            {
                return (false, $"CSV file parsing ended with error: {ex.Message}", schema);
            }
            stream.Position = 0;
        }

        #endregion

        return (true, "File check successful", schema);
    }

    public static async Task CheckCsvFile(this TfImportFileToPageContextItem item)
    {
        if (item.FileContent is null)
        {
            item.IsSuccess = false;
            item.Message = "CSVDataProvider cannot process this file: No file content";
            return;
        }

        item.FileContent.Position = 0;
        using var memoryStream = new MemoryStream();
        await item.FileContent.CopyToAsync(memoryStream);
        memoryStream.Position = 0;
        
        
        var (isSuccess, message, schema) =
            await memoryStream.CheckCsvFile((CsvDataProvider)item.DataProvider!, item.FileName);
        item.SchemaInfo = schema;
        item.ProcessStream.ReportProgress(new TfProgressStreamItem()
        {
            Type = TfProgressStreamItemType.Debug,
            Message = message
        });
        item.IsSuccess = isSuccess;
        item.Message = isSuccess ? "CSVDataProvider can process this file" : "CSVDataProvider cannot process this file";
    }

    public static async Task CreateFromCsvFile(this TfImportFileToPageContextItem item)
    {
        try
        {
            item.ProcessStream.ReportProgress(new TfProgressStreamItem()
            {
                Message = "CsvDataProvider provider creation started...",
                Type = TfProgressStreamItemType.Normal,
            });
            item.ProcessStream.ReportProgress(new TfProgressStreamItem()
            {
                Message = "CsvDataProvider is validating file and discovering its data schema",
                Type = TfProgressStreamItemType.Normal,
            });   
            #region << Validate >>

            if (item.FileContent is null)
            {
                item.ProcessStream.ReportProgress(new TfProgressStreamItem()
                {
                    Message = "CsvDataProvider cannot process this file as it is empty",
                    Type = TfProgressStreamItemType.Error,
                });
                item.IsSuccess = false;
                item.Message = "File parse error";
                return;
            }

            await item.CheckCsvFile();
            if (!item.IsSuccess)
            {
                item.ProcessStream.ReportProgress(new TfProgressStreamItem()
                {
                    Message = "CsvDataProvider cannot process this file, due to parse error",
                    Type = TfProgressStreamItemType.Error,
                });
                item.IsSuccess = false;
                item.Message = "File parse error";
                return;
            }

            if (item.SchemaInfo is null)
            {
                item.ProcessStream.ReportProgress(new TfProgressStreamItem()
                {
                    Message = "CsvDataProvider cannot process this file, schema cannot be evaluated",
                    Type = TfProgressStreamItemType.Error,
                });
                item.IsSuccess = false;
                item.Message = "File parse error";
                return;
            }

            #endregion
            item.ProcessStream.ReportProgress(new TfProgressStreamItem()
            {
                Message = "CsvDataProvider successfully discovered the files data schema",
                Type = TfProgressStreamItemType.Normal,
            });            
            

            item.ProcessStream.ReportProgress(new TfProgressStreamItem()
            {
                Message = "CsvDataProvider created!",
                Type = TfProgressStreamItemType.Normal,
            });
        }
        catch (Exception ex)
        {
            item.ProcessStream.ReportProgress(new TfProgressStreamItem()
            {
                Message = $"CsvDataProvider creation failed! Message: {ex.Message}",
                Type = TfProgressStreamItemType.Error,
            });
        }
    }

    public static List<string> GetHeaderAsync(this MemoryStream stream,
        CultureInfo? culture = null)
    {
        stream.Position = 0;
        culture ??= Thread.CurrentThread.CurrentCulture;
        var config = new CsvConfiguration(culture)
        {
            PrepareHeaderForMatch = args => args.Header.ToLower(),
            Encoding = Encoding.UTF8,
            IgnoreBlankLines = true,
            BadDataFound = null,
            TrimOptions = TrimOptions.Trim,
            MissingFieldFound = null,
            DetectDelimiter = true,
            HasHeaderRecord = false
        };
        using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
        using var csvReader = new CsvReader(reader, config);
        csvReader.Read();
        var headerColumns = new List<string>();
        for (int i = 0; i < csvReader.ColumnCount; i++)
        {
            headerColumns.Add(csvReader.GetField(i)?.ToString() ?? String.Empty);
        }

        if (headerColumns.Count == 0)
            return new List<string>();

        if (!headerColumns.IsLikelyHeader())
            return new List<string>();

        return headerColumns;
    }

    private static bool IsLikelyHeader(this List<string> fields)
    {
        if (fields.Count == 0) return false;
        // Count numeric and date fields
        int numericCount = 0;
        int dateCount = 0;
        bool hasDuplicated = false;
        HashSet<string> foundColumnsHs = new();

        foreach (var field in fields)
        {
            var fieldParsed = field?.Trim().ToLowerInvariant() ?? string.Empty;
            if (foundColumnsHs.Contains(fieldParsed))
                hasDuplicated = true;

            foundColumnsHs.Add((fieldParsed));
            if (TryParseNumber(fieldParsed, out _)) numericCount++;
            if (TryParseDate(fieldParsed, out _)) dateCount++;
        }

        // If < 50% of fields are numeric/date → likely a header
        return !hasDuplicated && (numericCount + dateCount) == 0;
    }

    private static bool TryParseNumber(string s, out double result)
        => double.TryParse(s, out result);

    private static bool TryParseDate(string s, out DateTime result)
        => DateTime.TryParse(s, out result);
}