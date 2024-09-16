namespace WebVella.Tefter.UseCases.AppStart;

internal partial class StateInitUseCase
{
	private readonly AuthenticationStateProvider _authenticationStateProvider;
	private readonly IJSRuntime _jsRuntime;
	private readonly IIdentityManager _identityManager;
	private readonly ITfSpaceManager _spaceManager;
	private readonly NavigationManager _navigationManager;

	public StateInitUseCase(
		AuthenticationStateProvider authenticationStateProvider,
		IJSRuntime jsRuntime,
		IIdentityManager identityManager,
		ITfSpaceManager spaceManager,
		NavigationManager navigationManager)
	{
		_authenticationStateProvider = authenticationStateProvider;
		_jsRuntime = jsRuntime;
		_identityManager = identityManager;
		_spaceManager = spaceManager;
		_navigationManager = navigationManager;
	}

	internal bool IsBusy { get; set; } = true;
	internal async Task<TfState> InitState()
	{
		var result = new TfState();

		//Init User
		var user = await InitUserAsync();
		if (user is null) return null; //should redirect to login
		result = result with { CurrentUser = user };

		//Init Culture
		var culture = await InitCulture(result.CurrentUser);
		if (culture is null) return null;//should reload
		result = result with { Culture = culture };


		//Init User spaces
		var urlData = _navigationManager.GetUrlData();
		var uri = new Uri(_navigationManager.Uri);
		if (
			urlData.FirstNode == RouteDataFirstNode.Home
			|| urlData.FirstNode == RouteDataFirstNode.FastAccess
			|| urlData.FirstNode == RouteDataFirstNode.Space
			)
		{
			result = result with { CurrentUserSpaces = await InitUserSpaces(result.CurrentUser) };
		}

		return result;
	}

	internal async Task<TfUserState> InitUserState()
	{
		var result = new TfUserState();

		//Init User
		var user = await InitUserAsync();
		if (user is null) return null; //should redirect to login
		result = result with { CurrentUser = user };

		//Init Culture
		var culture = await InitCulture(result.CurrentUser);
		if (culture is null) return null;//should reload
		result = result with { Culture = culture };
		return result;
	}

	//Was fix for logout but not needed anymore
	//internal async Task OnLocationChange()
	//{
	//	var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
	//	var user = authState.User;
	//	//Temporary fix for multitab logout
	//	var cookie = await new CookieService(_jsRuntime).GetAsync(Constants.TEFTER_AUTH_COOKIE_NAME);

	//	if (cookie is null
	//	|| user.Identity is null || !user.Identity.IsAuthenticated)
	//	{
	//		_navigationManager.NavigateTo(TfConstants.LoginPageUrl, true);
	//		return;
	//	}
	//}
}
