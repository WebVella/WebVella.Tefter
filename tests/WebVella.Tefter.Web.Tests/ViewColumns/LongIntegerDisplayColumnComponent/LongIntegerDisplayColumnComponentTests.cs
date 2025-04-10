namespace WebVella.Tefter.Web.Tests.ViewColumns;

public class LongIntegerDisplayColumnComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		// Act
		var cut = Context.RenderComponent<TfLongIntegerDisplayColumnComponent>(args => args
		.Add(x => x.Context, new TfSpaceViewColumnScreenRegion())
		);

		// Assert
		cut.Markup.Should().Be("<div></div>");

		Context.DisposeComponents();
	}

}
