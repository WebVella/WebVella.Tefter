namespace WebVella.Tefter.Web.Components;
public partial class TfSpaceViewDetails : TfBaseComponent
{
	[Inject] protected IState<SpaceState> SpaceState { get; set; }
	[Inject] protected IStateSelection<ScreenState, bool> ScreenStateSidebarExpanded { get; set; }
	[Inject] private SpaceUseCase UC { get; set; }
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
			ActionSubscriber.SubscribeToAction<SpaceViewMetaChangedAction>(this, On_StateViewMetaChanged);
			ActionSubscriber.SubscribeToAction<SpaceViewDataChangedAction>(this, On_StateViewDataChanged);
		}
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

	private void On_StateViewMetaChanged(SpaceViewMetaChangedAction action)
	{
		InvokeAsync(async () =>
		{
			await InvokeAsync(StateHasChanged);
		});
	}
	private void On_StateViewDataChanged(SpaceViewDataChangedAction action)
	{
		InvokeAsync(async () =>
		{
			await InvokeAsync(StateHasChanged);
		});

	}
	private async Task _goFirstPage()
	{
		if (UC.IsBusy) return;
		UC.IsBusy = true;
		await InvokeAsync(StateHasChanged);
		//await UC.DataProviderDataTableGoFirstPage(providerId: DataProviderDetailsState.Value.Provider.Id);
		UC.IsBusy = false;
		await InvokeAsync(StateHasChanged);
	}
	private async Task _goPreviousPage()
	{
		if (UC.IsBusy) return;
		UC.IsBusy = true;
		await InvokeAsync(StateHasChanged);
		//await UC.DataProviderDataTableGoPreviousPage(providerId: DataProviderDetailsState.Value.Provider.Id);
		UC.IsBusy = false;
		await InvokeAsync(StateHasChanged);
	}
	private async Task _goNextPage()
	{
		if (UC.IsBusy) return;
		UC.IsBusy = true;
		await InvokeAsync(StateHasChanged);
		//await UC.DataProviderDataTableGoNextPage(providerId: DataProviderDetailsState.Value.Provider.Id);
		UC.IsBusy = false;
		await InvokeAsync(StateHasChanged);
	}
	private async Task _goLastPage()
	{
		if (UC.IsBusy) return;
		UC.IsBusy = true;
		await InvokeAsync(StateHasChanged);
		//await UC.DataProviderDataTableGoLastPage(providerId: DataProviderDetailsState.Value.Provider.Id);
		UC.IsBusy = false;
		await InvokeAsync(StateHasChanged);
	}

	private async Task _goOnPage(int page)
	{
		if (UC.IsBusy) return;
		UC.IsBusy = true;
		await InvokeAsync(StateHasChanged);
		UC.Page = page;
		//await UC.DataProviderDataTableGoOnPage(providerId: DataProviderDetailsState.Value.Provider.Id);
		UC.IsBusy = false;
		await InvokeAsync(StateHasChanged);
	}
}