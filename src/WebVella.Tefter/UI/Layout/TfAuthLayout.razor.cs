namespace WebVella.Tefter.UI.Layout;

public partial class TfAuthLayout : LayoutComponentBase, IAsyncDisposable
{
	[Inject] public ITfService TfService { get; set; } = null!;
	[Inject] protected ITfConfigurationService TfConfigurationService { get; set; } = null!;
	[Inject] protected NavigationManager Navigator { get; set; } = null!;
	[Inject] protected IJSRuntime JsRuntime { get; set; } = null!;
	[Inject] protected AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;
	[Inject] protected IToastService ToastService { get; set; } = null!;
	
	private TfState _state = new();
	private TfUser _currentUser = new();
	private bool _isLoaded = false;
	private string _urlInitialized = string.Empty;
	private string _styles = String.Empty;
	private IDisposable? _locationChangingHandler;

	public TfState GetState() => _state;
	public ValueTask DisposeAsync()
	{
		_locationChangingHandler?.Dispose();
		return ValueTask.CompletedTask;
	}	
	
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		//User checks
		var user = await TfService.GetUserFromCookieAsync(
			jsRuntime: JsRuntime,
			authStateProvider: AuthenticationStateProvider);

		if (user is null)
		{
			Navigator.NavigateTo(TfConstants.LoginPageUrl, true);
			return;
		}

		var uri = new Uri(Navigator.Uri);
		var queryDictionary = System.Web.HttpUtility.ParseQueryString(uri.Query);
		Uri? startupUri = null;
		if (!String.IsNullOrWhiteSpace(user.Settings.StartUpUrl))
		{
			if (user.Settings.StartUpUrl.StartsWith("http:"))
				startupUri = new Uri(user.Settings.StartUpUrl);
			else
				startupUri = new Uri(TfConfigurationService.BaseUrl + user.Settings.StartUpUrl);
		}
		if (uri.LocalPath == "/" && startupUri is not null && uri.LocalPath != startupUri.LocalPath
		    && queryDictionary[TfConstants.NoDefaultRedirectQueryName] is null)
		{
			Navigator.NavigateTo(user.Settings.StartUpUrl ?? "/", true);
		}
		else
		{
			if (!_checkAccess(Navigator.Uri))
				Navigator.NavigateTo(string.Format(TfConstants.NoAccessPage));

			_currentUser = user;
			//init state
			_init(Navigator.Uri);
			_urlInitialized = Navigator.Uri;			
			_isLoaded = true;
		}
	}

	protected override void OnAfterRender(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		if (firstRender)
		{
			_locationChangingHandler = Navigator.RegisterLocationChangingHandler(Navigator_LocationChanging);
		}
	}

	private ValueTask Navigator_LocationChanging(LocationChangingContext args)
	{
		if (_urlInitialized != args.TargetLocation)
		{
			if (!_checkAccess(args.TargetLocation))
			{
				ToastService.ShowError("Access Denied");
				args.PreventNavigation();
				return ValueTask.CompletedTask;		
			}

			_init(args.TargetLocation);
			_urlInitialized = args.TargetLocation;
		}
		return ValueTask.CompletedTask;
	}	
	
	private void _init(string url)
	{
		if(String.IsNullOrWhiteSpace(_state.Uri))
			_state = TfService.GetAppState(Navigator, _currentUser, url,null);
		else
			_state = TfService.GetAppState(Navigator, _currentUser, url,_state);

		_styles = (_state.Space?.Color ?? TfColor.Purple500).GenerateStylesForAccentColor();
	}

	private bool _checkAccess(string? url = null)
	{
		if (TfService.UserHasAccess(_currentUser, Navigator, url))
			return true;
		return false;
	}
}