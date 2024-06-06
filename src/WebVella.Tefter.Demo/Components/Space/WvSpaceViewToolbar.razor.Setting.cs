namespace WebVella.Tefter.Demo.Components;
public partial class WvSpaceViewToolbar : WvBaseComponent
{

	//Filter
	private WvViewSettingSelector _settingSelector;
	private async Task OnSettingsClick()
	{
		await _settingSelector.ToggleSelector();
	}
	private void OnSettingChange(ViewSettingChangedEventArgs args){ 
	
	}

}