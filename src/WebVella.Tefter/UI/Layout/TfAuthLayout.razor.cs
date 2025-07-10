namespace WebVella.Tefter.UI.Layout;
public partial class TfAuthLayout : LayoutComponentBase, IAsyncDisposable
{
	[Inject] protected ITfUserUIService TfUserUIService { get; set; } = default!;
	[Inject] protected NavigationManager Navigator { get; set; } = default!;

	public ValueTask DisposeAsync()
	{
		Navigator.LocationChanged -= Navigator_LocationChanged;
		TfUserUIService.CurrentUserChanged -= CurrentUser_Changed;
		return ValueTask.CompletedTask;
	}

	private bool _isLoaded = false;

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();

		var user = await TfUserUIService.GetCurrentUserAsync();

		if (user is null)
		{
			Navigator.NavigateTo(TfConstants.LoginPageUrl, true);
			return;
		}
		var uri = new Uri(Navigator.Uri);
		var queryDictionary = System.Web.HttpUtility.ParseQueryString(uri.Query);

		if (uri.LocalPath == "/" && !String.IsNullOrWhiteSpace(user.Settings.StartUpUrl)
			&& queryDictionary[TfConstants.NoDefaultRedirectQueryName] is null)
		{
			Navigator.NavigateTo(user.Settings.StartUpUrl);
		}
		_isLoaded = true;
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
		await InvokeAsync(async () => await _checkAccess());
	}

	private async void Navigator_LocationChanged(object? sender, LocationChangedEventArgs e)
	{
		await InvokeAsync(async () => await _checkAccess());
	}

	private async Task _checkAccess()
	{
		var currentUser = await TfUserUIService.GetCurrentUserAsync();
		if (currentUser is not null && TfUserUIService.UserHasAccess(currentUser, Navigator))
			return;

		Navigator.NavigateTo(string.Format(TfConstants.NoAccessPage));
	}
}