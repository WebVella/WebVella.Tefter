namespace WebVella.Tefter.Tests;
using ClosedXML.Excel;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using WebVella.Tefter.Models;
using WebVella.Tefter.Utility;

public partial class ExcelTemplatesTests : TemplateTagTestsBase
{
	protected static readonly AsyncLock locker = new AsyncLock();

	#region << Arguments >>
	[Fact]
	public async Task ShouldHaveTemplate()
	{
		using (await locker.LockAsync())
		{
			var result = new TfExcelTemplateProcessResult();
			Func<bool> action = () => { result.ProcessExcelTemplate(SampleData); return true; };
			action.Should().Throw<Exception>("No template provided!");
		}
	}

	[Fact]
	public async Task ShouldHaveDataSource()
	{
		using (await locker.LockAsync())
		{
			var result = new TfExcelTemplateProcessResult();
			result.TemplateWorkbook = _loadWorkbook("TemplatePlacement1.xlsx");
			Func<bool> action = () => { result.ProcessExcelTemplate(null); return true; };
			action.Should().Throw<Exception>("No datasource provided!");
		}
	}
	#endregion

	#region << Placement >>
	[Fact]
	public async Task Placement1_Styles()
	{
		// {{position}} | {{name}}
		using (await locker.LockAsync())
		{
			//Given
			var fileName = "TemplatePlacement1.xlsx";
			var result = new TfExcelTemplateProcessResult();
			result.TemplateWorkbook = _loadWorkbook(fileName);
			//When
			result.ProcessExcelTemplatePlacement(SampleData);
			//Assert
			_generalResultChecks(result);

			result.TemplateWorkbook.Worksheets.Count.Should().Be(1);
			result.ResultWorkbook.Worksheets.Count.Should().Be(1);

			result.Contexts.Count.Should().Be(2);

			_checkRangeDimensions(result.Contexts[0].TemplateRange, 1, 1, 1, 1);
			_checkRangeDimensions(result.Contexts[0].ResultRange, 1, 1, 1, 1);
			_checkRangeDimensions(result.Contexts[1].TemplateRange, 1, 2, 1, 3);
			_checkRangeDimensions(result.Contexts[1].ResultRange, 1, 2, 1, 3);
			_checkCellPropertiesCopy(result);
			var tempWs = result.TemplateWorkbook.Worksheets.First();
			var resultWs = result.ResultWorkbook.Worksheets.First();
			_compareRowProperties(tempWs.Row(1), resultWs.Row(1));
			_compareColumnProperties(tempWs.Column(1), resultWs.Column(1));
			_compareColumnProperties(tempWs.Column(2), resultWs.Column(2));


			_saveWorkbook(result.ResultWorkbook, fileName);
		}
	}

	[Fact]
	public async Task Placement2_MultiWorksheet()
	{
		// {{position}} | {{name}}
		using (await locker.LockAsync())
		{
			//Given
			var fileName = "TemplatePlacement2.xlsx";
			var result = new TfExcelTemplateProcessResult();
			result.TemplateWorkbook = _loadWorkbook(fileName);
			//When
			result.ProcessExcelTemplatePlacement(SampleData);
			//Assert
			_generalResultChecks(result);

			result.TemplateWorkbook.Worksheets.Count.Should().Be(2);
			result.ResultWorkbook.Worksheets.Count.Should().Be(2);

			result.Contexts.Count.Should().Be(3);


			_saveWorkbook(result.ResultWorkbook, fileName);
		}
	}

	[Fact]
	public async Task Placement3_Data()
	{
		// {{position}} | {{name}}
		using (await locker.LockAsync())
		{
			//Given
			var fileName = "TemplatePlacement3.xlsx";
			var result = new TfExcelTemplateProcessResult();
			result.TemplateWorkbook = _loadWorkbook(fileName);
			//When
			result.ProcessExcelTemplatePlacement(SampleData);
			//Assert
			_generalResultChecks(result);
			result.TemplateWorkbook.Worksheets.Count.Should().Be(1);
			result.ResultWorkbook.Worksheets.Count.Should().Be(1);
			result.Contexts.Count.Should().Be(6);
			_checkRangeDimensions(result.Contexts[0].TemplateRange, 1, 1, 1, 1);
			_checkRangeDimensions(result.Contexts[0].ResultRange, 1, 1, 5, 1);

			_checkRangeDimensions(result.Contexts[1].TemplateRange, 1, 2, 1, 2);
			_checkRangeDimensions(result.Contexts[1].ResultRange, 1, 2, 5, 2);

			_checkRangeDimensions(result.Contexts[2].TemplateRange, 1, 3, 1, 3);
			_checkRangeDimensions(result.Contexts[2].ResultRange, 1, 3, 1, 3);

			_checkRangeDimensions(result.Contexts[3].TemplateRange, 1, 4, 1, 4);
			_checkRangeDimensions(result.Contexts[3].ResultRange, 1, 4, 5, 4);

			_checkRangeDimensions(result.Contexts[4].TemplateRange, 1, 5, 1, 5);
			_checkRangeDimensions(result.Contexts[4].ResultRange, 1, 5, 1, 5);

			_checkRangeDimensions(result.Contexts[5].TemplateRange, 1, 6, 1, 6);
			_checkRangeDimensions(result.Contexts[5].ResultRange, 1, 6, 1, 6);
			_saveWorkbook(result.ResultWorkbook, fileName);
		}
	}

