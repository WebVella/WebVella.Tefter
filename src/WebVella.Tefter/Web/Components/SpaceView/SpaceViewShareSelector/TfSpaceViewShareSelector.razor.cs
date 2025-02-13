namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceView.SpaceViewShareSelector.TfSpaceViewShareSelector", "WebVella.Tefter")]
public partial class TfSpaceViewShareSelector : TfBaseComponent
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }

	[Inject] private AppStateUseCase UC { get; set; }
	private bool _open = false;
	private string _exportExcelUrl = "/api/export/export-view";
	private string _exportSelectionBtnId = "tfExportSelectionBtn";
	private string _exportAllBtnId = "tfExportAllBtn";
	public async Task ToggleSelector()
	{
		_open = !_open;
		await InvokeAsync(StateHasChanged);
	}
	private async Task _copyLinkToClipboard()
	{
		await JSRuntime.InvokeVoidAsync("Tefter.copyToClipboard", Navigator.Uri);
		ToastService.ShowSuccess(LOC("Link copied"));
	}

	private string _getExportSelectionData()
	{
		if (TfAppState.Value.SpaceView is null || TfAppState.Value.SpaceNode is null) return null;
		return JsonSerializer.Serialize(new TucExportViewData()
		{
			SelectedRows = TfAppState.Value.SelectedDataRows,
			RouteState = TfAppState.Value.Route
		});
	}

	private string _getExportAllData()
	{
		if (TfAppState.Value.SpaceView is null || TfAppState.Value.SpaceNode is null) return null;
		return JsonSerializer.Serialize(new TucExportViewData()
		{
			SelectedRows = new(),
			RouteState = TfAppState.Value.Route
		});
	}


	private async Task _exportSelection()
	{
		await JSRuntime.InvokeAsync<object>("Tefter.clickElementById", _exportSelectionBtnId);
	}
	private async Task _exportAll()
	{
		await JSRuntime.InvokeAsync<object>("Tefter.clickElementById", _exportAllBtnId);
	}

	private async Task _bookmarkView()
	{
		try
		{
			var submit = new TucBookmark
			{
				Id = Guid.NewGuid(),
				SpaceViewId = TfAppState.Value.SpaceView.Id,
				UserId = TfAppState.Value.CurrentUser.Id,
				CreatedOn = DateTime.Now,
				Description = String.Empty,//initially nothing is added for convenience
				Name = TfAppState.Value.SpaceView.Name + " " + DateTime.Now.ToString("dd-MM-yyyy HH:mm"),
				Url = null
			};

			var (bookmarks, saves) = await UC.CreateBookmarkAsync(submit);

			ToastService.ShowSuccess(LOC("View is now bookmarked"));
			Dispatcher.Dispatch(new SetAppStateAction(
				component: this,
				state: TfAppState.Value with
				{
					CurrentUserBookmarks = bookmarks,
					CurrentUserSaves = saves,
					ActiveSpaceViewBookmark = submit
				}
			));
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
				UserId = TfAppState.Value.CurrentUser.Id,
				CreatedOn = DateTime.Now,
				Description = String.Empty,//initially nothing is added for convenience
				Name = TfAppState.Value.SpaceView.Name + " " + DateTime.Now.ToString("dd-MM-yyyy HH:mm"),
				Url = new Uri(Navigator.Uri).PathAndQuery
			};
			var (bookmarks, saves) = await UC.CreateBookmarkAsync(submit);

			ToastService.ShowSuccess(LOC("URL is now saved"));

			Dispatcher.Dispatch(new SetAppStateAction(
				component: this,
				state: TfAppState.Value with
				{
					CurrentUserBookmarks = bookmarks,
					CurrentUserSaves = saves,
					ActiveSpaceViewSavedUrl = submit
				}
			));
			var query = new Dictionary<string, object>();
			query[TfConstants.ActiveSaveQueryName] = submit.Id;
			await Navigator.ApplyChangeToUrlQuery(query);

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