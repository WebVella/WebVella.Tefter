namespace WebVella.Tefter.Demo.Components;
public partial class WvSpaceViewToolbar : WvBaseComponent
{

	//Filter
	private WvViewExportSelector _exportSelector;
	private async Task OnExportClick()
	{
		await _exportSelector.ToggleSelector();
	}

	private void OnExportChange(SpaceViewExportChangedEventArgs args){ 
	
	}

}