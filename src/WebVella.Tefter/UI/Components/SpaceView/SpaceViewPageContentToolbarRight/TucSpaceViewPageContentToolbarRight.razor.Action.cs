namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceViewPageContentToolbarRight : TfBaseComponent
{
	private TucSpaceViewActionSelector _actionSelector;
	private async Task OnActionClick()
	{
		await _actionSelector.ToggleSelector();
	}

	//private void OnActionChange(SpaceViewActionChangedEventArgs args)
	//{

	//}
}