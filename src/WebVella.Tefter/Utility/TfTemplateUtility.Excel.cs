using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebVella.Tefter.Utility;
internal static partial class TfTemplateUtility
{
	public static void ProcessExcelTemplate(this TfExcelTemplateProcessResult result, TfDataTable dataSource)
	{
		if (dataSource is null) throw new Exception("No datasource provided!");
		if (result.TemplateWorkbook is null) throw new Exception("No template provided!");
		result.ProcessExcelTemplatePlacement(dataSource);
	}

	public static void ProcessExcelTemplatePlacement(this TfExcelTemplateProcessResult result, TfDataTable dataSource)
	{
		foreach (IXLWorksheet tempWs in result.TemplateWorkbook.Worksheets)
		{
			var resultWs = result.ResultWorkbook.AddWorksheet();
			var resultCurrentRow = 1;
			var resultCurrentColumn = 1;

			foreach (IXLRow tempRow in tempWs.RowsUsed())
			{
				foreach (IXLCell tempCell in tempRow.CellsUsed(XLCellsUsedOptions.All))
				{
					var mergedRange = tempCell.MergedRange();
					//Process only the first cell in the merge
					if (mergedRange is not null)
					{
						var margeRangeCells = mergedRange.CellsUsed(XLCellsUsedOptions.All);
						if (margeRangeCells.First().Address.ToString() != tempCell.Address.ToString())
						{
							continue;
						}
					}

					var resultCell = resultWs.Cell(resultCurrentRow, resultCurrentColumn);
					CopyCellProperties(tempCell, resultCell);
					var tagResults = ProcessTemplateTag(tempCell.Value.ToString(), dataSource);
					resultCell.Value = tagResults[0].Value;
				}
			}
		}
	}

	public static void ProcessExcelTemplateDependencies(this TfExcelTemplateProcessResult result, TfDataTable dataSource)
	{

	}

	public static void ProcessExcelTemplateData(this TfExcelTemplateProcessResult result, TfDataTable dataSource)
	{
		foreach (IXLWorksheet tempWs in result.TemplateWorkbook.Worksheets)
		{
			var resultWs = result.ResultWorkbook.AddWorksheet();
			if (dataSource.Rows.Count > 0)
			{
				var processedWsNameResult = ProcessTemplateTag(tempWs.Name, new TfDataTable(dataSource, 0));
				resultWs.Name = processedWsNameResult[0].Value;
			}


			foreach (IXLRow tempRow in tempWs.RowsUsed())
			{
				foreach (IXLColumn tempColumn in tempWs.ColumnsUsed())
				{

				}
			}
		}
	}

	private static void CopyCellProperties(IXLCell origin, IXLCell destination)
	{
		destination.ShowPhonetic = origin.ShowPhonetic;
		destination.FormulaA1 = origin.FormulaA1;
		destination.FormulaR1C1 = origin.FormulaR1C1;
		if(origin.FormulaReference is not null)
			destination.FormulaReference = origin.FormulaReference;
		destination.ShareString = origin.ShareString;
		destination.Style = origin.Style;
		destination.Active = origin.Active;
	}
}
