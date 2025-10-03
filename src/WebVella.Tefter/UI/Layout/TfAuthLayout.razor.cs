namespace WebVella.Tefter.UI.Layout;

public partial class TfAuthLayout : LayoutComponentBase, IAsyncDisposable
{
	[Inject] public ITfService TfService { get; set; } = null!;
	[Inject] protected ITfConfigurationService TfConfigurationService { get; set; } = null!;
	[Inject] protected NavigationManager Navigator { get; set; } = null!;
	[Inject] protected IJSRuntime JsRuntime { get; set; } = null!;
	[Inject] protected AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;

	public ValueTask DisposeAsync()
	{
		Navigator.LocationChanged -= Navigator_LocationChanged;

		return ValueTask.CompletedTask;
	}

	public event EventHandler<TfNavigationState> NavigationStateChangedEvent = null!;
	public TfUser CurrentUser = null!;
	public TfNavigationState NavigationState => Navigator.GetRouteState(); 
	public TfNavigationMenu NavigationMenu => TfService.GetNavigationMenu(Navigator, CurrentUser);

	private bool _isLoaded = false;

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();

		var user = await TfService.GetUserFromCookieAsync(
				jsRuntime:JsRuntime,
				authStateProvider: AuthenticationStateProvider);

		if (user is null)
		{
			Navigator.NavigateTo(TfConstants.LoginPageUrl, true);
			return;
		}

		CurrentUser = user;
		var uri = new Uri(Navigator.Uri);
		var queryDictionary = System.Web.HttpUtility.ParseQueryString(uri.Query);
		Uri? startupUri = null;
		if (!String.IsNullOrWhiteSpace(CurrentUser.Settings.StartUpUrl))
		{
			if (CurrentUser.Settings.StartUpUrl.StartsWith("http:"))
				startupUri = new Uri(CurrentUser.Settings.StartUpUrl);
			else
				startupUri = new Uri(TfConfigurationService.BaseUrl + CurrentUser.Settings.StartUpUrl);
		}


		if (uri.LocalPath == "/" && startupUri is not null && uri.LocalPath != startupUri.LocalPath
		    && queryDictionary[TfConstants.NoDefaultRedirectQueryName] is null)
		{
			Navigator.NavigateTo(CurrentUser.Settings.StartUpUrl ?? "/", true);
		}
		else
		{
			_checkAccess();
			_isLoaded = true;
		}
	}

	protected override void OnAfterRender(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		if (firstRender)
		{
			Navigator.LocationChanged += Navigator_LocationChanged;
		}
	}

	private void Navigator_LocationChanged(object? sender, LocationChangedEventArgs e)
	{
			_checkAccess();
			NavigationStateChangedEvent?.Invoke(this,Navigator.GetRouteState());
	}

	private void _checkAccess()
	{
		if (CurrentUser is not null && TfService.UserHasAccess(CurrentUser, Navigator))
			return;

		Navigator.NavigateTo(string.Format(TfConstants.NoAccessPage));
	}
}