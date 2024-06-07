namespace WebVella.Tefter.Demo.Components;
public partial class WvSpaceViewToolbar : WvBaseComponent
{

	private WvViewSortSelector _sortSelector;
	private async Task OnSortClick()
	{
		await _sortSelector.ToggleSelector();
	}
	private void OnSortChange(SpaceViewSortChangedEventArgs args)
	{

	}

}