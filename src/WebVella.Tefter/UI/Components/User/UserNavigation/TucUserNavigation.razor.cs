using System.Threading.Tasks;

namespace WebVella.Tefter.UI.Components;

public partial class TucUserNavigation : TfBaseComponent, IDisposable
{
	private bool _visible = false;
	private bool _helpVisible = false;
	private bool _isAdmin = false;
	private List<TfMenuItem> _adminMenu = new List<TfMenuItem>();

	public void Dispose()
	{
		TfUIService.NavigationStateChanged -= On_NavigationStateChanged;
		TfUIService.CurrentUserChanged -= CurrentUser_Changed;
		KeyCodeService.UnregisterListener(HandleKeyDownAsync);
	}

	protected override async Task OnInitializedAsync()
	{
		base.OnInitialized();
		await initAdmin(null);
		KeyCodeService.RegisterListener(HandleKeyDownAsync);
		TfUIService.NavigationStateChanged += On_NavigationStateChanged;
		TfUIService.CurrentUserChanged += CurrentUser_Changed;
	}
	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		if (UriInitialized != args.Uri)
			await initAdmin(args);
		StateHasChanged();
	}

	private void CurrentUser_Changed(object? sender, TfUser? user)
	{
		StateHasChanged();
	}

	public Task HandleKeyDownAsync(FluentKeyCodeEventArgs args)
	{
		if (args.CtrlKey && args.Key == KeyCode.KeyG)
			_openGlobalSearch();
		return Task.CompletedTask;
	}

	private async Task initAdmin(TfNavigationState? navState = null)
	{
		if(navState is null)
			navState = TfAuthLayout.NavigationState;

		_adminMenu = new();
		if (TfAuthLayout.CurrentUser!.IsAdmin)
		{
			_adminMenu.Add(new TfMenuItem
			{
				Url = TfConstants.AdminDashboardUrl,
				Tooltip = LOC("Administration"),
				IconCollapsed = TfConstants.GetIcon("Settings"),
				IconExpanded = TfConstants.GetIcon("Settings"),
				//Text = LOC("Admin"),
				Selected = navState.RouteNodes.Count > 0 && navState.RouteNodes[0] == RouteDataNode.Admin
			});
			_adminMenu.Add(new TfMenuItem
			{
				Url = "#",
				OnClick = EventCallback.Factory.Create(this, _openGlobalSearch),
				Tooltip = LOC("Global Search [CTRL+G]"),
				IconCollapsed = TfConstants.GetIcon("Search"),
				IconExpanded = TfConstants.GetIcon("Search"),
			});
		}
		UriInitialized = navState.Uri;
	}

	void _onClick()
	{
		_visible = !_visible;
	}

	Task _openGlobalSearch()
	{
		ToastService.ShowSuccess("Global search");
		Console.WriteLine("Clicked");
		return Task.CompletedTask;
	}

	async Task _setUrlAsStartup()
	{
		var uri = new Uri(Navigator.Uri);
		try
		{
			var user = await TfUIService.SetStartUpUrl(
						userId: TfAuthLayout.CurrentUser.Id,
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
		await TfUIService.LogoutAsync();
		Navigator.NavigateTo(TfConstants.LoginPageUrl, true);

	}


}