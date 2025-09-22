using ClosedXML.Excel;
using CsvHelper;
using CsvHelper.Configuration;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using WebVella.Tefter.DataProviders.Excel.Models;
using WebVella.Tefter.Exceptions;
using WebVella.Tefter.Models;
using WebVella.Tefter.Services;
using WebVella.Tefter.Utility;

namespace WebVella.Tefter.DataProviders.Excel.Addons;

public class ExcelDataProvider : ITfDataProviderAddon
{

    public const string ID = "7be5a3cd-c922-4e20-99d5-5555f141133c";
    public const string NAME = "Excel Data Provider";
    public const string DESCRIPTION = "Provide data from Excel file";
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
            TfExcelColumnType.Unknown.ToDescriptionString(),
            TfExcelColumnType.Text.ToDescriptionString(),
            TfExcelColumnType.Number.ToDescriptionString(),
            TfExcelColumnType.DateTime.ToDescriptionString(),
            TfExcelColumnType.Boolean.ToDescriptionString(),
            TfExcelColumnType.Guid.ToDescriptionString(),
            TfExcelColumnType.Currency.ToDescriptionString(),
            TfExcelColumnType.Percentage.ToDescriptionString(),
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
            case "UNKNOWN":
                return new List<TfDatabaseColumnType> { TfDatabaseColumnType.Text }.AsReadOnly();
            case "TEXT":
                return new List<TfDatabaseColumnType> { TfDatabaseColumnType.Text }.AsReadOnly();
            case "NUMBER":
                return new List<TfDatabaseColumnType> { TfDatabaseColumnType.Number }.AsReadOnly();
            case "DATETIME":
                return new List<TfDatabaseColumnType> { TfDatabaseColumnType.DateTime, TfDatabaseColumnType.DateOnly }.AsReadOnly();
            case "BOOLEAN":
                return new List<TfDatabaseColumnType> { TfDatabaseColumnType.Boolean }.AsReadOnly();
            case "GUID":
                return new List<TfDatabaseColumnType> { TfDatabaseColumnType.Guid }.AsReadOnly();
            case "CURRENCY":
                return new List<TfDatabaseColumnType> { TfDatabaseColumnType.Number }.AsReadOnly();
            case "PERCENTAGE":
                return new List<TfDatabaseColumnType> { TfDatabaseColumnType.Number }.AsReadOnly();
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
        if (provider is null)
            throw new ArgumentNullException(nameof(provider));

        var currentCulture = Thread.CurrentThread.CurrentCulture;
        var currentUICulture = Thread.CurrentThread.CurrentUICulture;

        var result = new List<TfDataProviderDataRow>();

