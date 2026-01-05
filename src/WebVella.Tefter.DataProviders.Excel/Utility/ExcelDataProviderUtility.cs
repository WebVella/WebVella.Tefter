using System.Text.RegularExpressions;
using ClosedXML.Excel;
using WebVella.Tefter.DataProviders.Excel.Models;
using WebVella.Tefter.Utility;

namespace WebVella.Tefter.DataProviders.Excel;

public class ExcelDataProviderUtility
{
    public (bool, string, TfDataProviderSourceSchemaInfo?) CheckExcelFile(
        MemoryStream? stream,
        ExcelDataProvider provider,
        string filepath,
        CultureInfo? culture = null)
    {
        if (stream is null)
            return (false, "File content is empty", null);
        if (stream.Length == 0)
            return (false, "File content is empty", null);

        if (Path.GetExtension(filepath).ToLower() != ".xls" && Path.GetExtension(filepath).ToLower() != ".xlsx")
            return (false, "Can process only files with '.xlsx' or '.xls' extensions", null);

        culture ??= Thread.CurrentThread.CurrentCulture;
        TfDataProviderSourceSchemaInfo? schema;

        #region << Check for Schema >>

        try
        {
            schema = GetSchemaInfo(
                stream: stream,
                provider: provider,
                culture: culture);
            stream.Position = 0;
        }
        catch (Exception ex)
        {
            return (false, $"Cannot evaluate the file schema. Error: {ex.Message}", null);
        }

        #endregion

        return (true, "File check successful", schema);
    }

    public bool CheckExcelFile(TfSpacePageCreateFromFileContextItem item)
    {
        if (item.FileContent is null)
        {
            item.ProcessStream.ReportProgress(new TfProgressStreamItem
            {
                Message = "ExcelDataProvider cannot process this file: No file content",
                Type = TfProgressStreamItemType.Debug
            });
            return false;
        }

        item.FileContent.Position = 0;
        using var memoryStream = new MemoryStream();
        item.FileContent.CopyTo(memoryStream);
        memoryStream.Position = 0;

        var (isSuccess, message, schema) =
            CheckExcelFile(memoryStream, (ExcelDataProvider)item.ProcessContext.UsedDataProviderAddon!, item.FileName);
        if (isSuccess)
            item.ProcessContext.DataSchemaInfo = schema;

        item.ProcessStream.ReportProgress(new TfProgressStreamItem()
        {
            Type = TfProgressStreamItemType.Debug,
            Message = message
        });

        return isSuccess;
    }

    private TfDataProviderSourceSchemaInfo GetSchemaInfo(MemoryStream stream,
        ExcelDataProvider provider,
        CultureInfo? culture = null)
    {
        stream.Position = 0;
        using var copy = new MemoryStream();
        stream.CopyTo(copy);
        copy.Position = 0;


        int maxSampleSize = 200;
        var result = new TfDataProviderSourceSchemaInfo();

        culture ??= Thread.CurrentThread.CurrentCulture;

        Dictionary<string, List<TfDatabaseColumnType>> suggestedColumnTypes = new();
        Dictionary<string, List<TfExcelColumnType>> suggestedSourceColumnTypes = new();

        using (var wb = new XLWorkbook(copy))
        {
            if (wb.Worksheets.Count == 0) return result;
            var ws = wb.Worksheets.First();
            var totalRecords = ws.Rows().Count();
            if (totalRecords <= 1) return result;
            if (ws.LastColumnUsed() is null) return result;
            int totalColumns = ws.LastColumnUsed()!.ColumnNumber();
            var colNameDict = new Dictionary<int, string>();

            #region << Process Header >>

            {
                for (int i = 1; i <= totalColumns; i++)
                {
                    var value = ws.Cell(1, i).Value.ToString();
                    var columnName = value.ToSourceColumnName();
                    if (!result.SourceColumnDefaultDbType.TryAdd(columnName, TfDatabaseColumnType.Text))
                        throw new Exception(
                            $"Column with the name '{columnName}' is found multiple times in the source");

                    colNameDict[i] = columnName;
                }
            }

            #endregion

            #region << Process Rows >>

            HashSet<int> rowIndexToReadHs = totalRecords.GenerateSampleIndexesForList(maxSampleSize, skipCount: 1);
            var rowIndex = 0;
            foreach (var row in ws.Rows())
            {
                if (!rowIndexToReadHs.Contains(rowIndex))
                {
                    rowIndex++;
                    continue;
                }

                var allColumnsAreBlank = true;
                for (int i = 1; i <= totalColumns; i++)
                {
                    var cell = ws.Cell(row.RowNumber(), i);
                    if (!cell.Value.IsBlank)
                    {
                        allColumnsAreBlank = false;
                        break;
                    }
                }

                if (allColumnsAreBlank)
                {
                    rowIndex++;
                    continue;
                }

                for (int i = 1; i <= totalColumns; i++)
                {
                    if (!colNameDict.ContainsKey(i))
                        throw new Exception($"Column with the index '{i}' was not initialized");
                    var columnName = colNameDict[i];
                    if (!result.SourceColumnDefaultDbType.ContainsKey(columnName))
                        throw new Exception($"Column with the name '{columnName}' was not initialized");
                    var cell = ws.Cell(row.RowNumber(), i);
                    ProcessCell(
                        columnName: columnName,
                        cell: cell,
                        result: result,
                        suggestedColumnTypes: suggestedColumnTypes,
                        suggestedSourceColumnTypes: suggestedSourceColumnTypes,
                        culture: culture);
                }

                rowIndex++;
            }

            #endregion
        }


        foreach (var key in result.SourceColumnDefaultDbType.Keys)
        {
            var columnType = TfDatabaseColumnType.Text;
            if (suggestedColumnTypes.ContainsKey(key))
                columnType = suggestedColumnTypes[key].GetTypeFromOptions();

            result.SourceColumnDefaultDbType[key] = columnType;
        }

        foreach (var providerDataType in provider.GetSupportedSourceDataTypes())
        {
            var supportedDbList = provider.GetDatabaseColumnTypesForSourceDataType(providerDataType);
            result.SourceTypeSupportedDbTypes[providerDataType] = supportedDbList.ToList();
        }

        foreach (var columnName in suggestedSourceColumnTypes.Keys)
        {
            result.SourceColumnDefaultSourceType[columnName] =
                GetTypeFromOptions(suggestedSourceColumnTypes[columnName]).ToDescriptionString();
        }


        stream.Position = 0;
        return result;
    }