	[Fact]
	public async Task Placement4_WrongColumnName()
	{
		// {{position}} | {{name}}
		using (await locker.LockAsync())
		{
			//Given
			var fileName = "TemplatePlacement4.xlsx";
			var result = new TfExcelTemplateProcessResult();
			result.TemplateWorkbook = _loadWorkbook(fileName);
			//When
			result.ProcessExcelTemplatePlacement(SampleData);
			//Assert
			_generalResultChecks(result);

			result.TemplateWorkbook.Worksheets.Count.Should().Be(1);
			result.ResultWorkbook.Worksheets.Count.Should().Be(1);

			result.Contexts.Count.Should().Be(1);
			_checkRangeDimensions(result.Contexts[0].TemplateRange, 1, 1, 1, 1);
			_checkRangeDimensions(result.Contexts[0].ResultRange, 1, 1, 5, 1);

			_saveWorkbook(result.ResultWorkbook, fileName);
		}
	}

	[Fact]
	public async Task Placement6_Data_Multiline()
	{
		// {{position}} | {{name}}
		using (await locker.LockAsync())
		{
			//Given
			var fileName = "TemplatePlacement5.xlsx";
			var result = new TfExcelTemplateProcessResult();
			result.TemplateWorkbook = _loadWorkbook(fileName);
			//When
			result.ProcessExcelTemplatePlacement(SampleData);
			//Assert
			_generalResultChecks(result);

			result.TemplateWorkbook.Worksheets.Count.Should().Be(1);
			result.ResultWorkbook.Worksheets.Count.Should().Be(1);

			result.Contexts.Count.Should().Be(2);

			_checkRangeDimensions(result.Contexts[0].TemplateRange, 1, 1, 1, 1);
			_checkRangeDimensions(result.Contexts[0].ResultRange, 1, 1, 5, 1);
			_checkRangeDimensions(result.Contexts[1].TemplateRange, 2, 1, 2, 1);
			_checkRangeDimensions(result.Contexts[1].ResultRange, 6, 1, 10, 1);
			_saveWorkbook(result.ResultWorkbook, fileName);
		}
	}

	[Fact]
	public async Task Placement6_Data_HorizontalFlow()
	{
		// {{position}} | {{name}}
		using (await locker.LockAsync())
		{
			//Given
			var fileName = "TemplatePlacement6.xlsx";
			var result = new TfExcelTemplateProcessResult();
			result.TemplateWorkbook = _loadWorkbook(fileName);
			//When
			result.ProcessExcelTemplatePlacement(SampleData);
			//Assert
			_generalResultChecks(result);

			result.TemplateWorkbook.Worksheets.Count.Should().Be(1);
			result.ResultWorkbook.Worksheets.Count.Should().Be(1);

			result.Contexts.Count.Should().Be(6);

			_checkRangeDimensions(result.Contexts[0].TemplateRange, 1, 1, 1, 1);
			_checkRangeDimensions(result.Contexts[0].ResultRange, 1, 1, 1, 5);
			_checkRangeDimensions(result.Contexts[1].TemplateRange, 1, 2, 1, 2);
			_checkRangeDimensions(result.Contexts[1].ResultRange, 1, 6, 1, 6);
			_checkRangeDimensions(result.Contexts[2].TemplateRange, 1, 3, 1, 3);
			_checkRangeDimensions(result.Contexts[2].ResultRange, 1, 7, 5, 7);
			_checkRangeDimensions(result.Contexts[3].TemplateRange, 2, 1, 2, 1);
			_checkRangeDimensions(result.Contexts[3].ResultRange, 6, 1, 6, 5);
			_checkRangeDimensions(result.Contexts[4].TemplateRange, 2, 2, 2, 2);
			_checkRangeDimensions(result.Contexts[4].ResultRange, 6, 6, 6, 6);
			_checkRangeDimensions(result.Contexts[5].TemplateRange, 2, 3, 2, 3);
			_checkRangeDimensions(result.Contexts[5].ResultRange, 6, 7, 6, 7);
			_saveWorkbook(result.ResultWorkbook, fileName);
		}
	}

