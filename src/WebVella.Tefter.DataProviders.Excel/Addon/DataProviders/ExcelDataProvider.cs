using ClosedXML.Excel;
using CsvHelper;
using CsvHelper.Configuration;
using DocumentFormat.OpenXml.Math;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
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
            TfExcelColumnType.Text.ToDescriptionString(),
            TfExcelColumnType.Number.ToDescriptionString(),
            TfExcelColumnType.DateTime.ToDescriptionString(),
            TfExcelColumnType.Boolean.ToDescriptionString(),
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
                return new List<TfDatabaseColumnType> { TfDatabaseColumnType.Text,
                    TfDatabaseColumnType.ShortText,
                    TfDatabaseColumnType.Guid,
                    }.AsReadOnly();
            case "NUMBER":
                return new List<TfDatabaseColumnType> { TfDatabaseColumnType.Number,
                    TfDatabaseColumnType.ShortInteger,
                    TfDatabaseColumnType.Integer,
                    TfDatabaseColumnType.LongInteger,
                    }.AsReadOnly();
            case "DATETIME":
                return new List<TfDatabaseColumnType> { TfDatabaseColumnType.DateTime, TfDatabaseColumnType.DateOnly }.AsReadOnly();
            case "BOOLEAN":
                return new List<TfDatabaseColumnType> { TfDatabaseColumnType.Boolean }.AsReadOnly();
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
            Thread.CurrentThread.CurrentUICulture = culture;

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
                    return ReadExcelStream(stream, provider, settings, culture, synchLog);
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
                    return ReadExcelStream(stream, provider, settings, culture, synchLog);
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
        var settings = JsonSerializer.Deserialize<ExcelDataProviderSettings>(provider.SettingsJson);
        if (settings is null) settings = new();
        var culture = new CultureInfo(settings.CultureName);

        Stream stream;
        if (string.IsNullOrWhiteSpace(settings.Filepath))
            throw new Exception("Provider csv file path is not specified.");

        if (settings.Filepath.ToLowerInvariant().StartsWith("tefter://"))
        {
            var tfService = provider.ServiceProvider.GetService<ITfService>();

            var file = tfService!.GetRepositoryFileByUri(settings.Filepath);

            if (file is null)
                throw new Exception($"File '{settings.Filepath}' is not found.");

            stream = tfService.GetRepositoryFileContentAsFileStream(file.Filename);
        }
        else
        {
            stream = new FileStream(settings.Filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }
        using var memoryStream = new MemoryStream();
        stream.Position = 0;
        stream.CopyTo(memoryStream);
        memoryStream.Position = 0;

        var (isSuccess, message, schemaInfo) = new ExcelDataProviderUtility().CheckExcelFile(memoryStream,
            filepath: settings.Filepath,
            provider: this);
        if (!isSuccess)
            throw new Exception(message);

        if (schemaInfo is null)
            throw new Exception("File Schema cannot be parsed");

        return schemaInfo;
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

    public async Task GenerateDataProviderCreationRequest(
        TfSpacePageCreateFromFileContextItem item,ITfService tfService)
    {
        await Task.Delay(0);
        item.ProcessContext.UsedDataProviderAddon = this;
        item.ProcessStream.ReportProgress(new TfProgressStreamItem()
        {
            Message = "ExcelDataProvider is validating file and discovering its data schema...",
            Type = TfProgressStreamItemType.Debug,
        });
        var culture = Thread.CurrentThread.CurrentCulture;
        #region << Validate >>

        if (item.FileContent is null)
        {
            item.ProcessStream.ReportProgress(new TfProgressStreamItem()
            {
                Message = "ExcelDataProvider cannot process this file as it is empty!",
                Type = TfProgressStreamItemType.Debug,
            });
            item.IsSuccess = false;
            return;
        }

        var checkResult = new ExcelDataProviderUtility().CheckExcelFile(item);
        if (!checkResult)
        {
            item.ProcessStream.ReportProgress(new TfProgressStreamItem()
            {
                Message = "ExcelDataProvider cannot process this file, due to parse error!",
                Type = TfProgressStreamItemType.Debug,
            });
            return;
        }

        if (item.ProcessContext.DataSchemaInfo is null)
        {
            item.ProcessStream.ReportProgress(new TfProgressStreamItem()
            {
                Message = "ExcelDataProvider cannot process this file, schema cannot be evaluated!",
                Type = TfProgressStreamItemType.Debug,
            });
            return;
        }
        var columnsData = new List<string>();
        foreach (var columnName in item.ProcessContext.DataSchemaInfo.SourceColumnDefaultDbType.Keys)
        {
            columnsData.Add($"{columnName}({item.ProcessContext.DataSchemaInfo.SourceColumnDefaultDbType[columnName]})");
        }
        item.ProcessStream.ReportProgress(new TfProgressStreamItem()
        {
            Message = $"Columns found: {String.Join(", ", columnsData)}",
            Type = TfProgressStreamItemType.Debug,
        });
        #endregion


        item.ProcessStream.ReportProgress(new TfProgressStreamItem()
        {
            Message = "File validation is successful for ExcelDataProvider!",
            Type = TfProgressStreamItemType.Debug,
        });
        item.ProcessStream.ReportProgress(new TfProgressStreamItem()
        {
            Message = "ExcelDataProvider is preparing model for creation...",
            Type = TfProgressStreamItemType.Debug,
        });
        //Create Repository File
        var repFile = tfService.CreateRepositoryFile(item.FileName, item.FileContent, item.User.Id);
        item.ProcessContext.CreatedRepositoryFiles.Add(repFile.Filename);
        item.ProcessContext.DataProviderCreationRequest = new TfImportFileToPageDataProviderCreationRequest()
        {
            Name = repFile.Filename,
            SettingsJson = JsonSerializer.Serialize(new ExcelDataProviderSettings
            {
                Filepath = repFile.Uri.ToString(),
                AdvancedSetting = new ExcelDataProviderSettingsAdvanced()
                {
                    ColumnImportParseFormat = new()
                },
                CultureName = culture.Name,
            }),
            SynchPrimaryKeyColumns = new(),
            SynchScheduleEnabled = false,
            SynchScheduleMinutes = 60
        };
        item.ProcessStream.ReportProgress(new TfProgressStreamItem()
        {
            Message = "ExcelDataProvider model preparation done!",
            Type = TfProgressStreamItemType.Debug,
        });
    }

    private ReadOnlyCollection<TfDataProviderDataRow> ReadExcelStream(
        Stream stream,
        TfDataProvider provider,
        ExcelDataProviderSettings settings,
        CultureInfo culture,
        ITfDataProviderSychronizationLog synchLog)
    {
        var result = new List<TfDataProviderDataRow>();

        synchLog.Log($"start processing Excel file");

        using var copy = new MemoryStream();
        stream.CopyTo(copy);
        copy.Position = 0;

        using (var wb = new XLWorkbook(copy))
        {
            if (wb.Worksheets.Count == 0) return result.AsReadOnly();
            var ws = wb.Worksheets.First();
            var totalRecords = ws.Rows().Count();
            if (totalRecords <= 1) return result.AsReadOnly();
            if (ws.LastColumnUsed() is null) return result.AsReadOnly();
            int totalColumns = ws.LastColumnUsed()!.ColumnNumber();
            var sourceColumns = provider.Columns.Where(x => !string.IsNullOrWhiteSpace(x.SourceName));
            var colNamePositionDict = new Dictionary<string, int>();
            #region << Process Header >>
            {
                var row = ws.Rows().First();
                for (int i = 1; i <= totalColumns; i++)
                {
                    var value = ws.Cell(1, i).Value.ToString();
                    var columnName = value.ToSourceColumnName();
                    colNamePositionDict[columnName] = i;
                }
            }
            #endregion

            //Starts from 2 as the first row is the header
            for (int rowPosition = 2; rowPosition <= totalRecords; rowPosition++)
            {
                TfDataProviderDataRow row = new TfDataProviderDataRow();
                foreach (var dbColumn in sourceColumns)
                {
                    if (String.IsNullOrWhiteSpace(dbColumn.DbName)) continue;
                    if (String.IsNullOrWhiteSpace(dbColumn.SourceName)) continue;
                    if (!colNamePositionDict.ContainsKey(dbColumn.SourceName))
                    {
                        var ex = new Exception($"Source column '{dbColumn.SourceName}' is not found in csv.");
                        synchLog.Log($"Source column '{dbColumn.SourceName}' is not found in csv.", ex);
                        throw ex;
                    }

                    var colPosition = colNamePositionDict[dbColumn.SourceName];
                    var cell = ws.Cell(rowPosition, colPosition);
                    try
                    {
                        row[dbColumn.DbName] = ConvertValue(
                            dbColumn,
                            cell,
                            settings: settings,
                            culture: culture);
                    }
                    catch (Exception ex)
                    {
                        synchLog.Log($"failed to process value for row index={rowPosition}, source column='{dbColumn.SourceName}'," +
                            $" provider column='{dbColumn.DbName}', provider column type='{dbColumn.DbType}'," +
                            $"  value='{cell.Value.ToString()}'", ex);
                        throw;
                    }
                }
                result.Add(row);
            }
            if (synchLog != null)
                synchLog.Log($"successfully processed {totalRecords - 1} rows from csv file");
        }
        return result.AsReadOnly();
    }

    private object? ConvertValue(
        TfDataProviderColumn column,
        IXLCell cell,
        ExcelDataProviderSettings settings,
        CultureInfo culture)
    {
        if (cell.Value.IsBlank) return null;
        var stringValue = cell.Value.ToString();
        if (string.IsNullOrEmpty(stringValue) || stringValue?.ToLowerInvariant() == "null")
            return null;

        string? columnImportParseFormat = null;
        if (settings is not null && settings.AdvancedSetting is not null
            && settings.AdvancedSetting.ColumnImportParseFormat is not null
            && settings.AdvancedSetting.ColumnImportParseFormat.ContainsKey(column.DbName!))
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
                    if (cell.DataType == XLDataType.Boolean)
                        return cell.Value.GetBoolean();
                    if (Boolean.TryParse(stringValue, out bool parsedValue))
                        return parsedValue;

                    throw new Exception($"Cannot convert value='{stringValue}' to boolean value for column {column.SourceName}");
                }

            case TfDatabaseColumnType.Guid:
                {
                    if (Guid.TryParse(stringValue, out Guid parsedValue))
                        return parsedValue;

                    throw new Exception($"Cannot convert value='{stringValue}' to GUID value for column {column.SourceName}");
                }

            case TfDatabaseColumnType.DateTime:
                {
                    if (cell.DataType == XLDataType.DateTime)
                        return cell.Value.GetDateTime();

                    if (!String.IsNullOrWhiteSpace(columnImportParseFormat)
                        && DateTime.TryParseExact(stringValue, columnImportParseFormat, culture, DateTimeStyles.AssumeLocal, out DateTime parsedValueExact))
                        return parsedValueExact;
                    if (DateTime.TryParse(stringValue, out DateTime parsedValue))
                        return parsedValue;

                    throw new Exception($"Cannot convert value='{stringValue}' to DateTime value for column {column.SourceName}");
                }

            case TfDatabaseColumnType.DateOnly:
                {
                    if (cell.DataType == XLDataType.DateTime)
                    {
                        return DateOnly.FromDateTime(cell.Value.GetDateTime());
                    }

                    if (!String.IsNullOrWhiteSpace(columnImportParseFormat)
                        && DateTime.TryParseExact(stringValue, columnImportParseFormat, culture, DateTimeStyles.AssumeLocal, out DateTime parsedValueExact))
                    {
                        //There are problems with DateOnly parse exact, so we use DateTime
                        return new DateOnly(parsedValueExact.Year, parsedValueExact.Month, parsedValueExact.Day);
                    }
                    if (DateOnly.TryParse(stringValue, out DateOnly parsedValue))
                        return parsedValue;

                    throw new Exception($"Cannot convert value='{stringValue}' to DateOnly value for column {column.SourceName}");
                }

            case TfDatabaseColumnType.ShortInteger:
                {
                    if (cell.DataType == XLDataType.Number)
                    {
                        var number = cell.Value.GetNumber();
                        try
                        {
                            return (short)number;
                        }
                        catch
                        {
                            throw new Exception($"Cannot convert value='{stringValue?.ToString()}' to ShortInteger value for column {column.SourceName}");
                        }
                    }

                    if (short.TryParse(stringValue, out short parsedValue))
                        return parsedValue;

                    throw new Exception($"Cannot convert value='{stringValue?.ToString()}' to ShortInteger value for column {column.SourceName}");
                }

            case TfDatabaseColumnType.Integer:
                {
                    if (cell.DataType == XLDataType.Number)
                    {
                        var number = cell.Value.GetNumber();
                        try
                        {
                            return (int)number;
                        }
                        catch
                        {
                            throw new Exception($"Cannot convert value='{stringValue}' to Integer value for column {column.SourceName}");
                        }
                    }
                    if (int.TryParse(stringValue, out int parsedValue))
                        return parsedValue;

                    throw new Exception($"Cannot convert value='{stringValue}' to Integer value for column {column.SourceName}");
                }

            case TfDatabaseColumnType.LongInteger:
                {
                    if (cell.DataType == XLDataType.Number)
                    {
                        var number = cell.Value.GetNumber();
                        try
                        {
                            return (long)number;
                        }
                        catch
                        {
                            throw new Exception($"Cannot convert value='{stringValue}' to LongInteger value for column {column.SourceName}");
                        }
                    }

                    if (long.TryParse(stringValue, out long parsedValue))
                        return parsedValue;

                    throw new Exception($"Cannot convert value='{stringValue}' to LongInteger value for column {column.SourceName}");
                }

            case TfDatabaseColumnType.Number:
                {
                    if (cell.DataType == XLDataType.Number)
                    {
                        var number = cell.Value.GetNumber();
                        try
                        {
                            return (decimal)number;
                        }
                        catch
                        {
                            throw new Exception($"Cannot convert value='{stringValue}' to Number value for column {column.SourceName}");
                        }
                    }

                    if (decimal.TryParse(stringValue, out decimal parsedValue))
                        return parsedValue;

                    throw new Exception($"Cannot convert value='{stringValue}' to Number value for column {column.SourceName}");
                }

            default:
                throw new Exception($"Not supported source type for column {column.SourceName}");
        }
    }

    private string LOC(string name) => name;

    


}
