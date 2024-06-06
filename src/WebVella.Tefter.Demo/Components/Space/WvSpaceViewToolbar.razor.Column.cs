namespace WebVella.Tefter.Demo.Components;
public partial class WvSpaceViewToolbar : WvBaseComponent
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