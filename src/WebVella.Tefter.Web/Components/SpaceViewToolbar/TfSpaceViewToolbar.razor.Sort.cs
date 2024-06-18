namespace WebVella.Tefter.Web.Components;
public partial class TfSpaceViewToolbar : TfBaseComponent
{

	private TfSpaceViewSortSelector _sortSelector;
	private async Task OnSortClick()
	{
		await _sortSelector.ToggleSelector();
	}
	//private void OnSortChange(SpaceViewSortChangedEventArgs args)
	//{

	//}


}