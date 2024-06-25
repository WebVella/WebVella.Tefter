using WebVella.Tefter.Web.Components.SpaceViewPropertiesSelector;
namespace WebVella.Tefter.Web.Components.SpaceViewToolbar;
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