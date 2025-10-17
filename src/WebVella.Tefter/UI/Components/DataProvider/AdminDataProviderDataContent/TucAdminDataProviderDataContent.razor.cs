namespace WebVella.Tefter.UI.Components;

public partial class TucAdminDataProviderDataContent : TfBaseComponent, IDisposable
{
	[Inject] protected TfGlobalEventProvider TfEventProvider { get; set; } = null!;
	private TfDataProvider? _provider = null;
	private TfNavigationState _navState = null!;
	private bool _isDataLoading = false;
	private bool _showSystemColumns = false;
	private bool _showJoinKeyColumns = false;
	private bool _showProviderColumns = true;
	private long _totalRows = 0;
	private TfDataTable? _data = null;
	private bool _syncRunning = false;
	public void Dispose()
	{
		Navigator.LocationChanged -= On_NavigationStateChanged;
		TfEventProvider.Dispose();
	}
	protected override async Task OnInitializedAsync()
	{
		await _init(TfAuthLayout.GetState().NavigationState);
		Navigator.LocationChanged += On_NavigationStateChanged;
		TfEventProvider.DataProviderUpdatedEvent += On_DataProviderUpdated;
	}
	private void On_NavigationStateChanged(object? caller, LocationChangedEventArgs args)
	{
		InvokeAsync(async () =>
		{
			if (UriInitialized != args.Location)
				await _init(TfAuthLayout.GetState().NavigationState);
		});
	}

	private async Task On_DataProviderUpdated(TfDataProviderUpdatedEvent args)
	{
		await InvokeAsync(async () =>
		{
			await _init(TfAuthLayout.GetState().NavigationState);
		});
	}

	private async Task _init(TfNavigationState navState)
	{
		try
		{
			if (navState.DataProviderId is null)
			{
				_provider = null;
				await InvokeAsync(StateHasChanged);
				return;
			}
			_navState = navState;
			_provider = TfService.GetDataProvider(_navState.DataProviderId.Value);
			if (_provider is null)
				return;
			_syncRunning = TfService.GetDataProviderSynchronizationTasks(_provider.Id, status: TfSynchronizationStatus.InProgress).Count > 0;

			_isDataLoading = true;
			await InvokeAsync(StateHasChanged);
			_totalRows = TfService.GetDataProviderRowsCount(_provider.Id);
			_data = TfService.QueryDataProvider(
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
	private void _toggleCustomColumns()
	{
		_showProviderColumns = !_showProviderColumns;
		StateHasChanged();
	}

	private async Task _deleteAllData()
	{
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need all data deleted? This operation can take minutes!")))
			return;
		TfService.DeleteAllProviderRows(_provider!.Id);
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
				new ()
				{
					PreventDismissOnOverlayClick = true,
					PreventScroll = true,
					Width = TfConstants.DialogWidthLarge,
					TrapFocus = false
				});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			await _init(TfAuthLayout.GetState().NavigationState);
			await InvokeAsync(StateHasChanged);
		}
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
				new ()
				{
					PreventDismissOnOverlayClick = true,
					PreventScroll = true,
					Width = TfConstants.DialogWidthLarge,
					TrapFocus = false
				});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			await _init(TfAuthLayout.GetState().NavigationState);
			await InvokeAsync(StateHasChanged);
		}
	}
	private async Task _deleteRow(TfDataRow row)
	{
		//TODO BOZ: implement
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

	private bool _columnIsVisible(TfDataColumn column)
	{
		if (column.OriginType == TfDataColumnOriginType.System
		    || column.OriginType == TfDataColumnOriginType.Identity)
			return _showSystemColumns;
		else if(column.OriginType == TfDataColumnOriginType.SharedColumn
		        || column.OriginType == TfDataColumnOriginType.JoinedProviderColumn)
			return _showJoinKeyColumns;
		return _showProviderColumns;
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
