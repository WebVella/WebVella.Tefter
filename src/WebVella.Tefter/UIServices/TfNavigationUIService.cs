namespace WebVella.Tefter.UIServices;

public partial interface ITfNavigationUIService
{
	event EventHandler<TfNavigationState> NavigationStateChanged;
	Task<TfNavigationState> GetNavigationState(NavigationManager navigator);
	Task<TfNavigationMenu> GetNavigationMenu(NavigationManager navigator);
}
public partial class TfNavigationUIService : ITfNavigationUIService
{
	#region << Ctor >>
	private static readonly AsyncLock _asyncLock = new AsyncLock();
	private readonly ITfService _tfService;
	private readonly ITfMetaService _metaService;
	private readonly IStringLocalizer<TfSpaceUIService> LOC;
	private TfNavigationState _navState = new();
	private TfNavigationMenu _navMenu = new();
	public TfNavigationUIService(IServiceProvider serviceProvider)
	{
		_tfService = serviceProvider.GetService<ITfService>() ?? default!;
		_metaService = serviceProvider.GetService<ITfMetaService>() ?? default!;
		LOC = serviceProvider.GetService<IStringLocalizer<TfSpaceUIService>>() ?? default!;
	}
	#endregion

	#region << Events >>
	public event EventHandler<TfNavigationState> NavigationStateChanged = default!;
	#endregion

