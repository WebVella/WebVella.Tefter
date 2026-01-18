namespace WebVella.Tefter.UI.Components;

public partial class TucSpaceViewPageContent : TfBaseComponent, IAsyncDisposable
{
	#region << Init >>

	[Parameter] public TfSpacePageAddonContext? Context { get; set; } = null;

	// State
	private DotNetObjectReference<TucSpaceViewPageContent> _objectRef = null!;
	private bool _isDataLoading = false;
	private bool _selectAllLoading = false;
	private ReadOnlyDictionary<Guid, ITfSpaceViewColumnTypeAddon> _columnTypeMetaDict = null!;
	private TfNavigationState _navState = null!;
	private TfUser _currentUser = null!;
	private TfSpace? _space = null;
	private TfSpacePage? _spacePage = null;
	private TfSpaceView? _spaceView = null;
	private TfDataset? _dataset = null;
	private TfDataProvider? _dataProvider = null;
	private TfSpaceViewPreset? _preset = null;
	private TfDataTable? _data = null;
	private List<TfSpaceViewColumn> _spaceViewColumns = new();
	private List<TfDataProvider>? _allDataProviders = null;
	private List<TfSharedColumn>? _allSharedColumns = null;
	private List<Guid> _selectedDataRows = new();
	private List<Guid> _editedDataRows = new();
	private bool _editAll = false;
	private readonly string _tableId = "space-view-table";
	private RenderFragment _caretDownInactive = null!;
	private RenderFragment _caretDown = null!;
	private RenderFragment _caretUp = null!;
	private bool _hasPinnedData = false;
	private IAsyncDisposable? _spacePageUpdatedEventSubscriber = null;
	private IAsyncDisposable? _spaceViewColumnUpdatedEventSubscriber = null;
	private IAsyncDisposable? _spaceViewDataReloadEventSubscriber = null;
	private IAsyncDisposable? _spaceViewDataUpdatedEventSubscriber = null;
	private IAsyncDisposable? _spaceViewUpdatedEventSubscriber = null;
	private IAsyncDisposable? _userUpdatedEventSubscriber = null;

	public async ValueTask DisposeAsync()
	{
		Navigator.LocationChanged -= On_NavigationStateChanged;
		if (_spacePageUpdatedEventSubscriber is not null)
			await _spacePageUpdatedEventSubscriber.DisposeAsync();
		if (_spaceViewColumnUpdatedEventSubscriber is not null)
			await _spaceViewColumnUpdatedEventSubscriber.DisposeAsync();
		if (_spaceViewDataReloadEventSubscriber is not null)
			await _spaceViewDataReloadEventSubscriber.DisposeAsync();
		if (_spaceViewDataUpdatedEventSubscriber is not null)
			await _spaceViewDataUpdatedEventSubscriber.DisposeAsync();
		if (_spaceViewUpdatedEventSubscriber is not null)
			await _spaceViewUpdatedEventSubscriber.DisposeAsync();
		if (_userUpdatedEventSubscriber is not null)
			await _userUpdatedEventSubscriber.DisposeAsync();
		_objectRef.Dispose();
		try
		{
			await JSRuntime.InvokeAsync<bool>("Tefter.removeColumnResizeListener", ComponentId);
			await JSRuntime.InvokeAsync<bool>("Tefter.removeColumnSortListener", ComponentId);
		}
		catch (Exception)
		{
			//ignored
		}
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Context is null)
			throw new Exception("Context cannot be null");
		_currentUser = Context!.CurrentUser;
		_columnTypeMetaDict = TfMetaService.GetSpaceViewColumnTypeDictionary();
		_objectRef = DotNetObjectReference.Create(this);
		await _init(TfAuthLayout.GetState().NavigationState, false);
		_caretDownInactive = builder =>
		{
			builder.OpenComponent<FluentIcon<Icon>>(0);
			builder.AddAttribute(1, "Value",
				TfConstants.GetIcon("ArrowSortUpLines", IconSize.Size16));
			builder.CloseComponent();
		};
		_caretDown = builder =>
		{
			builder.OpenComponent<FluentIcon<Icon>>(0);
			builder.AddAttribute(1, "Value",
				TfConstants.GetIcon("ArrowSortDownLines", IconSize.Size16));
			builder.CloseComponent();
		};
		_caretUp = builder =>
		{
			builder.OpenComponent<FluentIcon<Icon>>(0);
			builder.AddAttribute(1, "Value",
				TfConstants.GetIcon("ArrowSortUpLines", IconSize.Size16));
			builder.CloseComponent();
		};
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			Navigator.LocationChanged += On_NavigationStateChanged;

