using Microsoft.AspNetCore.Localization;

namespace WebVella.Tefter.Web.Components.StateProvider;
public partial class TfStateProvider : TfBaseComponent
{
	[Parameter]
	public EventCallback<bool> LoadingChange { get; set; }

	[Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; }
	[Inject] private IKeyCodeService KeyCodeService { get; set; }
	[Inject] protected IState<SessionState> SessionState { get; set; }

	private bool _isLoading = true;
	private bool _sessionInited = false;
	protected override async ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			KeyCodeService.UnregisterListener(OnKeyDownAsync);
			SessionState.StateChanged -= SessionState_StateChanged;
			Navigator.LocationChanged -= Navigator_LocationChanged;
		}

		await base.DisposeAsyncCore(disposing);
	}
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		if (firstRender)
		{
			var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
			var user = authState.User;
			//Temporary fix for multitab logout
			var cookie = await new CookieService(JSRuntimeSrv).GetAsync(Constants.TEFTER_AUTH_COOKIE_NAME);
			if (cookie is null
			|| user.Identity is null || !user.Identity.IsAuthenticated)
			{
				Navigator.NavigateTo(TfConstants.LoginPageUrl, true);
				return;
			}

			var tfUser = ((TfIdentity)authState.User.Identity).User;

			Dispatcher.Dispatch(new SetUserAction(tfUser));
			//Trigger session init
			var urlData = NavigatorExt.GetUrlData(Navigator);
			SessionState.StateChanged += SessionState_StateChanged;
			Dispatcher.Dispatch(new GetSessionAction(
				userId: tfUser.Id,
				spaceId: urlData.SpaceId,
				spaceDataId: urlData.SpaceDataId,
				spaceViewId: urlData.SpaceViewId));

			KeyCodeService.RegisterListener(OnKeyDownAsync);
			Navigator.LocationChanged += Navigator_LocationChanged;
		}
	}

	protected override bool ShouldRender() => false;

	/// <summary>
	/// Sets the Session states and shows the contents
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void SessionState_StateChanged(object sender, EventArgs e)
	{
		if (_sessionInited) return;
		InvokeAsync(async () =>
		{
			if (SessionState.Value.IsLoading) return;
			var cultureCookie = await new CookieService(JSRuntimeSrv).GetAsync(CookieRequestCultureProvider.DefaultCookieName);
			CultureInfo cookieCultureInfo = null;
			ProviderCultureResult cultureCookieValue = CookieRequestCultureProvider.ParseCookieValue(cultureCookie.Value);
			if (cultureCookieValue != null && cultureCookieValue.UICultures.Count > 0)
			{
				try
				{
					var cookieCulture = CultureInfo.GetCultureInfo(cultureCookieValue.UICultures.First().ToString());
					if(TfConstants.CultureOptions.Any(x=> x.CultureInfo.Name ==  cookieCulture.Name)) {
						cookieCultureInfo = cookieCulture;
					}
				}
				//in case there is unrecognized culture in the cookie
				catch{}
			}
			var culture = SessionState.Value.CultureOption is null ? TfConstants.CultureOptions[0].CultureInfo : CultureInfo.GetCultureInfo(SessionState.Value.CultureOption.CultureCode);

			if (cookieCultureInfo is null || cookieCultureInfo.Name != culture.Name)
			{
				CultureInfo.CurrentCulture = culture;
				CultureInfo.CurrentUICulture = culture;

				await new CookieService(JSRuntimeSrv).SetAsync(CookieRequestCultureProvider.DefaultCookieName,
						CookieRequestCultureProvider.MakeCookieValue(
							new RequestCulture(
								culture,
								culture)), DateTimeOffset.Now.AddYears(30));

				NavigatorExt.ReloadCurrentUrl(Navigator);
			}
			_sessionInited = true;
			if (_isLoading)
			{
				_isLoading = false;
				await LoadingChange.InvokeAsync(_isLoading);
			}
		});
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
			var cookie = await new CookieService(JSRuntimeSrv).GetAsync(Constants.TEFTER_AUTH_COOKIE_NAME);

			if (cookie is null
			|| user.Identity is null || !user.Identity.IsAuthenticated)
			{
				Navigator.NavigateTo(TfConstants.LoginPageUrl, true);
				return;
			}
		});
	}

	/// <summary>
	/// Monitors Key events globally
	/// </summary>
	/// <param name="args"></param>
	/// <returns></returns>
	public Task OnKeyDownAsync(FluentKeyCodeEventArgs args)
	{
		Console.WriteLine("keydown");
		DispatchUtils.DispatchKeyDown(
		dispatcher: Dispatcher,
		sessionState: SessionState,
		args: args);

		return Task.CompletedTask;
	}
}