    private TfExcelColumnType GetSourceTypeFromCell(IXLCell cell, TfDatabaseColumnType type)
    {
        switch (cell.DataType)
        {
            case XLDataType.Blank:
            {
                switch (type)
                {
                    case TfDatabaseColumnType.ShortInteger:
                    case TfDatabaseColumnType.Integer:
                    case TfDatabaseColumnType.LongInteger:
                    case TfDatabaseColumnType.Number:
                        return TfExcelColumnType.Number;
                    case TfDatabaseColumnType.Boolean:
                        return TfExcelColumnType.Boolean;          
                    case TfDatabaseColumnType.DateOnly:
                    case TfDatabaseColumnType.DateTime:
                        return TfExcelColumnType.DateTime;                
                    case TfDatabaseColumnType.Guid:
                        return TfExcelColumnType.Guid;                         
                    default:
                        return TfExcelColumnType.Text;
                }
            }
            case XLDataType.Boolean:
                return TfExcelColumnType.Boolean;
            case XLDataType.Number:
            case XLDataType.TimeSpan:
                return TfExcelColumnType.Number;
            case XLDataType.DateTime:
                return TfExcelColumnType.DateTime;
            default:
            {
                switch (type)
                {
                    case TfDatabaseColumnType.ShortInteger:
                    case TfDatabaseColumnType.Integer:
                    case TfDatabaseColumnType.LongInteger:
                    case TfDatabaseColumnType.Number:
                        return TfExcelColumnType.Number;
                    case TfDatabaseColumnType.Boolean:
                        return TfExcelColumnType.Boolean;          
                    case TfDatabaseColumnType.DateOnly:
                    case TfDatabaseColumnType.DateTime:
                        return TfExcelColumnType.DateTime;    
                    case TfDatabaseColumnType.Guid:
                        return TfExcelColumnType.Guid;                           
                    default:
                        return TfExcelColumnType.Text;
                }
            }
                
        }
    }

    private TfExcelColumnType GetTypeFromOptions(List<TfExcelColumnType> options)
    {
        if (options.Count == 0) return TfExcelColumnType.Text;
        var distinctOptions = options.Distinct().ToList();
        if (distinctOptions.Count == 1) return distinctOptions[0];

        if (distinctOptions.Contains(TfExcelColumnType.Text)) return TfExcelColumnType.Text;
        if (distinctOptions.Contains(TfExcelColumnType.Number)) return TfExcelColumnType.Number;
        if (distinctOptions.Contains(TfExcelColumnType.DateTime)) return TfExcelColumnType.DateTime;
        if (distinctOptions.Contains(TfExcelColumnType.Boolean)) return TfExcelColumnType.Boolean;
        return TfExcelColumnType.Text;
    }

