using DocumentFormat.OpenXml.Office.CustomUI;

namespace WebVella.Tefter.UIServices;

public partial interface ITfNavigationUIService
{
	event EventHandler<TfNavigationState> NavigationStateChanged;
	Task<TfNavigationState> GetNavigationStateAsync(NavigationManager navigator);
	Task<TfNavigationMenu> GetNavigationMenu(NavigationManager navigator, TfUser? currentUser);
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
	public async Task<TfNavigationState> GetNavigationStateAsync(NavigationManager navigator)
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
	public async Task<TfNavigationMenu> GetNavigationMenu(NavigationManager navigator, TfUser? currentUser)
	{
		if (currentUser is null) return new TfNavigationMenu();

		using (await _asyncLock.LockAsync())
		{
			_navState = navigator.GetRouteState();
			var navMenu = new TfNavigationMenu();
			navMenu.Uri = navigator.Uri;
			if (_navState.RouteNodes[0] == RouteDataNode.Admin)
			{
				navMenu.SpaceName = TfConstants.AdminMenuTitle;
				navMenu.SpaceColor = TfConstants.AdminColor;
				navMenu.SpaceIcon = TfConstants.SettingsIcon;


				var users = _tfService.GetUsers();
				var roles = _tfService.GetRoles();
				var providers = _tfService.GetDataProviders();
				var sharedColumns = _tfService.GetSharedColumns();
				var dataIdentities = _tfService.GetDataIdentities();
				var addonPages = _metaService.GetAdminAddonPages();
				var templates = _tfService.GetTemplates();

				navMenu.Menu = generateAdminMenu(
					routeState: _navState,
					users: users,
					roles: roles,
					providers: providers,
					sharedColumns: sharedColumns,
					identities: dataIdentities,
					templates: templates,
					addonPages: addonPages);
			}
			else if (_navState.RouteNodes[0] == RouteDataNode.Home
				|| _navState.RouteNodes[0] == RouteDataNode.Space)
			{
				var spaces = _tfService.GetSpacesList();
				var pages = _tfService.GetAllSpacePages();
				var views = _tfService.GetAllSpaceViews();
				var data = _tfService.GetAllSpaceData();
				navMenu.Menu = generateSpaceMenu(
					routeState: _navState,
					spaces: spaces,
					pages: pages,
					views: views,
					data: data,
					currentUser: currentUser);
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
			foreach (TfTemplateResultType item in Enum.GetValues<TfTemplateResultType>())
			{
				var typeTemplates = templates.Where(x => x.ResultType == item).ToList();
				rootMenu.Items.Add(new TfMenuItem()
				{
					Id = $"tf-templates-link-{(int)item}",
					//IconCollapsed = TfConstants.TemplateIcon,
					//IconExpanded = TfConstants.TemplateIcon,
					//IconColor = TfConstants.AdminColor,
					Selected = routeState.HasNode(RouteDataNode.Templates, 1) && routeState.NodesDict.Count >= 3
						&& routeState.TemplateResultType == item,
					Url = typeTemplates.Count == 0
					? string.Format(TfConstants.AdminTemplatesTypePageUrl, ((int)item).ToString())
					: string.Format(TfConstants.AdminTemplatesTemplatePageUrl, ((int)item).ToString(), typeTemplates[0].Id),
					Text = String.Format(LOC[TfConstants.AdminTemplateMenuTitle], LOC[item.ToDescriptionString()])
				});
			}
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
	private List<TfMenuItem> generateSpaceMenu(TfNavigationState routeState,
		TfUser currentUser,
		List<TfSpace> spaces,
		List<TfSpacePage> pages,
		List<TfSpaceView> views,
		List<TfSpaceData> data)
	{
		var menuItems = new List<TfMenuItem>();

		#region << Spaces >>
		Dictionary<Guid, List<TfSpacePage>> pageDict = new();
		Dictionary<Guid, List<TfSpaceView>> viewDict = new();
		Dictionary<Guid, List<TfSpaceData>> dataDict = new();

		if (currentUser.IsAdmin)
		{
			foreach (var page in pages)
			{
				if (!pageDict.ContainsKey(page.SpaceId))
					pageDict[page.SpaceId] = new();

				pageDict[page.SpaceId].Add(page);
			}
			foreach (var spaceId in pageDict.Keys)
			{
				pageDict[spaceId] = pageDict[spaceId].OrderBy(x => x.Position).ToList();
			}

			foreach (var view in views)
			{
				if (!viewDict.ContainsKey(view.SpaceId))
					viewDict[view.SpaceId] = new();

				viewDict[view.SpaceId].Add(view);
			}
			foreach (var spaceId in viewDict.Keys)
			{
				viewDict[spaceId] = viewDict[spaceId].OrderBy(x => x.Position).ToList();
			}

			foreach (var item in data)
			{
				if (!dataDict.ContainsKey(item.SpaceId))
					dataDict[item.SpaceId] = new();

				dataDict[item.SpaceId].Add(item);
			}
			foreach (var spaceId in dataDict.Keys)
			{
				dataDict[spaceId] = dataDict[spaceId].OrderBy(x => x.Position).ToList();
			}
		}
		foreach (var space in spaces.OrderBy(x => x.Position))
		{
			var spacePages = pageDict.ContainsKey(space.Id) ? pageDict[space.Id] : new List<TfSpacePage>();
			var spaceViews = viewDict.ContainsKey(space.Id) ? viewDict[space.Id] : new List<TfSpaceView>();
			var spaceData = dataDict.ContainsKey(space.Id) ? dataDict[space.Id] : new List<TfSpaceData>();
			var rootMenu = new TfMenuItem()
			{
				Id = $"tf-space-{space.Id}",
				IconCollapsed = TfConstants.GetIcon(space.FluentIconName),
				IconExpanded = TfConstants.GetIcon(space.FluentIconName),
				IconColor = space.Color,
				Selected = routeState.RouteNodes.Count >= 2 && routeState.RouteNodes[0] == RouteDataNode.Space && routeState.SpaceId == space.Id,
				Url = spacePages.Count == 0
				? String.Empty
				: String.Format(TfConstants.SpaceNodePageUrl, space.Id, spacePages[0].Id),
				Tooltip = space.Name,
				Text = space.Name
			};
			var subNodeIconColor = space.Color;
			subNodeIconColor = TfColor.Gray400;
			if (currentUser.IsAdmin)
			{
				rootMenu.Items.Add(new TfMenuItem()
				{
					Id = $"tf-space-link-{space.Id}",
					IconCollapsed = TfConstants.SettingsIcon,
					IconExpanded = TfConstants.SettingsIcon,
					IconColor = subNodeIconColor,
					Selected = routeState.HasNode(RouteDataNode.Space, 0) && routeState.SpaceId == space.Id && routeState.RouteNodes.Count == 2,
					Url = String.Format(TfConstants.SpaceManagePageUrl, space.Id),
					Text = "Manage space",
					Tooltip = "Manage space"
				});
				if (spacePages.Count > 0)
				{
					rootMenu.Items.Add(new TfMenuItem()
					{
						Id = $"tf-space-pages-link-{space.Id}",
						IconCollapsed = TfConstants.PageIcon,
						IconExpanded = TfConstants.PageIcon,
						IconColor = subNodeIconColor,
						Selected = routeState.HasNode(RouteDataNode.Space, 0) && routeState.HasNode(RouteDataNode.Pages, 2) && routeState.SpaceId == space.Id,
						Url = String.Format(TfConstants.SpaceNodePageUrl, space.Id, spacePages[0].Id),
						Text = "Space pages",
						Tooltip = "Space pages"
					});
				}
				else
				{
					rootMenu.Items.Add(new TfMenuItem()
					{
						Id = $"tf-space-{space.Id}-add-page",
						IconCollapsed = TfConstants.AddIcon,
						IconExpanded = TfConstants.AddIcon,
						IconColor = subNodeIconColor,
						Selected = false,
						Url = String.Empty,
						Text = LOC["space page"],
						Tooltip = LOC["space page"],
						Data = new TfMenuItemData
						{
							SpaceId = space.Id,
							Type = TfMenuItemDataType.CreateSpacePage
						}
					});
				}
				if (spaceData.Count > 0)
				{
					rootMenu.Items.Add(new TfMenuItem()
					{
						Id = $"tf-space-data-link-{space.Id}",
						IconCollapsed = TfConstants.SpaceDataIcon,
						IconExpanded = TfConstants.SpaceDataIcon,
						IconColor = subNodeIconColor,
						Selected = routeState.HasNode(RouteDataNode.Space, 0) && routeState.HasNode(RouteDataNode.SpaceData, 2) && routeState.SpaceId == space.Id,
						Url = String.Format(TfConstants.SpaceDataPageUrl, space.Id, spaceData[0].Id),
						Text = "Space data",
						Tooltip = "Space data"
					});
				}
				else
				{
					rootMenu.Items.Add(new TfMenuItem()
					{
						Id = $"tf-space-data-link-{space.Id}",
						IconCollapsed = TfConstants.AddIcon,
						IconExpanded = TfConstants.AddIcon,
						IconColor = subNodeIconColor,
						Selected = false,
						Url = String.Empty,
						Text = LOC["space dataset"],
						Tooltip = LOC["space dataset"],
						Data = new TfMenuItemData
						{
							SpaceId = space.Id,
							Type = TfMenuItemDataType.CreateSpaceData
						}
					});
				}
				if (spaceViews.Count > 0)
				{
					rootMenu.Items.Add(new TfMenuItem()
					{
						Id = $"tf-space-views-link-{space.Id}",
						IconCollapsed = TfConstants.SpaceViewIcon,
						IconExpanded = TfConstants.SpaceViewIcon,
						IconColor = subNodeIconColor,
						Selected = routeState.HasNode(RouteDataNode.Space, 0) && routeState.HasNode(RouteDataNode.SpaceView, 2) && routeState.SpaceId == space.Id,
						Url = String.Format(TfConstants.SpaceViewPageUrl, space.Id, spaceViews[0].Id),
						Text = "Space views",
						Tooltip = "Space views"
					});
				}
				else
				{
					rootMenu.Items.Add(new TfMenuItem()
					{
						Id = $"tf-space-views-link-{space.Id}",
						IconCollapsed = TfConstants.AddIcon,
						IconExpanded = TfConstants.AddIcon,
						IconColor = subNodeIconColor,
						Selected = false,
						Url = String.Empty,
						Text = LOC["space view"],
						Tooltip = LOC["space view"],
						Data = new TfMenuItemData
						{
							SpaceId = space.Id,
							Type = TfMenuItemDataType.CreateSpaceView
						}
					});
				}
			}
			menuItems.Add(rootMenu);
		}
		#endregion

		#region << Add space >>
		menuItems.Add(new TfMenuItem()
		{
			Id = "tf-add-space",
			IconCollapsed = TfConstants.AddIcon,
			IconExpanded = TfConstants.AddIcon,
			IconColor = TfColor.White,
			Selected = false,
			Url = String.Empty,
			Tooltip = LOC["add new space"],
			Data = new TfMenuItemData
			{
				Type = TfMenuItemDataType.CreateSpace
			}
		});
		#endregion
		return menuItems;
	}
	#endregion
}
