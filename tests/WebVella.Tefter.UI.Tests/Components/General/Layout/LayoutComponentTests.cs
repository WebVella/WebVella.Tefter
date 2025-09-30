namespace WebVella.Tefter.UI.Tests.Components;
public class LayoutComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		// Act
		var cut = Context.RenderComponent<TfLayout>();

		// Assert
		cut.Find(".tf-layout");

		Context.DisposeComponents();
	}
}