    private void ProcessCell(string columnName, IXLCell cell,
        TfDataProviderSourceSchemaInfo result,
        Dictionary<string, List<TfDatabaseColumnType>> suggestedColumnTypes,
        Dictionary<string, List<TfExcelColumnType>> suggestedSourceColumnTypes,
        CultureInfo culture)
    {
        if (cell.DataType == XLDataType.Blank)
        {
            ProcessCellAsBlank(
                columnName: columnName,
                cell: cell,
                result: result,
                suggestedColumnTypes: suggestedColumnTypes,
                suggestedSourceColumnTypes: suggestedSourceColumnTypes,
                culture: culture);
            return;
        }

        if (cell.DataType == XLDataType.Boolean)
        {
            ProcessCellAsBoolean(
                columnName: columnName,
                cell: cell,
                result: result,
                suggestedColumnTypes: suggestedColumnTypes,
                suggestedSourceColumnTypes: suggestedSourceColumnTypes,
                culture: culture);
            return;
        }

        if (cell.DataType == XLDataType.Number)
        {
            ProcessCellAsNumber(
                columnName: columnName,
                cell: cell,
                result: result,
                suggestedColumnTypes: suggestedColumnTypes,
                suggestedSourceColumnTypes: suggestedSourceColumnTypes,
                culture: culture);
            return;
        }

        if (cell.DataType == XLDataType.Error)
        {
            ProcessCellAsError(
                columnName: columnName,
                cell: cell,
                result: result,
                suggestedColumnTypes: suggestedColumnTypes,
                suggestedSourceColumnTypes: suggestedSourceColumnTypes,
                culture: culture);
            return;
        }

        if (cell.DataType == XLDataType.DateTime)
        {
            ProcessCellAsDateTime(
                columnName: columnName,
                cell: cell,
                result: result,
                suggestedColumnTypes: suggestedColumnTypes,
                suggestedSourceColumnTypes: suggestedSourceColumnTypes,
                culture: culture);
            return;
        }

        if (cell.DataType == XLDataType.TimeSpan)
        {
            ProcessCellAsTimeSpan(
                columnName: columnName,
                cell: cell,
                result: result,
                suggestedColumnTypes: suggestedColumnTypes,
                suggestedSourceColumnTypes: suggestedSourceColumnTypes,
                culture: culture);
            return;
        }

        ProcessCellAsText(
            columnName: columnName,
            cell: cell,
            result: result,
            suggestedColumnTypes: suggestedColumnTypes,
            suggestedSourceColumnTypes: suggestedSourceColumnTypes,
            culture: culture);
    }

    private void ProcessCellAsBlank(string columnName, IXLCell cell,
        TfDataProviderSourceSchemaInfo result,
        Dictionary<string, List<TfDatabaseColumnType>> suggestedColumnTypes,
        Dictionary<string, List<TfExcelColumnType>> suggestedSourceColumnTypes,
        CultureInfo culture)
    {
        //Try Get formating from cell style or something
        var intendedType = GetIntendedDataType(cell);

        TfDatabaseColumnType type = TfDatabaseColumnType.Text;
        if (intendedType == XLDataType.Boolean)
            type = TfDatabaseColumnType.Boolean;
        else if (intendedType == XLDataType.Number)
            type = TfDatabaseColumnType.Number;
        else if (intendedType == XLDataType.TimeSpan)
            type = TfDatabaseColumnType.Number;
        else if (intendedType == XLDataType.DateTime)
            type = TfDatabaseColumnType.DateOnly;

        AddToSuggestedDictionaries(
            columnName: columnName,
            cell: cell,
            type: type,
            suggestedColumnTypes: suggestedColumnTypes,
            suggestedSourceColumnTypes: suggestedSourceColumnTypes);
    }

    private void ProcessCellAsBoolean(string columnName, IXLCell cell,
        TfDataProviderSourceSchemaInfo result,
        Dictionary<string, List<TfDatabaseColumnType>> suggestedColumnTypes,
        Dictionary<string, List<TfExcelColumnType>> suggestedSourceColumnTypes,
        CultureInfo culture)
    {
        TfDatabaseColumnType type = TfDatabaseColumnType.Boolean;
        if (cell.Value.IsBoolean)
        {
            result.SourceColumnDefaultValue.TryAdd(columnName, cell.Value.GetBoolean().ToString());
        }

        AddToSuggestedDictionaries(
            columnName: columnName,
            cell: cell,
            type: type,
            suggestedColumnTypes: suggestedColumnTypes,
            suggestedSourceColumnTypes: suggestedSourceColumnTypes);
    }

