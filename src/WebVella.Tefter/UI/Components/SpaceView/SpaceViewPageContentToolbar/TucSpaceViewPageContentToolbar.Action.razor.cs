namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceViewPageContentToolbar
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