	#region << Navigation >>
	public async Task<TfNavigationState> GetNavigationState(NavigationManager navigator)
	{

		using (await _asyncLock.LockAsync())
		{
			if (_navState.Uri == navigator.Uri)
			{
				return _navState;
			}
			_navState = navigator.GetRouteState();
			NavigationStateChanged?.Invoke(this, _navState);
			return _navState;
		}
	}
	public async Task<TfNavigationMenu> GetNavigationMenu(NavigationManager navigator)
	{

		using (await _asyncLock.LockAsync())
		{
			if (_navMenu.Uri == navigator.Uri)
			{
				return _navMenu;
			}
			if (_navState.Uri != navigator.Uri)
				_navState = navigator.GetRouteState();
			var navMenu = new TfNavigationMenu();
			navMenu.Uri = navigator.Uri;
			if (_navState.RouteNodes[0] == RouteDataNode.Admin)
			{
				navMenu.SpaceName = TfConstants.AdminMenuTitle;
				navMenu.SpaceColor = TfConstants.AdminColor;
				navMenu.SpaceIcon = TfConstants.AdminIcon;


				var users = _tfService.GetUsers();
				var roles = _tfService.GetRoles();
				var providers = _tfService.GetDataProviders();
				var sharedColumns = _tfService.GetSharedColumns();
				var dataIdentities = _tfService.GetDataIdentities();
				var addonPages = _metaService.GetRegionComponentsMeta(
					context: typeof(TfAdminPageScreenRegionContext),
					scope: null
				);
				var templates = _tfService.GetTemplates();

				navMenu.Menu = generateAdminMenu(
					routeState: _navState,
					users: users,
					roles: roles,
					providers: providers,
					sharedColumns: sharedColumns,
					identities: dataIdentities,
					templates:templates,
					addonPages: addonPages);
			}
			else
			{

			}
			_navMenu = navMenu;
			return navMenu;
		}
	}
	private List<TfMenuItem> generateAdminMenu(TfNavigationState routeState,
		ReadOnlyCollection<TfUser> users,
		ReadOnlyCollection<TfRole> roles,
		ReadOnlyCollection<TfDataProvider> providers,
		List<TfSharedColumn> sharedColumns,
		List<TfDataIdentity> identities,
		List<TfTemplate> templates,
		ReadOnlyCollection<TfScreenRegionComponentMeta> addonPages
		)
	{
		var menuItems = new List<TfMenuItem>();
		#region << Dashboard >>
		menuItems.Add(new TfMenuItem()
		{
			Id = "tf-dashboard-link",
			IconCollapsed = TfConstants.AdminDashboardIcon,
			IconExpanded = TfConstants.AdminDashboardIcon,
			IconColor = TfConstants.AdminColor,
			Selected = routeState.RouteNodes.Count == 1,
			Url = "/admin",
			Text = LOC[TfConstants.AdminDashboardMenuTitle]
		});
		#endregion
		#region << Users >>
		{
			var rootMenu = new TfMenuItem()
			{
				Id = "tf-access-link",
				IconCollapsed = TfConstants.AdminUsersIcon,
				IconExpanded = TfConstants.AdminUsersIcon,
				IconColor = TfConstants.AdminColor,
				Selected = routeState.HasNode(RouteDataNode.Users, 1)
					|| routeState.HasNode(RouteDataNode.Roles, 1),
				Url = null,
				Text = LOC[TfConstants.AdminAccessMenuTitle]
			};
			rootMenu.Items.Add(new TfMenuItem()
			{
				Id = "tf-users-link",
				//IconCollapsed = TfConstants.AdminUsersIcon,
				//IconExpanded = TfConstants.AdminUsersIcon,
				//IconColor = TfConstants.AdminColor,
				Selected = routeState.HasNode(RouteDataNode.Users, 1),
				Url = users.Count == 0
					? string.Format(TfConstants.AdminUsersPageUrl)
					: string.Format(TfConstants.AdminUserDetailsPageUrl, users[0].Id),
				Text = LOC[TfConstants.AdminUsersMenuTitle]
			});
			rootMenu.Items.Add(new TfMenuItem()
			{
				Id = "tf-roles-link",
				//IconCollapsed = TfConstants.AdminRoleIcon,
				//IconExpanded = TfConstants.AdminRoleIcon,
				//IconColor = TfConstants.AdminColor,
				Selected = routeState.HasNode(RouteDataNode.Roles, 1),
				Url = roles.Count == 0
					? string.Format(TfConstants.AdminRolesPageUrl)
					: string.Format(TfConstants.AdminRoleDetailsPageUrl, roles[0].Id),
				Text = LOC[TfConstants.AdminRolesMenuTitle]
			});

			menuItems.Add(rootMenu);
		}
		#endregion
		#region << Data Providers >>
		{
			var rootMenu = new TfMenuItem()
			{
				Id = "tf-data-link",
				IconCollapsed = TfConstants.AdminDataIcon,
				IconExpanded = TfConstants.AdminDataIcon,
				IconColor = TfConstants.AdminColor,
				Selected = routeState.HasNode(RouteDataNode.DataProviders, 1)
					|| routeState.HasNode(RouteDataNode.SharedColumns, 1)
					|| routeState.HasNode(RouteDataNode.DataIdentities, 1),
				Url = null,
				Text = LOC[TfConstants.AdminDataMenuTitle]
			};
			rootMenu.Items.Add(new TfMenuItem()
			{
				Id = "tf-data-providers-link",
				//IconCollapsed = TfConstants.AdminDataProviderIcon,
				//IconExpanded = TfConstants.AdminDataProviderIcon,
				//IconColor = TfConstants.AdminColor,
				Selected = routeState.HasNode(RouteDataNode.DataProviders, 1),
				Url = providers.Count == 0
					? string.Format(TfConstants.AdminDataProvidersPageUrl)
					: string.Format(TfConstants.AdminDataProviderDetailsPageUrl, providers[0].Id),
				Text = LOC[TfConstants.AdminDataProvidersMenuTitle]
			});
			rootMenu.Items.Add(new TfMenuItem()
			{
				Id = "tf-shared-columns-link",
				//IconCollapsed = TfConstants.AdminSharedColumnsIcon,
				//IconExpanded = TfConstants.AdminSharedColumnsIcon,
				//IconColor = TfConstants.AdminColor,
				Selected = routeState.HasNode(RouteDataNode.SharedColumns, 1),
				Url = sharedColumns.Count == 0
					? string.Format(TfConstants.AdminSharedColumnsPageUrl)
					: string.Format(TfConstants.AdminSharedColumnDetailsPageUrl, sharedColumns[0].Id),
				Text = LOC[TfConstants.AdminSharedColumnsMenuTitle]
			});
			rootMenu.Items.Add(new TfMenuItem()
			{
				Id = "tf-identities-link",
				//IconCollapsed = TfConstants.AdminDataIdentityIcon,
				//IconExpanded = TfConstants.AdminDataIdentityIcon,
				//IconColor = TfConstants.AdminColor,
				Selected = routeState.HasNode(RouteDataNode.DataIdentities, 1),
				Url = identities.Count == 0
					? string.Format(TfConstants.AdminDataIdentitiesPageUrl)
					: string.Format(TfConstants.AdminDataIdentityDetailsPageUrl, identities[0].DataIdentity),
				Text = LOC[TfConstants.AdminDataIdentitiesMenuTitle]
			});
			menuItems.Add(rootMenu);
		}
		#endregion
		#region << Content >>
		{
			var rootMenu = new TfMenuItem()
			{
				Id = "tf-content-link",
				IconCollapsed = TfConstants.ContentIcon,
				IconExpanded = TfConstants.ContentIcon,
				IconColor = TfConstants.AdminColor,
				Selected = routeState.HasNode(RouteDataNode.Templates, 1)
					|| routeState.HasNode(RouteDataNode.FileRepository, 1),
				Url = null,
				Text = LOC[TfConstants.AdminContentMenuTitle]
			};
			foreach (TfTemplateResultType item in Enum.GetValues<TfTemplateResultType>())
			{
				var typeTemplates = templates.Where(x=> x.ResultType == item).ToList();
				rootMenu.Items.Add(new TfMenuItem()
				{
					Id = $"tf-templates-link-{(int)item}",
					//IconCollapsed = TfConstants.TemplateIcon,
					//IconExpanded = TfConstants.TemplateIcon,
					//IconColor = TfConstants.AdminColor,
					Selected = routeState.HasNode(RouteDataNode.Templates, 1) && routeState.NodesDict.Count >= 3 
						&& routeState.TemplateResultType == item,
					Url = typeTemplates.Count == 0
					? string.Format(TfConstants.AdminTemplatesTypePageUrl,((int)item).ToString())
					: string.Format(TfConstants.AdminTemplatesTemplatePageUrl,((int)item).ToString(),typeTemplates[0].Id),
					Text = String.Format(LOC[TfConstants.AdminTemplateMenuTitle],LOC[item.ToDescriptionString()])
				});
			}

			rootMenu.Items.Add(new TfMenuItem()
			{
				Id = "tf-file-repository-link",
				//IconCollapsed = TfConstants.AdminFileRepositoryIcon,
				//IconExpanded = TfConstants.AdminFileRepositoryIcon,
				//IconColor = TfConstants.AdminColor,
				Selected = routeState.HasNode(RouteDataNode.FileRepository, 1),
				Url = string.Format(TfConstants.AdminFileRepositoryPageUrl),
				Text = LOC[TfConstants.AdminFileRepositoryMenuTitle]
			});
			menuItems.Add(rootMenu);
		}
		#endregion
		#region << Addons >>
		if (addonPages.Count > 0)
		{
			var rootMenu = new TfMenuItem()
			{
				Id = "tf-pages-link",
				IconCollapsed = TfConstants.ApplicationIcon,
				IconExpanded = TfConstants.ApplicationIcon,
				IconColor = TfConstants.AdminColor,
				Selected = routeState.HasNode(RouteDataNode.Pages, 1),
				Url = String.Format(TfConstants.AdminPagesSingleUrl, addonPages[0].Id),
				Text = LOC[TfConstants.AdminAddonsMenuTitle]
			};

			menuItems.Add(rootMenu);
		}
		#endregion

		return menuItems;
	}
	#endregion
}
