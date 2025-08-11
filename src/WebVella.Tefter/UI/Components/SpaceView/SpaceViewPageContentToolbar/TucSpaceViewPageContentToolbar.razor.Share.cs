namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceViewPageContentToolbar : TfBaseComponent
{
	private TucSpaceViewShareSelector _shareSelector;
	private async Task OnExportClick()
	{
		await _shareSelector.ToggleSelector();
	}

	//private void OnExportChange(SpaceViewExportChangedEventArgs args)
	//{

	//}
}