namespace WebVella.Tefter.UI.Components;

public partial class TucSpaceViewPageContent : TfBaseComponent, IAsyncDisposable
{
	#region << Init >>
	// Dependency Injection
	[Inject] private ITfSpaceUIService TfSpaceUIService { get; set; } = default!;
	[Inject] private ITfSpaceViewUIService TfSpaceViewUIService { get; set; } = default!;
	[Inject] private ITfSpaceDataUIService TfSpaceDataUIService { get; set; } = default!;
	[Inject] private ITfDataProviderUIService TfDataProviderUIService { get; set; } = default!;
	[Inject] private ITfSharedColumnUIService TfSharedColumnUIService { get; set; } = default!;
	[Inject] private ITfUserUIService TfUserUIService { get; set; } = default!;
	[Inject] private ITfMetaUIService TfMetaUIService { get; set; } = default!;
	[Inject] public ITfNavigationUIService TfNavigationUIService { get; set; } = default!;
	[Parameter] public TfSpacePageAddonContext? Context { get; set; } = null;

	// State
	private DotNetObjectReference<TucSpaceViewPageContent> _objectRef;
	private bool _isDataLoading = true;
	private bool _selectAllLoading = false;
	private ReadOnlyDictionary<Guid, TfSpaceViewColumnComponentAddonMeta> _componentMetaDict = default!;
	public TfNavigationState _navState = default!;
	private TfUser _currentUser = default!;
	private TfSpace? _space = null;
	private TfSpacePage? _spacePage = null;
	private TfSpaceView? _spaceView = null;
	private TfSpaceData? _spaceData = null;
	private TfDataProvider? _dataProvider = null;
	private TfSpaceViewPreset? _preset = null;
	private TfDataTable? _data = null;
	private List<TfSpaceViewColumn> _spaceViewColumns = new();
	private List<TfDataProvider>? _allDataProviders = null;
	private List<TfSharedColumn>? _allSharedColumns = null;
	private List<Guid> _selectedDataRows = new();
	private List<Guid> _editedDataRows = new();
	private bool _editAll = false;
	private Dictionary<string, object> _contextData = new();
	private string _tableId = "space-view-table";
	private RenderFragment _caretDownInactive = default!;
	private RenderFragment _caretDown = default!;
	private RenderFragment _caretUp = default!;
	private RenderFragment _manageIcon = default!;
	private StringBuilder _columnSortClass = new();

