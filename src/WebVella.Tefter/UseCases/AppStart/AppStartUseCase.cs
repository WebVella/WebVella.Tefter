namespace WebVella.Tefter.UseCases.AppStart;

internal partial class AppStartUseCase
{
	private readonly AuthenticationStateProvider _authenticationStateProvider;
	private readonly IJSRuntime _jsRuntime;
	private readonly IIdentityManager _identityManager;
	private readonly ITfSpaceManager _spaceManager;
	private readonly NavigationManager _navigationManager;

	public AppStartUseCase(
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
	internal async Task<TucInitState> InitState(){ 
		var result = new TucInitState();

		//Init User
		result.User = await InitUserAsync();	
		if(result.User is null) return null;

		//Init Culture
		result.CultureOption = await InitCulture(result.User);
		if(result.CultureOption is null) return null;

		//Init User spaces
		var uri = new Uri(_navigationManager.Uri);
		if(!uri.LocalPath.StartsWith("/admin")){ 
			result.UserSpaces = await InitUserSpaces(result.User);
		}

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
