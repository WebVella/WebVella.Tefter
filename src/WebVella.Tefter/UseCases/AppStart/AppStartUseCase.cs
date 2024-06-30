﻿namespace WebVella.Tefter.UseCases.AppStart;

internal partial class AppStartUseCase
{
	private readonly AuthenticationStateProvider _authenticationStateProvider;
	private readonly IJSRuntime _jsRuntime;
	private readonly IIdentityManager _identityManager;
	private readonly NavigationManager _navigationManager;
	public AppStartUseCase(
		AuthenticationStateProvider authenticationStateProvider,
		IJSRuntime jsRuntime,
		IIdentityManager identityManager,
		NavigationManager navigationManager)
	{
		_authenticationStateProvider = authenticationStateProvider;
		_jsRuntime = jsRuntime;
		_identityManager = identityManager;
		_navigationManager = navigationManager;
	}

	internal async Task<AppStartUseCase> InitializeAsync()
	{
		await UserInitializeAsync();
		await CultureInitializeAsync();
		return this;
	}
	internal bool IsLoading { get; set; } = true;
	internal TucUser User { get; set; }
	internal TucCultureOption CultureOption { get; set; }
	internal bool UserStateInited { get; set; } = false;
	internal bool CultureStateInited { get; set; } = false;
	internal bool ThemeStateInited { get; set; } = false;
	internal bool SidebarStateInited { get; set; } = false;
	internal bool AllInited
	{
		get => UserStateInited
		&& CultureStateInited
		&& ThemeStateInited
		&& SidebarStateInited;
	}
	internal async Task OnLocationChange()
	{
		var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
		var user = authState.User;
		//Temporary fix for multitab logout
		var cookie = await new CookieService(_jsRuntime).GetAsync(Constants.TEFTER_AUTH_COOKIE_NAME);

		if (cookie is null
		|| user.Identity is null || !user.Identity.IsAuthenticated)
		{
			_navigationManager.NavigateTo(TfConstants.LoginPageUrl, true);
			return;
		}
	}
}
