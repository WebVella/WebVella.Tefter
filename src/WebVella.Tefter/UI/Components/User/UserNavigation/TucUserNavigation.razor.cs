using System.Threading.Tasks;

namespace WebVella.Tefter.UI.Components;

[LocalizationResource("WebVella.Tefter.UI.Components.General.UserNavigation.TucUserNavigation", "WebVella.Tefter")]
public partial class TucUserNavigation : TfBaseComponent, IDisposable
{
	[Inject] private ITfUserUIService TfUserUIService { get; set; } = default!;
	[Inject] private ITfNavigationUIService TfNavigationUIService { get; set; } = default!;

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
		_currentUser = await TfUserUIService.GetCurrentUserAsync();
		await initAdmin(null);
		Navigator.LocationChanged += Navigator_LocationChanged;
		TfUserUIService.CurrentUserChanged += CurrentUser_Changed;
	}
	private async void Navigator_LocationChanged(object? sender, LocationChangedEventArgs e)
	{
		if (!Navigator.UrlHasState()) return;
		await initAdmin(e.Location);
		StateHasChanged();
	}

	private void CurrentUser_Changed(object? sender, TfUser? user)
	{
		_currentUser = user;
		StateHasChanged();
	}

	private async Task initAdmin(string? location)
	{

		var navState = await TfNavigationUIService.GetNavigationStateAsync(Navigator);

		_adminMenu = new();
		if (_currentUser!.IsAdmin)
		{
			if (navState.RouteNodes.Count > 0 && navState.RouteNodes[0] == RouteDataNode.Admin)
			{
				_adminMenu.Add(new TfMenuItem
				{
					Url = TfConstants.HomePageUrl,
					Tooltip = LOC("Exit Administration"),
					IconCollapsed = TfConstants.HomeIcon,
					IconExpanded = TfConstants.HomeIcon,
					Text = LOC("Home")
				});
			}
			else
			{
				_adminMenu.Add(new TfMenuItem
				{
					Url = TfConstants.AdminDashboardUrl,
					Tooltip = LOC("Administration"),
					IconCollapsed = TfConstants.AdminIcon,
					IconExpanded = TfConstants.AdminIcon,
					Text = LOC("Admin")
				});
			}
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