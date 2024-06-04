namespace WebVella.Tefter.Demo.Components;
public partial class WvSpaceItemToolbar : WvBaseComponent
{

	private WvViewSortSelector _sortSelector;
	private async Task OnSortClick()
	{
		await _sortSelector.ToggleSelector();
	}
	private void OnSortChange(ViewSortChangedEventArgs args)
	{

	}

}