using WebVella.Tefter.Web.Store.SessionState;
using WebVella.Tefter.Web.Store.UserState;

namespace WebVella.Tefter.Web.Layout;
public partial class MainLayout : FluxorLayout
{
	[Inject] private IKeyCodeService KeyCodeService { get; set; }
	[Inject] protected ITfService TfService { get; set; }
	[Inject] protected NavigationManager Navigator { get; set; }
	[Inject] protected IState<UserState> UserState { get; set; }
	[Inject] protected IState<SessionState> SessionState { get; set; }
	[Inject] protected IDispatcher dispatcher { get; set; }

	private bool _firstRender = true;
	private bool _isLoading = true;

	private string _spaceColor = string.Empty;

	protected override async ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			KeyCodeService.UnregisterListener(OnKeyDownAsync, OnKeyUpAsync);
			UserState.StateChanged -= UserState_StateChanged;
			SessionState.StateChanged -= SessionState_StateChanged;
			Navigator.LocationChanged -= Navigator_LocationChanged;
		}

		await base.DisposeAsyncCore(disposing);
	}
	protected override void OnAfterRender(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		if (firstRender)
		{
			KeyCodeService.RegisterListener(OnKeyDownAsync, OnKeyUpAsync);
			Navigator.LocationChanged += Navigator_LocationChanged;
		}
	}

	protected override void OnInitialized()
	{
		base.OnInitialized();
	}

	// Overridden to provide an async method that is run before any rendering
	public override async Task SetParametersAsync(ParameterView parameters)
	{
		// Apply the supplied Parameters to the Conponent
		// Must do this first
		parameters.SetParameterProperties(this);

		// Run the Pre Render code
		if (_firstRender)
		{
			await this.PreFirstRenderAsync();
			_firstRender = false;
		}

		// Run the base SetParametersAsync providing it with an empty ParameterView
		// We have already applied the parameters and then may already be stale
		// This runs the normal lifecycle methods
		await base.SetParametersAsync(ParameterView.Empty);
	}

	// Separate out this component's code to make it clear what we're doing
	// We use a ValueTask because it's cheaper if no real async await occurs
	public ValueTask PreFirstRenderAsync()
	{
		// Your async code goes here
		// You can run sync code too if you wish
		dispatcher.Dispatch(new GetUserAction());
		UserState.StateChanged += UserState_StateChanged;
		return ValueTask.CompletedTask;
	}

	//Step 1: init user
	private void UserState_StateChanged(object sender, EventArgs e)
	{
		if (UserState.Value.IsLoading) return;

		if (UserState.Value.User is null)
		{
			Navigator.NavigateTo(TfConstants.LoginPageUrl);
			return;
		}

		var urlData = NavigatorExt.GetUrlData(Navigator);
		dispatcher.Dispatch(new GetSessionAction(
			userId: UserState.Value.User.Id,
			spaceId: urlData.SpaceId,
			spaceDataId: urlData.SpaceDataId,
			spaceViewId: urlData.SpaceViewId));
		SessionState.StateChanged += SessionState_StateChanged;
	}
	//Step 1: init session
	private void SessionState_StateChanged(object sender, EventArgs e)
	{
		InvokeAsync(async () =>
		{
			if (SessionState.Value.IsLoading) return;
			_isLoading = false;
			await InvokeAsync(StateHasChanged);
		});
	}
	private void Navigator_LocationChanged(object sender, EventArgs e)
	{
		InvokeAsync(() =>
		{
			if (_isLoading) return;
			_initLocationChange();
		});
	}
	private void _initLocationChange()
	{
		if (UserState is null || UserState.Value is null
		|| UserState.Value.IsLoading || UserState.Value.User is null) return;
		var urlData = NavigatorExt.GetUrlData(Navigator);

		dispatcher.Dispatch(new GetSessionAction(
	userId: UserState.Value.User.Id,
	spaceId: urlData.SpaceId,
	spaceDataId: urlData.SpaceDataId,
	spaceViewId: urlData.SpaceViewId));
	}

	public Task OnKeyDownAsync(FluentKeyCodeEventArgs args)
	{
		Console.WriteLine("keydown");
		DispatchUtils.DispatchKeyDown(
		dispatcher: dispatcher,
		sessionState: SessionState,
		userState: UserState,
		args: args);

		return Task.CompletedTask;
	}

	public Task OnKeyUpAsync(FluentKeyCodeEventArgs args)
	{
		Console.WriteLine("keyup");
		return Task.CompletedTask;
	}
}