namespace WebVella.Tefter.UI.Components;

[LocalizationResource("WebVella.Tefter.UI.Components.General.UserNavigation.TucUserNavigation", "WebVella.Tefter")]
public partial class TucUserNavigation : TfBaseComponent, IDisposable
{
	[Inject] private ITfUserUIService TfUserUIService { get; set; } = default!;

	private TfUser? _currentUser = null;
	private bool _visible = false;
	private bool _helpVisible = false;
	private bool _isAdmin = false;
	private List<TfMenuItem> _adminMenu = new List<TfMenuItem>();

	public void Dispose()
	{
		Navigator.LocationChanged -= Navigator_LocationChanged;
		TfUserUIService.CurrentUserChanged -= CurrentUser_Changed;
	}

	protected override async Task OnInitializedAsync()
	{
		base.OnInitialized();
		_currentUser = await TfUserUIService.GetCurrentUser();
		initAdmin(null);
		Navigator.LocationChanged += Navigator_LocationChanged;
		TfUserUIService.CurrentUserChanged += CurrentUser_Changed;
	}
	private void Navigator_LocationChanged(object? sender, LocationChangedEventArgs e)
	{
		if (!Navigator.UrlHasState()) return;
		initAdmin(e.Location);
		StateHasChanged();
	}

	private void CurrentUser_Changed(object? sender, TfUser? user)
	{
		_currentUser = user;
		StateHasChanged();
	}

	private void initAdmin(string? location)
	{

		Uri? uri = null;
		if (string.IsNullOrEmpty(location))
		{
			uri = new Uri(Navigator.Uri);
		}
		else
		{
			uri = new Uri(location);
		}
		_isAdmin = uri.LocalPath.StartsWith("/admin");

		_adminMenu = new();
		if (_currentUser!.IsAdmin)
		{
			_adminMenu.Add(new TfMenuItem
			{
				Url = TfConstants.AdminDashboardUrl,
				Tooltip = @LOC("Administration"),
				IconCollapsed = TfConstants.AdminIcon,
				IconExpanded = TfConstants.AdminIcon
			});
		}
	}

	private void _onClick()
	{
		_visible = !_visible;
	}

	private async Task _setUrlAsStartup()
	{
		if (_currentUser is null) return;
		var uri = new Uri(Navigator.Uri);
		try
		{
			var user = await TfUserUIService.SetStartUpUrl(
						userId: _currentUser.Id,
						url: uri.PathAndQuery
					);
			ToastService.ShowSuccess(LOC("Startup URL was successfully changed!"));
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
	}

	private async Task _logout()
	{
		await TfUserUIService.LogoutAsync();
		Navigator.NavigateTo(TfConstants.LoginPageUrl, true);

	}


}