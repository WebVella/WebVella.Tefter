namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceViewPageContentToolbar : TfBaseComponent
{
	private TucSpaceViewBookmarkSelector _bookmarkSelector;

	private TfBookmark? _activeBookmark = null;
	private TfBookmark? _activeSavedUrl = null;
	private async Task OnBookmarkClick()
	{
		await _bookmarkSelector.ToggleSelector();
	}

	//private void OnExportChange(SpaceViewExportChangedEventArgs args)
	//{

	//}
}