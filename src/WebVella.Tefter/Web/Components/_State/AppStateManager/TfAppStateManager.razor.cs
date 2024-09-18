namespace WebVella.Tefter.Web.Components;
public partial class TfAppStateManager : FluxorComponent
{
	[Inject] protected NavigationManager Navigator { get; set; }
	[Inject] public IActionSubscriber ActionSubscriber { get; set; }
	[Inject] public IDispatcher Dispatcher { get; set; }
	[Inject] private IState<TfUserState> TfUserState { get; set; }
	[Inject] private AppStateUseCase UC { get; set; }
	[Parameter] public RenderFragment ChildContent { get; set; }
	private readonly AsyncLock locker = new AsyncLock();
	private bool _isBusy = true;

	private Guid _oldRenderLock = Guid.Empty;
	private Guid _newRenderLock = Guid.Empty;

	protected override ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			Navigator.LocationChanged -= Navigator_LocationChanged;
		}
		return base.DisposeAsyncCore(disposing);
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		if (firstRender)
		{
			await _init(null, new TfAppState());
			Navigator.LocationChanged += Navigator_LocationChanged;
		}

	}

	protected override bool ShouldRender()
	{
		if(_oldRenderLock == _newRenderLock) return false;
		_oldRenderLock = _newRenderLock;

		return true;
	}


	private async Task _init(string url, TfAppState oldState)
	{
		using (await locker.LockAsync())
		{
			if (TfUserState.Value.CurrentUser is null) return;
			var state = await UC.InitState(TfUserState.Value.CurrentUser, url, oldState);
			Dispatcher.Dispatch(new SetAppStateAction(
				component: this,
				state: state with { IsBusy = false }
			));
			RegenRenderLock();
			await InvokeAsync(StateHasChanged);
		}
	}

	private void Navigator_LocationChanged(object sender, LocationChangedEventArgs e)
	{
		InvokeAsync(async () =>
		{
			await _init(e.Location, null);
		});


	}

	private void RegenRenderLock() => _newRenderLock = Guid.NewGuid();

}
