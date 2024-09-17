namespace WebVella.Tefter.Web.Components;
public partial class TfSpaceViewDetails : TfBaseComponent
{
	[Inject] protected IState<TfUserState> TfUserState { get; set; }
	[Inject] protected IState<TfRouteState> TfRouteState { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] private IKeyCodeService KeyCodeService { get; set; }
	[Inject] private SpaceUseCase UC { get; set; }

	protected override async ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			KeyCodeService.UnregisterListener(OnKeyDownAsync);
		}
		await base.DisposeAsyncCore(disposing);
	}
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await UC.Init(this.GetType());
	}
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			var viewData = await UC.IInitSpaceViewDetailsAfterRender(TfAppState.Value);
			Dispatcher.Dispatch(new InitSpaceViewDetailsAction(
				component: this,
				page:TfAppState.Value.Page,
				pageSize:TfAppState.Value.PageSize,
				search:TfAppState.Value.SearchQuery,
				filters:TfAppState.Value.Filters,
				sorts:TfAppState.Value.Sorts,
				selectedDataRows:TfAppState.Value.SelectedDataRows,
				spaceViewData: viewData
			));
			UC.IsBusy = false;
			await InvokeAsync(StateHasChanged);
			ActionSubscriber.SubscribeToAction<SpaceStateChangedAction>(this, On_StateChanged);
			KeyCodeService.RegisterListener(OnKeyDownAsync);
		}
	}

	public Task OnKeyDownAsync(FluentKeyCodeEventArgs args)
	{
		if (args.Key == KeyCode.PageUp) _goNextPage();
		else if (args.Key == KeyCode.PageDown) _goPreviousPage();
		return Task.CompletedTask;
	}

	private void On_StateChanged(SpaceStateChangedAction action)
	{
		if (action.Component == this) return;
		InvokeAsync(async () =>
		{
			UC.IsBusy = true;
			await InvokeAsync(StateHasChanged);
			var viewData = await UC.IInitSpaceViewDetailsAfterRender(TfAppState.Value);
			Dispatcher.Dispatch(new InitSpaceViewDetailsAction(
				component: this,
				page:TfAppState.Value.Page,
				pageSize:TfAppState.Value.PageSize,
				search:TfAppState.Value.SearchQuery,
				filters:TfAppState.Value.Filters,
				sorts:TfAppState.Value.Sorts,
				selectedDataRows:TfAppState.Value.SelectedDataRows,
				spaceViewData: viewData
			));
			UC.IsBusy = false;
			await InvokeAsync(StateHasChanged);
		});

	}

	private void _goFirstPage()
	{
		Dispatcher.Dispatch(new SetSpacePagingAction(
			component: this,
			page: 1,
			pageSize: TfAppState.Value.PageSize
		));
	}
	private void _goPreviousPage()
	{
		var page = TfAppState.Value.Page - 1;
		if (page < 1) page = 1;
		Dispatcher.Dispatch(new SetSpacePagingAction(
			component: this,
			page: page,
			pageSize: TfAppState.Value.PageSize
		));
	}
	private void _goNextPage()
	{
		if (TfAppState.Value.SpaceViewData is null
		|| TfAppState.Value.SpaceViewData.Rows.Count == 0)
			return;

		Dispatcher.Dispatch(new SetSpacePagingAction(
			component: this,
			page: TfAppState.Value.Page + 1,
			pageSize: TfAppState.Value.PageSize
		));
	}
	private void _goLastPage()
	{
		Dispatcher.Dispatch(new SetSpacePagingAction(
			component: this,
			page: -1,
			pageSize: TfAppState.Value.PageSize
		));
	}
	private void _goOnPage(int page)
	{
		if (page < 1) page = 1;
		Dispatcher.Dispatch(new SetSpacePagingAction(
			component: this,
			page: page,
			pageSize: TfAppState.Value.PageSize
		));
	}

	private Dictionary<string, object> _getColumnComponentContext(TucSpaceViewColumn column, TfDataTable dataTable, int rowIndex)
	{
		var componentData = new Dictionary<string, object>();

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
		Dispatcher.Dispatch(new ToggleSpaceViewItemSelectionAction(
			component: this,
			idList: selectedItems
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

		Dispatcher.Dispatch(new ToggleSpaceViewItemSelectionAction(
			component: this,
			idList: selectedItems
		));
	}


}