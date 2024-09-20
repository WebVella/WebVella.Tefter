namespace WebVella.Tefter.Web.Components;
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
				new TucBookmark() with { SpaceViewId = TfAppState.Value.SpaceView.Id, Url = null },
				new DialogParameters()
				{
					PreventDismissOnOverlayClick = true,
					PreventScroll = true,
					Width = TfConstants.DialogWidthLarge
				});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
		}
	}

	private async Task _bookmarkRemove()
	{
		try
		{
			var bookmark = TfAppState.Value.CurrentUserBookmarks.FirstOrDefault(x=> x.SpaceViewId == TfAppState.Value.SpaceView.Id);
			if(bookmark is null) return;
			var result = await UC.DeleteBookmark(bookmark);
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