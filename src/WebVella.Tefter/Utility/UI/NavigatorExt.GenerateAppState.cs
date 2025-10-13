namespace WebVella.Tefter.Utility;

using WebVella.Tefter.Models;

public static partial class NavigatorExt
{
	internal static TfState GenerateAppState(this NavigationManager navigator,
		ITfService _tfService,
		ITfMetaService _metaService,
		IStringLocalizer<TfService> LOC,
		TfUser currentUser,
		string? urlOverride = null,
		TfState? oldState = null,
		TfSpace? updatedSpace = null)
	{
		var navState = navigator.GetRouteState(urlOverride);
		var appState = new TfState();
		appState.Uri = navigator.Uri;
		appState.NavigationState = navState;

		//User
		appState.User = currentUser;


		//Get user saves and bookmarks
		if (oldState is not null && oldState.User.Id == currentUser.Id)
		{
			appState.UserBookmarks = oldState.UserBookmarks;
			appState.UserSaves = oldState.UserSaves;
		}
		else
		{
			appState.UserBookmarks = _tfService.GetBookmarksListForUser(currentUser.Id);
			appState.UserSaves = _tfService.GetSavesListForUser(currentUser.Id);
		}

		if (navState.RouteNodes.Count == 0) { }
		else if (navState.RouteNodes[0] == RouteDataNode.Admin)
		{
			if (!currentUser.IsAdmin)
				throw new Exception("Current user is not admin");

			var addonPages = _metaService.GetAdminAddonPages();
			appState.Menu = navState.generateAdminMenu(
				LOC: LOC,
				addonPages: addonPages);
			appState.generateBreadcrumb(LOC, navState, addonPages: addonPages);
		}
		else if (navState.RouteNodes[0] == RouteDataNode.Home
		         || navState.RouteNodes[0] == RouteDataNode.Space)
		{
			if (navState.SpaceId is not null)
			{
				//get space
				if (updatedSpace is not null)
				{
					appState.Space = updatedSpace;
				}
				else if (oldState is not null && oldState.Space is not null && oldState.Space.Id == navState.SpaceId)
					appState.Space = oldState.Space;
				else
					appState.Space = _tfService.GetSpace(navState.SpaceId.Value);

				if (appState.Space is not null)
				{
					//get space pages
					if (oldState is not null && oldState.Space?.Id == appState.Space.Id
					                         && oldState.SpacePages is not null)
						appState.SpacePages = oldState.SpacePages;
					else
						appState.SpacePages = _tfService.GetSpacePages(navState.SpaceId.Value);

					if (navState.SpacePageId is null)
						appState.SpacePage = null;
					else if (navState.SpacePageId == oldState?.SpacePage?.Id)
						appState.SpacePage = oldState?.SpacePage;
					else
						appState.SpacePage =_tfService.GetSpacePage(navState.SpacePageId.Value);


					appState.Menu = navState.generateSpaceMenu(
						spacePages: appState.SpacePages);

					appState.generateBreadcrumb(LOC, navState, spacePages: appState.SpacePages);
				}
			}
		}

		return appState;
	}

