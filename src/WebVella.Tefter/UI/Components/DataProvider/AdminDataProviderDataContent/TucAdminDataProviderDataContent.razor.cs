namespace WebVella.Tefter.UI.Components;
public partial class TucAdminDataProviderDataContent : TfBaseComponent, IDisposable
{
	[Inject] public ITfNavigationUIService TfNavigationUIService { get; set; } = default!;
	[Inject] public ITfUserUIService TfUserUIService { get; set; } = default!;
	[Inject] public ITfDataProviderUIService TfDataProviderUIService { get; set; } = default!;

	private TfDataProvider? _provider = null;
	private TfUser _currentUser = default!;
	private TfNavigationState _navState = default!;
	private bool _isDataLoading = false;
	private bool _showSystemColumns = false;
	private bool _showJoinKeyColumns = false;
	private bool _showCustomColumns = true;
	private long _totalRows = 0;
	private TfDataTable? _data = null;
	public void Dispose()
	{
		TfNavigationUIService.NavigationStateChanged -= On_NavigationStateChanged;
		TfDataProviderUIService.DataProviderUpdated -= On_DataProviderUpdated;
	}
	protected override async Task OnInitializedAsync()
	{
		await _init();
		TfNavigationUIService.NavigationStateChanged += On_NavigationStateChanged;
		TfDataProviderUIService.DataProviderUpdated += On_DataProviderUpdated;
	}
	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		if (UriInitialized != args.Uri)
			WvBlazorTraceService.OnSignal(this,"On_NavigationDataChanged");
			await _init(args);
	}

	private async void On_DataProviderUpdated(object? caller, TfDataProvider args)
	{
		await _init(null);
	}

	private async Task _init(TfNavigationState? navState = null)
	{
		if (navState == null)
			navState = await TfNavigationUIService.GetNavigationStateAsync(Navigator);

		try
		{
			if (navState.DataProviderId is null)
			{
				_provider = null;
				await InvokeAsync(StateHasChanged);
				return;
			}
			_navState = navState;
			_provider = TfDataProviderUIService.GetDataProvider(_navState.DataProviderId.Value);
			var user = await TfUserUIService.GetCurrentUserAsync();
			if (user is null)
				throw new Exception("Current user not found");
			_currentUser = user!;
			if (_provider is null)
				return;

			_isDataLoading = true;
			await InvokeAsync(StateHasChanged);
			_totalRows = TfDataProviderUIService.GetDataProviderRowsCount(_provider.Id);
			_data = TfDataProviderUIService.QueryDataProvider(
				providerId: _provider.Id,
				search: _navState.Search ?? String.Empty,
				page: _navState.Page ?? 1,
				pageSize: _navState.PageSize ?? _currentUser.Settings.PageSize ?? TfConstants.PageSize);
		}
		finally
		{
			_isDataLoading = false;
			UriInitialized = navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}

	private void _toggleSystemColumns()
	{
		_showSystemColumns = !_showSystemColumns;
		StateHasChanged();
	}
	private void _toggleJoinKeyColumns()
	{
		_showJoinKeyColumns = !_showJoinKeyColumns;
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
		TfDataProviderUIService.DeleteAllProviderRows(_provider!.Id);
		ToastService.ShowSuccess(LOC("Data provider data deletion is triggered!"));
		Navigator.ReloadCurrentUrl();
	}

	private async Task _onSearch(string value)
	{
		if (_isDataLoading) return;
		_isDataLoading = true;
		var queryDict = new Dictionary<string, object?>{
			{ TfConstants.SearchQueryName, value}
		};
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}

	private async Task _goFirstPage()
	{
		if (_isDataLoading) return;
		if (_navState.Page == 1) return;
		_isDataLoading = true;
		var queryDict = new Dictionary<string, object?>{
			{ TfConstants.PageQueryName, 1}
		};
		await Navigator.ApplyChangeToUrlQuery(queryDict);
		WvBlazorTraceService.OnSignal(this,"_goFirstPage");
	}
	private async Task _goPreviousPage()
	{
		if (_isDataLoading) return;
		var page = _navState.Page - 1;
		if (page < 1) page = 1;
		if (_navState.Page == page) return;
		_isDataLoading = true;
		var queryDict = new Dictionary<string, object?>{
			{ TfConstants.PageQueryName, page}
		};
		queryDict[TfConstants.PageQueryName] = page;
		await Navigator.ApplyChangeToUrlQuery(queryDict);
		WvBlazorTraceService.OnSignal(this,"_goPreviousPage");
	}
	private async Task _goNextPage()
	{
		if (_isDataLoading) return;
		if (_totalRows == 0)
			return;

		var page = _navState.Page + 1;
		if (page < 1) page = 1;
		if (_navState.Page == page) return;
		_isDataLoading = true;
		var queryDict = new Dictionary<string, object?>{
			{ TfConstants.PageQueryName, page}
		};
		await Navigator.ApplyChangeToUrlQuery(queryDict);
		WvBlazorTraceService.OnSignal(this,"_goNextPage");
	}
	private async Task _goLastPage()
	{
		if (_isDataLoading) return;
		if (_navState.Page == -1) return;
		_isDataLoading = true;
		var queryDict = new Dictionary<string, object?>{
			{ TfConstants.PageQueryName, -1}
		};
		await Navigator.ApplyChangeToUrlQuery(queryDict);
		WvBlazorTraceService.OnSignal(this,"_goLastPage");
	}
	private async Task _goOnPage(int page)
	{
		if (_isDataLoading) return;
		if (page < 1 && page != -1) page = 1;
		if (_navState.Page == page) return;
		_isDataLoading = true;
		var queryDict = new Dictionary<string, object?>{
			{ TfConstants.PageQueryName, page}
		};
		await Navigator.ApplyChangeToUrlQuery(queryDict);
		WvBlazorTraceService.OnSignal(this,"_goOnPage");
	}

	private async Task _pageSizeChange(int pageSize)
	{
		if (_isDataLoading) return;
		if (pageSize < 0) pageSize = TfConstants.PageSize;
		if (_navState.PageSize == pageSize) return;
		try
		{
			var user = await TfUserUIService.SetPageSize(
						userId: _currentUser.Id,
						pageSize: pageSize == TfConstants.PageSize ? null : pageSize
					);
		}
		catch { }

		_isDataLoading = true;
		var queryDict = new Dictionary<string, object?>{
			{ TfConstants.PageSizeQueryName, pageSize}
		};
		queryDict[TfConstants.PageSizeQueryName] = pageSize;
		await Navigator.ApplyChangeToUrlQuery(queryDict);
		WvBlazorTraceService.OnSignal(this,"_pageSizeChange");
	}

	private bool _columnIsVisible(TfDataColumn column)
	{
		if (_showJoinKeyColumns && column.IsShared) return true;
		if (_showSystemColumns && column.IsSystem) return true;
		if (_showCustomColumns && !column.IsSystem) return true;

		return false;
	}
}
