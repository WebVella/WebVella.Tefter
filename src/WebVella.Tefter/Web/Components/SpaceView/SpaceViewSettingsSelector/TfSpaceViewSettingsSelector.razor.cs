namespace WebVella.Tefter.Web.Components;
public partial class TfSpaceViewSettingsSelector : TfBaseComponent
{
	[Inject] protected IState<TfUserState> TfUserState { get; set; }
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
		try
		{
			var submit = new TucBookmark
			{
				Id = Guid.NewGuid(),
				SpaceViewId = TfAppState.Value.SpaceView.Id,
				UserId = TfUserState.Value.CurrentUser.Id,
				CreatedOn = DateTime.Now,
				Description = String.Empty,//initially nothing is added for convenience
				Name = TfAppState.Value.SpaceView.Name,
				Url = null
			};
			var result = await UC.CreateBookmarkAsync(submit);
			ProcessServiceResponse(result);
			if (result.IsSuccess)
			{
				ToastService.ShowSuccess(LOC("View is now bookmarked"));
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

	private async Task _saveViewUrl()
	{
		try
		{
			var submit = new TucBookmark
			{
				Id = Guid.NewGuid(),
				SpaceViewId = TfAppState.Value.SpaceView.Id,
				UserId = TfUserState.Value.CurrentUser.Id,
				CreatedOn = DateTime.Now,
				Description = String.Empty,//initially nothing is added for convenience
				Name = TfAppState.Value.SpaceView.Name,
				Url = new Uri(Navigator.Uri).PathAndQuery
			};
			var result = await UC.CreateBookmarkAsync(submit);
			ProcessServiceResponse(result);
			if (result.IsSuccess)
			{
				ToastService.ShowSuccess(LOC("URL is now saved"));
				Dispatcher.Dispatch(new SetAppStateAction(
					component: this,
					state: TfAppState.Value with
					{
						CurrentUserBookmarks = result.Value.Item1,
						CurrentUserSaves = result.Value.Item2,
						ActiveSpaceViewSavedUrl = submit
					}
				));
				var query = new Dictionary<string,object>();
				query[TfConstants.ActiveSaveQueryName] = submit.Id;
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