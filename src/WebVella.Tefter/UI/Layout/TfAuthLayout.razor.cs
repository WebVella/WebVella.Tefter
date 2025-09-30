namespace WebVella.Tefter.UI.Layout;

public partial class TfAuthLayout : LayoutComponentBase, IAsyncDisposable
{
	[Inject] public ITfUIService TfUIService { get; set; } = default!;
	[Inject] protected ITfConfigurationService TfConfigurationService { get; set; } = default!;
	[Inject] protected NavigationManager Navigator { get; set; } = default!;

	public ValueTask DisposeAsync()
	{
		Navigator.LocationChanged -= Navigator_LocationChanged;
		TfUIService.CurrentUserChanged -= CurrentUser_Changed;
		return ValueTask.CompletedTask;
	}

	public TfUser CurrentUser = default!;
	public TfNavigationState NavigationState = default!;
	public TfNavigationMenu NavigationMenu = default!;

	private bool _isLoaded = false;

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();

		var user = await TfUIService.GetCurrentUserAsync();

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
			NavigationState = await TfUIService.GetNavigationStateAsync(Navigator);
			NavigationMenu = await TfUIService.GetNavigationMenu(Navigator, CurrentUser);
		}

	}

	protected override void OnAfterRender(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		if (firstRender)
		{
			Navigator.LocationChanged += Navigator_LocationChanged;
			TfUIService.CurrentUserChanged += CurrentUser_Changed;
		}
	}

	private async void CurrentUser_Changed(object? sender, TfUser? user)
	{
		if (user is null)
		{
			Navigator.NavigateTo(TfConstants.LoginPageUrl, true);
			return;
		}
		CurrentUser = user;
		await InvokeAsync(() => _checkAccess());
	}

	private async void Navigator_LocationChanged(object? sender, LocationChangedEventArgs e)
	{
		await InvokeAsync(async () =>
		{
			_checkAccess();
			var navState = await TfUIService.GetNavigationStateAsync(Navigator);
			if (NavigationState?.Uri != navState.Uri)
			{
				NavigationState = navState;
				NavigationMenu = await TfUIService.GetNavigationMenu(Navigator, CurrentUser);
				TfUIService.InvokeNavigationStateChanged(NavigationState);
			}
		});

	}

	private void _checkAccess()
	{
		if (CurrentUser is not null && TfUIService.UserHasAccess(CurrentUser, Navigator))
			return;

		Navigator.NavigateTo(string.Format(TfConstants.NoAccessPage));
	}
}