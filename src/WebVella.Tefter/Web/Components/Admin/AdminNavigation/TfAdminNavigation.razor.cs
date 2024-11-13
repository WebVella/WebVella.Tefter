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

	private void On_AppChanged(SetAppStateAction action)
	{
		InvokeAsync(async () =>
		{
			generateMenu();
			await InvokeAsync(StateHasChanged);
		});
	}

	private void generateMenu()
	{
		menuItems.Clear();
		if(TfAppState.Value.Route is null) return;
		menuItems.Add(new TucMenuItem()
		{
			Id = "tf-dashboard-link",
			IconCollapsed = TfConstants.AdminDashboardIcon,
			IconColor = TfConstants.AdminThemeColor,
			Selected = TfAppState.Value.Route.SecondNode == RouteDataSecondNode.Dashboard,
			Url = "/admin",
			Text = LOC(TfConstants.AdminDashboardMenuTitle)
		});

		menuItems.Add(new TucMenuItem()
		{
			Id = "tf-users-link",
			IconCollapsed = TfConstants.AdminUsersIcon,
			IconColor = TfConstants.AdminThemeColor,
			Selected = TfAppState.Value.Route.SecondNode == RouteDataSecondNode.Users,
			Url = String.Format(TfConstants.AdminUsersPageUrl),
			Text = LOC(TfConstants.AdminUsersMenuTitle)
		});

		menuItems.Add(new TucMenuItem()
		{
			Id = "tf-data-providers-link",
			IconCollapsed = TfConstants.AdminDataProvidersIcon,
			IconColor = TfConstants.AdminThemeColor,
			Selected = TfAppState.Value.Route.SecondNode == RouteDataSecondNode.DataProviders,
			Url = String.Format(TfConstants.AdminDataProvidersPageUrl),
			Text = LOC(TfConstants.AdminDataProvidersMenuTitle)
		});
		menuItems.Add(new TucMenuItem()
		{
			Id = "tf-shared-columns-link",
			IconCollapsed = TfConstants.AdminSharedColumnsIcon,
			IconColor = TfConstants.AdminThemeColor,
			Selected = TfAppState.Value.Route.SecondNode == RouteDataSecondNode.SharedColumns,
			Url = TfConstants.AdminSharedColumnsPageUrl,
			Text = LOC(TfConstants.AdminSharedColumnsMenuTitle)
		});
		menuItems.Add(new TucMenuItem()
		{
			Id = "tf-file-repository-link",
			IconCollapsed = TfConstants.AdminFileRepositoryIcon,
			IconColor = TfConstants.AdminThemeColor,
			Selected = TfAppState.Value.Route.SecondNode == RouteDataSecondNode.FileRepository,
			Url = TfConstants.AdminFileRepositoryPageUrl,
			Text = LOC(TfConstants.AdminFileRepositoryMenuTitle)
		});
		if (TfAppState.Value.Pages.Count > 0)
		{
			menuItems.Add(new TucMenuItem()
			{
				Id = "tf-pages-link",
				IconCollapsed = TfConstants.ApplicationIcon,
				IconColor = TfConstants.AdminThemeColor,
				Selected = TfAppState.Value.Route.SecondNode == RouteDataSecondNode.Pages,
				Url = String.Format(TfConstants.AdminPagesSingleUrl, TfAppState.Value.Pages[0].Id),
				Text = LOC(TfConstants.AdminPagesMenuTitle)
			});
		}
	}

}