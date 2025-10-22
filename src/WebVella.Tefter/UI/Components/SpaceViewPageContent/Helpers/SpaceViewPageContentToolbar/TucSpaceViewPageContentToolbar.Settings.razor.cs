namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceViewPageContentToolbar
{
	private TucSpaceViewSettingsSelector _settingsSelector;
	private async Task OnSettingsClick()
	{
		await _settingsSelector.ToggleSelector();
	}
}