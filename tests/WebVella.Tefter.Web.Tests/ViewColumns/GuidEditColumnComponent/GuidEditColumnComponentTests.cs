namespace WebVella.Tefter.Web.Tests.ViewColumns;

public class GuidEditColumnComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		// Act
		var cut = Context.RenderComponent<TfGuidEditColumnComponent>(args => args
		.Add(x => x.Context, new TfSpaceViewColumnScreenRegionContext())
		);

		// Assert
		cut.Find("fluent-text-field");

		Context.DisposeComponents();
	}

}
