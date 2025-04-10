namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.AdminDataProviderData.TfAdminDataProviderData", "WebVella.Tefter")]
public partial class TfAdminDataProviderData : TfBaseComponent
{
	[Inject] private AppStateUseCase UC { get; set; }
	[Inject] private UserStateUseCase UserUC { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] protected IState<TfUserState> TfUserState { get; set; }

	private bool _isDataLoading = false;
	private bool _showSystemColumns = true;
	private bool _showSharedKeyColumns = true;
	private bool _showCustomColumns = true;

	protected override async ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			ActionSubscriber.UnsubscribeFromAllActions(this);
		}
		await base.DisposeAsyncCore(disposing);
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		if (firstRender)
		{
			_isDataLoading = false;
			await InvokeAsync(StateHasChanged);
			ActionSubscriber.SubscribeToAction<SetAppStateAction>(this, On_AppChanged);
		}
	}

	private void On_AppChanged(SetAppStateAction action)
	{
		InvokeAsync(async () =>
		{
			_isDataLoading = false;
			await InvokeAsync(StateHasChanged);
		});
	}

	private void _toggleSystemColumns()
	{
		_showSystemColumns = !_showSystemColumns;
		StateHasChanged();
	}
	private void _toggleSharedKeyColumns()
	{
		_showSharedKeyColumns = !_showSharedKeyColumns;
		StateHasChanged();
	}
	private void _toggleCustomColumns()
	{
		_showCustomColumns = !_showCustomColumns;
		StateHasChanged();
	}

	private async Task _deleteAllData()
	{
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need all data deleted? This operation can take minutes!")))
			return;
		await UC.DeleteAllProviderData(TfAppState.Value.AdminDataProvider.Id);
		ToastService.ShowSuccess(LOC("Data provider data deletion is triggered!"));
		Navigator.ReloadCurrentUrl();
	}

	private async Task _onSearch(string value)
	{
		if (_isDataLoading) return;
		_isDataLoading = true;
		var queryDict = new Dictionary<string, object>{
			{ TfConstants.SearchQueryName, value}
		};
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}

	private async Task _goFirstPage()
	{
		if (_isDataLoading) return;
		if (TfAppState.Value.Route.Page == 1) return;
		_isDataLoading = true;
		var queryDict = new Dictionary<string, object>{
			{ TfConstants.PageQueryName, 1}
		};
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}
	private async Task _goPreviousPage()
	{
		if (_isDataLoading) return;
		var page = TfAppState.Value.Route.Page - 1;
		if (page < 1) page = 1;
		if (TfAppState.Value.Route.Page == page) return;
		_isDataLoading = true;
		var queryDict = new Dictionary<string, object>{
			{ TfConstants.PageQueryName, page}
		};
		queryDict[TfConstants.PageQueryName] = page;
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}
	private async Task _goNextPage()
	{
		if (_isDataLoading) return;
		if (TfAppState.Value.AdminDataProviderData is null
		|| TfAppState.Value.AdminDataProviderData.Rows.Count == 0)
			return;

		var page = TfAppState.Value.Route.Page + 1;
		if (page < 1) page = 1;
		if (TfAppState.Value.Route.Page == page) return;
		_isDataLoading = true;
		var queryDict = new Dictionary<string, object>{
			{ TfConstants.PageQueryName, page}
		};
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}
	private async Task _goLastPage()
	{
		if (_isDataLoading) return;
		if (TfAppState.Value.Route.Page == -1) return;
		_isDataLoading = true;
		var queryDict = new Dictionary<string, object>{
			{ TfConstants.PageQueryName, -1}
		};
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}
	private async Task _goOnPage(int page)
	{
		if (_isDataLoading) return;
		if (page < 1 && page != -1) page = 1;
		if (TfAppState.Value.Route.Page == page) return;
		_isDataLoading = true;
		var queryDict = new Dictionary<string, object>{
			{ TfConstants.PageQueryName, page}
		};
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}

	private async Task _pageSizeChange(int pageSize)
	{
		if (_isDataLoading) return;
		if (pageSize < 0) pageSize = TfConstants.PageSize;
		if (TfAppState.Value.Route.PageSize == pageSize) return;
		try
		{
			var user = await UserUC.SetPageSize(
						userId: TfUserState.Value.CurrentUser.Id,
						pageSize: pageSize == TfConstants.PageSize ? null : pageSize
					);

				Dispatcher.Dispatch(new SetUserStateAction(
					component: this,
					oldStateHash: TfUserState.Value.Hash,
					state: TfUserState.Value with { Hash = Guid.NewGuid(), CurrentUser = user }));
		}
		catch { }

		_isDataLoading = true;
		var queryDict = new Dictionary<string, object>{
			{ TfConstants.PageSizeQueryName, pageSize}
		};
		queryDict[TfConstants.PageSizeQueryName] = pageSize;
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}

	private bool _columnIsVisible(TfDataColumn column)
	{
		if (_showSharedKeyColumns && column.IsShared) return true;
		if (_showSystemColumns && column.IsSystem) return true;
		if (_showCustomColumns && !column.IsSystem) return true;

		return false;
	}
}