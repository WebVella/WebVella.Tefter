namespace WebVella.Tefter.UI.Tests.Components;

public class SpaceViewShareSelectorComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		// Act
		var cut = Context.RenderComponent<TfSpaceViewShareSelector>();
		await cut.Instance.ToggleSelector();
		// Assert
		cut.Find(".view-export-selector-menu");

		Context.DisposeComponents();
	}

}
