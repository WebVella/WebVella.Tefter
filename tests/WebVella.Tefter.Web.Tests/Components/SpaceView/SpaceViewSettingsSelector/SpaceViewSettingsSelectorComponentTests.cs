namespace WebVella.Tefter.Web.Tests.Components;

public class SpaceViewSettingsSelectorComponentTests : BaseTest
{

	[Fact]
	public async Task RendersCorrectly()
	{
		//Given
		var Context = GetTestContext();
		// Act
		var cut = Context.RenderComponent<TfSpaceViewSettingsSelector>();
		await cut.Instance.ToggleSelector();
		// Assert
		cut.Find(".view-setting-selector-menu");

		Context.DisposeComponents();
	}

}
