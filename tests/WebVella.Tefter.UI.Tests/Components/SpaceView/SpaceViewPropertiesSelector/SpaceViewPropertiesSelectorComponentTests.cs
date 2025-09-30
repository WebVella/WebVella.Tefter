namespace WebVella.Tefter.UI.Tests.Components;

public class SpaceViewPropertiesSelectorComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		// Act
		var cut = Context.RenderComponent<TfSpaceViewPropertiesSelector>();
		await cut.Instance.ToggleSelector();
		// Assert
		cut.Find(".grid-properties-selector-menu");

		Context.DisposeComponents();
	}

}
