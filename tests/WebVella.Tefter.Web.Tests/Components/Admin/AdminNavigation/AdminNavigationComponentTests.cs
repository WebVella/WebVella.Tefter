namespace WebVella.Tefter.Web.Tests.Components;
public class AdminNavigationComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		// Act
		var cut = Context.RenderComponent<TfAdminNavigation>();

		// Assert
		cut.Find(".tf-navigation");

		Context.DisposeComponents();
	}
}
