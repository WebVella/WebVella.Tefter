namespace WebVella.Tefter.Demo.Components;
public partial class WvSpaceItemToolbar : WvBaseComponent
{

	private WvViewFilterSelector _filterSelector;
	private async Task OnFilterClick()
	{
		await _filterSelector.ToggleSelector();
	}
	private void OnFilterChange(ViewFilterChangedEventArgs args){ 
	
	}

}