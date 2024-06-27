using Microsoft.AspNetCore.Localization;

namespace WebVella.Tefter.Web.Components.StateInitializer;
public partial class TfStateInitializer : TfBaseComponent
{
	[Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; }
	[Parameter] public RenderFragment ChildContent { get; set; }
	private bool _isLoading = true;
	private TfStateInitializerContext context = new();

	protected override async ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			ActionSubscriber.UnsubscribeFromAllActions(this);
			Navigator.LocationChanged -= Navigator_LocationChanged;
		}

		await base.DisposeAsyncCore(disposing);
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		if (firstRender)
		{
			var user = (await AuthenticationStateProvider.GetAuthenticationStateAsync()).User;

			//Temporary fix for multitab logout- we check the cookie as well
			var cookie = await new CookieService(JSRuntime).GetAsync(Constants.TEFTER_AUTH_COOKIE_NAME);
			if (cookie is null || user.Identity is null || !user.Identity.IsAuthenticated ||
				(user.Identity as TfIdentity) is null ||
				(user.Identity as TfIdentity).User is null )
			{
				Navigator.NavigateTo(TfConstants.LoginPageUrl, true);
				return;
			}

			//Subscribe for state set results
			//so we can know when all states are inited
			ActionSubscriber.SubscribeToAction<SetUserActionResult>(this, initUserStateResult);
			ActionSubscriber.SubscribeToAction<SetCultureActionResult>(this, initCultureStateResult);
			ActionSubscriber.SubscribeToAction<SetThemeActionResult>(this, initThemeStateResult);
			ActionSubscriber.SubscribeToAction<SetSidebarActionResult>(this, initSidebarStateResult);

			//Setup states
			var tfUser = ((TfIdentity)user.Identity).User;
			initUserState(tfUser);
			await initCultureState(tfUser);
			initThemeState(tfUser);
			initSidebarState(tfUser);

			//For the logout fix
			Navigator.LocationChanged += Navigator_LocationChanged;

			//_isLoading = false;
			//await InvokeAsync(StateHasChanged);
		}
	}

	/// <summary>
	/// Inits user state from user
	/// </summary>
	/// <param name="user"></param>
	private void initUserState(User user)
	{
		Dispatcher.Dispatch(new SetUserAction(user));
	}

	/// <summary>
	/// Processes the user state init action result
	/// </summary>
	/// <param name="action"></param>
	private void initUserStateResult(SetUserActionResult action)
	{
		context.UserStateInited = true;
		CheckAllInited();
	}

	/// <summary>
	/// Inits the culture state from user
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private async Task initCultureState(User user)
	{

		var cultureCookie = await new CookieService(JSRuntime).GetAsync(CookieRequestCultureProvider.DefaultCookieName);
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
		var culture = String.IsNullOrWhiteSpace(user.Settings.CultureName)
				? TfConstants.CultureOptions[0].CultureInfo
				: CultureInfo.GetCultureInfo(user.Settings.CultureName);

		if (cookieCultureInfo is null || cookieCultureInfo.Name != culture.Name)
		{
			CultureInfo.CurrentCulture = culture;
			CultureInfo.CurrentUICulture = culture;

			await new CookieService(JSRuntime).SetAsync(CookieRequestCultureProvider.DefaultCookieName,
					CookieRequestCultureProvider.MakeCookieValue(
						new RequestCulture(
							culture,
							culture)), DateTimeOffset.Now.AddYears(30));

			NavigatorExt.ReloadCurrentUrl(Navigator);
		}
		else
		{
			var cultureOption = TfConstants.CultureOptions.FirstOrDefault(x => x.CultureCode == culture.Name);
			if (cultureOption is null) cultureOption = TfConstants.CultureOptions[0];
			Dispatcher.Dispatch(new SetCultureAction(
				userId: user.Id,
				culture: cultureOption,
				persist: false
			));
		}
	}

	/// <summary>
	/// Processes the culture state init action result
	/// </summary>
	/// <param name="action"></param>
	private void initCultureStateResult(SetCultureActionResult action)
	{
		context.CultureStateInited = true;
		CheckAllInited();
	}

	/// <summary>
	/// Inits the theme state from user
	/// </summary>
	/// <param name="user"></param>
	private void initThemeState(User user)
	{
		Dispatcher.Dispatch(new SetThemeAction(
			userId: user.Id,
			themeMode: user.Settings?.ThemeMode ?? TfConstants.DefaultThemeMode,
			themeColor: user.Settings?.ThemeColor ?? TfConstants.DefaultThemeColor,
			persist: false
		));
	}

	/// <summary>
	/// Processes the theme state init action result
	/// </summary>
	/// <param name="action"></param>
	private void initThemeStateResult(SetThemeActionResult action)
	{
		if (!_isLoading)
		{
			NavigatorExt.ReloadCurrentUrl(Navigator);
			return;
		}
		context.ThemeStateInited = true;
		CheckAllInited();

	}

	/// <summary>
	/// Inits the sidebar state from user
	/// </summary>
	/// <param name="user"></param>
	private void initSidebarState(User user)
	{
		Dispatcher.Dispatch(new SetSidebarAction(
		userId: user.Id,
		sidebarExpanded: user.Settings?.IsSidebarOpen ?? true,
		persist: false
	));
	}

	/// <summary>
	/// Processes the sidebar state init action result
	/// </summary>
	/// <param name="action"></param>
	private void initSidebarStateResult(SetSidebarActionResult action)
	{
		context.SidebarStateInited = true;
		CheckAllInited();
	}

	/// <summary>
	/// If all inited, removes the loading state
	/// </summary>
	private void CheckAllInited()
	{
		if (context.AllInited)
		{
			_isLoading = false;
			StateHasChanged();
		}
	}

	/// <summary>
	/// Fixing a problem when loging out from one tab leaves the user logged on the others
	/// Space data
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Navigator_LocationChanged(object sender, LocationChangedEventArgs e)
	{
		InvokeAsync(async () =>
		{
			var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
			var user = authState.User;
			//Temporary fix for multitab logout
			var cookie = await new CookieService(JSRuntime).GetAsync(Constants.TEFTER_AUTH_COOKIE_NAME);

			if (cookie is null
			|| user.Identity is null || !user.Identity.IsAuthenticated)
			{
				Navigator.NavigateTo(TfConstants.LoginPageUrl, true);
				return;
			}
		});
	}

}

public class TfStateInitializerContext
{
	public bool UserStateInited { get; set; } = false;
	public bool CultureStateInited { get; set; } = false;
	public bool ThemeStateInited { get; set; } = false;
	public bool SidebarStateInited { get; set; } = false;

	public bool AllInited
	{
		get => UserStateInited
		&& CultureStateInited
		&& ThemeStateInited
		&& SidebarStateInited;
	}
}