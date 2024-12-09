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
			var tempWsUsedRowsCount = tempWs.LastRowUsed().RowNumber();
			var tempWsUsedColumnsCount = tempWs.LastColumnUsed().ColumnNumber();
			var resultWs = result.ResultWorkbook.AddWorksheet();
			var resultCurrentRow = 1;
			for (var rowIndex = 0; rowIndex < tempWsUsedRowsCount; rowIndex++)
			{
				IXLRow tempRow = tempWs.Row(rowIndex + 1);
				var resultCurrentColumn = 1;
				var tempRowResultRowsUsed = 0;

				for (var colIndex = 0; colIndex < tempWsUsedColumnsCount; colIndex++)
				{
					IXLCell tempCell = tempWs.Cell(tempRow.RowNumber(), (colIndex + 1));
					var tempRangeStartRowNumber = resultCurrentRow;
					var tempRangeStartColumnNumber = resultCurrentColumn;
					var tempRangeEndRowNumber = resultCurrentRow;
					var tempRangeEndColumnNumber = resultCurrentColumn;
					var templateMergedRowCount = 1;
					var templateMergedColumnCount = 1;
					var mergedRange = tempCell.MergedRange();
					List<IXLRange> resultRangeSlots = new List<IXLRange>();
					//Process only the first cell in the merge
					if (mergedRange is not null)
					{
						var margeRangeCells = mergedRange.CellsUsed(XLCellsUsedOptions.All);
						if (margeRangeCells.First().Address.ToString() != tempCell.Address.ToString())
						{
							continue;
						}
						templateMergedRowCount = mergedRange.RowCount();
						templateMergedColumnCount = mergedRange.ColumnCount();
					}

					tempRangeEndRowNumber = tempRangeEndRowNumber + templateMergedRowCount - 1;
					tempRangeEndColumnNumber = tempRangeEndColumnNumber + templateMergedColumnCount - 1;

					var tagProcessResult = ProcessTemplateTag(tempCell.Value.ToString(), dataSource);
					if (tagProcessResult[0].Tags.Count > 0)
					{
						var isFlowHorizontal = IsFlowHorizontal(tagProcessResult[0]);
						for (var i = 0; i < tagProcessResult.Count; i++)
						{
							var dsRowStarRowNumber = tempRangeStartRowNumber;
							var dsRowStartColumnNumber = tempRangeStartColumnNumber;

							var dsRowEndRowNumber = tempRangeEndRowNumber;
							var dsRowEndColumnNumber = tempRangeEndColumnNumber;

							//if vertical - increase rows with the merge
							if (!isFlowHorizontal)
							{
								dsRowStarRowNumber = tempRangeStartRowNumber + (templateMergedRowCount * i);
								dsRowEndRowNumber = tempRangeEndRowNumber + (templateMergedRowCount * i);
							}
							//if hirozontal - increase columns with the merge
							else
							{
								dsRowStartColumnNumber = tempRangeStartColumnNumber + (templateMergedColumnCount * i);
								dsRowEndColumnNumber = tempRangeEndColumnNumber + (templateMergedColumnCount * i);
							}


							IXLRange dsRowResultRange = resultWs.Range(
								firstCellRow: dsRowStarRowNumber,
								firstCellColumn: dsRowStartColumnNumber,
								lastCellRow: dsRowEndRowNumber,
								lastCellColumn: dsRowEndColumnNumber);
							dsRowResultRange.Merge();
							CopyCellProperties(tempCell, dsRowResultRange);
							var templateRange = mergedRange is not null ? mergedRange : tempCell.AsRange();
							CopyColumnsProperties(templateRange, dsRowResultRange, tempWs, resultWs);
							resultRangeSlots.Add(dsRowResultRange);
						}
						if (!isFlowHorizontal)
						{
							tempRangeEndRowNumber = tempRangeStartRowNumber + (tagProcessResult.Count * templateMergedRowCount) - 1;
						}
						else
						{
							tempRangeEndColumnNumber = tempRangeStartColumnNumber + (tagProcessResult.Count * templateMergedColumnCount) - 1;
						}
					}
					else
					{
						tempRangeEndRowNumber = resultCurrentRow + templateMergedRowCount - 1;
						tempRangeEndColumnNumber = resultCurrentColumn + templateMergedColumnCount - 1;
						IXLRange dsRowResultRange = resultWs.Range(
								firstCellRow: tempRangeStartRowNumber,
								firstCellColumn: tempRangeStartColumnNumber,
								lastCellRow: tempRangeEndRowNumber,
								lastCellColumn: tempRangeEndColumnNumber);
						dsRowResultRange.Merge();
						dsRowResultRange.Value = tempCell.Value;

						CopyCellProperties(tempCell, dsRowResultRange);

						var templateRange = mergedRange is not null ? mergedRange : tempCell.AsRange();
						CopyColumnsProperties(templateRange, dsRowResultRange, tempWs, resultWs);

						resultRangeSlots.Add(dsRowResultRange);
					}

					IXLRange resultRange = resultWs.Range(
							firstCellRow: tempRangeStartRowNumber,
							firstCellColumn: tempRangeStartColumnNumber,
							lastCellRow: tempRangeEndRowNumber,
							lastCellColumn: tempRangeEndColumnNumber);

					//Copy column styles


					//Create context
					var context = new TfExcelTemplateContext()
					{
						Id = Guid.NewGuid(),
						Tags = tagProcessResult.Count == 0 ? new() : tagProcessResult[0].Tags,
						TemplateWorksheet = tempWs.Worksheet,
						ResultWorksheet = resultWs.Worksheet,
						TemplateRange = mergedRange is not null ? mergedRange : tempCell.AsRange(),
						ResultRange = resultRange,
						ResultRangeSlots = resultRangeSlots,
					};
					result.Contexts.Add(context);

					if (tempRowResultRowsUsed < resultRange.RowCount())
						tempRowResultRowsUsed = resultRange.RowCount();

					resultCurrentColumn = resultCurrentColumn + resultRange.ColumnCount();
				}
				//Copy Row Styles
				for (var i = 0; i < tempRowResultRowsUsed; i++)
				{
					var resultRow = resultWs.Row(resultCurrentRow + i);
					CopyRowProperties(tempRow, resultRow);
				}

				resultCurrentRow += tempRowResultRowsUsed;
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
				resultWs.Name = processedWsNameResult[0].ValueString;
			}


			foreach (IXLRow tempRow in tempWs.RowsUsed())
			{
				foreach (IXLColumn tempColumn in tempWs.ColumnsUsed())
				{

				}
			}
		}
	}

	private static void CopyCellProperties(IXLCell template, IXLRange result)
	{
		foreach (IXLCell destination in result.Cells())
		{
			destination.ShowPhonetic = template.ShowPhonetic;
			destination.FormulaA1 = template.FormulaA1;
			destination.FormulaR1C1 = template.FormulaR1C1;
			if (template.FormulaReference is not null)
				destination.FormulaReference = template.FormulaReference;
			destination.ShareString = template.ShareString;
			destination.Style = template.Style;
			destination.Active = template.Active;
		}
	}

	private static void CopyRowProperties(IXLRow origin, IXLRow result)
	{
		result.OutlineLevel = origin.OutlineLevel;
		result.Height = origin.Height;
	}
	private static void CopyColumnsProperties(IXLRange templateRange, IXLRange resultRange, IXLWorksheet templateWs, IXLWorksheet resultWs)
	{
		var dsRowResultColumnsCount = resultRange.ColumnCount();
		for (var i = 0; i < templateRange.ColumnCount(); i++)
		{
			if (dsRowResultColumnsCount - 1 < i) continue;
			var tempColumn = templateWs.Column(templateRange.RangeAddress.FirstAddress.ColumnNumber + i);
			var resultColumn = resultWs.Column(resultRange.RangeAddress.FirstAddress.ColumnNumber + i);
			resultColumn.OutlineLevel = tempColumn.OutlineLevel;
			resultColumn.Width = tempColumn.Width;
		}
	}

	private static bool IsFlowHorizontal(TfTemplateTagResult tagResult)
	{
		var allTagsHorizontal = true;
		foreach (var tag in tagResult.Tags)
		{
			if (tag.ParamGroups.Count == 0)
			{
				allTagsHorizontal = false;
				break;
			}
			var paramGroup = tag.ParamGroups[0];
			if (paramGroup.Parameters.Count == 0)
			{
				allTagsHorizontal = false;
				break;
			}
			foreach (var parameter in paramGroup.Parameters)
			{
				if (parameter.Type.FullName == typeof(TfTemplateTagDataFlowParameter).FullName)
				{
					var paramObj = (TfTemplateTagDataFlowParameter)parameter;
					if (paramObj.Value == TfTemplateTagDataFlow.Vertical)
					{
						allTagsHorizontal = false;
						break;
					}
				}
			}
		}
		return allTagsHorizontal;
	}
}
