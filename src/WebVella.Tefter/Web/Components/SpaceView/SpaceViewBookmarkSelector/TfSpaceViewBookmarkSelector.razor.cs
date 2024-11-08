namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceView.SpaceViewBookmarkSelector.TfSpaceViewBookmarkSelector", "WebVella.Tefter")]
public partial class TfSpaceViewBookmarkSelector : TfBaseComponent
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] private AppStateUseCase UC { get; set; }

	private bool _open = false;
	public async Task ToggleSelector()
	{
		_open = !_open;
		await InvokeAsync(StateHasChanged);
	}
	private async Task _bookmarkEdit()
	{
		var dialog = await DialogService.ShowDialogAsync<TfSpaceViewBookmarkManageDialog>(
				TfAppState.Value.ActiveSpaceViewBookmark,
				new DialogParameters()
				{
					PreventDismissOnOverlayClick = true,
					PreventScroll = true,
					Width = TfConstants.DialogWidthLarge,
			TrapFocus = false
				});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
				var resultObj = (Tuple<TucBookmark,List<TucBookmark>,List<TucBookmark>>)result.Data;
				ToastService.ShowSuccess(LOC("Bookmark saved"));
				Dispatcher.Dispatch(new SetAppStateAction(
					component: this,
					state: TfAppState.Value with
					{
						CurrentUserBookmarks = resultObj.Item2,
						CurrentUserSaves = resultObj.Item3,
						ActiveSpaceViewBookmark = resultObj.Item1
					}
				));
		}
	}

	private async Task _bookmarkRemove()
	{
		try
		{
			var bookmark = TfAppState.Value.CurrentUserBookmarks.FirstOrDefault(x=> x.SpaceViewId == TfAppState.Value.SpaceView.Id);
			if(bookmark is null) return;
			var result = await UC.DeleteBookmarkAsync(bookmark);
			ProcessServiceResponse(result);
			if (result.IsSuccess)
			{
				ToastService.ShowSuccess(LOC("Bookmark removed"));
				Dispatcher.Dispatch(new SetAppStateAction(
					component: this,
					state: TfAppState.Value with
					{
						CurrentUserBookmarks = result.Value.Item1,
						CurrentUserSaves = result.Value.Item2,
						ActiveSpaceViewBookmark = null
					}
				));
			}
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			await InvokeAsync(StateHasChanged);
		}
	}
}