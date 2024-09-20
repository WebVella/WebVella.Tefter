namespace WebVella.Tefter.Web.Components;
public partial class TfSpaceViewSettingsSelector : TfBaseComponent
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] private AppStateUseCase UC { get; set; }
	private bool _open = false;
	private bool _selectorLoading = false;

	private void _init()
	{
	}

	public async Task ToggleSelector()
	{
		_open = !_open;
		if (_open)
		{
			_selectorLoading = true;
			await InvokeAsync(StateHasChanged);
			_init();

			_selectorLoading = false;
			await InvokeAsync(StateHasChanged);
		}
	}

	private void _manageView()
	{
		Navigator.NavigateTo(String.Format(TfConstants.SpaceViewManagePageUrl, TfAppState.Value.Space.Id, TfAppState.Value.SpaceView.Id));
	}
	private void _manageData()
	{
		Navigator.NavigateTo(String.Format(TfConstants.SpaceDataPageUrl, TfAppState.Value.Space.Id, TfAppState.Value.SpaceView.SpaceDataId));
	}

	private async Task _deleteView()
	{
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this view deleted?")))
			return;
		try
		{

			Result result = UC.DeleteSpaceView(TfAppState.Value.SpaceView.Id);
			ProcessServiceResponse(result);
			if (result.IsSuccess)
			{
				ToastService.ShowSuccess(LOC("Space view deleted"));
				if (TfAppState.Value.SpaceViewList.Count > 0)
					Navigator.NavigateTo(String.Format(TfConstants.SpaceViewPageUrl, TfAppState.Value.Space.Id, TfAppState.Value.SpaceViewList[0].Id), true);
				else
					Navigator.NavigateTo(String.Format(TfConstants.SpacePageUrl, TfAppState.Value.Space.Id), true);
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
	private async Task _bookmarkView()
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

	private async Task _saveViewUrl()
	{
		var dialog = await DialogService.ShowDialogAsync<TfSpaceViewBookmarkManageDialog>(
				new TucBookmark() with { SpaceViewId = TfAppState.Value.SpaceView.Id, Url = new Uri(Navigator.Uri).LocalPath },
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
	private async Task _saveViewUrlAs()
	{
		var dialog = await DialogService.ShowDialogAsync<TfSpaceViewBookmarkManageDialog>(
				new TucBookmark() with { SpaceViewId = TfAppState.Value.SpaceView.Id, Url = new Uri(Navigator.Uri).LocalPath },
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
}