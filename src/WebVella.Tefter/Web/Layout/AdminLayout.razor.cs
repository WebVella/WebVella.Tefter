namespace WebVella.Tefter.Web.Layout;
public partial class AdminLayout : FluxorLayout
{
	[Inject] protected IState<TfUserState> UserState { get; set; }
	[Inject] public IActionSubscriber ActionSubscriber { get; set; }
	[Inject] public IWvBlazorTraceService WvBlazorTraceService { get; set; }
	[Inject] protected NavigationManager Navigator { get; set; }
	[Inject] private AppStateUseCase UC { get; set; }

	protected override async ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			ActionSubscriber.UnsubscribeFromAllActions(this);
			Navigator.LocationChanged -= Navigator_LocationChanged;
		}
		await base.DisposeAsyncCore(disposing);
	}

	protected override void OnInitialized()
	{
		base.OnInitialized();
		_checkAccess();
	}

	protected override void OnAfterRender(bool firstRender)
	{
		WvBlazorTraceService.OnSignal(this,signalName:"layout-refresh");
		base.OnAfterRender(firstRender);
		if (firstRender)
		{
			Navigator.LocationChanged += Navigator_LocationChanged;
			ActionSubscriber.SubscribeToAction<SetAppStateAction>(this, On_StateChanged);
		}
	}

	private void Navigator_LocationChanged(object sender, LocationChangedEventArgs e)
	{
		_checkAccess();
	}

	private void On_StateChanged(SetAppStateAction action)
	{
		_checkAccess();
	}

	private void _checkAccess()
	{
		if (UC.UserHasAccess(UserState.Value.CurrentUser, Navigator)) return;
		Navigator.NavigateTo(string.Format(TfConstants.NoAccessPage));
	}
}