    private void ProcessCellAsNumber(string columnName, IXLCell cell,
        TfDataProviderSourceSchemaInfo result,
        Dictionary<string, List<TfDatabaseColumnType>> suggestedColumnTypes,
        Dictionary<string, List<TfExcelColumnType>> suggestedSourceColumnTypes,
        CultureInfo culture)
    {
        TfDatabaseColumnType type = TfDatabaseColumnType.Number;
        var str = cell.Value.ToString();
        if (!String.IsNullOrWhiteSpace(str))
        {
            result.SourceColumnDefaultValue.TryAdd(columnName, str);

            if (short.TryParse(str, out short shortValue))
                type = TfDatabaseColumnType.LongInteger;
            else if (Int32.TryParse(str, out int intValue))
                type = TfDatabaseColumnType.LongInteger;
            else if (Int64.TryParse(str, out long bigintValue))
                type = TfDatabaseColumnType.LongInteger;
            else if (decimal.TryParse(str, culture, out decimal decimalValue))
                type = TfDatabaseColumnType.Number;
        }

        AddToSuggestedDictionaries(
            columnName: columnName,
            cell: cell,
            type: type,
            suggestedColumnTypes: suggestedColumnTypes,
            suggestedSourceColumnTypes: suggestedSourceColumnTypes);
    }

    private void ProcessCellAsText(string columnName, IXLCell cell,
        TfDataProviderSourceSchemaInfo result,
        Dictionary<string, List<TfDatabaseColumnType>> suggestedColumnTypes,
        Dictionary<string, List<TfExcelColumnType>> suggestedSourceColumnTypes,
        CultureInfo culture)
    {
        TfDatabaseColumnType type = TfDatabaseColumnType.Text;

        var cellValue = cell.Value.ToString();
        if (!String.IsNullOrWhiteSpace(cellValue))
        {
            result.SourceColumnDefaultValue.TryAdd(columnName, cellValue);

            string? importFormat = null;
            if (cell.Style is not null)
            {
                if (cell.Style.DateFormat is not null
                    && String.IsNullOrWhiteSpace(cell.Style.DateFormat.Format))
                {
                    importFormat = cell.Style.DateFormat.Format;
                }
            }

            type =
                SourceToColumnTypeConverter.GetDataTypeFromString(cellValue, culture, importFormat);
        }

        AddToSuggestedDictionaries(
            columnName: columnName,
            cell: cell,
            type: type,
            suggestedColumnTypes: suggestedColumnTypes,
            suggestedSourceColumnTypes: suggestedSourceColumnTypes);
    }

    private void ProcessCellAsError(string columnName, IXLCell cell,
        TfDataProviderSourceSchemaInfo result,
        Dictionary<string, List<TfDatabaseColumnType>> suggestedColumnTypes,
        Dictionary<string, List<TfExcelColumnType>> suggestedSourceColumnTypes,
        CultureInfo culture)
    {
        //Try Get formating from cell style or something
        var intendedType = GetIntendedDataType(cell);

        TfDatabaseColumnType type = TfDatabaseColumnType.Text;
        if (intendedType == XLDataType.Boolean)
            type = TfDatabaseColumnType.Boolean;
        else if (intendedType == XLDataType.Number)
            type = TfDatabaseColumnType.Number;
        else if (intendedType == XLDataType.TimeSpan)
            type = TfDatabaseColumnType.Number;
        else if (intendedType == XLDataType.DateTime)
            type = TfDatabaseColumnType.DateOnly;


        AddToSuggestedDictionaries(
            columnName: columnName,
            cell: cell,
            type: type,
            suggestedColumnTypes: suggestedColumnTypes,
            suggestedSourceColumnTypes: suggestedSourceColumnTypes);
    }

    private void ProcessCellAsDateTime(string columnName, IXLCell cell,
        TfDataProviderSourceSchemaInfo result,
        Dictionary<string, List<TfDatabaseColumnType>> suggestedColumnTypes,
        Dictionary<string, List<TfExcelColumnType>> suggestedSourceColumnTypes,
        CultureInfo culture)
    {
        TfDatabaseColumnType type = TfDatabaseColumnType.DateTime;
        var cellValue = cell.Value.ToString();
        if (!String.IsNullOrWhiteSpace(cellValue))
        {
            var value = cell.Value.GetDateTime();

            if (value is { Hour: 0, Minute: 0, Second: 0 })
                type = TfDatabaseColumnType.DateOnly;

            string? importFormat = null;
            if (cell.Style is not null)
            {
                if (cell.Style.DateFormat is not null
                    && String.IsNullOrWhiteSpace(cell.Style.DateFormat.Format))
                {
                    importFormat = cell.Style.DateFormat.Format;
                }
            }


            if (type == TfDatabaseColumnType.DateOnly)
            {
                cellValue = DateOnly.FromDateTime(value).ToString(importFormat, culture);
                result.SourceColumnDefaultValue.TryAdd(columnName, cellValue);
            }
            else
            {
                cellValue = value.ToString(importFormat, culture);
                result.SourceColumnDefaultValue.TryAdd(columnName, cellValue);
            }
        }

        AddToSuggestedDictionaries(
            columnName: columnName,
            cell: cell,
            type: type,
            suggestedColumnTypes: suggestedColumnTypes,
            suggestedSourceColumnTypes: suggestedSourceColumnTypes);
    }

