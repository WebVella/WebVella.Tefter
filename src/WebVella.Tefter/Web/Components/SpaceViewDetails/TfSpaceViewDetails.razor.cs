namespace WebVella.Tefter.Web.Components;
public partial class TfSpaceViewDetails : TfBaseComponent
{
	[Inject] protected IState<SpaceState> SpaceState { get; set; }
	[Inject] protected IStateSelection<ScreenState, bool> ScreenStateSidebarExpanded { get; set; }
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
		ScreenStateSidebarExpanded.Select(x => x?.SidebarExpanded ?? true);
	}
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			var viewData = await UC.IInitSpaceViewDetailsAfterRender(SpaceState.Value);
			UC.IsBusy = false;
			await InvokeAsync(StateHasChanged);
			Dispatcher.Dispatch(new SetSpaceViewMetaAction(
				spaceViewColumns: UC.ViewColumns
			));
			Dispatcher.Dispatch(new SetSpaceViewDataAction(
				spaceViewData: viewData
			));
			ActionSubscriber.SubscribeToAction<SpaceStateChangedAction>(this, On_StateChanged);
			KeyCodeService.RegisterListener(OnKeyDownAsync);
		}
	}

	public async Task OnKeyDownAsync(FluentKeyCodeEventArgs args)
	{
		if (args.Key == KeyCode.PageUp) await _goNextPage();
		else if (args.Key == KeyCode.PageDown) await _goPreviousPage();


	}

	private void On_StateChanged(SpaceStateChangedAction action)
	{
		InvokeAsync(async () =>
		{
			UC.IsBusy = true;
			await InvokeAsync(StateHasChanged);
			var viewData = await UC.IInitSpaceViewDetailsAfterRender(SpaceState.Value);
			UC.IsBusy = false;
			await InvokeAsync(StateHasChanged);
			Dispatcher.Dispatch(new SetSpaceViewMetaAction(
				spaceViewColumns: UC.ViewColumns
			));
			Dispatcher.Dispatch(new SetSpaceViewDataAction(
				spaceViewData: viewData
			));
		});

	}

	private async Task _goFirstPage()
	{
		UC.Page = 1;
		var viewData = await UC.IInitSpaceViewDetailsAfterRender(SpaceState.Value);
		Dispatcher.Dispatch(new SetSpaceViewDataAction(
			spaceViewData: viewData
		));
	}
	private async Task _goPreviousPage()
	{
		UC.Page--;
		if (UC.Page < 1) UC.Page = 1;
		var viewData = await UC.IInitSpaceViewDetailsAfterRender(SpaceState.Value);
		Dispatcher.Dispatch(new SetSpaceViewDataAction(
			spaceViewData: viewData
		));
	}
	private async Task _goNextPage()
	{
		UC.Page++;
		var viewData = await UC.IInitSpaceViewDetailsAfterRender(SpaceState.Value);
		Dispatcher.Dispatch(new SetSpaceViewDataAction(
			spaceViewData: viewData
		));
	}
	private async Task _goLastPage()
	{
		UC.Page = -1;
		var viewData = await UC.IInitSpaceViewDetailsAfterRender(SpaceState.Value);
		Dispatcher.Dispatch(new SetSpaceViewDataAction(
			spaceViewData: viewData
		));
	}

	private async Task _goOnPage(int page)
	{
		UC.Page = page;
		if (UC.Page < 1) UC.Page = 1;
		var viewData = await UC.IInitSpaceViewDetailsAfterRender(SpaceState.Value);
		Dispatcher.Dispatch(new SetSpaceViewDataAction(
			spaceViewData: viewData
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
		var row = SpaceState.Value.SpaceViewData.Rows[rowIndex];	
		object rowId = row[TfConstants.TEFTER_ITEM_ID_PROP_NAME];
		if (rowId is not null)
		{
			return SpaceState.Value.SelectedDataRows.Contains((Guid)rowId);
		}
		return false;
	}

	private void _toggleItemSelection(int rowIndex, bool isChecked)
	{
		var row = SpaceState.Value.SpaceViewData.Rows[rowIndex];	
		object rowId = row[TfConstants.TEFTER_ITEM_ID_PROP_NAME];
		if (rowId is not null)
		{
			Dispatcher.Dispatch(new ToggleSpaceViewItemSelectionAction(
				idList: new List<Guid>{(Guid)rowId},
				isSelected:isChecked
			));
		}

	}


}