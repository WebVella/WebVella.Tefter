﻿namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceView.SpaceViewDetails.TfSpaceViewDetails", "WebVella.Tefter")]
public partial class TfSpaceViewDetails : TfBaseComponent
{
	// Dependency Injection
	[Inject] protected IState<TfUserState> TfUserState { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] private IKeyCodeService KeyCodeService { get; set; }
	[Inject] private AppStateUseCase UC { get; set; }
	[Inject] private UserStateUseCase UserUC { get; set; }

	// Parameters
	[Parameter] public Guid? SpaceViewId { get; set; }

	// State
	private bool _isDataLoading = true;
	private bool _selectAllLoading = false;
	private Dictionary<Guid, TucSpaceViewColumnComponent> _componentMetaDict;

	// Lifecycle Methods
	protected override async ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			ActionSubscriber.UnsubscribeFromAllActions(this);
		}
		await base.DisposeAsyncCore(disposing);
	}

	protected override void OnInitialized()
	{
		base.OnInitialized();
		_componentMetaDict = UC.GetSpaceViewColumnComponentDict();
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		if (firstRender)
		{
			_isDataLoading = false;
			await InvokeAsync(StateHasChanged);
			ActionSubscriber.SubscribeToAction<SetAppStateAction>(this, HandleAppStateChange);
		}
	}
	protected override bool ShouldRender() => !_isDataLoading && base.ShouldRender();

	// Event Handlers
	private async void HandleAppStateChange(SetAppStateAction action)
	{
		await InvokeAsync(() =>
		{
			_isDataLoading = false;
			StateHasChanged();
		});
	}
	public async Task HandleKeyDownAsync(FluentKeyCodeEventArgs args)
	{
		if (args.Key == KeyCode.PageUp) await _goNextPage();
		else if (args.Key == KeyCode.PageDown) await _goPreviousPage();
	}

	// Navigation Methods
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
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}
	private async Task _goNextPage()
	{
		if (_isDataLoading) return;
		if (TfAppState.Value.SpaceViewData is null
		|| TfAppState.Value.SpaceViewData.Rows.Count == 0)
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
		await Navigator.ApplyChangeToUrlQuery(queryDict);
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
	private async Task _onFilter(List<TucFilterBase> filters)
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
	private async Task _onSort(List<TucSort> sorts)
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
		var queryDict = new Dictionary<string, object>{
			{ TfConstants.SearchQueryName, null},
			{ TfConstants.FiltersQueryName, null},
			{ TfConstants.SortsQueryName, null}
		};
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}

	//Select Handlers
	private async Task _onSelectAll()
	{
		try
		{
			_selectAllLoading = true;
			await InvokeAsync(StateHasChanged);
			await Task.Delay(1);
			TucSpaceViewPreset preset = null;
			if (TfAppState.Value.Route.SpaceViewPresetId is not null)
				preset = TfAppState.Value.SpaceView.Presets.GetPresetById(TfAppState.Value.Route.SpaceViewPresetId.Value);
			var resultSrv = UC.GetSpaceDataIdList(
							spaceDataId: TfAppState.Value.SpaceView.SpaceDataId.Value,
							presetFilters: preset is not null ? preset.Filters : null,
							presetSorts: preset is not null ? preset.SortOrders : null,
							userFilters: TfAppState.Value.SpaceViewFilters,
							userSorts: TfAppState.Value.SpaceViewSorts,
							search: TfAppState.Value.Route.Search,
							page: null,
							pageSize: null
					);
			Dispatcher.Dispatch(new SetAppStateAction(
				component: this,
				state: TfAppState.Value with { SelectedDataRows = resultSrv }));
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
			Dispatcher.Dispatch(new SetAppStateAction(
				component: this,
				state: TfAppState.Value with { SelectedDataRows = new() }));
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
		if (TfAppState.Value.Route.SpaceViewPresetId is null || TfAppState.Value.SpaceView.Presets.Count == 0) return "";

		var preset = TfAppState.Value.SpaceView.Presets.GetPresetById(TfAppState.Value.Route.SpaceViewPresetId.Value);
		if (preset is null) return "";

		var result = new StringBuilder();
		var path = new List<TucSpaceViewPreset>();
		ModelHelpers.FillPresetPathById(TfAppState.Value.SpaceView.Presets, TfAppState.Value.Route.SpaceViewPresetId.Value, path);
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

	private Task _onRowChanged(TfDataTable value)
	{
		try
		{
			var dataTable = UC.SaveDataDataTable(value);

			//Apply changed to the datatable
			var viewData = TfAppState.Value.SpaceViewData.Clone();
			if (viewData is null || viewData.Rows.Count == 0 || dataTable.Rows.Count == 0) return Task.CompletedTask;
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				TfDataRow row = dataTable.Rows[i];
				Guid tfId = (Guid)row[TfConstants.TEFTER_ITEM_ID_PROP_NAME];
				var currentRow = viewData.Rows[tfId];

				for (int j = 0; j < dataTable.Columns.Count; j++)
				{
					TfDataColumn column = dataTable.Columns[j];
					currentRow[column.Name] = row[column.Name];
				}
			}
			Dispatcher.Dispatch(new SetAppStateAction(component: this,
				state: TfAppState.Value with { SpaceViewData = viewData }));

			return Task.CompletedTask;
		}
		catch (Exception ex)
		{
			throw new TfException(LOC("Data change failed!"), ex);
		}
	}

	private Dictionary<string, object> _getColumnComponentContext(TucSpaceViewColumn column, TfDataTable dataTable, int rowIndex)
	{
		var componentData = new Dictionary<string, object>();
		componentData[TfConstants.SPACE_VIEW_COMPONENT_ROW_CHANGED_PROPERTY_NAME] = EventCallback.Factory.Create<TfDataTable>(this, _onRowChanged);
		componentData[TfConstants.SPACE_VIEW_COMPONENT_CONTEXT_PROPERTY_NAME] = new TfSpaceViewColumnScreenRegionContext
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
		var row = TfAppState.Value.SpaceViewData.Rows[rowIndex];
		object rowId = row[TfConstants.TEFTER_ITEM_ID_PROP_NAME];
		if (rowId is not null)
		{
			return TfAppState.Value.SelectedDataRows.Contains((Guid)rowId);
		}
		return false;
	}

	private void _toggleItemSelection(int rowIndex, bool isChecked)
	{
		Guid rowId = (Guid)TfAppState.Value.SpaceViewData.Rows[rowIndex][TfConstants.TEFTER_ITEM_ID_PROP_NAME];
		var selectedItems = TfAppState.Value.SelectedDataRows.ToList();
		if (isChecked)
		{
			if (!selectedItems.Contains(rowId)) selectedItems.Add(rowId);
		}
		else
		{
			selectedItems.RemoveAll(x => x == rowId);
		}
		Dispatcher.Dispatch(new SetAppStateAction(
			component: this,
			state: TfAppState.Value with
			{
				SelectedDataRows = selectedItems
			}
		));
	}

	private void _toggleSelectAll(bool isChecked)
	{
		if (TfAppState.Value.SpaceViewData is null
		|| TfAppState.Value.SpaceViewData.Rows is null
		|| TfAppState.Value.SpaceViewData.Rows.Count == 0)
			return;
		var selectedItems = TfAppState.Value.SelectedDataRows.ToList();
		for (int i = 0; i < TfAppState.Value.SpaceViewData.Rows.Count; i++)
		{
			var rowId = (Guid)TfAppState.Value.SpaceViewData.Rows[i][TfConstants.TEFTER_ITEM_ID_PROP_NAME];
			if (!isChecked)
			{
				selectedItems.RemoveAll(x => x == rowId);
			}
			else if (!selectedItems.Any(x => x == rowId))
			{
				selectedItems.Add(rowId);

			}
		}
		Dispatcher.Dispatch(new SetAppStateAction(
			component: this,
			state: TfAppState.Value with
			{
				SelectedDataRows = selectedItems
			}
		));
	}

	private Dictionary<int, Tuple<int?, string, string>> _generateColumnConfigurationCss(TucSpaceView view, List<TucSpaceViewColumn> columns)
	{
		int _freezeLeftColumnsCount = TfAppState.Value.SpaceView.Settings.FreezeStartingNColumns ?? 0;
		int _freezeLeftWidth = 0;
		int _freezeRightColumnsCount = TfAppState.Value.SpaceView.Settings.FreezeFinalNColumns ?? 0;
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
}