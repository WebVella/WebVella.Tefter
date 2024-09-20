﻿namespace WebVella.Tefter.Web.Components;
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
	private async Task _editUrl()
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
	private async Task _saveUrlAs()
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
	private async Task _removeUrl()
	{
		try
		{
			var save = TfAppState.Value.CurrentUserSaves.FirstOrDefault(x => x.Id == TfAppState.Value.ActiveSaveId);
			if (save is null) return;
			var result = await UC.DeleteBookmark(save);
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
						ActiveSaveId = null
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