
namespace WebVella.Tefter.Web.Components;
public partial class TfUserNavigation
{
	[Inject] protected IState<UserState> UserState { get; set; }
	[Inject] protected IState<SessionState> SessionState { get; set; }

	private bool _visible = false;
	private bool _isAdmin = false;
	private bool _auxLoaded = false;

	protected override async ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			Navigator.LocationChanged -= Navigator_LocationChanged;
		}
		await base.DisposeAsyncCore(disposing);
	}


	protected override void OnAfterRender(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		if (firstRender)
		{
			initAdmin(null);
			StateHasChanged();
			Navigator.LocationChanged += Navigator_LocationChanged;
		}
	}

	private void Navigator_LocationChanged(object sender, LocationChangedEventArgs e)
	{
			initAdmin(e.Location);
			StateHasChanged();
	}

	private void initAdmin(string location)
	{
		Uri uri = null;
		if(string.IsNullOrEmpty(location)) {
			uri = new Uri(Navigator.Uri);
		}
		else{
			uri = new Uri(location);
		}
		_isAdmin = uri.LocalPath.StartsWith("/admin");
	}

	private void _onClick()
	{
		_visible = !_visible;
	}

	private async Task _setTheme()
	{
		var themeSettings = new ThemeSettings { ThemeMode = SessionState.Value.ThemeMode, ThemeColor = SessionState.Value.ThemeColor };
		var dialog = await DialogService.ShowDialogAsync<TfThemeSetDialog>(themeSettings, new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			var response = (ThemeSettings)result.Data;
			Dispatcher.Dispatch(new SetUIAction(UserState.Value.User.Id, response.ThemeMode, response.ThemeColor, SessionState.Value.SidebarExpanded));

		}
	}
	private void _editProfile()
	{
		ToastService.ShowToast(ToastIntent.Warning, "Will open edit profile modal");
	}
	private void _logout()
	{
		Dispatcher.Dispatch(new LogoutUserAction());

	}

	private void _alertsClick()
	{
		ToastService.ShowToast(ToastIntent.Warning, "Navigates to usr alerts");
	}

	private void _adminClick()
	{
		Navigator.NavigateTo("/admin");
	}

	private void _adminExitClick()
	{
		Navigator.NavigateTo("/");
	}

	private void _helpClick()
	{
		ToastService.ShowToast(ToastIntent.Warning, "Dropdown with menu to help section, license and about Tefter.bg");
	}


}