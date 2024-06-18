namespace WebVella.Tefter.Web.Components;
public partial class TfSpaceViewToolbar : TfBaseComponent
{

	//Filter
	private TfSpaceViewShareSelector _shareSelector;
	private async Task OnExportClick()
	{
		await _shareSelector.ToggleSelector();
	}

	//private void OnExportChange(SpaceViewExportChangedEventArgs args)
	//{

	//}

}