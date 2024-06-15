using WebVella.Tefter.Web.Store.SessionState;
using WebVella.Tefter.Web.Store.UserState;

namespace WebVella.Tefter.Web.Layout;
public partial class MainLayout : FluxorLayout
{
	[Inject] protected ITfService TfService { get; set; }
	[Inject] protected NavigationManager Navigator { get; set; }
	[Inject] protected IState<UserState> UserState { get; set; }
	[Inject] protected IState<SessionState> SessionState { get; set; }
	[Inject] protected IDispatcher dispatcher { get; set; }

	private bool _firstRender = true;
	private bool _isLoading = true;

	private string _spaceColor = string.Empty;

	protected override ValueTask DisposeAsyncCore(bool disposing)
	{
		UserState.StateChanged -= UserState_StateChanged;
		SessionState.StateChanged -= SessionState_StateChanged;
		Navigator.LocationChanged -= Navigator_LocationChanged;
		return base.DisposeAsyncCore(disposing);
	}

	protected override void OnInitialized()
	{
		base.OnInitialized();
		Navigator.LocationChanged += Navigator_LocationChanged;
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

		Console.WriteLine($"******* UserState_StateChanged 1");

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

		Console.WriteLine($"******* UserState_StateChanged 2");
	}
	//Step 1: init session
	private void SessionState_StateChanged(object sender, EventArgs e)
	{
		if (SessionState.Value.IsLoading) return;
		Console.WriteLine($"******* SessionState_StateChanged 1");
		_isLoading = false;
		StateHasChanged();
		Console.WriteLine($"******* SessionState_StateChanged 1");
	}

	private void Navigator_LocationChanged(object sender, EventArgs e)
	{
		Console.WriteLine($"******* Navigator_LocationChanged 1");
		if (_isLoading) return;
		_initLocationChange();

		Console.WriteLine($"******* Navigator_LocationChanged 2");
	}

	private void _initLocationChange()
	{
		Console.WriteLine($"******* _initLocationChange 1");
		var urlData = NavigatorExt.GetUrlData(Navigator);

		Space space = null;
		SpaceData spaceData = null;
		SpaceView spaceView = null;

		if (urlData.SpaceId is not null)
		{
			if (urlData.SpaceId.HasValue)
			{
				if (SessionState.Value.SpaceDict is not null 
					&& SessionState.Value.SpaceDict.ContainsKey(urlData.SpaceId.Value))
				{
					space = SessionState.Value.SpaceDict[urlData.SpaceId.Value];
					spaceData = space.GetActiveData(urlData.SpaceDataId);
					if(spaceData is not null)
						spaceView = spaceData.GetActiveView(urlData.SpaceViewId);
				}
			}
		}
		if (space is null) space = SessionState.Value.Space;
		if (spaceData is null) spaceData = SessionState.Value.SpaceData;
		if (spaceView is null) spaceView = SessionState.Value.SpaceView;

		var hasSpaceDataChange = false;
		var sessionData = SessionState.Value;
		if (urlData.SpaceId != SessionState.Value.SpaceRouteId) hasSpaceDataChange = true;
		if (urlData.SpaceDataId != SessionState.Value.SpaceDataRouteId) hasSpaceDataChange = true;
		if (urlData.SpaceViewId != SessionState.Value.SpaceViewRouteId) hasSpaceDataChange = true;
		if (space?.Id != SessionState.Value.Space?.Id) hasSpaceDataChange = true;
		if (spaceData?.Id != SessionState.Value.SpaceData?.Id) hasSpaceDataChange = true;
		if (spaceView?.Id != SessionState.Value.SpaceView?.Id) hasSpaceDataChange = true;


		if (hasSpaceDataChange)
		{
			dispatcher.Dispatch(new SessionActiveSpaceDataChangeAction(
			spaceId: urlData.SpaceId,
			spaceDataId: urlData.SpaceDataId,
			spaceViewId: urlData.SpaceViewId,
			space: space,
			spaceData: spaceData,
			spaceView: spaceView
			));
		}
		Console.WriteLine($"******* _initLocationChange 2");
	}

}