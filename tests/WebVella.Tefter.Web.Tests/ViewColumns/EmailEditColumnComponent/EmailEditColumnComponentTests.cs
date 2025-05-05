namespace WebVella.Tefter.Web.Tests.ViewColumns;

public class EmailEditColumnComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var columnName = "name";
		var dt = new TfDataTable();
		dt.Columns.Add(new TfDataColumn(dt,columnName,Database.TfDatabaseColumnType.Text,true,false,false,false));
		var Context = GetTestContext();
		// Act
		var cut = Context.RenderComponent<TfEmailEditColumnComponent>(args => args
		.Add(x => x.RegionContext, new TfSpaceViewColumnScreenRegionContext()
		{
			DataTable = dt,
			DataMapping = new Dictionary<string, string>{ {"Value",columnName} }
		})
		);

		// Assert
		cut.Find("fluent-text-field");

		Context.DisposeComponents();
	}

}
