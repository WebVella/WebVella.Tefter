namespace WebVella.Tefter.Web.Tests.Components;
public class NavigationComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		// Act
		var cut = Context.RenderComponent<TfNavigation>();

		// Assert
		cut.Find(".tf-layout__navigation");

		Context.DisposeComponents();
	}
}
