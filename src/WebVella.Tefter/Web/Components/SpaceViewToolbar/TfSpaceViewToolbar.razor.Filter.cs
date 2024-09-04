namespace WebVella.Tefter.Web.Components;
public partial class TfSpaceViewToolbar : TfBaseComponent
{
	private TfSpaceViewFilterSelector _filterSelector;
	private async Task OnFilterClick()
	{
		await _filterSelector.ToggleSelector();
	}
	//private void OnFilterChange(SpaceViewFilterChangedEventArgs args)
	//{

	//}

}