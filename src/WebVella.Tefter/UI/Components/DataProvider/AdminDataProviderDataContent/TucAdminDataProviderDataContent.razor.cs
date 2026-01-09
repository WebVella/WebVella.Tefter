namespace WebVella.Tefter.UI.Components;

public partial class TucAdminDataProviderDataContent : TfBaseComponent, IAsyncDisposable
{
	private TfDataProvider? _provider = null;
	private TfNavigationState _navState = null!;
	private bool _isDataLoading = false;
	private bool _showSystemColumns = false;
	private bool _showProviderColumns = true;
	private long _totalRows = 0;
	private TfDataTable? _data = null;
	private bool _syncRunning = false;
	private Guid? _deletedRowId = null;
	private IAsyncDisposable? _dataProviderUpdatedEventSubscriber = null;

	public async ValueTask DisposeAsync()
	{
		Navigator.LocationChanged -= On_NavigationStateChanged;
		if (_dataProviderUpdatedEventSubscriber is not null)
			await _dataProviderUpdatedEventSubscriber.DisposeAsync();
	}

	protected override async Task OnInitializedAsync()
	{
		await _init(TfAuthLayout.GetState().NavigationState);
		Navigator.LocationChanged += On_NavigationStateChanged;

		_dataProviderUpdatedEventSubscriber = await TfEventBus.SubscribeAsync<TfDataProviderUpdatedEventPayload>(
			handler: On_DataProviderUpdatedEventAsync,
			matchKey: (_) => true);
	}

	private void On_NavigationStateChanged(object? caller, LocationChangedEventArgs args)
	{
		InvokeAsync(async () =>
		{
			if (UriInitialized != args.Location)
				await _init(TfAuthLayout.GetState().NavigationState);
		});
	}

	private async Task On_DataProviderUpdatedEventAsync(string? key, TfDataProviderUpdatedEventPayload? payload)
	{
		if (payload is null) return;
		if (payload.DataProvider.Id != _provider?.Id) return;
		if (key == TfAuthLayout.GetSessionId().ToString())
			await _init(TfAuthLayout.GetState().NavigationState);
		else
			await TfEventBus.PublishAsync(key: key, new TfPageOutdatedAlertEventPayload());
	}

	private async Task _init(TfNavigationState navState)
	{
		if (navState.DataProviderId is null)
		{
			_provider = null;
			await InvokeAsync(StateHasChanged);
			return;
		}

		_navState = navState;
		try
		{
			_provider = TfService.GetDataProvider(_navState.DataProviderId.Value);
			if (_provider is null)
				return;
			_syncRunning =
				TfService.GetDataProviderSynchronizationTasks(_provider.Id, status: TfSynchronizationStatus.InProgress)
					.Count > 0;

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
				row.OnEdit = () =>
				{
					InvokeAsync(async () =>
					{
						await _editRow(row);
					});
				};
				row.OnDelete = () =>
				{
					InvokeAsync(async () =>
					{
						await _deleteRow(row);
					});
				};
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
		if (!await JSRuntime.InvokeAsync<bool>("confirm",
			    LOC("Are you sure that you need all data deleted? This operation can take minutes!")))
			return;
		TfService.DeleteAllProviderRows(_provider!.Id);
		ToastService.ShowSuccess(LOC("Data provider data deletion is triggered!"));
		await TfEventBus.PublishAsync(TfAuthLayout.GetSessionId().ToString(),
			new TfDataProviderDataChangedEventPayload(_provider!));
		Navigator.ReloadCurrentUrl();
	}

	private async Task _addRow()
	{
		if (_provider is null || _data is null) return;
		var dialog = await DialogService.ShowDialogAsync<TucDataProviderManageDataDialog>(
			new TfManageDataProviderRowContext { Provider = _provider, RowId = null, Data = _data },
			new()
			{
				PreventDismissOnOverlayClick = true,
				PreventScroll = true,
				Width = TfConstants.DialogWidthLarge,
				TrapFocus = false
			});
		var result = await dialog.Result;
		if (result is { Cancelled: false, Data: not null })
		{
			await _init(TfAuthLayout.GetState().NavigationState);
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _editRow(TfDataRow row)
	{
		if (_provider is null || _data is null) return;
		var dialog = await DialogService.ShowDialogAsync<TucDataProviderManageDataDialog>(
			new TfManageDataProviderRowContext { Provider = _provider, RowId = row.GetRowId(), Data = _data },
			new()
			{
				PreventDismissOnOverlayClick = true,
				PreventScroll = true,
				Width = TfConstants.DialogWidthLarge,
				TrapFocus = false
			});
		var result = await dialog.Result;
		if (result is { Cancelled: false, Data: not null })
		{
			await _init(TfAuthLayout.GetState().NavigationState);
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _deleteRow(TfDataRow row)
	{
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this record deleted?")))
			return;
		if (_deletedRowId is not null)
			return;
		try
		{
			_deletedRowId = row.GetRowId();
			await InvokeAsync(StateHasChanged);
			await Task.Delay(1);

			TfService.DeleteDataProviderRowByTfId(_provider!, _deletedRowId.Value);
			await _init(TfAuthLayout.GetState().NavigationState);
			await TfEventBus.PublishAsync(TfAuthLayout.GetSessionId().ToString(),
				new TfDataProviderDataChangedEventPayload(_provider!));
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			_deletedRowId = null;
			await InvokeAsync(StateHasChanged);
		}
	}


	private async Task _onSearch(string value)
	{
		if (_isDataLoading) return;
		_isDataLoading = true;
		var queryDict = new Dictionary<string, object?> { { TfConstants.SearchQueryName, value } };
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}

	private bool _columnIsVisible(TfDataColumn column)
	{
		if (column.Origin == TfDataColumnOriginType.System
		    || column.Origin == TfDataColumnOriginType.Identity)
			return _showSystemColumns;
		else if (column.Origin == TfDataColumnOriginType.SharedColumn
		         || column.Origin == TfDataColumnOriginType.JoinedProviderColumn)
			return false;
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