    private void ProcessCellAsTimeSpan(string columnName, IXLCell cell,
        TfDataProviderSourceSchemaInfo result,
        Dictionary<string, List<TfDatabaseColumnType>> suggestedColumnTypes,
        Dictionary<string, List<TfExcelColumnType>> suggestedSourceColumnTypes,
        CultureInfo culture)
    {
        ProcessCellAsNumber(
            columnName: columnName,
            cell: cell,
            result: result,
            suggestedColumnTypes: suggestedColumnTypes,
            suggestedSourceColumnTypes: suggestedSourceColumnTypes,
            culture: culture);
    }

    private void AddToSuggestedDictionaries(string columnName,
        IXLCell cell,
        TfDatabaseColumnType type,
        Dictionary<string, List<TfDatabaseColumnType>> suggestedColumnTypes,
        Dictionary<string, List<TfExcelColumnType>> suggestedSourceColumnTypes)
    {
        if (!suggestedColumnTypes.ContainsKey(columnName)) suggestedColumnTypes[columnName] = new();
        suggestedColumnTypes[columnName].Add(type);

        if (!suggestedSourceColumnTypes.ContainsKey(columnName))
            suggestedSourceColumnTypes[columnName] = new();
        suggestedSourceColumnTypes[columnName].Add(GetSourceTypeFromCell(cell,type));
    }

    private XLDataType GetIntendedDataType(IXLCell cell)
    {
        // 1. If the cell has a value, return the actual type.
        if (!cell.IsEmpty())
        {
            return cell.DataType;
        }

        var numberFormat = cell.Style.NumberFormat;

        // 2. Check for Text Format explicitly
        // ID 49 is the standard "@" text format
        if (numberFormat.NumberFormatId == 49 || numberFormat.Format == "@")
        {
            return XLDataType.Text;
        }

        // 3. Check for Standard Date/Time IDs
        // Excel standard IDs: 14-22 are dates/times, 45-47 are duration/time
        if ((numberFormat.NumberFormatId >= 14 && numberFormat.NumberFormatId <= 22) ||
            (numberFormat.NumberFormatId >= 45 && numberFormat.NumberFormatId <= 47))
        {
            return XLDataType.DateTime;
        }

        // 4. Check for Standard Number IDs
        // 1-13 are various number/currency, 37-44 are accounting, 48 is scientific
        if ((numberFormat.NumberFormatId >= 1 && numberFormat.NumberFormatId <= 13) ||
            (numberFormat.NumberFormatId >= 37 && numberFormat.NumberFormatId <= 44) ||
            numberFormat.NumberFormatId == 48)
        {
            return XLDataType.Number;
        }

        // 5. Analyze Custom Format Strings (if ID is not standard)
        // If the format contains typical date characters not enclosed in quotes
        if (IsCustomDateFormat(numberFormat.Format))
        {
            return XLDataType.DateTime;
        }

        if (IsCustomNumberFormat(numberFormat.Format))
        {
            return XLDataType.Number;
        }

        // Default to Text or String if "General" (ID 0) or unknown
        return XLDataType.Text;
    }

    private bool IsCustomDateFormat(string formatCode)
    {
        // Remove literal text enclosed in quotes "..." to avoid false positives
        string codeWithoutLiterals = Regex.Replace(formatCode, "\"[^\"]*\"", "");

        // Look for date/time tokens: y, m, d, h, s, am/pm
        // NOTE: 'm' is tricky as it can be month or minute, but usually implies Date/Time context
        return Regex.IsMatch(codeWithoutLiterals, @"[ymdhs]|am\/pm", RegexOptions.IgnoreCase);
    }

    private bool IsCustomNumberFormat(string formatCode)
    {
        // Simple check for number placeholders
        return formatCode.IndexOfAny(new[] { '0', '#', '?' }) >= 0;
    }
}