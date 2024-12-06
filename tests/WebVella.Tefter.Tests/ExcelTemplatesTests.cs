namespace WebVella.Tefter.Tests;
using ClosedXML.Excel;
using WebVella.Tefter.Utility;

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
			var ds = new TfDataTable();
			ds.Columns.Add(new TfDataColumn(ds, "position", TfDatabaseColumnType.Integer, false, false, false));
			ds.Columns.Add(new TfDataColumn(ds, "name", TfDatabaseColumnType.Text, false, false, false));
			ds.Columns.Add(new TfDataColumn(ds, "price", TfDatabaseColumnType.Number, false, false, false));
			var values = new List<Tuple<int, string, decimal>>(){
				new Tuple<int, string, decimal>(1,"item1",(decimal)3.26),
				new Tuple<int, string, decimal>(2,"item2",(decimal)5.6),
				new Tuple<int, string, decimal>(3,"item3",(decimal)4.34),
				new Tuple<int, string, decimal>(4,"item4",(decimal)86.36),
				new Tuple<int, string, decimal>(5,"item5",(decimal)55.55),
			};
			foreach (var item in values)
			{
				var dsrow = new TfDataRow(ds, new object[ds.Columns.Count]);
				dsrow["position"] = item.Item1;
				dsrow["name"] = item.Item2;
				dsrow["price"] = item.Item3;
				ds.Rows.Add(dsrow);
			}
			var path = Path.Combine(Environment.CurrentDirectory, "Files\\template1.xlsx");
			var di = new DirectoryInfo(path);
			var fi = new FileInfo(path);
			var templateWB = new XLWorkbook(path);
			var resultWB = new XLWorkbook();
			foreach (var ws in templateWB.Worksheets)
			{
				var resultWsName = ws.Name;
				var resultWsNameTags = TfTemplateUtility.GetTagsFromTemplate(resultWsName);
				if (resultWsNameTags.Count > 0)
				{

				}
				resultWB.AddWorksheet(ws);


				var rowCount = ws.RowsUsed().Count();
				var colCount = ws.ColumnsUsed().Count();
				for (var row = 1; row <= rowCount; row++)
				{
					for (var col = 1; col <= colCount; col++)
					{
						var cell = ws.Cell(row, col);
						var tags = TfTemplateUtility.GetTagsFromTemplate(cell.Value.ToString());
						if (tags.Count == 0) continue;


					}
				}
			}
		}
	}
}
