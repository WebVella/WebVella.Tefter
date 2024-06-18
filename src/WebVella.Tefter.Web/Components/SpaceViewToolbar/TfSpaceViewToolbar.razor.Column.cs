namespace WebVella.Tefter.Web.Components;
public partial class TfSpaceViewToolbar : TfBaseComponent
{
	private TfSpaceViewPropertiesSelector _propertiesSelector;
	private async Task OnGridPropertiesClick()
	{
		await _propertiesSelector.ToggleSelector();
	}
	//private void OnGridPropertiesChange(GridPropertiesChangedEventArgs args)
	//{

	//}

}