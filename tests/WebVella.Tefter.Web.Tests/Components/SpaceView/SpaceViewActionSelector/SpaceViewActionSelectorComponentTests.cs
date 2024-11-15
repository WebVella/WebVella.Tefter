namespace WebVella.Tefter.Web.Tests.Components;

public class SpaceViewActionSelectorComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		// Act
		var cut = Context.RenderComponent<TfSpaceViewActionSelector>();
		await cut.Instance.ToggleSelector();
		// Assert
		cut.Find(".view-export-selector-menu");

		Context.DisposeComponents();
	}

}
