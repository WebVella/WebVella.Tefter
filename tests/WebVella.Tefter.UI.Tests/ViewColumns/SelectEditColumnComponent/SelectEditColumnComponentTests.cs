namespace WebVella.Tefter.UI.Tests.ViewColumns;

public class SelectEditColumnComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var columnName = "name";
		var dt = new TfDataTable();
		dt.Columns.Add(new TfDataColumn(dt,columnName,Database.TfDatabaseColumnType.Text,true,false,false,false, false));
		var Context = GetTestContext();
		Context.RenderComponent<FluentMenuProvider>();
		// Act
		var cut = Context.RenderComponent<TfSelectEditColumnComponent>(args => args
		.Add(x => x.RegionContext, new TfSpaceViewColumnScreenRegionContext()
		{
			DataTable = dt,
			DataMapping = new Dictionary<string, string>{ {"Value",columnName} }
		})
		);

		// Assert
		cut.Find("fluent-button");

		Context.DisposeComponents();
	}

}
