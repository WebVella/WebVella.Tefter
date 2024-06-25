using WebVella.Tefter.Web.Components.SpaceViewSettingsSelector;
namespace WebVella.Tefter.Web.Components.SpaceViewToolbar;
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