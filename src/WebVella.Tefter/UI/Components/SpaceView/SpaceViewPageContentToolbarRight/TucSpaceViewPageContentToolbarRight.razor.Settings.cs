namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceViewPageContentToolbarRight : TfBaseComponent
{
	private TucSpaceViewSettingsSelector _settingSelector;
	private async Task OnSettingsClick()
	{
		await _settingSelector.ToggleSelector();
	}
	//private void OnSettingChange(SpaceViewSettingChangedEventArgs args)
	//{

	//}
}