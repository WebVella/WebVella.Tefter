namespace WebVella.Tefter.Web.Components;
public partial class TfAppStateManager : FluxorComponent
{
	[Inject] protected NavigationManager Navigator { get; set; }
	[Inject] public IActionSubscriber ActionSubscriber { get; set; }
	[Inject] public IDispatcher Dispatcher { get; set; }
	[Inject] private IState<TfUserState> TfUserState { get; set; }
	[Inject] private IState<TfAppState> TfAppState { get; set; }
	[Inject] private IState<TfAuxDataState> TfAuxDataState { get; set; }
	[Inject] private AppStateUseCase UC { get; set; }
	[Parameter] public RenderFragment ChildContent { get; set; }

	private readonly AsyncLock locker = new AsyncLock();
	private Guid _renderedUserStateHash = Guid.Empty;
	private Guid _renderedAppStateHash = Guid.Empty;
	private bool _isBusy = true;
	protected override async ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			ActionSubscriber.UnsubscribeFromAllActions(this);
		}
		await base.DisposeAsyncCore(disposing);
	}

	protected override bool ShouldRender()
	{
		if (_renderedUserStateHash == TfUserState.Value.Hash
			&& _renderedAppStateHash == TfAppState.Value.Hash) return false;
		_renderedUserStateHash = TfUserState.Value.Hash;
		_renderedAppStateHash = TfAppState.Value.Hash;
		base.ShouldRender();
		return true;
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		if (firstRender)
		{
			await _init(null, new TfAppState(), new TfAuxDataState());
			_isBusy = false;
			_renderedUserStateHash = Guid.NewGuid(); //force rerender
			await InvokeAsync(StateHasChanged);
			ActionSubscriber.SubscribeToAction<SetRouteStateAction>(this, On_RouteChanged);
		}
	}

	private async Task _init(string url, TfAppState oldState, TfAuxDataState oldAuxState)
	{
		using (await locker.LockAsync())
		{
			if (TfUserState.Value.CurrentUser is null) return;
			var (appState,auxDataState) = await UC.InitState(TfUserState.Value.CurrentUser, url, oldState, oldAuxState);
			Dispatcher.Dispatch(new SetAppStateAction(
				component: null,
				state: appState
			));
			Dispatcher.Dispatch(new SetAuxDataStateAction(
				component: null,
				state: auxDataState
			));
		}
	}

	private void On_RouteChanged(SetRouteStateAction action)
	{
		InvokeAsync(async () =>
		{
			await _init(null, TfAppState.Value, TfAuxDataState.Value);
			//the change in the user state should triggger rerender later
		});
	}

}
