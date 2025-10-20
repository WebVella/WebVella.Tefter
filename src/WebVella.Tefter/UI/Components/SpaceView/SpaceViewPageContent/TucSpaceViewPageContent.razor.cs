﻿namespace WebVella.Tefter.UI.Components;

public partial class TucSpaceViewPageContent : TfBaseComponent, IAsyncDisposable
{
	#region << Init >>

	[Inject] protected TfGlobalEventProvider TfEventProvider { get; set; } = null!;
	[Parameter] public TfSpacePageAddonContext? Context { get; set; } = null;

	// State
	private DotNetObjectReference<TucSpaceViewPageContent> _objectRef;
	private bool _isDataLoading = false;
	private bool _selectAllLoading = false;
	private ReadOnlyDictionary<Guid, ITfSpaceViewColumnTypeAddon> _columnTypeMetaDict = null!;
	private TfNavigationState _navState = null!;
	private TfUser _currentUser = null!;
	private TfSpace? _space = null;
	private TfSpacePage? _spacePage = null;
	private TfSpaceView? _spaceView = null;
	private TfDataset? _spaceData = null;
	private TfDataProvider? _dataProvider = null;
	private TfSpaceViewPreset? _preset = null;
	private TfDataTable? _data = null;
	private List<TfSpaceViewColumn> _spaceViewColumns = new();
	private List<TfDataProvider>? _allDataProviders = null;
	private List<TfSharedColumn>? _allSharedColumns = null;
	private List<Guid> _selectedDataRows = new();
	private List<Guid> _editedDataRows = new();
	private bool _editAll = false;
	private Dictionary<string, object> _contextViewData = new();
	private string _tableId = "space-view-table";
	private RenderFragment _caretDownInactive = null!;
	private RenderFragment _caretDown = null!;
	private RenderFragment _caretUp = null!;
	private RenderFragment _manageIcon = null!;
	private string? _managePageUrl = null;
	private Dictionary<string, object>? _manageBtnAttributes = new();
	private string _userChangeCheckHash = String.Empty;

	public async ValueTask DisposeAsync()
	{
		Navigator.LocationChanged -= On_NavigationStateChanged;
		TfEventProvider?.DisposeAsync();
		_objectRef?.Dispose();
		try
		{
			await JSRuntime.InvokeAsync<bool>("Tefter.removeColumnResizeListener", ComponentId);
			await JSRuntime.InvokeAsync<bool>("Tefter.removeColumnSortListener", ComponentId);
		}
		catch { }
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Context is null)
			throw new Exception("Context cannot be null");
		_columnTypeMetaDict = TfMetaService.GetSpaceViewColumnTypeDictionary();
		_objectRef = DotNetObjectReference.Create(this);
		await _init(TfAuthLayout.GetState().NavigationState, false);
		_caretDownInactive = builder =>
		{
			builder.OpenComponent<FluentIcon<Icon>>(0);
			builder.AddAttribute(1, "Value",
				TfConstants.GetIcon("ArrowSortUpLines", IconSize.Size16)!.WithColor("var(--neutral-stroke-rest)"));
			builder.CloseComponent();
		};
		_caretDown = builder =>
		{
			builder.OpenComponent<FluentIcon<Icon>>(0);
			builder.AddAttribute(1, "Value",
				TfConstants.GetIcon("ArrowSortDownLines", IconSize.Size16)!.WithColor("var(--neutral-stroke-rest)"));
			builder.CloseComponent();
		};
		_caretUp = builder =>
		{
			builder.OpenComponent<FluentIcon<Icon>>(0);
			builder.AddAttribute(1, "Value",
				TfConstants.GetIcon("ArrowSortUpLines", IconSize.Size16)!.WithColor("var(--neutral-stroke-rest)"));
			builder.CloseComponent();
		};
		_manageIcon = builder =>
		{
			builder.OpenComponent<FluentIcon<Icon>>(0);
			builder.AddAttribute(1, "Value",
				TfConstants.GetIcon("Settings", IconSize.Size16)!.WithColor("var(--neutral-stroke-rest)"));
			builder.CloseComponent();
		};
		_manageBtnAttributes!.Add("title", LOC("Manage Page"));
		_managePageUrl = string.Format(TfConstants.SpacePagePageManageUrl, Context.SpacePage.SpaceId,
			Context.SpacePage.Id).GenerateWithLocalAsReturnUrl(Navigator.Uri);
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			Navigator.LocationChanged += On_NavigationStateChanged;
			TfEventProvider.UserUpdatedEvent += On_UserChanged;
			TfEventProvider.SpaceViewColumnsChangedEvent += On_SpaceViewColumnsChanged;
			TfEventProvider.SpaceViewDataChangedEvent += On_SpaceViewDataChanged;
			await JSRuntime.InvokeVoidAsync("Tefter.makeTableResizable", _tableId);
			await JSRuntime.InvokeAsync<bool>("Tefter.addColumnResizeListener",
				_objectRef, ComponentId, "OnColumnResized");
			await JSRuntime.InvokeAsync<bool>("Tefter.addColumnSortListener",
				_objectRef, ComponentId, "OnColumnSort");
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

