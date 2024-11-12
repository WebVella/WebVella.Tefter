namespace WebVella.Tefter.Web.Tests.Components;

public class HomeToolbarComponentTests : BaseTest
{

	[Fact]
	public void RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		// Act
		var cut = Context.RenderComponent<TfHome>();

		// Assert
		cut.Find(".home-toolbar");

		Context.DisposeComponents();
	}

}
