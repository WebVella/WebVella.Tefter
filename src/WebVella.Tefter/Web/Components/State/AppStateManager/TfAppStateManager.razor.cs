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
	//private IDisposable navigationChangingRegistration;
	protected override async ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			Navigator.LocationChanged -= Navigator_LocationChanged;
			//navigationChangingRegistration?.Dispose();
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
			Navigator.LocationChanged += Navigator_LocationChanged;
			//navigationChangingRegistration = Navigator.RegisterLocationChangingHandler(LocationChangingHandler);
		}
	}

	private void Navigator_LocationChanged(object sender, LocationChangedEventArgs e)
	{
		if(!Navigator.UrlHasState()) return;
		InvokeAsync(async () =>
		{

			await _init(e.Location,TfAppState.Value, TfAuxDataState.Value);
		});
	}

	private async Task _init(string url, TfAppState oldState, TfAuxDataState oldAuxState)
	{
		using (await locker.LockAsync())
		{
			if (TfUserState.Value.CurrentUser is null) return;
			var (appState, auxDataState) = await UC.InitState(TfUserState.Value.CurrentUser, url, oldState, oldAuxState);
			Dispatcher.Dispatch(new SetAppStateAction(
				component: null,
				state: appState
			));
			Dispatcher.Dispatch(new SetAuxDataStateAction(
				component: null,
				state: auxDataState
			));
			_isBusy = false;
			_renderedUserStateHash = Guid.NewGuid();
			await InvokeAsync(StateHasChanged);
		}
	}

	//private async ValueTask LocationChangingHandler(LocationChangingContext args)
	//{
	//	await _init(args.TargetLocation, TfAppState.Value, TfAuxDataState.Value);
	//	_renderedUserStateHash = Guid.NewGuid();
	//	await InvokeAsync(StateHasChanged);
	//}
	

}
