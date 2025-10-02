using DocumentFormat.OpenXml.Office.CustomUI;

namespace WebVella.Tefter.UIServices;

public partial interface ITfUIService
{
	event EventHandler<TfNavigationState> NavigationStateChanged;
	void InvokeNavigationStateChanged(TfNavigationState newState);
	TfNavigationMenu GetNavigationMenu(NavigationManager navigator, TfUser? currentUser);
}
public partial class TfUIService : ITfUIService
{
	#region << Events >>
	public event EventHandler<TfNavigationState> NavigationStateChanged = null!;
	#endregion

	#region << Navigation >>

	public void InvokeNavigationStateChanged(TfNavigationState newState)
		=> NavigationStateChanged?.Invoke(this, newState);



	public TfNavigationMenu GetNavigationMenu(NavigationManager navigator, TfUser? currentUser)
	{
		if (currentUser is null) return new TfNavigationMenu();

		lock (_lockObject)
		{
			var navState = navigator.GetRouteState();
			var navMenu = new TfNavigationMenu();
			navMenu.Uri = navigator.Uri;
			if (navState.RouteNodes is null || navState.RouteNodes.Count == 0)
			{
			}
			else if (navState.RouteNodes[0] == RouteDataNode.Admin)
			{
				if (!currentUser.IsAdmin)
					throw new Exception("Current user is not admin");
				navMenu.SpaceName = "Administration";
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
					routeState: navState,
					users: users,
					roles: roles,
					providers: providers,
					sharedColumns: sharedColumns,
					identities: dataIdentities,
					templates: templates,
					addonPages: addonPages);
			}
			else if (navState.RouteNodes[0] == RouteDataNode.Home
				|| navState.RouteNodes[0] == RouteDataNode.Space)
			{
				var spaces = _tfService.GetSpacesListForUser(currentUser.Id);
				var pages = _tfService.GetAllSpacePages();
				navMenu.Menu = generateSpaceMenu(
					routeState: navState,
					spaces: spaces,
					pages: pages,
					currentUser: currentUser);
			}
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
		List<TfSpacePage> pages)
	{
		var menuItems = new List<TfMenuItem>();

		#region << Spaces >>
		Dictionary<Guid, List<TfSpacePage>> pageDict = new();
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
		foreach (var space in spaces.OrderBy(x => x.Position))
		{
			var spacePages = pageDict.ContainsKey(space.Id) ? pageDict[space.Id] : new List<TfSpacePage>();
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
			if (currentUser.IsAdmin && spacePages.Count == 0)
			{
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
				{
					rootMenu.Items.Add(new TfMenuItem()
					{
						Id = $"tf-space-{space.Id}-delete",
						IconCollapsed = TfConstants.GetIcon("Delete"),
						IconExpanded = TfConstants.GetIcon("Delete"),
						IconColor = TfColor.Red500,
						Url = String.Empty,
						Text = LOC["Delete Space"],
						Tooltip = LOC["delete this space"],
						Data = new TfMenuItemData
						{
							SpaceId = space.Id,
							MenuType = TfMenuItemType.DeleteSpace
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