	private static List<TfMenuItem> generateAdminMenu(this TfNavigationState routeState,
		IStringLocalizer<TfService> LOC,
		ReadOnlyCollection<TfScreenRegionComponentMeta> addonPages)
	{
		var menuItems = new List<TfMenuItem>();

		#region << Dashboard >>

		menuItems.Add(new TfMenuItem()
		{
			Id = "tf-dashboard-link",
			IconCollapsed = TfConstants.GetIcon("Board"),
			IconExpanded = TfConstants.GetIcon("Board"),
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
				Selected = routeState.HasNode(RouteDataNode.Users, 1)
				           || routeState.HasNode(RouteDataNode.Roles, 1),
				Url = string.Format(TfConstants.AdminUsersPageUrl),
				Text = LOC["Access"],
				Expanded = true
			};
			rootMenu.Items.Add(new TfMenuItem()
			{
				Id = "tf-users-link",
				Selected = routeState.HasNode(RouteDataNode.Users, 1),
				Url = string.Format(TfConstants.AdminUsersPageUrl),
				Text = LOC["Users"]
			});
			rootMenu.Items.Add(new TfMenuItem()
			{
				Id = "tf-roles-link",
				Selected = routeState.HasNode(RouteDataNode.Roles, 1),
				Url = string.Format(TfConstants.AdminRolesPageUrl),
				Text = LOC["Roles"],
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
				Selected = routeState.HasNode(RouteDataNode.DataProviders, 1)
				           || routeState.HasNode(RouteDataNode.SharedColumns, 1)
				           || routeState.HasNode(RouteDataNode.DataIdentities, 1),
				Url = null,
				Text = LOC["Data"],
				Expanded = true
			};
			rootMenu.Items.Add(new TfMenuItem()
			{
				Id = "tf-data-providers-link",
				Selected = routeState.HasNode(RouteDataNode.DataProviders, 1),
				Url = string.Format(TfConstants.AdminDataProvidersPageUrl),
				Text = LOC["Data Providers"]
			});
			rootMenu.Items.Add(new TfMenuItem()
			{
				Id = "tf-shared-columns-link",
				Selected = routeState.HasNode(RouteDataNode.SharedColumns, 1),
				Url = string.Format(TfConstants.AdminSharedColumnsPageUrl),
				Text = LOC["Shared Columns"]
			});
			rootMenu.Items.Add(new TfMenuItem()
			{
				Id = "tf-identities-link",
				Selected = routeState.HasNode(RouteDataNode.DataIdentities, 1),
				Url = string.Format(TfConstants.AdminDataIdentitiesPageUrl),
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
				Selected = routeState.HasNode(RouteDataNode.Templates, 1)
				           || routeState.HasNode(RouteDataNode.FileRepository, 1),
				Url = null,
				Text = LOC["Content"],
				Expanded = true
			};

			rootMenu.Items.Add(new TfMenuItem()
			{
				Id = "tf-file-repository-link",
				Selected = routeState.HasNode(RouteDataNode.FileRepository, 1),
				Url = string.Format(TfConstants.AdminFileRepositoryPageUrl),
				Text = LOC["File Repository"]
			});
			rootMenu.Items.Add(new TfMenuItem()
			{
				Id = "tf-templates-link",
				Selected = routeState.HasNode(RouteDataNode.Templates, 1),
				Url = string.Format(TfConstants.AdminTemplatesPageUrl),
				Text = LOC["Templates"]
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
				IconCollapsed = TfConstants.GetIcon("AppFolder"),
				IconExpanded = TfConstants.GetIcon("AppFolder"),
				Selected = routeState.HasNode(RouteDataNode.Pages, 1),
				Url = null,
				Text = LOC["Addons"],
				Expanded = true
			};

			foreach (var addonPage in addonPages)
			{
				rootMenu.Items.Add(new()
				{
					Id = $"tf-addon-{addonPage.Id}",
					Selected = routeState.PageId == addonPage.Id,
					Url = string.Format(TfConstants.AdminPagesSingleUrl, addonPage.Id),
					Text = addonPage.Name
				});
			}

			menuItems.Add(rootMenu);
		}

		#endregion

		return menuItems;
	}

	private static List<TfMenuItem> generateSpaceMenu(this TfNavigationState navState,
		List<TfSpacePage> spacePages)
	{
		var menuItems = new List<TfMenuItem>();
		foreach (var page in spacePages)
		{
			menuItems.Add(page.generateMenuItemForPage(navState));
		}

		return menuItems;
	}

	private static TfMenuItem generateMenuItemForPage(this TfSpacePage page, TfNavigationState navState)
	{
		var item = new TfMenuItem()
		{
			Id = TfConverters.ConvertGuidToHtmlElementId(page.Id),
			IconCollapsed = TfConstants.GetIcon(page.FluentIconName),
			IconExpanded = TfConstants.GetIcon(page.FluentIconName),
			Selected = page.Id == navState.SpacePageId,
			Expanded = true,
			ChildSelected = page.GetChildPagesPlainList().Any(x => x.Id == navState.SpacePageId),
			Url = page.Type == TfSpacePageType.Folder
				? null
				: String.Format(TfConstants.SpacePagePageUrl, page.SpaceId, page.Id),
			Text = page.Name,
			Tooltip = page.Name
		};

		foreach (var childPage in page.ChildPages)
		{
			item.Items.Add(childPage.generateMenuItemForPage(navState));
		}

		return item;
	}

	private static void generateBreadcrumb(this TfState appState, IStringLocalizer<TfService> LOC,
		TfNavigationState navState,
		ReadOnlyCollection<TfScreenRegionComponentMeta>? addonPages = null,
		List<TfSpacePage>? spacePages = null)
	{
		var menu = new List<TfMenuItem>();
		menu.Add(new()
		{
			Id = "tf-app-switch",
			Text = LOC["Spaces"],
			IconCollapsed = TfConstants.GetIcon("AppFolder", IconSize.Size16),
			Url = "#"
		});

		if (navState.HasNode(RouteDataNode.Admin, 0))
		{
			menu.Add(new() { Id = "tf-admin", Text = LOC["Administration"], Url = TfConstants.AdminDashboardUrl });

			var accessNode = new TfMenuItem()
			{
				Id = "tf-admin-access", Text = LOC["Access"], Url = TfConstants.AdminUsersPageUrl
			};

			var dataNode = new TfMenuItem()
			{
				Id = "tf-admin-data", Text = LOC["Data"], Url = TfConstants.AdminDataProvidersPageUrl
			};

			var contentNode = new TfMenuItem()
			{
				Id = "tf-admin-content", Text = LOC["Content"], Url = TfConstants.AdminFileRepositoryPageUrl
			};

			var addonsNode = new TfMenuItem()
			{
				Id = "tf-admin-addons",
				Text = LOC["Addons"],
				Url = addonPages is not null && addonPages.Count > 0
					? String.Format(TfConstants.AdminPagesSingleUrl, addonPages[0].Id)
					: null
			};

			foreach (RouteDataNode node in navState.RouteNodes)
			{
				switch (node)
				{
					case RouteDataNode.Admin:
						//already added
						break;
					case RouteDataNode.Users:
						menu.Add(accessNode);
						if (navState.RouteNodes.Last() != node)
						{
							menu.Add(new()
							{
								Id = "tf-admin-access-users",
								Text = LOC["Users"],
								Url = TfConstants.AdminUsersPageUrl
							});
						}

						break;
					case RouteDataNode.Roles:
						menu.Add(accessNode);
						if (navState.RouteNodes.Last() != node)
						{
							menu.Add(new()
							{
								Id = "tf-admin-access-roles",
								Text = LOC["Roles"],
								Url = TfConstants.AdminRolesPageUrl
							});
						}

						break;
					case RouteDataNode.DataProviders:
						menu.Add(dataNode);
						if (navState.RouteNodes.Last() != node)
						{
							menu.Add(new()
							{
								Id = "tf-admin-data-data-providers",
								Text = LOC["Data Providers"],
								Url = TfConstants.AdminDataProvidersPageUrl
							});
						}
						break;
					case RouteDataNode.SharedColumns:
						menu.Add(dataNode);
						if (navState.RouteNodes.Last() != node)
						{
							menu.Add(new()
							{
								Id = "tf-admin-data-shared-columns",
								Text = LOC["Shared Columns"],
								Url = TfConstants.AdminSharedColumnsPageUrl
							});
						}

						break;
					case RouteDataNode.DataIdentities:
						menu.Add(dataNode);
						if (navState.RouteNodes.Last() != node)
						{
							menu.Add(new()
							{
								Id = "tf-admin-data-data-identities",
								Text = LOC["Data Identities"],
								Url = TfConstants.AdminDataIdentitiesPageUrl
							});
						}

						break;
					case RouteDataNode.FileRepository:
						menu.Add(contentNode);
						if (navState.RouteNodes.Last() != node)
						{
							menu.Add(new()
							{
								Id = "tf-admin-content-file-repository",
								Text = LOC["File Repository"],
								Url = TfConstants.AdminFileRepositoryPageUrl
							});
						}

						break;
					case RouteDataNode.Templates:
						menu.Add(contentNode);
						if (navState.RouteNodes.Last() != node)
						{
							menu.Add(new()
							{
								Id = "tf-admin-content-templates",
								Text = LOC["Templates"],
								Url = TfConstants.AdminTemplatesPageUrl
							});
						}

						break;
					case RouteDataNode.Pages:
						menu.Add(addonsNode);
						TfScreenRegionComponentMeta? addonpage = null;
						if (addonPages is not null)
							addonpage = addonPages.FirstOrDefault(x => x.Id == navState.PageId);
						if (addonpage is not null)
						{
							menu.Add(new()
							{
								Id = $"tf-admin-addon-page-{addonpage.Id}",
								Text = addonpage.Name,
								Url = String.Format(TfConstants.AdminPagesSingleUrl, addonpage.Id)
							});
						}

						break;
					default:
						break;
				}
			}
		}
		else if (navState.RouteNodes[0] == RouteDataNode.Space)
		{
			if (appState.Space is not null)
			{
				menu.Add(new()
				{
					Id = $"tf-space-{appState.Space.Id}",
					Text = appState.Space.Name,
					Url = spacePages is not null && spacePages.Count > 0
						? String.Format(TfConstants.SpacePagePageUrl, appState.Space.Id, spacePages[0].Id)
						: null
				});
			}
		}

		appState.Breadcrumb = menu;
	}
}