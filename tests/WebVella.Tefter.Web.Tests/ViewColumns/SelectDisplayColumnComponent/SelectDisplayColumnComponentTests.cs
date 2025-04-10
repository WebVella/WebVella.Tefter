using FluentAssertions.Equivalency;

namespace WebVella.Tefter.Web.Tests.ViewColumns;

public class SelectDisplayColumnComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		// Act
		var cut = Context.RenderComponent<TfSelectDisplayColumnComponent>(args => args
		.Add(x => x.Context, new TfSpaceViewColumnScreenRegion())
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
