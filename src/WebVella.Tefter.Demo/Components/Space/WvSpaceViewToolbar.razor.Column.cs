namespace WebVella.Tefter.Demo.Components;
public partial class WvSpaceViewToolbar : WvBaseComponent
{

	private WvGridPropertiesSelector _gridPropertiesSelector;
	private async Task OnGridPropertiesClick()
	{
		await _gridPropertiesSelector.ToggleSelector();
	}
	private void OnGridPropertiesChange(GridPropertiesChangedEventArgs args)
	{

	}

}