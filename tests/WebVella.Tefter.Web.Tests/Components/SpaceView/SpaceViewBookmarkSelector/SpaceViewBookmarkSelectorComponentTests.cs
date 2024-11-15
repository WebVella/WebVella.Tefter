namespace WebVella.Tefter.Web.Tests.Components;

public class SpaceViewBookmarkSelectorComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		// Act
		var cut = Context.RenderComponent<TfSpaceViewBookmarkSelector>();
		await cut.Instance.ToggleSelector();
		// Assert
		cut.Find(".view-export-selector-menu");

		Context.DisposeComponents();
	}

}
