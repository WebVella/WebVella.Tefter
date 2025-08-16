namespace WebVella.Tefter.UI.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceView.SpaceViewPageContent.TucSpaceViewPageContent", "WebVella.Tefter")]
public partial class TucSpaceViewPageContent : TfBaseComponent, IAsyncDisposable
{
	// Dependency Injection
	[Inject] private ITfSpaceUIService TfSpaceUIService { get; set; } = default!;
	[Inject] private ITfSpaceViewUIService TfSpaceViewUIService { get; set; } = default!;
	[Inject] private ITfSpaceDataUIService TfSpaceDataUIService { get; set; } = default!;
	[Inject] private ITfUserUIService TfUserUIService { get; set; } = default!;
	[Inject] private ITfMetaUIService TfMetaUIService { get; set; } = default!;
	[Inject] public ITfNavigationUIService TfNavigationUIService { get; set; } = default!;

	// State
	private DotNetObjectReference<TucSpaceViewPageContent> _objectRef;
	private bool _isDataLoading = true;
	private bool _selectAllLoading = false;
	private ReadOnlyDictionary<Guid, TfSpaceViewColumnComponentAddonMeta> _componentMetaDict = default!;
	public TfNavigationState _navState = default!;
	private int _page = 1;
	private int _pageSize = TfConstants.PageSize;
	private TfUser _currentUser = default!;
	private TfSpace? _space = null;
	private TfSpacePage? _spacePage = null;
	private TfSpaceView? _spaceView = null;
	private TfSpaceData? _spaceData = null;
	private TfSpaceViewPreset? _preset = null;
	private TfDataTable? _data = null;
	private List<TfSpaceViewColumn> _spaceViewColumns = new();
	private List<Guid> _selectedDataRows = new();
	private Dictionary<string, object> _contextData = new();
	private string _tableId = "space-view-table";
	public async ValueTask DisposeAsync()
	{
		TfNavigationUIService.NavigationStateChanged -= On_NavigationStateChanged;
		_objectRef?.Dispose();
		try
		{
			await JSRuntime.InvokeAsync<bool>("Tefter.removeColumnResizeListener", ComponentId);
		}
		catch { }
	}
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		_componentMetaDict = TfMetaUIService.GetSpaceViewColumnComponentDict();
		_objectRef = DotNetObjectReference.Create(this);
		await _init();
		_isDataLoading = false;
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			TfNavigationUIService.NavigationStateChanged += On_NavigationStateChanged;
			await JSRuntime.InvokeVoidAsync("Tefter.makeTableResizable", _tableId);
			await JSRuntime.InvokeAsync<bool>(
								 "Tefter.addColumnResizeListener",
								 _objectRef, ComponentId, "OnColumnResized");
		}
	}
	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		if (UriInitialized != args.Uri)
			await _init(navState: args);
	}

	private async Task _init(TfNavigationState? navState = null)
	{
		if (navState == null)
			_navState = await TfNavigationUIService.GetNavigationStateAsync(Navigator);
		else
			_navState = navState;
		try
		{
			Guid? oldViewId = _spaceView is not null ? _spaceView.Id : null;
			_spaceView = null;
			if (_navState.SpaceId is null || _navState.SpacePageId is null)
				return;

			_spacePage = TfSpaceUIService.GetSpacePage(_navState.SpacePageId.Value);
			if (_spacePage is null || _spacePage.SpaceId != _navState.SpaceId.Value)
				return;

			if (_spacePage.Type != TfSpacePageType.Page && _spacePage.ComponentType.FullName != typeof(TucSpaceViewSpacePageAddon).FullName)
				return;
			var options = JsonSerializer.Deserialize<TfSpaceViewSpacePageAddonOptions>(_spacePage.ComponentOptionsJson);
			if (options is null || options.SpaceViewId is null)
				return;
			_spaceView = TfSpaceViewUIService.GetSpaceView(options.SpaceViewId.Value);
			if (_spaceView is null)
				return;

			if (oldViewId != options.SpaceViewId.Value)
			{
				_contextData = new();
				_space = TfSpaceUIService.GetSpace(_spaceView.SpaceId);
				_spaceData = TfSpaceDataUIService.GetSpaceData(_spaceView.SpaceDataId);
				_spaceViewColumns = TfSpaceViewUIService.GetViewColumns(_spaceView.Id);
				_selectedDataRows = new();
			}
			_currentUser = await TfUserUIService.GetCurrentUserAsync()
				?? throw new Exception("User cannot be null");
			_page = _navState.Page ?? 1;
			_pageSize = _navState.PageSize ?? TfConstants.PageSize;

			_preset = null;
			if (_navState.SpaceViewPresetId is not null)
				_preset = _spaceView!.Presets.GetPresetById(_navState.SpaceViewPresetId.Value);
			_data = TfSpaceDataUIService.QuerySpaceData(
							spaceDataId: _spaceView!.SpaceDataId,
							presetFilters: _preset is not null ? _preset.Filters : null,
							presetSorts: _preset is not null ? _preset.SortOrders : null,
							userFilters: _navState!.Filters,
							userSorts: _navState.Sorts,
							search: _navState.Search,
							page: _page,
							pageSize: _pageSize
					);
			//Process last page (-1) case
			if (_page == -1)
			{
				_page = _data.QueryInfo.Page ?? 1;
				await Navigator.ApplyChangeToUrlQuery(TfConstants.PageQueryName, _page);
			}
		}
		finally
		{
			_isDataLoading = false;
			UriInitialized = _navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}


	// Navigation Methods
	private async Task _goFirstPage()
	{
		if (_isDataLoading) return;
		if (_page == 1) return;
		_isDataLoading = true;
		await Navigator.ApplyChangeToUrlQuery(TfConstants.PageQueryName, null);
	}
	private async Task _goPreviousPage()
	{
		if (_isDataLoading) return;
		var page = (_navState?.Page ?? 1) - 1;
		if (page < 1) page = 1;
		if (_page == page) return;
		_isDataLoading = true;
		await Navigator.ApplyChangeToUrlQuery(TfConstants.PageQueryName, page == 1 ? null : page);
	}
	private async Task _goNextPage()
	{
		if (_isDataLoading) return;
		if (_data is null
		|| _data.Rows.Count == 0)
			return;
		var page = (_navState?.Page ?? 1) + 1;
		if (page < 1) page = 1;
		if (_page == page) return;
		_isDataLoading = true;
		await Navigator.ApplyChangeToUrlQuery(TfConstants.PageQueryName, page == 1 ? null : page);
	}
	private async Task _goLastPage()
	{
		if (_isDataLoading) return;
		if (_page == -1) return;
		_isDataLoading = true;
		await Navigator.ApplyChangeToUrlQuery(TfConstants.PageQueryName, -1);
	}
	private async Task _goOnPage(int page)
	{
		if (_isDataLoading) return;
		if (page < 1 && page != -1) page = 1;
		if (_page == page) return;
		_isDataLoading = true;
		await Navigator.ApplyChangeToUrlQuery(TfConstants.PageQueryName, page == 1 ? null : page);
	}
	private async Task _pageSizeChange(int pageSize)
	{
		if (_isDataLoading) return;
		if (pageSize < 0) pageSize = TfConstants.PageSize;
		if (_pageSize == pageSize) return;
		try
		{
			var currentUser = await TfUserUIService.GetCurrentUserAsync();
			var user = await TfUserUIService.SetPageSize(
						userId: currentUser!.Id,
						pageSize: pageSize == TfConstants.PageSize ? null : pageSize
					);
		}
		catch { }

		_isDataLoading = true;
		await Navigator.ApplyChangeToUrlQuery(TfConstants.PageSizeQueryName, pageSize);
	}

	// Search, Filter, Sort Handlers
	private async Task _onSearch(string value)
	{
		if (_isDataLoading) return;
		_isDataLoading = true;
		var queryDict = new Dictionary<string, object>{
			{ TfConstants.SearchQueryName, value}
		};
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}
	private async Task _onFilter(List<TfFilterBase> filters)
	{
		if (_isDataLoading) return;
		_isDataLoading = true;
		var queryDict = new Dictionary<string, object>();
		if (filters is null || filters.Count == 0)
			queryDict[TfConstants.FiltersQueryName] = null;
		else
			queryDict[TfConstants.FiltersQueryName] = NavigatorExt.SerializeFiltersForUrl(filters, false);
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}
	private async Task _onSort(List<TfSort> sorts)
	{
		if (_isDataLoading) return;
		_isDataLoading = true;
		var queryDict = new Dictionary<string, object>();
		if (sorts is null || sorts.Count == 0)
			queryDict[TfConstants.SortsQueryName] = null;
		else
			queryDict[TfConstants.SortsQueryName] = NavigatorExt.SerializeSortsForUrl(sorts, false);
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}
	private async Task _onClearFilter()
	{
		if (_isDataLoading) return;
		_isDataLoading = true;
		var queryDict = new Dictionary<string, object?>{
			{ TfConstants.SearchQueryName, null},
			{ TfConstants.FiltersQueryName, null},
			{ TfConstants.SortsQueryName, null},
			{TfConstants.PageQueryName,1}
		};
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}

	private async Task _onClearPersonalization()
	{

	}

	//Select Handlers
	private async Task _onSelectAll()
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
							userFilters: _navState!.Filters,
							userSorts: _navState.Sorts,
							search: _navState.Search
					);
		}
		catch { }
		finally
		{
			_selectAllLoading = false;
			await InvokeAsync(StateHasChanged);
		}
	}
	private async Task _onDeSelectAll()
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
			await InvokeAsync(StateHasChanged);
		}
	}

	// Utility Methods
	private string _generatePresetPathHtml()
	{
		if (_navState.SpaceViewPresetId is null || _spaceView.Presets.Count == 0) return "";

		var preset = _spaceView.Presets.GetPresetById(_navState.SpaceViewPresetId.Value);
		if (preset is null) return "";

		var result = new StringBuilder();
		var path = new List<TfSpaceViewPreset>();
		ModelHelpers.FillPresetPathById(_spaceView.Presets, _navState.SpaceViewPresetId.Value, path);
		if (path.Count == 0) return "";
		path.Reverse();
		foreach (var item in path)
		{
			result.Append($"<span class=\"page-header-divider\">:</span>");
			result.Append(item.Name);
		}
		return result.ToString();

	}

	private string _getEmbeddedStyles()
	{
		var sb = new StringBuilder();
		sb.AppendLine("<style>");
		sb.AppendLine("html:root .tf-layout__body__main {");
		//sb.AppendLine($"--tf-grid-row-selected: {TfAppState.Value.SpaceGridSelectedColor};");
		//sb.AppendLine($"--space-color: {TfAppState.Value.SpaceColorString};");
		//sb.AppendLine($"--accent-base-color: {TfAppState.Value.SpaceColorString};");
		//sb.AppendLine($"--accent-fill-rest: {TfAppState.Value.SpaceColorString};");
		//sb.AppendLine($"--tf-grid-border-color: {TfAppState.Value.SpaceBorderColor};");
		sb.AppendLine("}");
		sb.AppendLine("</style>");
		return sb.ToString();
	}

	private async Task _onRowChanged(TfDataTable value)
	{
		try
		{
			var dataTable = TfSpaceDataUIService.SaveDataDataTable(value);

			//Apply changed to the datatable
			if (_data is null || _data.Rows.Count == 0 || _data.Rows.Count == 0) return;
			var changedRow = dataTable.Rows[0];
			var changedRowTfId = (Guid)changedRow[TfConstants.TEFTER_ITEM_ID_PROP_NAME];
			var clonedData = _data.Clone();
			for (int i = 0; i < clonedData.Rows.Count; i++)
			{
				TfDataRow row = clonedData.Rows[i];
				Guid tfId = (Guid)row[TfConstants.TEFTER_ITEM_ID_PROP_NAME];
				if (changedRowTfId != tfId) continue;

				for (int j = 0; j < clonedData.Columns.Count; j++)
				{
					TfDataColumn column = clonedData.Columns[j];
					row[column.Name] = changedRow[column.Name];
				}
			}
			_data = clonedData;
			await InvokeAsync(StateHasChanged);
		}
		catch (Exception ex)
		{
			throw new TfException(LOC("Data change failed!"), ex);
		}
	}

	private Dictionary<string, object> _getColumnComponentContext(TfSpaceViewColumn column, TfDataTable dataTable, int rowIndex)
	{
		var componentData = new Dictionary<string, object>();
		componentData[TfConstants.SPACE_VIEW_COMPONENT_ROW_CHANGED_PROPERTY_NAME] = EventCallback.Factory.Create<TfDataTable>(this, _onRowChanged);
		componentData[TfConstants.SPACE_VIEW_COMPONENT_CONTEXT_PROPERTY_NAME] = new TfSpaceViewColumnScreenRegionContext(_contextData)
		{
			Mode = TfComponentPresentationMode.Display,
			ComponentOptionsJson = column.ComponentOptionsJson,
			DataMapping = column.DataMapping,
			DataTable = dataTable,
			RowIndex = rowIndex,
			QueryName = column.QueryName,
			SpaceViewId = column.SpaceViewId,
			SpaceViewColumnId = column.Id
		};
		return componentData;
	}

	private bool _getItemSelection(int rowIndex)
	{
		var row = _data.Rows[rowIndex];
		object rowId = row[TfConstants.TEFTER_ITEM_ID_PROP_NAME];
		if (rowId is not null)
		{
			return _selectedDataRows.Contains((Guid)rowId);
		}
		return false;
	}

	private void _toggleItemSelection(int rowIndex, bool isChecked)
	{
		Guid rowId = (Guid)_data.Rows[rowIndex][TfConstants.TEFTER_ITEM_ID_PROP_NAME];
		if (isChecked)
		{
			if (!_selectedDataRows.Contains(rowId)) _selectedDataRows.Add(rowId);
		}
		else
		{
			_selectedDataRows.RemoveAll(x => x == rowId);
		}
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
	}

	private Dictionary<int, Tuple<int?, string, string>> _generateColumnConfigurationCss(TfSpaceView view, List<TfSpaceViewColumn> columns)
	{
		int _freezeLeftColumnsCount = _spaceView.Settings.FreezeStartingNColumns ?? 0;
		int _freezeLeftWidth = 0;
		int _freezeRightColumnsCount = _spaceView.Settings.FreezeFinalNColumns ?? 0;
		int _freezeRightWidth = 0;
		var result = new Dictionary<int, Tuple<int?, string, string>>();

		//Position 0 is always the checkbox , width 40
		result[0] = new Tuple<int?, string, string>(40, "tf--sticky", "");
		_freezeLeftWidth += 40;
		for (int i = 0; i < columns.Count; i++)
		{
			var column = columns[i];
			var position = i + 1;
			if (position > _freezeLeftColumnsCount && position <= (columns.Count - _freezeRightColumnsCount))
			{
				result[position] = new Tuple<int?, string, string>(column.Settings.Width, "", "");
			}
			else if (position <= _freezeLeftColumnsCount)
			{
				var width = column.Settings.Width ?? 140;
				var lastLeftSticyClass = position == _freezeLeftColumnsCount ? "tf--sticky-last" : "";
				result[position] = new Tuple<int?, string, string>(width, $"tf--sticky {lastLeftSticyClass}", $"left:{(_freezeLeftWidth)}px;");
				_freezeLeftWidth += width;
			}
			else
			{
				result[position] = new Tuple<int?, string, string>(column.Settings.Width, "", "");
			}

		}

		for (int i = columns.Count - 1; i >= 0; i--)
		{
			var column = columns[i];
			var position = i + 1;
			if (position > (columns.Count - _freezeRightColumnsCount))
			{
				var width = column.Settings.Width ?? 140;
				var firstRightSticyClass = position == (columns.Count - _freezeRightColumnsCount + 1) ? "tf--sticky-first" : "";
				result[position] = new Tuple<int?, string, string>(width, $"tf--sticky {firstRightSticyClass}", $"right:{(_freezeRightWidth)}px;");
				_freezeRightWidth += width;

			}
		}

		return result;

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

	[JSInvokable("OnColumnResized")]
	public async Task OnColumnResized(int position, int width)
	{
		var column = _spaceViewColumns.SingleOrDefault(x => x.Position == position);
		if (column is null) return;
		await TfUserUIService.SetViewColumnPersonalizationWidth(
		 userId: _currentUser.Id,
		 spaceViewId: _spaceView.Id,
		 spaceViewColumnId: column.Id,
		 width: width);
	}
}