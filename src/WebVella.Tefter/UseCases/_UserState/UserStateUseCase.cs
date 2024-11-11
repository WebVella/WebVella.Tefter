using Microsoft.AspNetCore.Localization;

namespace WebVella.Tefter.UseCases.UserState;

internal partial class UserStateUseCase
{
	private readonly AuthenticationStateProvider _authenticationStateProvider;
	private readonly IJSRuntime _jsRuntime;
	private readonly IIdentityManager _identityManager;
	private readonly ITfSpaceManager _spaceManager;
	private readonly NavigationManager _navigationManager;

	public UserStateUseCase(IServiceProvider serviceProvider)
	{
		_authenticationStateProvider = serviceProvider.GetService<AuthenticationStateProvider>();
		_jsRuntime = serviceProvider.GetService<IJSRuntime>();
		_identityManager = serviceProvider.GetService<IIdentityManager>();
		_spaceManager = serviceProvider.GetService<ITfSpaceManager>();
		_navigationManager = serviceProvider.GetService<NavigationManager>();
	}


	internal virtual async Task<TfUserState> InitUserState()
	{
		var result = new TfUserState();

		//Init User
		var user = await GetUserFromCookieAsync();
		if (user is null) return null; //should redirect to login
		result = result with { CurrentUser = user };

		//Init Culture
		var culture = await InitCulture(result.CurrentUser);
		if (culture is null) return null;//should reload
		result = result with { Culture = culture };

		//Init Theme LocalStorage
		TucThemeSettings userTheme = null;
		var userThemeJson = await GetUnprotectedLocalStorage(TfConstants.UIThemeLocalKey);
		if(!String.IsNullOrWhiteSpace(userThemeJson))
		try{ 
			userTheme = JsonSerializer.Deserialize<TucThemeSettings>(userThemeJson);
		}catch{ }
		if(userTheme is null){ 
			userTheme = new TucThemeSettings { ThemeMode = user.Settings.ThemeMode, ThemeColor = user.Settings.ThemeColor };
			await SetUnprotectedLocalStorage(TfConstants.UIThemeLocalKey, JsonSerializer.Serialize(userTheme));
		}
		
		return result;
	}

	//user
	internal virtual async Task<User> GetUserWithChecks(Guid userId)
	{
		Result<User> userResult = await _identityManager.GetUserAsync(userId);
		if (userResult.IsFailed) throw new Exception("GetUserAsync failed");
		if (userResult.Value is null) throw new Exception("User not found failed");
		return userResult.Value;
	}

	internal virtual async Task<TucUser> GetUserFromCookieAsync()
	{
		var user = (await _authenticationStateProvider.GetAuthenticationStateAsync())?.User;
		if(user is null) return null;
		//Temporary fix for multitab logout- we check the cookie as well
		var cookie = await new CookieService(_jsRuntime).GetAsync(Constants.TEFTER_AUTH_COOKIE_NAME);
		if (cookie is null || user.Identity is null || !user.Identity.IsAuthenticated ||
			(user.Identity as TfIdentity) is null ||
			(user.Identity as TfIdentity).User is null)
		{
			//_navigationManager.NavigateTo(TfConstants.LoginPageUrl, true);
			return null;
		}
		var tfUser = ((TfIdentity)user.Identity).User;
		if (tfUser is null) return null;

		return new TucUser(tfUser);
	}

	public virtual async Task<Result<TucUser>> SetUserCulture(Guid userId, string cultureCode)
	{
		User user = await GetUserWithChecks(userId);

		var userBld = _identityManager.CreateUserBuilder(user);
		userBld.WithCultureCode(cultureCode);

		var saveResult = await _identityManager.SaveUserAsync(userBld.Build());
		if (saveResult.IsFailed)
			return Result.Fail(new Error("SaveUserAsync failed").CausedBy(saveResult.Errors));

		user = await GetUserWithChecks(userId);
		return Result.Ok(new TucUser(user));
	}

