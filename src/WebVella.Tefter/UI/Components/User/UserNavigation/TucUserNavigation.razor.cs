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
		TfState.NavigationStateChangedEvent -= On_NavigationStateChanged;
		KeyCodeService.UnregisterListener(HandleKeyDownAsync);
	}

	protected override async Task OnInitializedAsync()
	{
		base.OnInitialized();
		await initAdmin(TfState.NavigationState);
		KeyCodeService.RegisterListener(HandleKeyDownAsync);
		TfState.NavigationStateChangedEvent += On_NavigationStateChanged;
	}
	private async Task On_NavigationStateChanged(TfNavigationState args)
	{
		await InvokeAsync(async () => { 
			if (UriInitialized != args.Uri)
				await initAdmin(args);
			
			await InvokeAsync(StateHasChanged);
		});		
	}

	public Task HandleKeyDownAsync(FluentKeyCodeEventArgs args)
	{
		if (args.CtrlKey && args.Key == KeyCode.KeyG)
			_openGlobalSearch();
		return Task.CompletedTask;
	}

	private async Task initAdmin(TfNavigationState navState)
	{
		_adminMenu = new();
		if (TfState.User!.IsAdmin)
		{
			_adminMenu.Add(new TfMenuItem
			{
				Url = TfConstants.AdminDashboardUrl,
				Tooltip = LOC("Administration"),
				Text = LOC("ADMIN"),
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
		return Task.CompletedTask;
	}

	async Task _setUrlAsStartup()
	{
		var uri = new Uri(Navigator.Uri);
		try
		{
			var user = await TfService.SetStartUpUrl(
						userId: TfState.User.Id,
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
		await TfService.LogoutAsync(JSRuntime);
		Navigator.NavigateTo(TfConstants.LoginPageUrl, true);

	}


}