	#endregion

	#region << Excel Function >>
	//[Fact]
	//public async Task ExcelFunction1_RelativeRangeConversion()
	//{
	//	// {{position}} | {{name}}
	//	using (await locker.LockAsync())
	//	{
	//		//Given
	//		var fileName = "TemplateExcelFunction1.xlsx";
	//		var result = new TfExcelTemplateProcessResult();
	//		result.TemplateWorkbook = _loadWorkbook(fileName);
	//		//When
	//		result.ProcessExcelTemplatePlacement(SampleData);
	//		//Assert
	//		_generalResultChecks(result);



	//		_saveWorkbook(result.ResultWorkbook, fileName);
	//	}
	//}

	#endregion

	#region << DATA >>
	[Fact]
	public async Task ExcelData1_RepeatAndWorksheetName()
	{
		// {{position}} | {{name}}
		using (await locker.LockAsync())
		{
			//Given
			var fileName = "TemplateData1.xlsx";
			var result = new TfExcelTemplateProcessResult();
			result.TemplateWorkbook = _loadWorkbook(fileName);
			//When
			result.ProcessExcelTemplate(SampleData);
			//Assert
			_generalResultChecks(result);
			var ws = result.ResultWorkbook.Worksheets.First();
			ws.Name.Should().Be((string)SampleData.Rows[0]["name"]);

			for (int i = 0; i < SampleData.Rows.Count; i++)
			{
				var cellValueString = ws.Cell(i + 1, 1).Value.ToString();
				var rowValueString = SampleData.Rows[i]["position"]?.ToString();
				cellValueString.Should().Be(rowValueString);
			}


			_saveWorkbook(result.ResultWorkbook, fileName);
		}
	}

	[Fact]
	public async Task ExcelData1_RepeatAndWorksheetName_Horizontal()
	{
		// {{position}} | {{name}}
		using (await locker.LockAsync())
		{
			//Given
			var fileName = "TemplateData2.xlsx";
			var result = new TfExcelTemplateProcessResult();
			result.TemplateWorkbook = _loadWorkbook(fileName);
			//When
			result.ProcessExcelTemplate(SampleData);
			//Assert
			_generalResultChecks(result);
			var ws = result.ResultWorkbook.Worksheets.First();
			ws.Name.Should().Be((string)SampleData.Rows[0]["name"]);

			for (int i = 0; i < SampleData.Rows.Count; i++)
			{
				var cellValueString = ws.Cell(1, i + 1).Value.ToString();
				var rowValueString = SampleData.Rows[i]["position"]?.ToString();
				cellValueString.Should().Be(rowValueString);
			}


			_saveWorkbook(result.ResultWorkbook, fileName);
		}
	}

	[Fact]
	public async Task ExcelData1_RepeatAndWorksheetName_Mixed()
	{
		// {{position}} | {{name}}
		using (await locker.LockAsync())
		{
			//Given
			var fileName = "TemplateData3.xlsx";
			var result = new TfExcelTemplateProcessResult();
			result.TemplateWorkbook = _loadWorkbook(fileName);
			//When
			result.ProcessExcelTemplate(SampleData);
			//Assert
			_generalResultChecks(result);
			var ws = result.ResultWorkbook.Worksheets.First();
			ws.Name.Should().Be((string)SampleData.Rows[0]["name"]);

			for (int i = 0; i < SampleData.Rows.Count; i++)
			{
				var cellValueString = ws.Cell(1, i + 1).Value.ToString();
				var rowValueString = SampleData.Rows[i]["position"]?.ToString();
				cellValueString.Should().Be(rowValueString);
			}
			for (int i = 0; i < SampleData.Rows.Count; i++)
			{
				var cellValueString = ws.Cell(i + 2, 1).Value.ToString();
				var rowValueString = SampleData.Rows[i]["name"]?.ToString();
				cellValueString.Should().Be(rowValueString);
			}


			_saveWorkbook(result.ResultWorkbook, fileName);
		}
	}

