namespace WebVella.Tefter.Demo.Components;
public partial class WvSpaceItemToolbar : WvBaseComponent
{

	private WvViewColumnSelector _columnSelector;
	private async Task OnColumnClick()
	{
		await _columnSelector.ToggleSelector();
	}
	private void OnColumnChange(ViewColumnChangedEventArgs args)
	{

	}

}