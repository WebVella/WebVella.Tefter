namespace WebVella.Tefter.Web.Components;
public partial class TfSpaceViewToolbar : TfBaseComponent
{

	//Filter
	private TfSpaceViewBookmarkSelector _bookmarkSelector;
	private async Task OnBookmarkClick()
	{
		await _bookmarkSelector.ToggleSelector();
	}



	//private void OnExportChange(SpaceViewExportChangedEventArgs args)
	//{

	//}

}