	[Fact]
	public async Task ExcelData1_RepeatAndWorksheetName_HighLoad()
	{
		var ds = new TfDataTable();
		var colCount = 50;
		//var rowCount = 5;
		var rowCount = 500;
		var wb = new XLWorkbook();
		var ws = wb.Worksheets.Add();
		//Data
		{
			for (int i = 0; i < colCount; i++)
			{
				var colName = $"col{i}";
				ds.Columns.Add(new TfDataColumn(ds, colName, TfDatabaseColumnType.Text, false, false, false));
				ws.Cell(1, i + 1).Value = "{{" + colName + "}}";
			}
			for (int i = 0; i < rowCount; i++)
			{
				var position = i + 1;
				var dsrow = new TfDataRow(ds, new object[ds.Columns.Count]);
				for (int j = 0; j < colCount; j++)
				{
					dsrow[$"col{j}"] = $"cell-{i}-{j}";
				}
				ds.Rows.Add(dsrow);
			}
		}

		// {{position}} | {{name}}
		using (await locker.LockAsync())
		{
			//Given
			var fileName = "TemplateDataGenerated.xlsx";
			var result = new TfExcelTemplateProcessResult();
			var log = new List<TimeSpan>();
			result.TemplateWorkbook = wb;
			//When
			var sw = new Stopwatch();
			sw.Start();
			result.ProcessExcelTemplatePlacement(ds);
			log.Add(sw.Elapsed);
			sw.Restart();
			result.ProcessExcelTemplateDependencies(ds);
			log.Add(sw.Elapsed);
			sw.Restart();
			result.ProcessExcelTemplateData(ds);
			log.Add(sw.Elapsed);
			sw.Stop();
			Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(log));
			//Assert
			_generalResultChecks(result);

			_saveWorkbook(result.ResultWorkbook, fileName);
		}
	}


	#endregion

	#region << Private >>
	private void _generalResultChecks(TfExcelTemplateProcessResult result)
	{
		result.Should().NotBeNull();
		result.TemplateWorkbook.Should().NotBeNull();
		result.ResultWorkbook.Should().NotBeNull();
		result.Contexts.Should().NotBeNull();
		result.TemplateWorkbook.Worksheets.Should().NotBeNull();
		result.TemplateWorkbook.Worksheets.Count.Should().BeGreaterThan(0);
		result.ResultWorkbook.Worksheets.Should().NotBeNull();
		result.ResultWorkbook.Worksheets.Count.Should().BeGreaterThan(0);
	}

	private void _checkRangeDimensions(IXLRange range, int startRowNumber, int startColumnNumber, int lastRowNumber, int lastColumnNumber)
	{
		range.Should().NotBeNull();
		range.RangeAddress.Should().NotBeNull();
		range.RangeAddress.FirstAddress.Should().NotBeNull();
		range.RangeAddress.LastAddress.Should().NotBeNull();
		range.RangeAddress.FirstAddress.RowNumber.Should().Be(startRowNumber);
		range.RangeAddress.FirstAddress.ColumnNumber.Should().Be(startColumnNumber);
		range.RangeAddress.LastAddress.RowNumber.Should().Be(lastRowNumber);
		range.RangeAddress.LastAddress.ColumnNumber.Should().Be(lastColumnNumber);
	}

	private void _checkCellPropertiesCopy(TfExcelTemplateProcessResult result)
	{
		foreach (var context in result.Contexts)
		{
			var firstTemplateCell = context.TemplateRange.Cell(1, 1);
			var firstResultCell = context.ResultRange.Cell(1, 1);
			firstTemplateCell.Should().NotBeNull();
			firstResultCell.Should().NotBeNull();
			firstResultCell.ShowPhonetic.Should().Be(firstTemplateCell.ShowPhonetic);
			firstResultCell.FormulaA1.Should().Be(firstTemplateCell.FormulaA1);
			firstResultCell.FormulaR1C1.Should().Be(firstTemplateCell.FormulaR1C1);
			firstResultCell.ShareString.Should().Be(firstTemplateCell.ShareString);
			firstResultCell.Active.Should().Be(firstTemplateCell.Active);
			_compareStyle(firstTemplateCell, firstResultCell);
		}
	}
	private void _compareStyle(IXLCell template, IXLCell result)
	{
		if (template.Style is null)
		{
			result.Style.Should().BeNull();
		}
		else
		{
			result.Style.Should().NotBeNull();
			_compareStyleAlignment(template.Style.Alignment, result.Style.Alignment);
			_compareStyleBorder(template.Style.Border, result.Style.Border);
			_compareStyleFormat(template.Style.DateFormat, result.Style.DateFormat);
			_compareStyleFill(template.Style.Fill, result.Style.Fill);
			_compareStyleFont(template.Style.Font, result.Style.Font);
			result.Style.IncludeQuotePrefix.Should().Be(template.Style.IncludeQuotePrefix);
			_compareStyleFormat(template.Style.NumberFormat, result.Style.NumberFormat);
			_compareStyleProtection(template.Style.Protection, result.Style.Protection);
		}
	}

	private void _compareStyleAlignment(IXLAlignment template, IXLAlignment result)
	{
		result.TopToBottom.Should().Be(template.TopToBottom);
		result.TextRotation.Should().Be(template.TextRotation);
		result.ShrinkToFit.Should().Be(template.ShrinkToFit);
		result.RelativeIndent.Should().Be(template.RelativeIndent);
		result.ReadingOrder.Should().Be(template.ReadingOrder);
		result.JustifyLastLine.Should().Be(template.JustifyLastLine);
		result.Indent.Should().Be(template.Indent);
		result.Vertical.Should().Be(template.Vertical);
		result.Horizontal.Should().Be(template.Horizontal);
		result.WrapText.Should().Be(template.WrapText);
	}
	private void _compareStyleBorder(IXLBorder template, IXLBorder result)
	{
		_compareStyleColor(template.DiagonalBorderColor, result.DiagonalBorderColor);
		result.LeftBorder.Should().Be(template.LeftBorder);
		_compareStyleColor(template.LeftBorderColor, result.LeftBorderColor);
		result.DiagonalBorder.Should().Be(template.DiagonalBorder);
		result.RightBorder.Should().Be(template.RightBorder);
		result.TopBorder.Should().Be(template.TopBorder);
		_compareStyleColor(template.TopBorderColor, result.TopBorderColor);
		result.BottomBorder.Should().Be(template.BottomBorder);
		_compareStyleColor(template.BottomBorderColor, result.BottomBorderColor);
		result.DiagonalUp.Should().Be(template.DiagonalUp);
		result.DiagonalDown.Should().Be(template.DiagonalDown);
		_compareStyleColor(template.RightBorderColor, result.RightBorderColor);
	}
	private void _compareStyleFormat(IXLNumberFormat template, IXLNumberFormat result)
	{
		result.NumberFormatId.Should().Be(template.NumberFormatId);
		result.Format.Should().Be(template.Format);
	}
	private void _compareStyleFill(IXLFill template, IXLFill result)
	{
		_compareStyleColor(template.BackgroundColor, result.BackgroundColor);
		_compareStyleColor(template.PatternColor, result.PatternColor);
		result.PatternType.Should().Be(template.PatternType);
	}
	private void _compareStyleFont(IXLFont template, IXLFont result)
	{
		result.Bold.Should().Be(template.Bold);
		result.Italic.Should().Be(template.Italic);
		result.Underline.Should().Be(template.Underline);
		result.Strikethrough.Should().Be(template.Strikethrough);
		result.VerticalAlignment.Should().Be(template.VerticalAlignment);
		result.Shadow.Should().Be(template.Shadow);
		result.FontSize.Should().Be(template.FontSize);
		_compareStyleColor(template.FontColor, result.FontColor);
		result.FontName.Should().Be(template.FontName);
		result.FontFamilyNumbering.Should().Be(template.FontFamilyNumbering);
		result.FontCharSet.Should().Be(template.FontCharSet);
		result.FontScheme.Should().Be(template.FontScheme);
	}
	private void _compareStyleProtection(IXLProtection template, IXLProtection result)
	{
		result.Locked.Should().Be(template.Locked);
		result.Hidden.Should().Be(template.Hidden);
	}
	private void _compareStyleColor(XLColor template, XLColor result)
	{
		if (template is null)
		{
			result.Should().BeNull();
		}
		else
		{
			result.Should().NotBeNull();
			result.ToString().Should().Be(template.ToString());
		}
	}

	private void _compareRowProperties(IXLRow template, IXLRow result)
	{
		result.OutlineLevel.Should().Be(template.OutlineLevel);
		result.Height.Should().Be(template.Height);
	}
	private void _compareColumnProperties(IXLColumn template, IXLColumn result)
	{
		result.OutlineLevel.Should().Be(template.OutlineLevel);
		result.Width.Should().Be(template.Width);
	}

	private XLWorkbook _loadWorkbook(string fileName)
	{
		var path = Path.Combine(Environment.CurrentDirectory, $"Files\\{fileName}");
		var fi = new FileInfo(path);
		fi.Exists.Should().BeTrue();
		var templateWB = new XLWorkbook(path);
		templateWB.Should().NotBeNull();

		return templateWB;
	}
	private void _saveWorkbook(XLWorkbook workbook, string fileName)
	{
		DirectoryInfo debugFolder = Directory.GetParent(Environment.CurrentDirectory);
		var projectFolder = debugFolder.Parent.Parent;

		var path = Path.Combine(projectFolder.FullName, $"FileResults\\result-{fileName}");
		workbook.SaveAs(path);
	}
	#endregion
}
