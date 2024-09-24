namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceView.SpaceViewDetails.TfSpaceViewDetails", "WebVella.Tefter")]
public partial class TfSpaceViewDetails : TfBaseComponent
{
	[Inject] protected IState<TfUserState> TfUserState { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] private IKeyCodeService KeyCodeService { get; set; }
	[Inject] private AppStateUseCase UC { get; set; }
	[Inject] private UserStateUseCase UserUC { get; set; }

	private bool _isDataLoading = true;

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
	protected override bool ShouldRender()
	{
		if (_isDataLoading) return false;
		return base.ShouldRender();
	}

	private void On_AppChanged(SetAppStateAction action)
	{
		InvokeAsync(async () =>
		{
			_isDataLoading = false;
			await InvokeAsync(StateHasChanged);
		});
	}

	public Task OnKeyDownAsync(FluentKeyCodeEventArgs args)
	{
		if (args.Key == KeyCode.PageUp) _goNextPage();
		else if (args.Key == KeyCode.PageDown) _goPreviousPage();
		return Task.CompletedTask;
	}

	private async Task _goFirstPage()
	{
		if (_isDataLoading) return;
		_isDataLoading = true;
		if (TfAppState.Value.SpaceViewPage == 1) return;
		var queryDict = new Dictionary<string, object>();
		queryDict[TfConstants.PageQueryName] = 1;
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}
	private async Task _goPreviousPage()
	{
		if (_isDataLoading) return;
		_isDataLoading = true;
		var page = TfAppState.Value.SpaceViewPage - 1;
		if (page < 1) page = 1;
		if (TfAppState.Value.SpaceViewPage == page) return;
		var queryDict = new Dictionary<string, object>();
		queryDict[TfConstants.PageQueryName] = page;
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}
	private async Task _goNextPage()
	{
		if (_isDataLoading) return;
		_isDataLoading = true;
		if (TfAppState.Value.SpaceViewData is null
		|| TfAppState.Value.SpaceViewData.Rows.Count == 0)
			return;

		var page = TfAppState.Value.SpaceViewPage + 1;
		if (page < 1) page = 1;
		if (TfAppState.Value.SpaceViewPage == page) return;

		var queryDict = new Dictionary<string, object>();
		queryDict[TfConstants.PageQueryName] = page;
		await Navigator.ApplyChangeToUrlQuery(queryDict);

	}
	private async Task _goLastPage()
	{
		if (_isDataLoading) return;
		_isDataLoading = true;
		if (TfAppState.Value.SpaceViewPage == -1) return;
		var queryDict = new Dictionary<string, object>();
		queryDict[TfConstants.PageQueryName] = -1;
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}
	private async Task _goOnPage(int page)
	{
		if (_isDataLoading) return;
		_isDataLoading = true;
		if (page < 1 && page != -1) page = 1;
		if (TfAppState.Value.SpaceViewPage == page) return;
		var queryDict = new Dictionary<string, object>();
		queryDict[TfConstants.PageQueryName] = page;
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}

	private async Task _pageSizeChange(int pageSize)
	{
		if (_isDataLoading) return;
		_isDataLoading = true;
		if (pageSize < 0) pageSize = TfConstants.PageSize;
		if (TfAppState.Value.SpaceViewPageSize == pageSize) return;
		try
		{
			var resultSrv = await UserUC.SetPageSize(
						userId: TfUserState.Value.CurrentUser.Id,
						pageSize: pageSize == TfConstants.PageSize ? null : pageSize
					);
			if (resultSrv.IsSuccess)
			{
				Dispatcher.Dispatch(new SetUserStateAction(
					component: this,
					state: TfUserState.Value with { CurrentUser = resultSrv.Value }));
			}
		}
		catch { }


		var queryDict = new Dictionary<string, object>();
		queryDict[TfConstants.PageSizeQueryName] = pageSize;
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}

	private async Task _onSearch(string value)
	{
		if (_isDataLoading) return;
		_isDataLoading = true;
		var queryDict = new Dictionary<string, object>();
		queryDict[TfConstants.SearchQueryName] = value;
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

	private Task _onRowChanged(TfDataTable value)
	{
		
		try
		{
			var result = UC.ChangeViewData(value);
			//ProcessServiceResponse(result); //should be handled by the component
			if (result.IsSuccess)
			{
				//Apply changed to the datatable
				var viewData = TfAppState.Value.SpaceViewData.Clone();
				if(viewData is null || viewData.Rows.Count == 0 || result.Value.Rows.Count == 0) return Task.CompletedTask;
				for (int i = 0; i < result.Value.Rows.Count; i++)
				{
					TfDataRow row = result.Value.Rows[i];
					Guid tfId = (Guid)row[TfConstants.TEFTER_ITEM_ID_PROP_NAME];
					var currentRow = viewData.Rows[tfId];

					for (int j = 0; j < result.Value.Columns.Count; j++)
					{
						TfDataColumn column = result.Value.Columns[j];
						currentRow[column.Name] = row[column.Name];
					}
				}
				Dispatcher.Dispatch(new SetAppStateAction(component: this,
					state: TfAppState.Value with { SpaceViewData = viewData }));
			}
			else{ 
				throw ResultUtils.ConvertResultToException(result,LOC("Data change failed!"));
			}
		}
		catch
		{
			throw;
		}
		return Task.CompletedTask;
	}

	private Dictionary<string, object> _getColumnComponentContext(TucSpaceViewColumn column, TfDataTable dataTable, int rowIndex)
	{
		var componentData = new Dictionary<string, object>();
		componentData[TfConstants.SPACE_VIEW_COMPONENT_ROW_CHANGED_PROPERTY_NAME] = EventCallback.Factory.Create<TfDataTable>(this, _onRowChanged);
		componentData[TfConstants.SPACE_VIEW_COMPONENT_CONTEXT_PROPERTY_NAME] = new TfComponentContext
		{
			Mode = TfComponentMode.Display,
			CustomOptionsJson = column.CustomOptionsJson,
			DataMapping = column.DataMapping,
			DataTable = dataTable,
			RowIndex = rowIndex,
			QueryName = column.QueryName,
			SelectedAddonId = column.SelectedAddonId,
			SpaceViewId = column.SpaceViewId,
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


}