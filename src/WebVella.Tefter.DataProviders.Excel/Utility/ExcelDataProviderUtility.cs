using ClosedXML.Excel;
using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
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
        TfDataProviderSourceSchemaInfo? schema = null;
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
                var row = ws.Rows().First();
                for (int i = 1; i <= totalColumns; i++)
                {
                    var value = ws.Cell(1, i).Value.ToString();
                    var columnName = value.ToSourceColumnName();
                    if (result.SourceColumnDefaultDbType.ContainsKey(columnName))
                        throw new Exception($"Column with the name '{columnName}' is found multiple times in the source");

                    result.SourceColumnDefaultDbType[columnName] = TfDatabaseColumnType.Text;
                    colNameDict[i] = columnName;
                }
            }
            #endregion

            #region << Process Rows >>
            HashSet<int> rowIndexToReadHS = totalRecords.GenerateSampleIndexesForList(maxSampleSize, skipCount: 1);
            var rowIndex = 0;
            foreach (var row in ws.Rows())
            {
                if (!rowIndexToReadHS.Contains(rowIndex))
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
                    var cellValue = cell.Value.ToString();

                    if (!result.SourceColumnDefaultValue.ContainsKey(columnName)
                        && !String.IsNullOrWhiteSpace(cellValue))
                        result.SourceColumnDefaultValue[columnName] = cellValue;

                    string? importFormat = null;
                    if (cell.Style is not null)
                    {
                        if (cell.Style.DateFormat is not null
                            && String.IsNullOrWhiteSpace(cell.Style.DateFormat.Format))
                        {
                            importFormat = cell.Style.DateFormat.Format;
                        }
                    }
                    TfDatabaseColumnType type = SourceToColumnTypeConverter.GetDataTypeFromString(cellValue, culture, importFormat);

                    if (!suggestedColumnTypes.ContainsKey(columnName)) suggestedColumnTypes[columnName] = new();
                    suggestedColumnTypes[columnName].Add(type);

                    if (!suggestedSourceColumnTypes.ContainsKey(columnName)) suggestedSourceColumnTypes[columnName] = new();
                    suggestedSourceColumnTypes[columnName].Add(GetSourceTypeFromCell(cell));
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

        var preferredSourceTypeForDbType = new Dictionary<TfDatabaseColumnType, string>();
        foreach (var providerDataType in provider.GetSupportedSourceDataTypes())
        {
            var supportedDBList = provider.GetDatabaseColumnTypesForSourceDataType(providerDataType);
            var supportedDbType = supportedDBList.Count > 0 ? supportedDBList.First() : TfDatabaseColumnType.Text;
            result.SourceTypeSupportedDbTypes[providerDataType] = supportedDBList.ToList();
            if (supportedDBList.Count > 0)
            {
                preferredSourceTypeForDbType[supportedDBList.First()] = providerDataType;
            }
        }
        foreach (var columnName in suggestedSourceColumnTypes.Keys)
        {
            result.SourceColumnDefaultSourceType[columnName] = GetTypeFromOptions(suggestedSourceColumnTypes[columnName]).ToDescriptionString();
        }


        stream.Position = 0;
        return result;
    }
    private TfExcelColumnType GetSourceTypeFromCell(IXLCell cell)
    {
        switch (cell.DataType)
        {
            case XLDataType.Boolean:
                return TfExcelColumnType.Boolean;
            case XLDataType.Number:
                return TfExcelColumnType.Number;
            case XLDataType.DateTime:
                return TfExcelColumnType.DateTime;
            default:
                return TfExcelColumnType.Text;
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
}
