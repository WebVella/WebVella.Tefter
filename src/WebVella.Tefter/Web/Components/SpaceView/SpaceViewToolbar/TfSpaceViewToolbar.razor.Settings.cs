namespace WebVella.Tefter.Web.Components;
public partial class TfSpaceViewToolbar : TfBaseComponent
{
	private TfSpaceViewSettingsSelector _settingSelector;
	private Task OnSettingsClick()
	{
		_settingSelector.ToggleSelector();
		return Task.CompletedTask;
	}
	//private void OnSettingChange(SpaceViewSettingChangedEventArgs args)
	//{

	//}

}