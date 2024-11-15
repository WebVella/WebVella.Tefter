namespace WebVella.Tefter.Web.Tests.Components;

public class SpaceViewLinkSaveSelectorComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		// Act
		var cut = Context.RenderComponent<TfSpaceViewLinkSaveSelector>();
		await cut.Instance.ToggleSelector();
		// Assert
		cut.Find(".view-export-selector-menu");

		Context.DisposeComponents();
	}

}
