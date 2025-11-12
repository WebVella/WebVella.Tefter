using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using WebVella.Tefter.Utility;

namespace WebVella.Tefter.DataProviders.Csv;

public class CsvDataProviderUtility
{
    public HashSet<int> GenerateSampleRowIndexHashset(MemoryStream stream,
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

    public (bool, string, TfDataProviderSourceSchemaInfo?, string?) CheckCsvFile(
        MemoryStream? stream,
        CsvDataProvider provider,
        string filepath,
        string? delimiter = null,
        CultureInfo? culture = null)
    {
        if (stream is null)
            return (false, "File content is empty", null, null);
        if (stream.Length == 0)
            return (false, "File content is empty", null, null);

        if (Path.GetExtension(filepath).ToLower() != ".csv")
            return (false, "Can process only files with '.csv' extension", null, null);

        culture ??= Thread.CurrentThread.CurrentCulture;

        #region << Check file contents for non printable Chars >>

        {
            stream.Position = 0;

            byte[] buffer = new byte[1024]; // 1KB buffer
            int bytesRead;
            using var reader = new StreamReader(stream, leaveOpen: true);
            while ((bytesRead = reader.BaseStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                try
                {
                    string chunk = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    if (!chunk.ContainsOnlyPrintableAndValidChars())
                        return (false, "File contents contain non printable or invalid characters", null,null);
                }
                catch (Exception)
                {
                    return (false, "File contents are not text", null,null);
                }
            }

            stream.Position = 0;
        }

        #endregion

        TfDataProviderSourceSchemaInfo? schema = null;
        string? delimiterFound = null;
        #region << Check for Schema >>

        try
        {
            schema = GetSchemaInfo(
                stream: stream,
                provider: provider,
                culture: culture,
                delimiter: delimiter,
                columnImportParseFormat: null);
            stream.Position = 0;
        }
        catch (Exception ex)
        {
            return (false, $"Cannot evaluate the file schema. Error: {ex.Message}", null,null);
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
                delimiterFound = csvReader.Configuration.Delimiter;
                
                if (String.IsNullOrWhiteSpace(delimiterFound))
                    return (false, "CSV delimiter cannot be determined", schema, null);

                try
                {
                    csvReader.Read();
                }
                catch (Exception)
                {
                    return (false, "CSV file cannot be red", schema, delimiterFound);
                }
            }
            catch (Exception ex)
            {
                return (false, $"CSV file parsing ended with error: {ex.Message}", schema, delimiterFound);
            }

            stream.Position = 0;
        }

        #endregion

        return (true, "File check successful", schema, delimiterFound);
    }

    public List<string> CsvDataProviderGetHeaderAsync(MemoryStream stream,
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
            headerColumns.Add(csvReader.GetField(i) ?? String.Empty);
        }

        if (headerColumns.Count == 0)
            return new List<string>();

        if (!IsLikelyHeader(headerColumns))
            return new List<string>();

        return headerColumns;
    }

    public (bool,TfDataProviderSourceSchemaInfo?,string?) CheckCsvFile(TfSpacePageCreateFromFileContextItem item)
    {
        if (item.FileContent is null)
        {
            item.ProcessStream.ReportProgress(new TfProgressStreamItem
            {
                Message = "CSVDataProvider cannot process this file: No file content",
                Type = TfProgressStreamItemType.Debug
            });
            return (false,null,null);
        }

        item.FileContent.Position = 0;
        using var memoryStream = new MemoryStream();
        item.FileContent.CopyTo(memoryStream);
        memoryStream.Position = 0;

        var (isSuccess, message, schema, delimiterFound) =
            CheckCsvFile(memoryStream, (CsvDataProvider)item.ProcessContext.UsedDataProviderAddon!, item.FileName);
        if (isSuccess)
            item.ProcessContext.DataSchemaInfo = schema;

        item.ProcessStream.ReportProgress(new TfProgressStreamItem()
        {
            Type = TfProgressStreamItemType.Debug,
            Message = message
        });

        return (isSuccess,schema,delimiterFound);
    }

    private TfDataProviderSourceSchemaInfo GetSchemaInfo(MemoryStream stream,
        CsvDataProvider provider,
        CultureInfo? culture = null,
        string? delimiter = null,
        Dictionary<string, string>? columnImportParseFormat = null)
    {
        stream.Position = 0;
        int maxSampleSize = 200;
        var result = new TfDataProviderSourceSchemaInfo();
        Dictionary<string, List<TfDatabaseColumnType>> suggestedColumnTypes = new();
        columnImportParseFormat ??= new Dictionary<string, string>();

        culture ??= Thread.CurrentThread.CurrentCulture;

        //Check if it has header
        var headerColumns = CsvDataProviderGetHeaderAsync(stream, culture);
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
            HashSet<int> rowIndexToReadHs = GenerateSampleRowIndexHashset(stream, maxSampleSize, skipCount: 1);
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
                    if (!result.SourceColumnDefaultDbType.TryAdd(columnName, TfDatabaseColumnType.Text))
                        throw new Exception(
                            $"Column with the name '{columnName}' is found multiple times in the source");
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

    private bool IsLikelyHeader(List<string> fields)
    {
        if (fields.Count == 0) return false;
        // Count numeric and date fields
        int numericCount = 0;
        int dateCount = 0;
        bool hasDuplicated = false;
        HashSet<string> foundColumnsHs = new();

        foreach (var field in fields)
        {
            var fieldParsed = String.Empty;
            if (!String.IsNullOrWhiteSpace(field))
                fieldParsed = field.Trim().ToLowerInvariant();
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