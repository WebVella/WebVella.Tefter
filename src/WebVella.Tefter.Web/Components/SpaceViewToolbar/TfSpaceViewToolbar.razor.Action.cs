namespace WebVella.Tefter.Web.Components;
public partial class TfSpaceViewToolbar : TfBaseComponent
{
	private TfSpaceViewActionSelector _actionSelector;
	private async Task OnActionClick()
	{
		await _actionSelector.ToggleSelector();
	}
	//private void OnActionChange(SpaceViewActionChangedEventArgs args)
	//{

	//}

}