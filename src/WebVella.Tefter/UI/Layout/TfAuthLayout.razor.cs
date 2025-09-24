namespace WebVella.Tefter.UI.Layout;
public partial class TfAuthLayout : LayoutComponentBase, IAsyncDisposable
{
	[Inject] protected ITfUserUIService TfUserUIService { get; set; } = default!;
	[Inject] protected ITfConfigurationService TfConfigurationService { get; set; } = default!;
	[Inject] public ITfNavigationUIService TfNavigationUIService { get; set; } = default!;
	[Inject] protected NavigationManager Navigator { get; set; } = default!;

	public ValueTask DisposeAsync()
	{
		Navigator.LocationChanged -= Navigator_LocationChanged;
		TfUserUIService.CurrentUserChanged -= CurrentUser_Changed;
		return ValueTask.CompletedTask;
	}

	private bool _isLoaded = false;
	private TfUser _currentUser = default!;
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();

		var user = await TfUserUIService.GetCurrentUserAsync();

		if (user is null)
		{
			Navigator.NavigateTo(TfConstants.LoginPageUrl, true);
			return;
		}
		_currentUser = user;
		var uri = new Uri(Navigator.Uri);
		var queryDictionary = System.Web.HttpUtility.ParseQueryString(uri.Query);
		Uri? startupUri = null;
		if (!String.IsNullOrWhiteSpace(_currentUser.Settings.StartUpUrl))
		{
			if (_currentUser.Settings.StartUpUrl.StartsWith("http:"))
				startupUri = new Uri(_currentUser.Settings.StartUpUrl);
			else
				startupUri = new Uri(TfConfigurationService.BaseUrl + _currentUser.Settings.StartUpUrl);
		}


		if (uri.LocalPath == "/" && startupUri is not null && uri.LocalPath != startupUri.LocalPath
			&& queryDictionary[TfConstants.NoDefaultRedirectQueryName] is null)
		{
			Navigator.NavigateTo(_currentUser.Settings.StartUpUrl ?? "/", true);
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
			TfUserUIService.CurrentUserChanged += CurrentUser_Changed;
		}
	}

	private async void CurrentUser_Changed(object? sender, TfUser? user)
	{
		if (user is null)
		{
			Navigator.NavigateTo(TfConstants.LoginPageUrl, true);
			return;
		}
		_currentUser = user;
		await InvokeAsync(() => _checkAccess());
	}

	private async void Navigator_LocationChanged(object? sender, LocationChangedEventArgs e)
	{
		await InvokeAsync(() => _checkAccess());
		//Get state so it can trigger NavStateChange if needed
		await TfNavigationUIService.GetNavigationStateAsync(Navigator);
	}

	private void _checkAccess()
	{
		if (_currentUser is not null && TfUserUIService.UserHasAccess(_currentUser, Navigator))
			return;

		Navigator.NavigateTo(string.Format(TfConstants.NoAccessPage));
	}
}