using FluentAssertions.Equivalency;

namespace WebVella.Tefter.Web.Tests.ViewColumns;

public class SelectDisplayColumnComponentTests : BaseTest
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
		var cut = Context.RenderComponent<TfSelectDisplayColumnComponent>(args => args
		.Add(x => x.RegionContext, new TfSpaceViewColumnScreenRegionContext()
		{
			DataTable = dt,
			DataMapping = new Dictionary<string, string>{ {"Value",columnName} }
		})
		);

		// Assert
		cut.Nodes.Length.Should().Be(1);
		var node = cut.Nodes[0];
		node.GetType().GetInterface(nameof(AngleSharp.Html.Dom.IHtmlDivElement)).Should().NotBeNull();
		var divEl = (AngleSharp.Html.Dom.IHtmlDivElement)node;
		divEl.ClassList.Length.Should().Be(0);
		Context.DisposeComponents();
	}

}
