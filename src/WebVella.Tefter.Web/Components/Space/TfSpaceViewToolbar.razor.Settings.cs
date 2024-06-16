namespace WebVella.Tefter.Web.Components;
public partial class TfSpaceViewToolbar : TfBaseComponent
{
	private TfSpaceViewSettingsSelector _settingSelector;
	private async Task OnSettingsClick()
	{
		await _settingSelector.ToggleSelector();
	}
	//private void OnSettingChange(SpaceViewSettingChangedEventArgs args)
	//{

	//}

}