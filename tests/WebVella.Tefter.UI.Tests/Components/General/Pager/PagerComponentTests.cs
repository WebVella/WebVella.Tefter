namespace WebVella.Tefter.UI.Tests.Components;
public class PagerComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		// Act
		var cut = Context.RenderComponent<TfPager>();

		// Assert
		cut.Find(".paginator-nav");

		Context.DisposeComponents();
	}
}