	private async Task On_UserChanged(TfUserUpdatedEvent args)
	{
		await InvokeAsync(async () =>
		{
			var contextHash = _getUserChangeHash(Context!.CurrentUser);
			var payloadHash = _getUserChangeHash(args.Payload);

			if (contextHash == payloadHash) return;

			Context.CurrentUser = args.Payload;
			await _init(TfAuthLayout.GetState().NavigationState);
		});
	}

	private async Task On_SpaceViewColumnsChanged(TfSpaceViewColumnsChangedEvent args)
	{
		await InvokeAsync(async () =>
		{
			if (args.UserId != TfAuthLayout.GetState().User.Id) return;
			if (args.SpaceViewId != _spaceView?.Id) return;
			_spaceViewColumns = args.Payload;
			await _init(TfAuthLayout.GetState().NavigationState);
		});
	}
	
	private async Task On_SpaceViewDataChanged(TfSpaceViewDataChangedEvent args)
	{
		await InvokeAsync(async () =>
		{
			if (args.UserId != TfAuthLayout.GetState().User.Id) return;
			if (args.SpaceViewId != _spaceView?.Id) return;
			OnDataChange(args.Payload);
		});
	}	

	private async Task _init(TfNavigationState navState, bool showLoading = true)
	{
		try
		{
			if (showLoading)
			{
				_isDataLoading = true;
				await InvokeAsync(StateHasChanged);
			}

			_navState = navState;

			Guid? oldViewId = _spaceView is not null ? _spaceView.Id : null;
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

			if (_allDataProviders is null)
				_allDataProviders = TfService.GetDataProviders().ToList();
			if (_allSharedColumns is null)
				_allSharedColumns = TfService.GetSharedColumns();

			if (oldViewId != options.SpaceViewId.Value)
			{
				_contextViewData = new();
				_spaceData = TfService.GetDataset(_spaceView.DatasetId);
				_dataProvider = _allDataProviders.FirstOrDefault(x => x.Id == _spaceData.DataProviderId);
				_selectedDataRows = new();
				_editedDataRows = new();
			}

			_currentUser = Context!.CurrentUser;
			_preset = null;
			if (_navState.SpaceViewPresetId is not null)
				_preset = _spaceView!.Presets.GetPresetById(_navState.SpaceViewPresetId.Value);

			_spaceViewColumns = TfService.GetSpaceViewColumnsList(_spaceView.Id);

			_data = TfService.QueryDataset(
				datasetId: _spaceView!.DatasetId,
				presetFilters: _preset is not null ? _preset.Filters : null,
				presetSorts: _preset is not null ? _preset.SortOrders : null,
				userFilters: _navState.Filters.ConvertQueryFilterToList(_spaceViewColumns, _allDataProviders,
					_allSharedColumns),
				userSorts: _navState.Sorts.ConvertQuerySortToList(_spaceViewColumns),
				search: _navState.Search,
				page: 1,
				pageSize: TfConstants.ItemsMaxLimit
			);

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

	#endregion

	#region << Public methods >>

	public TfDataTable? GetCurrentData() => _data;

	// Search, Filter, Sort Handlers
	public async Task OnSearch(string value)
	{
		if (_isDataLoading) return;
		var queryDict = new Dictionary<string, object> { { TfConstants.SearchQueryName, value } };
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}

	public async Task OnFilter(List<TfFilterQuery> filters)
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

	public async Task OnSort(List<TfSortQuery> sorts)
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
			_ = await TfService.RemoveSpaceViewPersonalizations(
				userId: _currentUser.Id,
				spaceViewId: _spaceView.Id,
				presetId: _preset?.Id
			);
		}
		catch { }
	}

