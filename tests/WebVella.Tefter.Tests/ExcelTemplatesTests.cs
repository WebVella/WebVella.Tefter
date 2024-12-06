namespace WebVella.Tefter.Tests;
using ClosedXML.Excel;

public partial class ExcelTemplatesTests
{
	protected static readonly AsyncLock locker = new AsyncLock();

	static ExcelTemplatesTests()
	{

	}

	[Fact]
	public async Task LoadExcelFile()
	{
		using (await locker.LockAsync())
		{
			var path = Path.Combine(Environment.CurrentDirectory, "Files\\template1.xlsx");
			var di = new DirectoryInfo(path);
			var fi = new FileInfo(path);
			var workbook = new XLWorkbook(path);
			foreach (var ws in workbook.Worksheets)
			{
				var rowCount = ws.RowsUsed().Count();
				var colCount = ws.ColumnsUsed().Count();
				for (var row = 1; row <= rowCount; row++)
				{
					for (var col = 1; col <= colCount; col++)
					{
						var cell = ws.Cell(row, col);
						var cell2 = cell.CopyTo(ws.Cell(10, 3));
					}
				}
			}
		}
	}
}
