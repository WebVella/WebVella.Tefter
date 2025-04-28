namespace WebVella.Tefter.Web.Layout;
public partial class AdminLayout : FluxorLayout
{
	[Inject] protected IState<TfUserState> UserState { get; set; }
	[Inject] public IActionSubscriber ActionSubscriber { get; set; }
	[Inject] protected NavigationManager Navigator { get; set; }

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
		if(UserState.Value.CurrentUser.Roles.Any(x=> x.Id == TfConstants.ADMIN_ROLE_ID))  return;
		Navigator.NavigateTo(string.Format(TfConstants.NoAccessPage));
	}
}