namespace WebVella.Tefter.Web.Components;
public partial class TfSpaceViewToolbar : TfBaseComponent
{

	//Filter
	private TfSpaceViewShareSelector _shareSelector;
	private Task OnExportClick()
	{
		_shareSelector.ToggleSelector();
		return Task.CompletedTask;
	}

	//private void OnExportChange(SpaceViewExportChangedEventArgs args)
	//{

	//}

}