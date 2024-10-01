namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.AdminNavigation.TfAdminNavigation", "WebVella.Tefter")]
public partial class TfAdminNavigation : TfBaseComponent
{
	[Inject] protected IState<TfUserState> TfUserState { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] protected IState<TfRouteState> TfRouteState { get; set; }
	private List<TucMenuItem> menuItems = new List<TucMenuItem>();

	protected override void OnInitialized()
	{
		base.OnInitialized();
		generateMenu();
	}

	private void generateMenu()
	{
		menuItems.Clear();
		menuItems.Add(new TucMenuItem()
		{
			Id = "tf-dashboard-link",
			Icon = TfConstants.AdminDashboardIcon,
			IconColor = TfConstants.AdminThemeColor,
			Match = NavLinkMatch.All,
			Url = "/admin",
			Title = LOC(TfConstants.AdminDashboardMenuTitle)
		});

		var usersUrl = String.Format(TfConstants.AdminUsersPageUrl);
		if (TfAppState.Value.AdminUsers.Count > 0)
		{
			usersUrl = String.Format(TfConstants.AdminUserDetailsPageUrl, TfAppState.Value.AdminUsers[0].Id);
		}
		menuItems.Add(new TucMenuItem()
		{
			Id = "tf-users-link",
			Icon = TfConstants.AdminUsersIcon,
			IconColor = TfConstants.AdminThemeColor,
			Match = NavLinkMatch.Prefix,
			Url = usersUrl,
			Title = LOC(TfConstants.AdminUsersMenuTitle)
		});

		var dpUrl = String.Format(TfConstants.AdminDataProvidersPageUrl);
		if (TfAppState.Value.AdminDataProviders.Count > 0)
		{
			dpUrl = String.Format(TfConstants.AdminDataProviderDetailsPageUrl, TfAppState.Value.AdminDataProviders[0].Id);
		}
		menuItems.Add(new TucMenuItem()
		{
			Id = "tf-data-providers-link",
			Icon = TfConstants.AdminDataProvidersIcon,
			IconColor = TfConstants.AdminThemeColor,
			Match = NavLinkMatch.Prefix,
			Url = dpUrl,
			Title = LOC(TfConstants.AdminDataProvidersMenuTitle)
		});
		menuItems.Add(new TucMenuItem()
		{
			Id = "tf-aux-columns-link",
			Icon = TfConstants.AdminAuxColumnsIcon,
			IconColor = TfConstants.AdminThemeColor,
			Match = NavLinkMatch.Prefix,
			Url = "/admin/aux-columns",
			Title = LOC(TfConstants.AdminAuxColumnsMenuTitle)
		});
		if (TfAppState.Value.Pages.Count > 0)
		{
			menuItems.Add(new TucMenuItem()
			{
				Id = "tf-pages-link",
				Icon = TfConstants.ApplicationIcon,
				IconColor = TfConstants.AdminThemeColor,
				Match = NavLinkMatch.Prefix,
				Url = String.Format(TfConstants.AdminPagesSingleUrl, TfAppState.Value.Pages[0].Slug),
				Title = LOC(TfConstants.AdminPagesMenuTitle)
			});
		}
	}

	private void _addSpaceHandler()
	{
		ToastService.ShowToast(ToastIntent.Warning, "Will open add new space modal");
	}

	private string _spaceSelectedClass(string url)
	{
		var uri = new Uri(Navigator.Uri);
		if (url == TfConstants.AdminDashboardUrl)
		{
			if (uri.LocalPath == url)
				return "selected";
			else
				return "";
		}


		if (uri.LocalPath.StartsWith(url))
			return "selected";

		return "";
	}

}