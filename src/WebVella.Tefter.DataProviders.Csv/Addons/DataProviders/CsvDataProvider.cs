using CsvHelper;
using CsvHelper.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using WebVella.Tefter.Exceptions;
using WebVella.Tefter.Models;
using WebVella.Tefter.Services;
using WebVella.Tefter.Utility;

namespace WebVella.Tefter.DataProviders.Csv.Addons;

public class CsvDataProvider : ITfDataProviderAddon
{

    public const string ID = "82883b60-197f-4f5a-8c6a-2bec16508816";
    public const string NAME = "Csv Data Provider";
    public const string DESCRIPTION = "Provide data from CSV formated file";
    public const string FLUENT_ICON_NAME = "DocumentTable";

    public Guid AddonId { get; init; } = new Guid(ID);
    public string AddonName { get; init; } = NAME;
    public string AddonDescription { get; init; } = DESCRIPTION;
    public string AddonFluentIconName { get; init; } = FLUENT_ICON_NAME;

    /// <summary>
    /// Return what types of data types it can process from the data source
    /// </summary>
    public ReadOnlyCollection<string> GetSupportedSourceDataTypes()
    {
        //sample only
        return new List<string> {
            "TEXT", //Keep text on first place as the first place is used as default type
			"SHORT_TEXT",
            "BOOLEAN",
            "DATE",
            "DATETIME",
            "NUMBER",
            "SHORT_INTEGER",
            "INTEGER",
            "LONG_INTEGER",
            "GUID"
        }.AsReadOnly();
    }
    /// <summary>
    /// Returns mapping between source data types and Tefter data types
    /// </summary>
    public ReadOnlyCollection<TfDatabaseColumnType> GetDatabaseColumnTypesForSourceDataType(
        string dataType)
    {
        switch (dataType)
        {
            case "TEXT":
                return new List<TfDatabaseColumnType> { TfDatabaseColumnType.Text }.AsReadOnly();
            case "SHORT_TEXT":
                return new List<TfDatabaseColumnType> { TfDatabaseColumnType.ShortText }.AsReadOnly();
            case "BOOLEAN":
                return new List<TfDatabaseColumnType> { TfDatabaseColumnType.Boolean }.AsReadOnly();
            case "NUMBER":
                return new List<TfDatabaseColumnType> { TfDatabaseColumnType.Number }.AsReadOnly();
            case "DATE":
                return new List<TfDatabaseColumnType> { TfDatabaseColumnType.DateOnly, TfDatabaseColumnType.DateTime }.AsReadOnly();
            case "DATETIME":
                return new List<TfDatabaseColumnType> { TfDatabaseColumnType.DateTime, TfDatabaseColumnType.DateOnly }.AsReadOnly();
            case "SHORT_INTEGER":
                return new List<TfDatabaseColumnType> { TfDatabaseColumnType.ShortInteger }.AsReadOnly();
            case "INTEGER":
                return new List<TfDatabaseColumnType> { TfDatabaseColumnType.Integer }.AsReadOnly();
            case "LONG_INTEGER":
                return new List<TfDatabaseColumnType> { TfDatabaseColumnType.LongInteger }.AsReadOnly();
            case "GUID":
                return new List<TfDatabaseColumnType> { TfDatabaseColumnType.Guid }.AsReadOnly();

        }
        return new List<TfDatabaseColumnType>().AsReadOnly();
    }

