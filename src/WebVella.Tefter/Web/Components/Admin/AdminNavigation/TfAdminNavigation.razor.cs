namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.AdminNavigation.TfAdminNavigation", "WebVella.Tefter")]
public partial class TfAdminNavigation : TfBaseComponent
{
	[Inject] protected IStateSelection<TfUserState, bool> SidebarExpanded { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	private List<TucMenuItem> menuItems = new List<TucMenuItem>();

	protected override async ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			ActionSubscriber.UnsubscribeFromAllActions(this);
		}
		await base.DisposeAsyncCore(disposing);
	}

	protected override void OnInitialized()
	{
		base.OnInitialized();
		SidebarExpanded.Select(x => x.SidebarExpanded);
		generateMenu();
	}

	protected override void OnAfterRender(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		if (firstRender)
			ActionSubscriber.SubscribeToAction<SetAppStateAction>(this, On_AppChanged);
	}

	private async void On_AppChanged(SetAppStateAction action)
	{
		await InvokeAsync(() =>
		{
			generateMenu();
			StateHasChanged();
		});
	}

	private void generateMenu()
	{
		menuItems.Clear();
		//var iconColor = "var(--tf-admin-color)";
		if (TfAppState.Value.Route is null) return;
		menuItems.Add(new TucMenuItem()
		{
			Id = "tf-dashboard-link",
			IconCollapsed = TfConstants.AdminDashboardIcon,
			Selected = TfAppState.Value.Route.RouteNodes.Count == 1,
			Url = "/admin",
			Text = LOC(TfConstants.AdminDashboardMenuTitle)
		});

		menuItems.Add(new TucMenuItem()
		{
			Id = "tf-users-link",
			IconCollapsed = TfConstants.AdminUsersIcon,
			Selected = TfAppState.Value.Route.HasNode(RouteDataNode.Users, 1)
				|| TfAppState.Value.Route.HasNode(RouteDataNode.Roles, 1),
			Url = string.Format(TfConstants.AdminUsersPageUrl),
			Text = LOC(TfConstants.AdminAccessMenuTitle)
		});

		menuItems.Add(new TucMenuItem()
		{
			Id = "tf-data-providers-link",
			IconCollapsed = TfConstants.AdminDataIcon,
			Selected = TfAppState.Value.Route.HasNode(RouteDataNode.DataProviders, 1)
			|| TfAppState.Value.Route.HasNode(RouteDataNode.SharedColumns, 1)
			|| TfAppState.Value.Route.HasNode(RouteDataNode.DataIdentities, 1),
			Url = string.Format(TfConstants.AdminDataProvidersPageUrl),
			Text = LOC(TfConstants.AdminLocalDataMenuTitle)
		});
		//menuItems.Add(new TucMenuItem()
		//{
		//	Id = "tf-shared-columns-link",
		//	IconCollapsed = TfConstants.AdminSharedColumnsIcon,
		//	Selected = TfAppState.Value.Route.HasNode(RouteDataNode.SharedColumns,1),
		//	Url = TfConstants.AdminSharedColumnsPageUrl,
		//	Text = LOC(TfConstants.AdminSharedColumnsMenuTitle)
		//});
		menuItems.Add(new TucMenuItem()
		{
			Id = "tf-content-link",
			IconCollapsed = TfConstants.ContentIcon,
			Selected = TfAppState.Value.Route.HasNode(RouteDataNode.Templates, 1)
			|| TfAppState.Value.Route.HasNode(RouteDataNode.FileRepository, 1),
			Url = TfConstants.AdminFileRepositoryPageUrl,
			Text = LOC(TfConstants.AdminLocalContentMenuTitle)
		});
		//menuItems.Add(new TucMenuItem()
		//{
		//	Id = "tf-templates-link",
		//	IconCollapsed = TfConstants.TemplateIcon,
		//	Selected = TfAppState.Value.Route.HasNode(RouteDataNode.Templates, 1),
		//	Url = string.Format(TfConstants.AdminTemplatesTypePageUrl, (int)TfTemplateResultType.File),
		//	Text = LOC(TfConstants.AdminTemplatesMenuTitle)
		//});
		//menuItems.Add(new TucMenuItem()
		//{
		//	Id = "tf-file-repository-link",
		//	IconCollapsed = TfConstants.AdminFileRepositoryIcon,
		//	Selected = TfAppState.Value.Route.HasNode(RouteDataNode.FileRepository,1),
		//	Url = TfConstants.AdminFileRepositoryPageUrl,
		//	Text = LOC(TfConstants.AdminFileRepositoryMenuTitle)
		//});
		if (TfAppState.Value.Pages.Count > 0)
		{
			menuItems.Add(new TucMenuItem()
			{
				Id = "tf-pages-link",
				IconCollapsed = TfConstants.ApplicationIcon,
				Selected = TfAppState.Value.Route.HasNode(RouteDataNode.Pages, 1),
				Url = string.Format(TfConstants.AdminPagesSingleUrl, TfAppState.Value.Pages[0].Id),
				Text = LOC(TfConstants.AdminAddonsMenuTitle)
			});
		}
	}

}