	internal virtual async Task<TucCultureOption> InitCulture(TucUser user)
	{
		var cultureCookie = await new CookieService(_jsRuntime).GetAsync(CookieRequestCultureProvider.DefaultCookieName);
		CultureInfo cookieCultureInfo = null;
		ProviderCultureResult cultureCookieValue = null;
		if (cultureCookie is not null)
			cultureCookieValue = CookieRequestCultureProvider.ParseCookieValue(cultureCookie.Value);
		if (cultureCookieValue != null && cultureCookieValue.UICultures.Count > 0)
		{
			try
			{
				var cookieCulture = CultureInfo.GetCultureInfo(cultureCookieValue.UICultures.First().ToString());
				if (TfConstants.CultureOptions.Any(x => x.CultureInfo.Name == cookieCulture.Name))
				{
					cookieCultureInfo = cookieCulture;
				}
			}
			//in case there is unrecognized culture in the cookie
			catch { }
		}

		var userCultureInfo = user is null || user.Settings is null || String.IsNullOrWhiteSpace(user.Settings.CultureName)
						? TfConstants.CultureOptions[0].CultureInfo
						: CultureInfo.GetCultureInfo(user.Settings.CultureName);

		if (cookieCultureInfo is null || cookieCultureInfo.Name != userCultureInfo.Name)
		{
			CultureInfo.CurrentCulture = userCultureInfo;
			CultureInfo.CurrentUICulture = userCultureInfo;

			await new CookieService(_jsRuntime).SetAsync(CookieRequestCultureProvider.DefaultCookieName,
					CookieRequestCultureProvider.MakeCookieValue(
						new RequestCulture(
							userCultureInfo,
							userCultureInfo)), DateTimeOffset.Now.AddYears(30));

			//_navigationManager.ReloadCurrentUrl();
			return null;
		}
		else
		{
			var cultureOption = TfConstants.CultureOptions.FirstOrDefault(x => x.CultureCode == userCultureInfo.Name);
			if (cultureOption is null) cultureOption = TfConstants.CultureOptions[0];
			return cultureOption;
		}
	}


	//Theme
	public virtual async Task<Result<TucUser>> SetUserTheme(Guid userId,
		DesignThemeModes themeMode, OfficeColor themeColor)
	{
		var user = await GetUserWithChecks(userId);
		var userBld = _identityManager.CreateUserBuilder(user);
		userBld
		.WithThemeMode(themeMode)
		.WithThemeColor(themeColor);
		var saveResult = await _identityManager.SaveUserAsync(userBld.Build());
		if (saveResult.IsFailed)
			return Result.Fail(new Error("SaveUserAsync failed").CausedBy(saveResult.Errors));

		await RemoveUnprotectedLocalStorage(TfConstants.UIThemeLocalKey);
		var themeSetting = new TucThemeSettings { ThemeMode = themeMode, ThemeColor = themeColor };
		await SetUnprotectedLocalStorage(TfConstants.UIThemeLocalKey, JsonSerializer.Serialize(themeSetting));
		user = await GetUserWithChecks(userId);
		return Result.Ok(new TucUser(user));
	}

	public virtual async Task<Result<TucUser>> SetStartUpUrl(Guid userId,
		string url)
	{
		var user = await GetUserWithChecks(userId);
		var userBld = _identityManager.CreateUserBuilder(user);
		userBld
		.WithStartUpUrl(url);
		var saveResult = await _identityManager.SaveUserAsync(userBld.Build());
		if (saveResult.IsFailed)
			return Result.Fail(new Error("SaveUserAsync failed").CausedBy(saveResult.Errors));
		user = await GetUserWithChecks(userId);
		return Result.Ok(new TucUser(user));
	}

	public virtual async Task<Result<TucUser>> SetPageSize(Guid userId,
		int? pageSize)
	{
		var user = await GetUserWithChecks(userId);
		var userBld = _identityManager.CreateUserBuilder(user);
		userBld
		.WithPageSize(pageSize);
		var saveResult = await _identityManager.SaveUserAsync(userBld.Build());
		if (saveResult.IsFailed)
			return Result.Fail(new Error("SaveUserAsync failed").CausedBy(saveResult.Errors));
		user = await GetUserWithChecks(userId);
		return Result.Ok(new TucUser(user));
	}

	internal virtual async Task SetUnprotectedLocalStorage(string key, string value)
	{
		await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, value);
	}

	internal virtual async Task RemoveUnprotectedLocalStorage(string key)
	{
		await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
	}

	internal virtual async Task<string> GetUnprotectedLocalStorage(string key)
	{
		return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", key);
	}

	//Sidebar
	public virtual async Task<Result<TucUser>> SetSidebarState(Guid userId,bool sidebarExpanded)
	{
		User user = await GetUserWithChecks(userId);
		var userBld = _identityManager.CreateUserBuilder(user).WithOpenSidebar(sidebarExpanded);
		var saveResult = await _identityManager.SaveUserAsync(userBld.Build());
		if (saveResult.IsFailed)
			return Result.Fail(new Error("SaveUserAsync failed").CausedBy(saveResult.Errors));
		user = await GetUserWithChecks(userId);
		return Result.Ok(new TucUser(user));
	}
}