        try
        {
            ExcelDataProviderSettings? settings = null;

            try
            {
                if (!String.IsNullOrWhiteSpace(provider.SettingsJson))
                {
                    synchLog.Log("start loading provider settings");
                    settings = JsonSerializer.Deserialize<ExcelDataProviderSettings>(provider.SettingsJson);
                    synchLog.Log("complete loading provider settings");
                }
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
                    return ReadExcelStream(stream, provider, settings, synchLog);
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
                    return ReadExcelStream(stream, provider, settings, synchLog);
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
        var settings = JsonSerializer.Deserialize<ExcelDataProviderSettings>(provider.SettingsJson);
        var culture = new CultureInfo(settings.CultureName);
        var result = new TfDataProviderSourceSchemaInfo();

        Stream stream;
        if (string.IsNullOrWhiteSpace(settings.Filepath))
            throw new Exception("Provider csv file path is not specified.");

        if (settings.Filepath.ToLowerInvariant().StartsWith("tefter://"))
        {
            var tfService = provider.ServiceProvider.GetService<ITfService>();

            var file = tfService.GetRepositoryFileByUri(settings.Filepath);

            if (file is null)
                throw new Exception($"File '{settings.Filepath}' is not found.");

            stream = tfService.GetRepositoryFileContentAsFileStream(file.Filename);
        }
        else
        {
            stream = new FileStream(settings.Filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }
        Dictionary<string, List<TfDatabaseColumnType>> suggestedColumnTypes = new();

        using (stream)
        {
            var columns = ReadExcelStream(stream, provider, settings);
        }
        return result;
    }

    /// <summary>
    /// Validates its custom settings on user submit
    /// </summary>
    public List<ValidationError> Validate(string settingsJson)
    {

        ExcelDataProviderSettings settings = new();
        if (!String.IsNullOrWhiteSpace(settingsJson))
        {
            try
            {
                settings = JsonSerializer.Deserialize<ExcelDataProviderSettings>(settingsJson);
            }
            catch { }
        }
        var errors = new List<ValidationError>();

        if (String.IsNullOrWhiteSpace(settings.Filepath))
        {
            errors.Add(new ValidationError(nameof(ExcelDataProviderSettings.Filepath), LOC("required")));
        }
        else
        {
            string extension = Path.GetExtension(settings.Filepath);
            if (extension != ".xlsx" && extension != ".xls")
            {
                errors.Add(new ValidationError(nameof(ExcelDataProviderSettings.Filepath), LOC("'.xlsx' or '.xls' file extension is required")));
            }
        }

        if (!String.IsNullOrWhiteSpace(settings.CultureName))
        {
            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures & ~CultureTypes.NeutralCultures);
            var culture = cultures.FirstOrDefault(c => c.Name.Equals(settings.CultureName, StringComparison.OrdinalIgnoreCase));
            if (culture == null)
                errors.Add(new ValidationError(nameof(ExcelDataProviderSettings.CultureName), LOC("invalid. format like 'en-US'")));
        }
        return errors;
    }


    private ReadOnlyCollection<TfDataProviderDataRow> ReadExcelStream(
        Stream stream,
        TfDataProvider provider,
        ExcelDataProviderSettings settings,
        ITfDataProviderSychronizationLog? synchLog = null)
    {
        var result = new List<TfDataProviderDataRow>();

        if (synchLog != null)
            synchLog.Log($"start processing csv file");

        using (var wb = new XLWorkbook(stream))
        {
            foreach (var ws in wb.Worksheets)
            {
                string? headerText = null;
                var columns = new List<TfExcelColumnInfo>();

                //if (settings.HeaderRow is not null)
                //{
                //    var headerRowObj = ws.Row(settings.HeaderRow.Value);
                //    if (headerRowObj == null)
                //        throw new InvalidOperationException($"Header row {settings.HeaderRow.Value} does not exist.");

                //    foreach (var cell in headerRowObj.CellsUsed())
                //    {
                //        var columnIndex = cell.Address.ColumnNumber;
                //        var headerText = cell.GetString();

                //        // Grab all non‑empty cells below the header to infer type
                //        var dataCells = ws.Range(columnIndex, settings.HeaderRow.Value + 1,
                //                                        columnIndex, ws.LastRowUsed().RowNumber())
                //                                .Cells()
                //                                .Where(c => !c.IsEmpty());

                //        var inferredType = dataCells.InferColumnType();
                //        columns.Add(new TfExcelColumnInfo { Header = headerText, Type = inferredType });
                //    }
                //}
            }

            //if (synchLog != null)
            //synchLog.Log($"successfully processed {rowCounter - 1} rows from csv file");
        }

        return result.AsReadOnly();
    }

    private object ConvertValue(
        TfDataProviderColumn column,
        object value,
        ExcelDataProviderSettings settings,
        CultureInfo culture)
    {
        //CSV source values are all string
        string stringValue = value?.ToString();

        if (string.IsNullOrEmpty(stringValue) || stringValue?.ToLowerInvariant() == "null")
            return null;

        return null;
        //string columnImportParseFormat = null;
        //if (settings is not null && settings.AdvancedSetting is not null
        //    && settings.AdvancedSetting.ColumnImportParseFormat is not null
        //    && settings.AdvancedSetting.ColumnImportParseFormat.ContainsKey(column.DbName))
        //{
        //    columnImportParseFormat = settings.AdvancedSetting.ColumnImportParseFormat[column.DbName];
        //}

        //switch (column.DbType)
        //{
        //    case TfDatabaseColumnType.ShortText:
        //    case TfDatabaseColumnType.Text:
        //        return stringValue;

        //    case TfDatabaseColumnType.Boolean:
        //        {
        //            if (Boolean.TryParse(value?.ToString(), out bool parsedValue))
        //                return parsedValue;

        //            throw new Exception($"Cannot convert value='{value?.ToString()}' to boolean value for column {column.SourceName}");
        //        }

        //    case TfDatabaseColumnType.Guid:
        //        {
        //            if (Guid.TryParse(value?.ToString(), out Guid parsedValue))
        //                return parsedValue;

        //            throw new Exception($"Cannot convert value='{value?.ToString()}' to GUID value for column {column.SourceName}");
        //        }

        //    case TfDatabaseColumnType.DateTime:
        //        {
        //            if (!String.IsNullOrWhiteSpace(columnImportParseFormat)
        //                && DateTime.TryParseExact(value?.ToString(), columnImportParseFormat, culture, DateTimeStyles.AssumeLocal, out DateTime parsedValueExact))
        //                return parsedValueExact;
        //            else if (DateTime.TryParse(value?.ToString(), out DateTime parsedValue))
        //                return parsedValue;

        //            throw new Exception($"Cannot convert value='{value?.ToString()}' to DateTime value for column {column.SourceName}");
        //        }

        //    case TfDatabaseColumnType.DateOnly:
        //        {
        //            if (!String.IsNullOrWhiteSpace(columnImportParseFormat)
        //                && DateTime.TryParseExact(value?.ToString(), columnImportParseFormat, culture, DateTimeStyles.AssumeLocal, out DateTime parsedValueExact))
        //            {
        //                //There are problems with DateOnly parse exact, so we use DateTime
        //                return new DateOnly(parsedValueExact.Year, parsedValueExact.Month, parsedValueExact.Day);
        //            }
        //            else if (DateOnly.TryParse(value?.ToString(), out DateOnly parsedValue))
        //                return parsedValue;

        //            throw new Exception($"Cannot convert value='{value?.ToString()}' to DateOnly value for column {column.SourceName}");
        //        }

        //    case TfDatabaseColumnType.ShortInteger:
        //        {
        //            if (short.TryParse(value?.ToString(), out short parsedValue))
        //                return parsedValue;

        //            throw new Exception($"Cannot convert value='{value?.ToString()}' to ShortInteger value for column {column.SourceName}");
        //        }

        //    case TfDatabaseColumnType.Integer:
        //        {
        //            if (int.TryParse(value?.ToString(), out int parsedValue))
        //                return parsedValue;

        //            throw new Exception($"Cannot convert value='{value?.ToString()}' to Integer value for column {column.SourceName}");
        //        }

        //    case TfDatabaseColumnType.LongInteger:
        //        {
        //            if (long.TryParse(value?.ToString(), out long parsedValue))
        //                return parsedValue;

        //            throw new Exception($"Cannot convert value='{value?.ToString()}' to LongInteger value for column {column.SourceName}");
        //        }

        //    case TfDatabaseColumnType.Number:
        //        {
        //            if (decimal.TryParse(value?.ToString(), out decimal parsedValue))
        //                return parsedValue;

        //            throw new Exception($"Cannot convert value='{value?.ToString()}' to Number value for column {column.SourceName}");
        //        }

        //    default:
        //        throw new Exception($"Not supported source type for column {column.SourceName}");
        //}
    }

    private string LOC(string name) => name;
}
