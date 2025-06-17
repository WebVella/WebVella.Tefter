namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.General.AdminHeaderNavigation.TfAdminHeaderNavigation", "WebVella.Tefter")]
public partial class TfAdminHeaderNavigation : TfBaseComponent
{
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
		_initMenu();
	}

	protected override void OnAfterRender(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		ActionSubscriber.SubscribeToAction<SetAppStateAction>(this, On_AppChanged);
	}

	private void On_AppChanged(SetAppStateAction action)
	{
		InvokeAsync(() =>
		{
			_initMenu();
			StateHasChanged();
		});
	}

	private void _initMenu()
	{
		menuItems = new();
		var routeData = TfAppState.Value?.Route?.RouteNodes;
		if (routeData is not null && routeData[0] == RouteDataNode.Admin)
		{
			if (TfAppState.Value.Route.HasNode(RouteDataNode.Users, 1)
				|| TfAppState.Value.Route.HasNode(RouteDataNode.Roles, 1))
			{
				menuItems.Add(new TucMenuItem()
				{
					Id = "tf-users-link",
					//IconCollapsed = TfConstants.AdminUsersIcon,
					Selected = TfAppState.Value.Route.HasNode(RouteDataNode.Users, 1),
					Url = string.Format(TfConstants.AdminUsersPageUrl),
					Text = LOC(TfConstants.AdminUsersMenuTitle)
				});
				menuItems.Add(new TucMenuItem()
				{
					Id = "tf-roles-link",
					//IconCollapsed = TfConstants.AdminRoleIcon,
					Selected = TfAppState.Value.Route.HasNode(RouteDataNode.Roles, 1),
					Url = string.Format(TfConstants.AdminRolesPageUrl),
					Text = LOC("Roles")
				});
			}
			else if (TfAppState.Value.Route.HasNode(RouteDataNode.DataProviders, 1)
			|| TfAppState.Value.Route.HasNode(RouteDataNode.SharedColumns, 1)
			|| TfAppState.Value.Route.HasNode(RouteDataNode.DataIdentities, 1)
			)
			{
				menuItems.Add(new TucMenuItem()
				{
					Id = "tf-data-providers-link",
					//IconCollapsed = TfConstants.AdminDataProvidersIcon,
					Selected = TfAppState.Value.Route.HasNode(RouteDataNode.DataProviders, 1),
					Url = string.Format(TfConstants.AdminDataProvidersPageUrl),
					Text = LOC(TfConstants.AdminDataProvidersMenuTitle)
				});
				menuItems.Add(new TucMenuItem()
				{
					Id = "tf-shared-columns-link",
					//IconCollapsed = TfConstants.AdminSharedColumnsIcon,
					Selected = TfAppState.Value.Route.HasNode(RouteDataNode.SharedColumns, 1),
					Url = TfConstants.AdminSharedColumnsPageUrl,
					Text = LOC(TfConstants.AdminSharedColumnsMenuTitle)
				});
				menuItems.Add(new TucMenuItem()
				{
					Id = "tf-data-identities-link",
					//IconCollapsed = TfConstants.AdminSharedColumnsIcon,
					Selected = TfAppState.Value.Route.HasNode(RouteDataNode.DataIdentities, 1),
					Url = TfConstants.AdminDataIdentitiesPageUrl,
					Text = LOC(TfConstants.AdminDataIdentitiesMenuTitle)
				});
			}
			else if (TfAppState.Value.Route.HasNode(RouteDataNode.FileRepository, 1)
			|| TfAppState.Value.Route.HasNode(RouteDataNode.Templates, 1))
			{
				menuItems.Add(new TucMenuItem()
				{
					Id = "tf-file-repository-link",
					//IconCollapsed = TfConstants.AdminFileRepositoryIcon,
					Selected = TfAppState.Value.Route.HasNode(RouteDataNode.FileRepository, 1),
					Url = TfConstants.AdminFileRepositoryPageUrl,
					Text = LOC(TfConstants.AdminFileRepositoryMenuTitle)
				});
				menuItems.Add(new TucMenuItem()
				{
					Id = "tf-templates-link",
					//IconCollapsed = TfConstants.TemplateIcon,
					Selected = TfAppState.Value.Route.HasNode(RouteDataNode.Templates, 1),
					Url = string.Format(TfConstants.AdminTemplatesTypePageUrl, (int)TfTemplateResultType.File),
					Text = LOC(TfConstants.AdminTemplatesMenuTitle)
				});
			}
		}
	}
}