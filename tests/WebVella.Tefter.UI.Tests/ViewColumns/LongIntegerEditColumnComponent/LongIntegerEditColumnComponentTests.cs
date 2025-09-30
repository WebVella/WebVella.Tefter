namespace WebVella.Tefter.UI.Tests.ViewColumns;

public class LongIntegerEditColumnComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var columnName = "name";
		var dt = new TfDataTable();
		dt.Columns.Add(new TfDataColumn(dt,columnName,Database.TfDatabaseColumnType.Text,true,false,false,false, false));
		var Context = GetTestContext();
		// Act
		var cut = Context.RenderComponent<TucLongIntegerEditColumnComponent>(args => args
		.Add(x => x.RegionContext, new TfSpaceViewColumnScreenRegionContext(ViewData)
		{
			DataTable = dt,
			DataMapping = new Dictionary<string, string>{ {"Value",columnName} }
		})
		);

		// Assert
		cut.Find("fluent-number-field");

		Context.DisposeComponents();
	}

}