    /// <summary>
    /// Gets data from the data source
    /// </summary>
    public ReadOnlyCollection<TfDataProviderDataRow> GetRows(
        TfDataProvider provider,
        ITfDataProviderSychronizationLog synchLog)
    {

        var currentCulture = Thread.CurrentThread.CurrentCulture;
        var currentUICulture = Thread.CurrentThread.CurrentUICulture;
        try
        {
            CsvDataProviderSettings? settings = null;

            try
            {
                synchLog.Log("start loading provider settings");
                settings = JsonSerializer.Deserialize<CsvDataProviderSettings>(provider.SettingsJson);
                synchLog.Log("complete loading provider settings");
            }
            catch (Exception ex)
            {
                synchLog.Log("failed loading provider settings", ex);
                throw;
            }
            if (settings is null)
                settings = new();

            var culture = new CultureInfo(settings.CultureName);

            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentCulture = culture;

            var config = new CsvConfiguration(culture)
            {
                PrepareHeaderForMatch = args => args.Header.ToLower(),
                Encoding = Encoding.UTF8,
                IgnoreBlankLines = true,
                BadDataFound = null,
                TrimOptions = TrimOptions.Trim,
                HasHeaderRecord = true,
                MissingFieldFound = null
            };

            switch (settings.Delimter)
            {
                case CsvDataProviderSettingsDelimiter.Semicolon:
                    config.Delimiter = ";";
                    break;
                case CsvDataProviderSettingsDelimiter.Tab:
                    config.Delimiter = "\t";
                    break;
                default:
                    config.Delimiter = ",";
                    break;
            }

            if (string.IsNullOrWhiteSpace(settings.Filepath))
                throw new Exception("Provider csv file path is not specified.");

            if (settings.Filepath.ToLowerInvariant().StartsWith("tefter://"))
            {
                var tfService = provider.ServiceProvider.GetService<ITfService>();

                TfRepositoryFile file = null;
                try
                {
                    file = tfService.GetRepositoryFileByUri(settings.Filepath);

                    if (file is null)
                    {
                        throw new Exception($"File '{settings.Filepath}' is not found.");
                    }
                }
                catch (Exception ex)
                {
                    synchLog.Log($"failed loading provider csv file from repository file {settings.Filepath}", ex);
                    throw;
                }

                using (var stream = tfService.GetRepositoryFileContentAsFileStream(file.Filename))
                {
                    return ReadCSVStream(stream, provider, config, settings, culture, synchLog);
                }
            }
            else
            {
                if (!File.Exists(settings.Filepath))
                {
                    var ex = new Exception($"File '{settings.Filepath}' is not found.");
                    synchLog.Log($"failed loading provider csv file from file system path {settings.Filepath}", ex);
                    throw ex;
                }

                using (var stream = new FileStream(settings.Filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    return ReadCSVStream(stream, provider, config, settings, culture, synchLog);
                }
            }

        }
        finally
        {
            Thread.CurrentThread.CurrentCulture = currentCulture;
            Thread.CurrentThread.CurrentUICulture = currentUICulture;
        }
    }

    /// <summary>
    /// Gets the data source schema
    /// </summary>
    public TfDataProviderSourceSchemaInfo GetDataProviderSourceSchema(TfDataProvider provider)
    {
        int maxSampleSize = 200;
        var settings = JsonSerializer.Deserialize<CsvDataProviderSettings>(provider.SettingsJson);
        if (settings is null) settings = new();
        var culture = new CultureInfo(settings.CultureName);
        var result = new TfDataProviderSourceSchemaInfo();
        var config = new CsvConfiguration(culture)
        {
            PrepareHeaderForMatch = args => args.Header.ToLower(),
            Encoding = Encoding.UTF8,
            IgnoreBlankLines = true,
            BadDataFound = null,
            TrimOptions = TrimOptions.Trim,
            HasHeaderRecord = true,
            MissingFieldFound = null
        };
        switch (settings.Delimter)
        {
            case CsvDataProviderSettingsDelimiter.Semicolon:
                config.Delimiter = ";";
                break;
            case CsvDataProviderSettingsDelimiter.Tab:
                config.Delimiter = "\t";
                break;
            default:
                config.Delimiter = ",";
                break;
        }
        if (string.IsNullOrWhiteSpace(settings.Filepath))
            return result;

        Stream stream;
        if (settings.Filepath.ToLowerInvariant().StartsWith("tefter://"))
        {
            var tfService = provider.ServiceProvider.GetService<ITfService>();

            var file = tfService.GetRepositoryFileByUri(settings.Filepath);

            if (file is null)
                return result;

            stream = tfService.GetRepositoryFileContentAsFileStream(file.Filename);
        }
        else
        {
            stream = new FileStream(settings.Filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }

        Dictionary<string, List<TfDatabaseColumnType>> suggestedColumnTypes = new();
        using (stream)
        {
            using (var reader = new StreamReader(stream))
            {
                //for dealing with auto=generation indexes
                //we need to split the sample between the first and the last records
                int totalRecords = 0;
                while (reader.ReadLine() != null)
                {
                    ++totalRecords;
                }
                //restart reader
                stream.Position = 0;
                reader.DiscardBufferedData();

                if (totalRecords <= 1)
                    return result;

                HashSet<int> rowIndexToReadHS = totalRecords.GenerateSampleIndexesForList(maxSampleSize, skipCount: 1);

                using (var csvReader = new CsvReader(reader, config))
                {
                    csvReader.Read();
                    csvReader.ReadHeader();
                    foreach (var item in csvReader.HeaderRecord)
                    {
                        var columnName = item.ToSourceColumnName();
                        if (result.SourceColumnDefaultDbType.ContainsKey(columnName))
                            throw new Exception($"Column with the name '{columnName}' is found multiple times in the source");
                        result.SourceColumnDefaultDbType[columnName] = TfDatabaseColumnType.Text;
                    }

                    var counter = 0;
                    while (csvReader.Read())
                    {
                        if (!rowIndexToReadHS.Contains(counter))
                        {
                            counter++;
                            continue;
                        }

                        foreach (var columnNameUnprocessed in csvReader.HeaderRecord)
                        {
                            var fieldValue = csvReader.GetField(columnNameUnprocessed)?.ToString();
                            var columnName = columnNameUnprocessed.ToSourceColumnName()
                                ;
                            if (!result.SourceColumnDefaultValue.ContainsKey(columnName)
                                && !String.IsNullOrWhiteSpace(fieldValue))
                                result.SourceColumnDefaultValue[columnName] = fieldValue;

                            string? importFormat = null;
                            if (settings.AdvancedSetting is not null
                                && settings.AdvancedSetting.ColumnImportParseFormat is not null
                                && settings.AdvancedSetting.ColumnImportParseFormat.ContainsKey(columnName))
                            {
                                importFormat = settings.AdvancedSetting.ColumnImportParseFormat[columnName];
                            }

                            TfDatabaseColumnType type = SourceToColumnTypeConverter.GetDataTypeFromString(fieldValue, culture, importFormat);

                            if (!suggestedColumnTypes.ContainsKey(columnName)) suggestedColumnTypes[columnName] = new();
                            suggestedColumnTypes[columnName].Add(type);
                        }
                        counter++;
                    }
                }
            }
        }

        foreach (var key in result.SourceColumnDefaultDbType.Keys)
        {
            var columnType = TfDatabaseColumnType.Text;
            if (suggestedColumnTypes.ContainsKey(key))
                columnType = suggestedColumnTypes[key].GetTypeFromOptions();

            result.SourceColumnDefaultDbType[key] = columnType;
        }
        var preferredSourceTypeForDbType = new Dictionary<TfDatabaseColumnType, string>();
        foreach (var providerDataType in provider.ProviderType.GetSupportedSourceDataTypes())
        {
            var supportedDBList = provider.ProviderType.GetDatabaseColumnTypesForSourceDataType(providerDataType);
            //var supportedDbType = supportedDBList.Count > 0 ? supportedDBList.First() : TfDatabaseColumnType.Text;
            result.SourceTypeSupportedDbTypes[providerDataType] = supportedDBList.ToList();
            if (supportedDBList.Count > 0)
            {
                preferredSourceTypeForDbType[supportedDBList.First()] = providerDataType;
            }
        }
        foreach (var columnName in result.SourceColumnDefaultDbType.Keys)
        {
            var dbType = result.SourceColumnDefaultDbType[columnName];
            if (preferredSourceTypeForDbType.ContainsKey(dbType))
                result.SourceColumnDefaultSourceType[columnName] = preferredSourceTypeForDbType[dbType];
            else
                result.SourceColumnDefaultSourceType[columnName] = GetSupportedSourceDataTypes().First();
        }

        return result;
    }

    /// <summary>
    /// Validates its custom settings on user submit
    /// </summary>
    public List<ValidationError> Validate(string settingsJson)
    {

        CsvDataProviderSettings settings = new();
        if (!String.IsNullOrWhiteSpace(settingsJson))
        {
            try
            {
                settings = JsonSerializer.Deserialize<CsvDataProviderSettings>(settingsJson);
            }
            catch { }
        }
        var errors = new List<ValidationError>();

        if (String.IsNullOrWhiteSpace(settings.Filepath))
        {
            errors.Add(new ValidationError(nameof(CsvDataProviderSettings.Filepath), LOC("required")));
        }
        else
        {
            string extension = Path.GetExtension(settings.Filepath);
            if (extension != ".csv")
            {
                errors.Add(new ValidationError(nameof(CsvDataProviderSettings.Filepath), LOC("'csv' file extension is required")));
            }
        }

        if (!String.IsNullOrWhiteSpace(settings.CultureName))
        {
            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures & ~CultureTypes.NeutralCultures);
            var culture = cultures.FirstOrDefault(c => c.Name.Equals(settings.CultureName, StringComparison.OrdinalIgnoreCase));
            if (culture == null)
                errors.Add(new ValidationError(nameof(CsvDataProviderSettings.CultureName), LOC("invalid. format like 'en-US'")));
        }
        return errors;
    }

    public Task<bool> CanBeCreatedFromFile(
        TfImportFileToPageContextItem item)
    {
        return Task.FromResult(true);
    }

    public async Task<TfImportFileToPageResult> CreatedFromFile(
        TfImportFileToPageContextItem item)
    {
        for (int i = 0; i < 7; i++)
        {
            await Task.Delay(500);
            var type = TfProgressStreamItemType.Normal;
            if(i%3 == 0)
                type = TfProgressStreamItemType.Warning;
            
            if(i%4 == 0)
                type = TfProgressStreamItemType.Error;
            if(i == 6)
                type = TfProgressStreamItemType.Success;
            item.ProcessStream.ReportProgress(new TfProgressStreamItem()
            {
                Message = $"message {i}",
                Type = type
            });
        }
        
        
        return new TfImportFileToPageResult();
    }


    private ReadOnlyCollection<TfDataProviderDataRow> ReadCSVStream(
        Stream stream,
        TfDataProvider provider,
        CsvConfiguration config,
        CsvDataProviderSettings settings,
        CultureInfo culture,
        ITfDataProviderSychronizationLog synchLog)
    {
        var result = new List<TfDataProviderDataRow>();

        synchLog.Log($"start processing csv file");

        using (var reader = new StreamReader(stream))
        using (var csvReader = new CsvReader(reader, config))
        {
            try
            {
                synchLog.Log($"start open and parse csv file");
                csvReader.Read();
                csvReader.ReadHeader();
                synchLog.Log($"complete and parse csv file");
            }
            catch (Exception ex)
            {
                synchLog.Log($"failed to open and parse csv file", ex);
                throw;
            }

            var sourceColumns = provider.Columns.Where(x => !string.IsNullOrWhiteSpace(x.SourceName));

            int rowCounter = 1;
            while (csvReader.Read())
            {
                Dictionary<string, string> sourceRow = new Dictionary<string, string>();

                try
                {
                    foreach (var column in csvReader.HeaderRecord)
                        sourceRow[column.ToSourceColumnName()] = csvReader.GetField(column);
                }
                catch (Exception ex)
                {
                    synchLog.Log($"failed to read row[{rowCounter}] data from csv file", ex);
                    throw;
                }


                TfDataProviderDataRow row = new TfDataProviderDataRow();
                foreach (var providerColumnWithSource in sourceColumns)
                {
                    var sourceName = providerColumnWithSource.SourceName.ToSourceColumnName();

                    if (!sourceRow.ContainsKey(sourceName))
                    {
                        var ex = new Exception($"Source column '{sourceName}' is not found in csv.");
                        synchLog.Log($"Source column '{sourceName}' is not found in csv.", ex);
                        throw ex;
                    }

                    try
                    {
                        row[providerColumnWithSource.DbName] = ConvertValue(
                            providerColumnWithSource,
                            sourceRow[sourceName],
                            settings: settings,
                            culture: culture);
                    }
                    catch (Exception ex)
                    {
                        var value = sourceRow[sourceName];
                        synchLog.Log($"failed to process value for row index={rowCounter}, source column='{sourceName}'," +
                            $" provider column='{providerColumnWithSource.DbName}', provider column type='{providerColumnWithSource.DbType}'," +
                            $"  value='{value}'", ex);
                        throw;
                    }
                }
                result.Add(row);
                rowCounter++;
            }

            synchLog.Log($"successfully processed {rowCounter - 1} rows from csv file");
        }

        return result.AsReadOnly();
    }

    private object ConvertValue(
        TfDataProviderColumn column,
        object value,
        CsvDataProviderSettings settings,
        CultureInfo culture)
    {
        //CSV source values are all string
        string stringValue = value?.ToString();

        if (string.IsNullOrEmpty(stringValue) || stringValue?.ToLowerInvariant() == "null")
            return null;

        string columnImportParseFormat = null;
        if (settings is not null && settings.AdvancedSetting is not null
            && settings.AdvancedSetting.ColumnImportParseFormat is not null
            && settings.AdvancedSetting.ColumnImportParseFormat.ContainsKey(column.DbName))
        {
            columnImportParseFormat = settings.AdvancedSetting.ColumnImportParseFormat[column.DbName];
        }

        switch (column.DbType)
        {
            case TfDatabaseColumnType.ShortText:
            case TfDatabaseColumnType.Text:
                return stringValue;

            case TfDatabaseColumnType.Boolean:
                {
                    if (Boolean.TryParse(value?.ToString(), out bool parsedValue))
                        return parsedValue;

                    throw new Exception($"Cannot convert value='{value?.ToString()}' to boolean value for column {column.SourceName}");
                }

            case TfDatabaseColumnType.Guid:
                {
                    if (Guid.TryParse(value?.ToString(), out Guid parsedValue))
                        return parsedValue;

                    throw new Exception($"Cannot convert value='{value?.ToString()}' to GUID value for column {column.SourceName}");
                }

            case TfDatabaseColumnType.DateTime:
                {
                    if (!String.IsNullOrWhiteSpace(columnImportParseFormat)
                        && DateTime.TryParseExact(value?.ToString(), columnImportParseFormat, culture, DateTimeStyles.AssumeLocal, out DateTime parsedValueExact))
                        return parsedValueExact;
                    else if (DateTime.TryParse(value?.ToString(), out DateTime parsedValue))
                        return parsedValue;

                    throw new Exception($"Cannot convert value='{value?.ToString()}' to DateTime value for column {column.SourceName}");
                }

            case TfDatabaseColumnType.DateOnly:
                {
                    if (!String.IsNullOrWhiteSpace(columnImportParseFormat)
                        && DateTime.TryParseExact(value?.ToString(), columnImportParseFormat, culture, DateTimeStyles.AssumeLocal, out DateTime parsedValueExact))
                    {
                        //There are problems with DateOnly parse exact, so we use DateTime
                        return new DateOnly(parsedValueExact.Year, parsedValueExact.Month, parsedValueExact.Day);
                    }
                    else if (DateOnly.TryParse(value?.ToString(), out DateOnly parsedValue))
                        return parsedValue;

                    throw new Exception($"Cannot convert value='{value?.ToString()}' to DateOnly value for column {column.SourceName}");
                }

            case TfDatabaseColumnType.ShortInteger:
                {
                    if (short.TryParse(value?.ToString(), out short parsedValue))
                        return parsedValue;

                    throw new Exception($"Cannot convert value='{value?.ToString()}' to ShortInteger value for column {column.SourceName}");
                }

            case TfDatabaseColumnType.Integer:
                {
                    if (int.TryParse(value?.ToString(), out int parsedValue))
                        return parsedValue;

                    throw new Exception($"Cannot convert value='{value?.ToString()}' to Integer value for column {column.SourceName}");
                }

            case TfDatabaseColumnType.LongInteger:
                {
                    if (long.TryParse(value?.ToString(), out long parsedValue))
                        return parsedValue;

                    throw new Exception($"Cannot convert value='{value?.ToString()}' to LongInteger value for column {column.SourceName}");
                }

            case TfDatabaseColumnType.Number:
                {
                    if (decimal.TryParse(value?.ToString(), out decimal parsedValue))
                        return parsedValue;

                    throw new Exception($"Cannot convert value='{value?.ToString()}' to Number value for column {column.SourceName}");
                }

            default:
                throw new Exception($"Not supported source type for column {column.SourceName}");
        }
    }

    private string LOC(string name) => name;
}
