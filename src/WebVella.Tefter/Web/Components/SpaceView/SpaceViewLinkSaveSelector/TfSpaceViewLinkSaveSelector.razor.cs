namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceView.SpaceViewLinkSaveSelector.TfSpaceViewLinkSaveSelector", "WebVella.Tefter")]
public partial class TfSpaceViewLinkSaveSelector : TfBaseComponent
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] private AppStateUseCase UC { get; set; }
	private bool _open = false;
	public async Task ToggleSelector()
	{
		_open = !_open;
		await InvokeAsync(StateHasChanged);
	}
	private async Task _updateUrl()
	{
		try
		{
			var submit = TfAppState.Value.ActiveSpaceViewSavedUrl with
			{
				Url = new Uri(Navigator.Uri).PathAndQuery
			};
			var result = await UC.UpdateBookmarkAsync(submit);
			ProcessServiceResponse(result);
			if (result.IsSuccess)
			{
				ToastService.ShowSuccess(LOC("URL updated"));
				Dispatcher.Dispatch(new SetAppStateAction(
					component: this,
					state: TfAppState.Value with
					{
						CurrentUserBookmarks = result.Value.Item1,
						CurrentUserSaves = result.Value.Item2,
						ActiveSpaceViewSavedUrl = submit
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

	private async Task _editUrl()
	{
		var dialog = await DialogService.ShowDialogAsync<TfSpaceViewBookmarkManageDialog>(
						TfAppState.Value.ActiveSpaceViewSavedUrl,
						new DialogParameters()
						{
							PreventDismissOnOverlayClick = true,
							PreventScroll = true,
							Width = TfConstants.DialogWidthLarge
						});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			var resultObj = (Tuple<TucBookmark,List<TucBookmark>,List<TucBookmark>>)result.Data;
			ToastService.ShowSuccess(LOC("URL saved"));
			Dispatcher.Dispatch(new SetAppStateAction(
				component: this,
				state: TfAppState.Value with
				{
					CurrentUserBookmarks = resultObj.Item2,
					CurrentUserSaves = resultObj.Item3,
					ActiveSpaceViewSavedUrl = resultObj.Item1
				}
			));
		}
	}
	private async Task _saveUrlAs()
	{
		var dialog = await DialogService.ShowDialogAsync<TfSpaceViewBookmarkManageDialog>(
						TfAppState.Value.ActiveSpaceViewSavedUrl with { Id = Guid.Empty},
						new DialogParameters()
						{
							PreventDismissOnOverlayClick = true,
							PreventScroll = true,
							Width = TfConstants.DialogWidthLarge
						});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			var resultObj = (Tuple<TucBookmark,List<TucBookmark>,List<TucBookmark>>)result.Data;
			ToastService.ShowSuccess(LOC("URL saved"));
			Dispatcher.Dispatch(new SetAppStateAction(
				component: this,
				state: TfAppState.Value with
				{
					CurrentUserBookmarks = resultObj.Item2,
					CurrentUserSaves = resultObj.Item3,
					//ActiveSpaceViewSavedUrl = resultObj.Item1 //should leave the old as it is
				}
			));
		}
	}
	private async Task _removeUrl()
	{
		try
		{
			if (TfAppState.Value.ActiveSpaceViewSavedUrl is null) return;
			var result = await UC.DeleteBookmarkAsync(TfAppState.Value.ActiveSpaceViewSavedUrl);
			ProcessServiceResponse(result);
			if (result.IsSuccess)
			{
				ToastService.ShowSuccess(LOC("Saved URL removed"));
				Dispatcher.Dispatch(new SetAppStateAction(
					component: this,
					state: TfAppState.Value with
					{
						CurrentUserBookmarks = result.Value.Item1,
						CurrentUserSaves = result.Value.Item2,
						ActiveSpaceViewSavedUrl = null
					}
				));
				var query = new Dictionary<string, object>();
				query[TfConstants.ActiveSaveQueryName] = null;
				await Navigator.ApplyChangeToUrlQuery(query);

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