	//Select Handlers
	public async Task OnSelectAll()
	{
		try
		{
			_selectAllLoading = true;
			await InvokeAsync(StateHasChanged);
			await Task.Delay(1);
			TfSpaceViewPreset? preset = null;
			if (_navState.SpaceViewPresetId is not null)
				preset = _spaceView!.Presets.GetPresetById(_navState.SpaceViewPresetId.Value);

			_selectedDataRows = TfService.QueryDatasetIdList(
				datasetId: _spaceView!.DatasetId,
				presetFilters: preset is not null ? preset.Filters : null,
				presetSorts: preset is not null ? preset.SortOrders : null,
				userFilters: _navState.Filters.ConvertQueryFilterToList(_spaceViewColumns, _allDataProviders,
					_allSharedColumns),
				userSorts: _navState.Sorts.ConvertQuerySortToList(_spaceViewColumns),
				search: _navState.Search
			);
		}
		catch { }
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
		catch { }
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

	public void OnDeleteRows(List<Guid> tfIds)
	{
		if (_spaceView is null) return;
		if (tfIds is null || tfIds.Count == 0) return;
		try
		{
			var spaceData = TfService.GetDataset(_spaceView.DatasetId);
			TfService.DeleteDataProviderRowsByTfId(
				providerId: spaceData.DataProviderId,
				idList: _selectedDataRows
			);
			ToastService.ShowSuccess(LOC("Records deleted"));
			Navigator.ReloadCurrentUrl();
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
	}

	public void OnDataChange(Dictionary<Guid, Dictionary<string, object>> change)
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
	
	public void OnEditRows(List<Guid> tfIds)
	{
		if (tfIds is null || tfIds.Count == 0) return;
		_editedDataRows = _editedDataRows.Union(tfIds).ToList();
		_generateMeta();
		StateHasChanged();
	}

	#endregion

	#region <<Utility Methods >>
	
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
			var value = _data.NewTableFromRowIds(change.RowId);
			if (value.Rows.Count == 0)
			{
				ToastService.ShowError(LOC("Row with id {0} is not found", change.RowId));
				return;
			}

			foreach (var columnName in change.DataChange.Keys)
			{
				value.Rows[0][columnName] = change.DataChange[columnName];
			}

			var changedRow = value.Rows[0];
			var changedRowTfId = changedRow.GetRowId();

			TfService.SaveDataTable(value);

			//Apply changed to the datatable
			if (_data is null || _data.Rows.Count == 0 || _data.Rows.Count == 0) return;

			for (int i = 0; i < _data.Rows.Count; i++)
			{
				TfDataRow row = _data.Rows[i];
				Guid tfId = row.GetRowId();
				if (changedRowTfId != tfId) continue;

				for (int j = 0; j < _data.Columns.Count; j++)
				{
					TfDataColumn column = _data.Columns[j];
					if (column.Origin == TfDataColumnOriginType.System ||
					    column.Origin == TfDataColumnOriginType.Identity) continue;
					row[column.Name] = changedRow[column.Name];
				}
			}
			_generateMeta();
			await InvokeAsync(StateHasChanged);
			ToastService.ShowSuccess(LOC("Data was updated"));
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

	private void _toggleSelectAll(bool isChecked)
	{
		if (_data is null
		    || _data.Rows is null
		    || _data.Rows.Count == 0)
			return;
		for (int i = 0; i < _data.Rows.Count; i++)
		{
			var rowId = (Guid)_data.Rows[i][TfConstants.TEFTER_ITEM_ID_PROP_NAME];
			if (!isChecked)
			{
				_selectedDataRows.RemoveAll(x => x == rowId);
			}
			else if (!_selectedDataRows.Any(x => x == rowId))
			{
				_selectedDataRows.Add(rowId);
			}
		}

		_generateMeta();
	}

	public bool _allDataRowsSelected()
	{
		if (_data is null || _data.Rows.Count == 0) return false;
		var allSelected = true;
		for (int i = 0; i < _data.Rows.Count; i++)
		{
			var rowId = (Guid)_data.Rows[i][TfConstants.TEFTER_ITEM_ID_PROP_NAME];
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
		if (!_spaceView.Settings.CanUpdateRows) return;
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
		if (!_spaceView.Settings.CanUpdateRows) return;
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
		if (_isDataLoading == true) return;
		var column = _spaceViewColumns.SingleOrDefault(x => x.Position == position);
		if (column is null) return;
		var sorts = TfService.CalculateViewPresetSortPersonalization(
			currentSorts: _navState.Sorts ?? new(),
			spaceViewId: _spaceView.Id,
			spaceViewColumnId: column.Id,
			hasShiftKey: hasShift);

		await OnSort(sorts is null ? new List<TfSortQuery>() : sorts);
	}

	[JSInvokable("OnColumnResized")]
	public async Task OnColumnResized(int position, short width)
	{
		var column = _spaceViewColumns.SingleOrDefault(x => x.Position == position);
		if (column is null) return;
		await TfService.SetViewPresetColumnPersonalization(
			userId: _currentUser.Id,
			spaceViewId: _spaceView.Id,
			presetId: _preset?.Id,
			spaceViewColumnId: column.Id,
			width: width);
	}

	private async Task _manageColumn(TfSpaceViewColumn column)
	{
		var dialog = await DialogService.ShowDialogAsync<TucSpaceViewColumnManageDialog>(
			column,
			new()
			{
				PreventDismissOnOverlayClick = true,
				PreventScroll = true,
				Width = TfConstants.DialogWidthLarge,
				TrapFocus = false
			});
		_ = await dialog.Result;
	}

	#endregion
}