			_spaceViewColumnUpdatedEventSubscriber =
				await TfEventBus.SubscribeAsync<TfSpaceViewColumnUpdatedEventPayload>(
					handler: On_SpaceViewColumnUpdatedEventAsync,
					matchKey: (_) => true);
			_spacePageUpdatedEventSubscriber = await TfEventBus.SubscribeAsync<TfSpacePageUpdatedEventPayload>(
				handler: On_SpacePageUpdatedEventAsync);
			_spaceViewDataReloadEventSubscriber = await TfEventBus.SubscribeAsync<TfSpaceViewDataReloadEventPayload>(
				handler: On_SpaceViewDataReloadEventAsync,
				matchKey: (_) => true);
			_spaceViewDataUpdatedEventSubscriber = await TfEventBus.SubscribeAsync<TfSpaceViewDataUpdatedEventPayload>(
				handler: On_SpaceViewDataUpdatedEventAsync,
				matchKey: (_) => true);
			_spaceViewUpdatedEventSubscriber = await TfEventBus.SubscribeAsync<TfSpaceViewUpdatedEventPayload>(
				handler: On_SpaceViewUpdatedEventAsync,
				matchKey: (_) => true);
			_userUpdatedEventSubscriber = await TfEventBus.SubscribeAsync<TfUserUpdatedEventPayload>(
				handler: On_UserUpdatedEventAsync,
				matchKey: (key) => key == TfAuthLayout.GetUserId().ToString());