	public async ValueTask DisposeAsync()
	{
		TfNavigationUIService.NavigationStateChanged -= On_NavigationStateChanged;
		TfUserUIService.UserUpdated -= On_UserChanged;
		TfSpaceViewUIService.SpaceViewColumnsChanged -= On_SpaceViewUpdated;
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
		_componentMetaDict = TfMetaUIService.GetSpaceViewColumnComponentDict();
		_objectRef = DotNetObjectReference.Create(this);
		await _init();
		_caretDownInactive = builder =>
		{
			builder.OpenComponent<FluentIcon<Icon>>(0);
			builder.AddAttribute(1, "Value", TfConstants.GetIcon("ArrowSortUpLines", IconSize.Size16)!.WithColor("var(--tf-caret-color)"));
			builder.CloseComponent();
		};
		_caretDown = builder =>
		{
			builder.OpenComponent<FluentIcon<Icon>>(0);
			builder.AddAttribute(1, "Value", TfConstants.GetIcon("ArrowSortDownLines", IconSize.Size16)!.WithColor("var(--tf-caret-color)"));
			builder.CloseComponent();
		};
		_caretUp = builder =>
		{
			builder.OpenComponent<FluentIcon<Icon>>(0);
			builder.AddAttribute(1, "Value", TfConstants.GetIcon("ArrowSortUpLines", IconSize.Size16)!.WithColor("var(--tf-caret-color)"));
			builder.CloseComponent();
		};
		_manageIcon = builder =>
		{
			builder.OpenComponent<FluentIcon<Icon>>(0);
			builder.AddAttribute(1, "Value", TfConstants.GetIcon("Settings", IconSize.Size16)!.WithColor("var(--tf-caret-color)"));
			builder.CloseComponent();
		};
		_isDataLoading = false;
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			TfNavigationUIService.NavigationStateChanged += On_NavigationStateChanged;
			TfUserUIService.UserUpdated += On_UserChanged;
			TfSpaceViewUIService.SpaceViewColumnsChanged += On_SpaceViewUpdated;
			await JSRuntime.InvokeVoidAsync("Tefter.makeTableResizable", _tableId);
			await JSRuntime.InvokeAsync<bool>("Tefter.addColumnResizeListener",
								 _objectRef, ComponentId, "OnColumnResized");
			await JSRuntime.InvokeAsync<bool>("Tefter.addColumnSortListener",
								 _objectRef, ComponentId, "OnColumnSort");
		}
	}
	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		if (UriInitialized != args.Uri)
		{
			await _init(navState: args);
		}
	}
	private async void On_UserChanged(object? caller, TfUser args)
	{
		if (Context is not null)
			Context.CurrentUser = args;
		await _init(null);
	}

	private async void On_SpaceViewUpdated(object? caller, List<TfSpaceViewColumn> args)
	{
		_spaceViewColumns = args;
		await _init(null);
	}

	private async Task _init(TfNavigationState? navState = null)
	{

		try
		{
			_isDataLoading = true;
			await InvokeAsync(StateHasChanged);

			if (navState == null)
				_navState = await TfNavigationUIService.GetNavigationStateAsync(Navigator);
			else
				_navState = navState;

			if (Context is null)
				throw new Exception("Context cannot be null");
			Guid? oldViewId = _spaceView is not null ? _spaceView.Id : null;
			_spaceView = null;
			if (_navState.SpaceId is null || _navState.SpacePageId is null)
				return;

			_spacePage = Context!.SpacePage;
			if (_spacePage is null || _spacePage.SpaceId != _navState.SpaceId.Value)
				return;
			_space = Context!.Space;
			if (_space is null)
				return;
			if (_spacePage.Type != TfSpacePageType.Page && _spacePage.ComponentType.FullName != typeof(TucSpaceViewSpacePageAddon).FullName)
				return;
			var options = JsonSerializer.Deserialize<TfSpaceViewSpacePageAddonOptions>(_spacePage.ComponentOptionsJson);
			if (options is null || options.SpaceViewId is null)
				return;
			_spaceView = TfSpaceViewUIService.GetSpaceView(options.SpaceViewId.Value);
			if (_spaceView is null)
				return;

			if (_allDataProviders is null)
				_allDataProviders = TfDataProviderUIService.GetDataProviders().ToList();
			if (_allSharedColumns is null)
				_allSharedColumns = TfSharedColumnUIService.GetSharedColumns();

			if (oldViewId != options.SpaceViewId.Value)
			{
				_contextData = new();
				_spaceData = TfSpaceDataUIService.GetSpaceData(_spaceView.SpaceDataId);
				_dataProvider = _allDataProviders.FirstOrDefault(x => x.Id == _spaceData.DataProviderId);
				_selectedDataRows = new();
				_editedDataRows = new();
			}
			_currentUser = Context!.CurrentUser;
			_preset = null;
			if (_navState.SpaceViewPresetId is not null)
				_preset = _spaceView!.Presets.GetPresetById(_navState.SpaceViewPresetId.Value);

			_spaceViewColumns = TfSpaceViewUIService.GetViewColumns(_spaceView.Id);

			_data = TfSpaceDataUIService.QuerySpaceData(
							spaceDataId: _spaceView!.SpaceDataId,
							presetFilters: _preset is not null ? _preset.Filters : null,
							presetSorts: _preset is not null ? _preset.SortOrders : null,
							userFilters: _navState.Filters.ConvertQueryFilterToList(_spaceViewColumns, _allDataProviders, _allSharedColumns),
							userSorts: _navState.Sorts.ConvertQuerySortToList(_spaceViewColumns),
							search: _navState.Search,
							page: 1,
							pageSize: TfConstants.ItemsMaxLimit
					);

			_generateMeta();
		}
		finally
		{
			_isDataLoading = false;
			UriInitialized = _navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}

	#endregion

	#region << Public methods >>
	// Search, Filter, Sort Handlers
	public async Task OnSearch(string value)
	{
		if (_isDataLoading) return;
		var queryDict = new Dictionary<string, object>{
			{ TfConstants.SearchQueryName, value}
		};
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
		var queryDict = new Dictionary<string, object?>{
			{ TfConstants.SearchQueryName, null},
			{ TfConstants.FiltersQueryName, null},
			{ TfConstants.SortsQueryName, null},
			{TfConstants.PageQueryName,1}
		};
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}

	public async Task OnClearPersonalization()
	{
		try
		{
			_ = await TfUserUIService.RemoveSpaceViewPersonalizations(
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

			_selectedDataRows = TfSpaceDataUIService.QuerySpaceDataIdList(
							spaceDataId: _spaceView!.SpaceDataId,
							presetFilters: preset is not null ? preset.Filters : null,
							presetSorts: preset is not null ? preset.SortOrders : null,
							userFilters: _navState.Filters.ConvertQueryFilterToList(_spaceViewColumns, _allDataProviders, _allSharedColumns),
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


	public TfDataTable? GetCurrentData() => _data;

	public void OnDeleteRows(List<Guid> tfIds)
	{
		if (_spaceView is null) return;
		if (tfIds is null || tfIds.Count == 0) return;
		try
		{
			var spaceData = TfSpaceDataUIService.GetSpaceData(_spaceView.SpaceDataId);
			TfDataProviderUIService.DeleteDataProviderRowsByTfId(
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

	public void OnEditRows(List<Guid> tfIds)
	{
		if (tfIds is null || tfIds.Count == 0) return;
		_editedDataRows = _editedDataRows.Union(tfIds).ToList();
		_generateMeta();
		StateHasChanged();
	}
	#endregion

	#region <<Utility Methods >>
	private async Task _onRowChanged(TfDataTable value)
	{
		try
		{
			if (value is null || value.Rows.Count == 0) return;
			var changedRow = value.Rows[0];
			var changedRowTfId = changedRow.GetRowId();

			var dataTable = TfSpaceDataUIService.SaveDataTable(value);

			//Apply changed to the datatable
			if (_data is null || _data.Rows.Count == 0 || _data.Rows.Count == 0) return;

			var clonedData = _data.Clone();
			for (int i = 0; i < clonedData.Rows.Count; i++)
			{
				TfDataRow row = clonedData.Rows[i];
				Guid tfId = row.GetRowId();
				if (changedRowTfId != tfId) continue;

				for (int j = 0; j < clonedData.Columns.Count; j++)
				{
					TfDataColumn column = clonedData.Columns[j];
					if (column.IsSystem) continue;
					row[column.Name] = changedRow[column.Name];
					Console.WriteLine(changedRow[column.Name]);
				}
			}
			_data = clonedData;
			_generateMeta();
			await InvokeAsync(StateHasChanged);
		}
		catch (Exception ex)
		{
			throw new TfException(LOC($"Data change failed! {ex.Message}"), ex);
		}
	}

	private void _toggleItemSelection(Guid rowId, bool isChecked)
	{
		if (isChecked)
		{
			if (!_selectedDataRows.Contains(rowId)) _selectedDataRows.Add(rowId);
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

	private void _setRowEditable(Guid rowId, bool isEditable)
	{
		if (isEditable)
		{
			if (!_spaceView.Settings.CanUpdateRows) return;
			if (!_editedDataRows.Contains(rowId)) _editedDataRows.Add(rowId);
		}
		else
		{
			_editedDataRows.RemoveAll(x => x == rowId);
		}
		_generateMeta();
	}

	[JSInvokable("OnColumnSort")]
	public async void OnColumnSort(int position, bool hasShift)
	{
		if (_isDataLoading == true) return;
		var column = _spaceViewColumns.SingleOrDefault(x => x.Position == position);
		if (column is null) return;
		var sorts = TfUserUIService.CalculateViewPresetSortPersonalization(
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
		await TfUserUIService.SetViewPresetColumnPersonalization(
		 userId: _currentUser.Id,
		 spaceViewId: _spaceView.Id,
		 preset: _preset?.Id,
		 spaceViewColumnId: column.Id,
		 width: width);
	}

	private async Task _manageColumn(TfSpaceViewColumn column)
	{

		var dialog = await DialogService.ShowDialogAsync<TucSpaceViewColumnManageDialog>(
				column,
				new DialogParameters()
				{
					PreventDismissOnOverlayClick = true,
					PreventScroll = true,
					Width = TfConstants.DialogWidthLarge,
					TrapFocus = false
				});
		var result = await dialog.Result;
	}
	#endregion
}