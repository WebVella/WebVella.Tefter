namespace WebVella.Tefter.UI.Components;

public partial class TucAdminDataProviderDataContent : TfBaseComponent, IDisposable
{
	private TfDataProvider? _provider = null;
	private TfNavigationState _navState = default!;
	private bool _isDataLoading = false;
	private bool _showSystemColumns = false;
	private bool _showJoinKeyColumns = false;
	private bool _showProviderColumns = true;
	private long _totalRows = 0;
	private TfDataTable? _data = null;
	private bool _syncRunning = false;
	public void Dispose()
	{
		TfUIService.NavigationStateChanged -= On_NavigationStateChanged;
		TfUIService.DataProviderUpdated -= On_DataProviderUpdated;
	}
	protected override async Task OnInitializedAsync()
	{
		await _init();
		TfUIService.NavigationStateChanged += On_NavigationStateChanged;
		TfUIService.DataProviderUpdated += On_DataProviderUpdated;
	}
	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		if (UriInitialized != args.Uri)
			await _init(args);
	}

	private async void On_DataProviderUpdated(object? caller, TfDataProvider args)
	{
		await _init(null);
	}

	private async Task _init(TfNavigationState? navState = null)
	{
		if (navState == null)
			navState = TfAuthLayout.NavigationState;

		try
		{
			if (navState.DataProviderId is null)
			{
				_provider = null;
				await InvokeAsync(StateHasChanged);
				return;
			}
			_navState = navState;
			_provider = TfUIService.GetDataProvider(_navState.DataProviderId.Value);
			if (_provider is null)
				return;
			_syncRunning = TfUIService.IsSyncRunning(_provider.Id);

			_isDataLoading = true;
			await InvokeAsync(StateHasChanged);
			_totalRows = TfUIService.GetDataProviderRowsCount(_provider.Id);
			_data = TfUIService.QueryDataProvider(
				providerId: _provider.Id,
				search: _navState.Search ?? String.Empty,
				page: 1,
				pageSize: TfConstants.ItemsMaxLimit);

			foreach (TfDataRow row in _data.Rows)
			{
				row.OnEdit = async () => await _editRow(row);
				row.OnDelete = async () => await _deleteRow(row);
			}

		}
		finally
		{
			_isDataLoading = false;
			UriInitialized = _navState.Uri;
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
		_showProviderColumns = !_showProviderColumns;
		StateHasChanged();
	}

	private async Task _deleteAllData()
	{
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need all data deleted? This operation can take minutes!")))
			return;
		TfUIService.DeleteAllProviderRows(_provider!.Id);
		ToastService.ShowSuccess(LOC("Data provider data deletion is triggered!"));
		Navigator.ReloadCurrentUrl();
	}

	private async Task _addRow()
	{
		if (_provider is null || _data is null) return;
		var dialog = await DialogService.ShowDialogAsync<TucDataProviderManageDataDialog>(
			new TfManageDataProviderRowContext
			{
				Provider = _provider,
				RowId = null,
				Data = _data
			},
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
		}
		//Navigator.ReloadCurrentUrl();
	}

	private async Task _editRow(TfDataRow row)
	{
		if (_provider is null || _data is null) return;
		var dialog = await DialogService.ShowDialogAsync<TucDataProviderManageDataDialog>(
			new TfManageDataProviderRowContext
			{
				Provider = _provider,
				RowId = row.GetRowId(),
				Data = _data
			},
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
		}
		//Navigator.ReloadCurrentUrl();
	}
	private async Task _deleteRow(TfDataRow row)
	{
		throw new NotImplementedException();
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

	private async Task _goLastPage()
	{
		if (_isDataLoading) return;
		if (_navState.Page == -1) return;
		_isDataLoading = true;
		var queryDict = new Dictionary<string, object?>{
			{ TfConstants.PageQueryName, -1}
		};
		await Navigator.ApplyChangeToUrlQuery(queryDict);
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
	}

	private async Task _pageSizeChange(int pageSize)
	{
		if (_isDataLoading) return;
		if (pageSize < 0) pageSize = TfConstants.PageSize;
		if (_navState.PageSize == pageSize) return;
		try
		{
			var user = await TfUIService.SetPageSize(
						userId: TfAuthLayout.CurrentUser.Id,
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
	}

	private bool _columnIsVisible(TfDataColumn column)
	{
		if (_showJoinKeyColumns && column.IsShared) return true;
		if (_showSystemColumns && column.IsSystem) return true;
		if (_showProviderColumns && !column.IsSystem) return true;

		return false;
	}


	private bool _hasVisibleColumns()
	{
		if (_data is null) return false;
		foreach (var column in _data.Columns)
		{
			if (_columnIsVisible(column)) return true;
		}
		return false;
	}
}