			try
			{
				await JSRuntime.InvokeVoidAsync("Tefter.makeTableResizable", _tableId);
				await JSRuntime.InvokeAsync<bool>("Tefter.addColumnResizeListener",
					_objectRef, ComponentId, "OnColumnResized");
				await JSRuntime.InvokeAsync<bool>("Tefter.addColumnSortListener",
					_objectRef, ComponentId, "OnColumnSort");
			}
			catch (Exception)
			{
				//ignored
			}
		}
	}

	private void On_NavigationStateChanged(object? caller, LocationChangedEventArgs args)
	{
		InvokeAsync(async () =>
		{
			if (UriInitialized != args.Location)
			{
				await _init(navState: TfAuthLayout.GetState().NavigationState);
			}
		});
	}

	private async Task On_UserUpdatedEventAsync(string? key, TfUserUpdatedEventPayload? payload)
	{
		if (payload is null) return;
		var contextHash = _getUserChangeHash(_currentUser);
		var payloadHash = _getUserChangeHash(payload.User);

		if (contextHash == payloadHash) return;

		_currentUser = payload.User with { Id = payload.User.Id };
		await _init(TfAuthLayout.GetState().NavigationState);
	}

	private async Task On_SpaceViewColumnUpdatedEventAsync(string? key, TfSpaceViewColumnUpdatedEventPayload? payload)
	{
		if (payload is null) return;
		var column = payload.SpaceViewColumns.Single(x => x.Id == payload.ColumnId);
		if (column.SpaceViewId != _spaceView?.Id) return;
		if (key == TfAuthLayout.GetSessionId().ToString())
		{
			_spaceViewColumns = payload.SpaceViewColumns;
			await _init(TfAuthLayout.GetState().NavigationState);
			await InvokeAsync(StateHasChanged);
		}
		else
			await TfEventBus.PublishAsync(key: key, new TfPageOutdatedAlertEventPayload());
	}

	private async Task On_SpaceViewUpdatedEventAsync(string? key, TfSpaceViewUpdatedEventPayload? payload)
	{
		if (payload is null) return;
		if (payload.SpaceView.Id != _spaceView?.Id) return;
		if (key == TfAuthLayout.GetSessionId().ToString())
		{
			_isDataLoading = true;
			await InvokeAsync(StateHasChanged);
			await _init(TfAuthLayout.GetState().NavigationState);
			_isDataLoading = false;
			await InvokeAsync(StateHasChanged);
		}
		else
			await TfEventBus.PublishAsync(key: key, new TfPageOutdatedAlertEventPayload());
	}

	private async Task On_SpaceViewDataReloadEventAsync(string? key, TfSpaceViewDataReloadEventPayload? payload)
	{
		if (payload is null) return;
		if (payload.SpaceViewId != _spaceView?.Id) return;
		if (key == TfAuthLayout.GetSessionId().ToString())
		{
			_selectedDataRows.Clear();
			await _init(TfAuthLayout.GetState().NavigationState);
		}
		else
			await TfEventBus.PublishAsync(key: key, new TfPageOutdatedAlertEventPayload());
	}

	private async Task On_SpaceViewDataUpdatedEventAsync(string? key, TfSpaceViewDataUpdatedEventPayload? payload)
	{
		if (payload is null) return;
		if (payload.SpaceViewId != _spaceView?.Id) return;
		if (key == TfAuthLayout.GetSessionId().ToString())
			OnDataChange(payload.DataDictionary);
		else
			await TfEventBus.PublishAsync(key: key, new TfPageOutdatedAlertEventPayload());
	}

	private async Task On_SpacePageUpdatedEventAsync(string? key, TfSpacePageUpdatedEventPayload? payload)
	{
		if (payload is null || payload.SpacePage.Id != _spacePage?.Id) return;
		_spacePage = payload.SpacePage;
		await InvokeAsync(StateHasChanged);
	}


	private async Task _init(TfNavigationState navState, bool showLoading = true)
	{
		_navState = navState;
		try
		{
			if (showLoading)
			{
				_isDataLoading = true;
				await InvokeAsync(StateHasChanged);
			}

			Guid? oldViewId = _spaceView?.Id;
			_spaceView = null;
			// if (_navState.SpaceId is null || _navState.SpacePageId is null)
			// 	return;

			_spacePage = Context!.SpacePage;
			// if (_spacePage is null || _spacePage.SpaceId != _navState.SpaceId.Value)
			// 	return;
			if (_spacePage is null)
				return;
			_space = Context!.Space;
			if (_space is null)
				return;
			if (_spacePage.Type != TfSpacePageType.Page &&
			    _spacePage.ComponentType.FullName != typeof(TucSpaceViewSpacePageAddon).FullName)
				return;
			var options = JsonSerializer.Deserialize<TfSpaceViewSpacePageAddonOptions>(_spacePage.ComponentOptionsJson);
			if (options is null || options.SpaceViewId is null)
				return;
			_spaceView = TfService.GetSpaceView(options.SpaceViewId.Value);
			if (_spaceView is null)
				return;

			_allDataProviders = TfService.GetDataProviders().ToList();
			_allSharedColumns = TfService.GetSharedColumns();
			if (oldViewId != options.SpaceViewId.Value)
			{
				_selectedDataRows.Clear();
				_editedDataRows.Clear();
			}

			_dataset = TfService.GetDataset(_spaceView.DatasetId);
			if (_dataset is null)
			{
				throw new Exception("Dataset was not found");
			}

			_dataProvider = _allDataProviders.FirstOrDefault(x => x.Id == _dataset!.DataProviderId);
			if (_dataProvider is null)
			{
				throw new Exception("Dataset was not found");
			}


			_preset = null;
			if (_navState.SpaceViewPresetId is not null)
				_preset = _spaceView!.Presets.GetPresetById(_navState.SpaceViewPresetId.Value);

			if (_preset is null && _spaceView!.Presets.Count > 0)
				_preset = _spaceView!.Presets[0];

			_spaceViewColumns = TfService.GetSpaceViewColumnsList(_spaceView.Id);

			string? pinnedDataIdentity = Navigator.GetStringFromQuery(TfConstants.DataIdentityIdQueryName);
			string? pinnedDataIdentityValue = Navigator.GetStringFromQuery(TfConstants.DataIdentityValueQueryName);
			_hasPinnedData = !string.IsNullOrWhiteSpace(pinnedDataIdentity) &&
			                 !string.IsNullOrWhiteSpace(pinnedDataIdentityValue);

			if (_hasPinnedData)
			{
				//Hardcoded relation is requested
				TfRelDataIdentityQueryInfo relInfo = new()
				{
					DataIdentity = pinnedDataIdentity!,
					RelDataIdentity = pinnedDataIdentity!,
					RelIdentityValues = [pinnedDataIdentityValue!]
				};
				_data = TfService.QueryDataProvider(_dataProvider,
					sorts: _dataset.SortOrders,
					relIdentityInfo: relInfo);
			}
			else
			{
				_data = TfService.QueryDataset(
					datasetId: _spaceView!.DatasetId,
					presetSearch: _preset?.Search,
					presetFilters: _preset?.Filters,
					presetSorts: _preset?.SortOrders,
					userSearch: _navState.Search,
					userFilters: _navState.Filters.ConvertQueryFilterToList(_spaceViewColumns, _allDataProviders,
						_allSharedColumns),
					userSorts: _navState.Sorts.ConvertQuerySortToList(_spaceViewColumns),
					page: 1,
					pageSize: TfConstants.ItemsMaxLimit
				);
			}

			foreach (TfDataRow row in _data.Rows)
			{
				row.OnEdit = () => _setRowEditable(row);
				row.OnDblClick = () => _rowDoubleClick(row);
				row.OnSelect = () => _toggleItemSelection(row);
			}

			_generateMeta();
		}
		finally
		{
			UriInitialized = _navState.Uri;
			if (showLoading)
			{
				_isDataLoading = false;
				await InvokeAsync(StateHasChanged);
			}
		}
	}

	private async Task _renamePage()
	{
		_ = await DialogService.ShowDialogAsync<TucSpacePageRenameDialog>(
			_spacePage!,
			new()
			{
				PreventDismissOnOverlayClick = true,
				PreventScroll = true,
				Width = TfConstants.DialogWidthSmall,
				TrapFocus = false,
				ShowTitle = false
			});
	}

	private async Task _addPreset()
	{
		var context = new TfPresetFilterManagementContext { Item = null, DateSet = _dataset, SpaceView = _spaceView };
		var dialog = await DialogService.ShowDialogAsync<TucPresetFilterManageDialog>(
			context,
			new()
			{
				PreventDismissOnOverlayClick = true,
				PreventScroll = true,
				Width = TfConstants.DialogWidthExtraLarge,
				TrapFocus = false
			});
		_ = await dialog.Result;
	}

	#endregion

	#region << Public methods >>

	public TfDataTable? GetCurrentData() => _data;

	// Search, Filter, Sort Handlers
	public async Task OnSearch(string value)
	{
		if (_isDataLoading) return;
		var queryDict = new Dictionary<string, object?> { { TfConstants.SearchQueryName, value } };
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}

	public async Task OnFilter(List<TfFilterQuery>? filters)
	{
		if (_isDataLoading) return;
		var queryDict = new Dictionary<string, object?>();
		if (filters is null || filters.Count == 0)
			queryDict[TfConstants.FiltersQueryName] = null;
		else
			queryDict[TfConstants.FiltersQueryName] = filters.SerializeFiltersForUrl(false);

		queryDict[TfConstants.PageQueryName] = 1;
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}

	public async Task OnSort(List<TfSortQuery>? sorts)
	{
		if (_isDataLoading) return;
		var queryDict = new Dictionary<string, object?>();
		if (sorts is null || sorts.Count == 0)
			queryDict[TfConstants.SortsQueryName] = null;
		else
			queryDict[TfConstants.SortsQueryName] = sorts.SerializeSortsForUrl(false);

		queryDict[TfConstants.PageQueryName] = 1;

		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}

	public async Task OnClearFilter()
	{
		if (_isDataLoading) return;
		var queryDict = new Dictionary<string, object?>
		{
			{ TfConstants.SearchQueryName, null },
			{ TfConstants.FiltersQueryName, null },
			{ TfConstants.SortsQueryName, null },
			{ TfConstants.PageQueryName, 1 }
		};
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}

	public async Task OnClearPersonalization()
	{
		try
		{
			var result = await TfService.RemoveSpaceViewPersonalizations(
				userId: _currentUser.Id,
				spaceViewId: _spaceView!.Id,
				presetId: _preset?.Id
			);
			await TfEventBus.PublishAsync(key: TfAuthLayout.GetSessionId(),
				payload: new TfUserUpdatedEventPayload(result));
		}
		catch (Exception)
		{
			//Do nothing
		}
	}

	//Select Handlers
	public async Task OnSelectAll()
	{
		try
		{
			_selectAllLoading = true;
			await InvokeAsync(StateHasChanged);
			await Task.Delay(1);
			_selectedDataRows = TfService.QueryDatasetIdList(
				datasetId: _spaceView!.DatasetId,
				presetFilters: _preset?.Filters,
				presetSorts: _preset?.SortOrders,
				userFilters: _navState.Filters.ConvertQueryFilterToList(_spaceViewColumns, _allDataProviders!,
					_allSharedColumns!),
				userSorts: _navState.Sorts.ConvertQuerySortToList(_spaceViewColumns),
				userSearch: _navState.Search
			);
		}
		catch (Exception)
		{
			//Do nothing
		}
		finally
		{
			_selectAllLoading = false;
			_generateMeta();
			await InvokeAsync(StateHasChanged);
		}
	}

	public async Task OnDeSelectAll()
	{
		try
		{
			_selectAllLoading = true;
			await InvokeAsync(StateHasChanged);
			await Task.Delay(1);
			_selectedDataRows = new();
		}
		catch (Exception)
		{
			//Do nothing
		}
		finally
		{
			_selectAllLoading = false;
			_generateMeta();
			await InvokeAsync(StateHasChanged);
		}
	}

	public void OnNewRow(TfDataTable dataTable)
	{
		if (_data is null) return;
		_data.Rows.Insert(0, dataTable.Rows[0]);
		_editedDataRows.Add(dataTable.Rows[0].GetRowId());
		_generateMeta();
		StateHasChanged();
	}

	public void ToggleEditAll()
	{
		_editAll = !_editAll;
		_generateMeta();
		StateHasChanged();
	}

	public bool GetEditAll() => _editAll;

	public void OnDeleteRows(List<Guid>? tfIds)
	{
		if (_spaceView is null) return;
		if (tfIds is null || tfIds.Count == 0) return;
		try
		{
			var spaceData = TfService.GetDataset(_spaceView.DatasetId);
			TfService.DeleteDataProviderRowsByTfId(
				providerId: spaceData!.DataProviderId,
				idList: _selectedDataRows
			);
			ToastService.ShowSuccess(LOC("Records deleted"));
			TfEventBus.Publish(TfAuthLayout.GetSessionId().ToString(),
				new TfDataProviderDataChangedEventPayload(_dataProvider!));
			Navigator.ReloadCurrentUrl();
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
	}

	public void OnDataChange(Dictionary<Guid, Dictionary<string, object>>? change)
	{
		if (_data is null || change is null || change.Keys.Count == 0) return;
		foreach (var rowId in change.Keys)
		{
			var row = _data.Rows[rowId];
			if (row is null) continue;
			foreach (var columnName in change[rowId].Keys)
			{
				var column = _data.Columns[columnName];
				if (column is null) continue;
				_data[rowId, columnName] = change[rowId][columnName];
			}
		}

		StateHasChanged();
	}

	public void OnEditRows(List<Guid>? tfIds)
	{
		if (tfIds is null || tfIds.Count == 0) return;
		_editedDataRows = _editedDataRows.Union(tfIds).ToList();
		_generateMeta();
		StateHasChanged();
	}

	#endregion

	#region <<Utility Methods >>

	private string _getNoDataString()
	{
		var last30Jobs =
			TfService.GetDataProviderSynchronizationTasks(_data!.QueryInfo.DataProviderId, page: 1, pageSize: 30);
		var syncRunning = last30Jobs.Any(x =>
			x.Status == TfSynchronizationStatus.Pending || x.Status == TfSynchronizationStatus.InProgress);
		if (syncRunning)
			return LOC("Data import is currently running. Please try again in a minute.");
		return LOC("No data");
	}

	private string _getUserChangeHash(TfUser? user)
	{
		if (user is null) return "";
		return JsonSerializer.Serialize(user.Settings.ViewPresetColumnPersonalizations)
		       + JsonSerializer.Serialize(user.Settings.ViewPresetSortPersonalizations);
	}

	private async Task _onRowChanged(TfSpaceViewColumnDataChange change)
	{
		if (_data is null) return;
		try
		{
			var rowDict = new Dictionary<string, object?>();
			foreach (var columnName in change.DataChange.Keys)
			{
				rowDict[columnName] = change.DataChange[columnName];
			}

			_ = TfService.UpdateProviderRow(
				tfId: change.RowId,
				providerId: _dataset!.DataProviderId,
				rowDict: rowDict
			);

			//Apply changed to the datatable
			if (_data is null || _data.Rows.Count == 0 || _data.Rows.Count == 0) return;

			for (int i = 0; i < _data.Rows.Count; i++)
			{
				TfDataRow row = _data.Rows[i];
				Guid tfId = row.GetRowId();
				if (change.RowId != tfId) continue;

				foreach (var column in _data.Columns)
				{
					if (column.Origin == TfDataColumnOriginType.System ||
					    column.Origin == TfDataColumnOriginType.Identity) continue;
					if (rowDict.ContainsKey(column.Name))
						row[column.Name] = rowDict[column.Name];
				}
			}

			_generateMeta();
			await InvokeAsync(StateHasChanged);
			ToastService.ShowSuccess(LOC("Data was updated"));
			await TfEventBus.PublishAsync(TfAuthLayout.GetSessionId().ToString(),
				new TfDataProviderDataChangedEventPayload(_dataProvider!));
		}
		catch (Exception ex)
		{
			throw new TfException(LOC($"Data change failed! {ex.Message}"), ex);
		}
	}

	private void _toggleItemSelection(TfDataRow row)
	{
		var rowId = row.GetRowId();
		if (!_selectedDataRows.Contains(rowId))
		{
			_selectedDataRows.Add(rowId);
		}
		else
		{
			_selectedDataRows.RemoveAll(x => x == rowId);
		}

		_generateMeta();
	}

	private async Task _toggleSelectAll(bool isChecked)
	{
		if (isChecked)
			await OnSelectAll();
		else
			await OnDeSelectAll();
	}

	public bool _allDataRowsSelected()
	{
		if (_data is null || _data.Rows.Count == 0) return false;
		var allSelected = true;
		for (int i = 0; i < _data.Rows.Count; i++)
		{
			var rowId = (Guid)(_data.Rows[i][TfConstants.TEFTER_ITEM_ID_PROP_NAME] ?? Guid.Empty);
			if (!_selectedDataRows.Contains(rowId))
			{
				allSelected = false;
				break;
			}
		}

		return allSelected;
	}

	private void _setRowEditable(TfDataRow row)
	{
		if (!_spaceView!.CanUpdateRecords(_currentUser)) return;
		var rowId = row.GetRowId();
		if (!_editedDataRows.Contains(rowId))
		{
			_editedDataRows.Add(rowId);
		}
		else
		{
			_editedDataRows.RemoveAll(x => x == rowId);
		}

		_generateMeta();
	}

	private void _rowDoubleClick(TfDataRow row)
	{
		if (!_spaceView!.CanUpdateRecords(_currentUser)) return;
		var rowId = row.GetRowId();
		if (!_editedDataRows.Contains(rowId))
		{
			_editedDataRows.Add(rowId);
		}

		_generateMeta();
	}


	[JSInvokable("OnColumnSort")]
	public async void OnColumnSort(int position, bool hasShift)
	{
		try
		{
			if (_isDataLoading) return;
			var column = _spaceViewColumns.SingleOrDefault(x => x.Position == position);
			if (column is null) return;
			var sorts = TfService.CalculateViewPresetSortPersonalization(
				currentSorts: _navState.Sorts ?? new(),
				spaceViewId: _spaceView!.Id,
				spaceViewColumnId: column.Id,
				hasShiftKey: hasShift);

			await OnSort(sorts);
		}
		catch (Exception)
		{
			// nothing
		}
	}

	[JSInvokable("OnColumnResized")]
	public async Task OnColumnResized(int position, short width)
	{
		var column = _spaceViewColumns.SingleOrDefault(x => x.Position == position);
		if (column is null) return;
		var result = await TfService.SetViewPresetColumnPersonalization(
			userId: _currentUser.Id,
			spaceViewId: _spaceView!.Id,
			presetId: _preset?.Id,
			spaceViewColumnId: column.Id,
			width: width);
		await TfEventBus.PublishAsync(key: TfAuthLayout.GetSessionId(),
			payload: new TfUserUpdatedEventPayload(result));
	}

	#endregion
}