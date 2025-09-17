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
			if (_navState.RouteNodes is null || _navState.RouteNodes.Count == 0)
			{
			}
			else if (_navState.RouteNodes[0] == RouteDataNode.Admin)
			{
				if (!currentUser.IsAdmin)
					throw new Exception("Current user is not admin");
				navMenu.SpaceName = TfConstants.AdminMenuTitle;
				navMenu.SpaceColor = TfConstants.AdminColor;
				navMenu.SpaceIcon = TfConstants.GetIcon("Settings")!;


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
				var spaces = _tfService.GetSpacesListForUser(currentUser.Id);
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
			IconCollapsed = TfConstants.GetIcon("Board"),
			IconExpanded = TfConstants.GetIcon("Board"),
			IconColor = TfConstants.AdminColor,
			Selected = routeState.RouteNodes.Count == 1,
			Url = "/admin",
			Text = LOC["Dashboard"]
		});
		#endregion
		#region << Users >>
		{
			var rootMenu = new TfMenuItem()
			{
				Id = "tf-access-link",
				IconCollapsed = TfConstants.GetIcon("People"),
				IconExpanded = TfConstants.GetIcon("People"),
				IconColor = TfConstants.AdminColor,
				Selected = routeState.HasNode(RouteDataNode.Users, 1)
					|| routeState.HasNode(RouteDataNode.Roles, 1),
				Url = null,
				Text = LOC["Access"]
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
				Text = LOC["Users"]
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
				Text = LOC["Roles"]
			});

			menuItems.Add(rootMenu);
		}
		#endregion
		#region << Data Providers >>
		{
			var rootMenu = new TfMenuItem()
			{
				Id = "tf-data-link",
				IconCollapsed = TfConstants.GetIcon("Database"),
				IconExpanded = TfConstants.GetIcon("Database"),
				IconColor = TfConstants.AdminColor,
				Selected = routeState.HasNode(RouteDataNode.DataProviders, 1)
					|| routeState.HasNode(RouteDataNode.SharedColumns, 1)
					|| routeState.HasNode(RouteDataNode.DataIdentities, 1),
				Url = null,
				Text = LOC["Data"]
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
				Text = LOC["Data Providers"]
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
				Text = LOC["Shared Columns"]
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
				Text = LOC["Data Identities"]
			});
			menuItems.Add(rootMenu);
		}
		#endregion
		#region << Content >>
		{
			var rootMenu = new TfMenuItem()
			{
				Id = "tf-content-link",
				IconCollapsed = TfConstants.GetIcon("Folder"),
				IconExpanded = TfConstants.GetIcon("Folder"),
				IconColor = TfConstants.AdminColor,
				Selected = routeState.HasNode(RouteDataNode.Templates, 1)
					|| routeState.HasNode(RouteDataNode.FileRepository, 1),
				Url = null,
				Text = LOC["Content"]
			};

			rootMenu.Items.Add(new TfMenuItem()
			{
				Id = "tf-file-repository-link",
				//IconCollapsed = TfConstants.AdminFileRepositoryIcon,
				//IconExpanded = TfConstants.AdminFileRepositoryIcon,
				//IconColor = TfConstants.AdminColor,
				Selected = routeState.HasNode(RouteDataNode.FileRepository, 1),
				Url = string.Format(TfConstants.AdminFileRepositoryPageUrl),
				Text = LOC["File Repository"]
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
					Text = String.Format(LOC["Template {0}"], LOC[item.ToDescriptionString()])
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
				IconCollapsed = TfConstants.GetIcon("AppFolder"),
				IconExpanded = TfConstants.GetIcon("AppFolder"),
				IconColor = TfConstants.AdminColor,
				Selected = routeState.HasNode(RouteDataNode.Pages, 1),
				Url = String.Format(TfConstants.AdminPagesSingleUrl, addonPages[0].Id),
				Text = LOC["Addons"]
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
		foreach (var space in spaces.OrderBy(x => x.Position))
		{
			var spacePages = pageDict.ContainsKey(space.Id) ? pageDict[space.Id] : new List<TfSpacePage>();
			var spaceViews = viewDict.ContainsKey(space.Id) ? viewDict[space.Id] : new List<TfSpaceView>();
			var spaceData = dataDict.ContainsKey(space.Id) ? dataDict[space.Id] : new List<TfSpaceData>();
			Guid? firstPageId = null;
			foreach (var page in spacePages)
			{
				var pageId = page.GetFirstNavigatedPageId();
				if (pageId is not null)
				{
					firstPageId = pageId;
					break;
				}
			}
			var rootMenu = new TfMenuItem()
			{
				Id = $"tf-space-{space.Id}",
				IconCollapsed = TfConstants.GetIcon(space.FluentIconName),
				IconExpanded = TfConstants.GetIcon(space.FluentIconName),
				IconColor = space.Color,
				Selected = routeState.RouteNodes.Count >= 2 && routeState.RouteNodes[0] == RouteDataNode.Space && routeState.SpaceId == space.Id,
				Url = firstPageId is null
				? String.Empty
				: String.Format(TfConstants.SpacePagePageUrl, space.Id, firstPageId),
				Tooltip = space.Name,
				Text = space.Name
			};
			var subNodeIconColor = TfColor.Gray400;
			if (currentUser.IsAdmin)
			{
				{
					if (spacePages.Count > 0)
					{
						rootMenu.Items.Add(new TfMenuItem()
						{
							Id = $"tf-space-{space.Id}-browse",
							IconCollapsed = TfConstants.GetIcon(space.FluentIconName),
							IconExpanded = TfConstants.GetIcon(space.FluentIconName),
							IconColor = subNodeIconColor,
							Url = firstPageId is null
							? String.Empty
							: String.Format(TfConstants.SpacePagePageUrl, space.Id, firstPageId),
							Text = LOC["Browse Space"],
							Tooltip = LOC["navigate to this space"]
						});
					}
				}
				{
					var parentNode = new TfMenuItem()
					{
						Id = TfConverters.ConvertGuidToHtmlElementId(new Guid("f226fb6c-82cc-47cd-b2a3-e7e07b6f5c9d")),
						IconCollapsed = TfConstants.GetIcon("BookDatabase"),
						IconExpanded = TfConstants.GetIcon("BookDatabase"),
						IconColor = subNodeIconColor,
						Text = LOC["Space Properties"],
						Tooltip = LOC["the current space"],
					};
					parentNode.Items.Add(new TfMenuItem()
					{
						Id = $"tf-space-link-{space.Id}-manage",
						IconCollapsed = TfConstants.GetIcon("Settings"),
						IconExpanded = TfConstants.GetIcon("Settings"),
						IconColor = subNodeIconColor,
						Url = String.Format(TfConstants.SpaceManagePageUrl, space.Id),
						Text = LOC["Space Settings"],
						Tooltip = LOC["Manage the current space settings"]
					});

					if (spaceData.Count > 0)
					{
						parentNode.Items.Add(new TfMenuItem()
						{
							Id = $"tf-space-data-link-{space.Id}-list",
							IconCollapsed = TfConstants.GetIcon("Database"),
							IconExpanded = TfConstants.GetIcon("Database"),
							IconColor = subNodeIconColor,
							Url = String.Format(TfConstants.SpaceDataPageUrl, space.Id, spaceData[0].Id),
							Text = LOC["Space Datasets"],
							Tooltip = LOC["browse all existing datasets"]
						});
					}
					//parentNode.Items.Add(new TfMenuItem()
					//{
					//	Id = $"tf-space-data-link-{space.Id}-add",
					//	IconCollapsed = TfConstants.GetIcon("AddCircle"),
					//	IconExpanded = TfConstants.GetIcon("AddCircle"),
					//	IconColor = subNodeIconColor,
					//	Selected = false,
					//	Url = String.Empty,
					//	Text = LOC["New Dataset"],
					//	Tooltip = LOC["add new dataset to this space"],
					//	Data = new TfMenuItemData
					//	{
					//		SpaceId = space.Id,
					//		MenuType = TfMenuItemType.CreateSpaceData
					//	}
					//});
					if (spaceViews.Count > 0)
					{
						parentNode.Items.Add(new TfMenuItem()
						{
							Id = $"tf-space-views-link-{space.Id}-list",
							IconCollapsed = TfConstants.GetIcon("Table"),
							IconExpanded = TfConstants.GetIcon("Table"),
							IconColor = subNodeIconColor,
							Url = String.Format(TfConstants.SpaceViewPageUrl, space.Id, spaceViews[0].Id),
							Text = LOC["Space views"],
							Tooltip = LOC["browse all existing views"]
						});
					}
					//parentNode.Items.Add(new TfMenuItem()
					//{
					//	Id = $"tf-space-views-link-{space.Id}-add",
					//	IconCollapsed = TfConstants.GetIcon("AddCircle"),
					//	IconExpanded = TfConstants.GetIcon("AddCircle"),
					//	IconColor = subNodeIconColor,
					//	Url = String.Empty,
					//	Text = LOC["Create View"],
					//	Tooltip = LOC["add new view to this space"],
					//	Data = new TfMenuItemData
					//	{
					//		SpaceId = space.Id,
					//		MenuType = TfMenuItemType.CreateSpaceView
					//	}
					//});
					if (spacePages.Count > 0)
					{
						parentNode.Items.Add(new TfMenuItem()
						{
							Id = $"tf-space-pages-link-{space.Id}-list",
							IconCollapsed = TfConstants.GetIcon("TextBulletListTree"),
							IconExpanded = TfConstants.GetIcon("TextBulletListTree"),
							IconColor = subNodeIconColor,
							Url = String.Format(TfConstants.SpaceManagePagesPageUrl, space.Id),
							Text = LOC["Space Pages"],
							Tooltip = LOC["browse all existing pages"]
						});
					}
					rootMenu.Items.Add(parentNode);
				}
				{
					rootMenu.Items.Add(new TfMenuItem()
					{
						Id = $"tf-space-{space.Id}-add-page",
						IconCollapsed = TfConstants.GetIcon("AddCircle"),
						IconExpanded = TfConstants.GetIcon("AddCircle"),
						IconColor = subNodeIconColor,
						Url = String.Empty,
						Text = LOC["New Page"],
						Tooltip = LOC["add new page to this space"],
						Data = new TfMenuItemData
						{
							SpaceId = space.Id,
							MenuType = TfMenuItemType.CreateSpacePage
						}
					});
				}
			}
			menuItems.Add(rootMenu);
		}
		#endregion

		#region << Add space >>
		if (currentUser.IsAdmin)
		{
			menuItems.Add(new TfMenuItem()
			{
				Id = "tf-add-space",
				IconCollapsed = TfConstants.GetIcon("Add"),
				IconExpanded = TfConstants.GetIcon("Add"),
				IconColor = TfColor.White,
				Selected = false,
				Url = String.Empty,
				Tooltip = LOC["add new space"],
				Data = new TfMenuItemData
				{
					MenuType = TfMenuItemType.CreateSpace
				}
			});
		}
		#endregion
		return menuItems;
	}

	#endregion
}
