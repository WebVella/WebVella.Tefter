namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.AdminNavigation.TfAdminNavigation", "WebVella.Tefter")]
public partial class TfAdminNavigation : TfBaseComponent
{
	[Inject] protected IState<TfUserState> TfUserState { get; set; }
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

		menuItems.Add(new TucMenuItem()
		{
			Id = "tf-users-link",
			Icon = TfConstants.AdminUsersIcon,
			IconColor = TfConstants.AdminThemeColor,
			Match = NavLinkMatch.Prefix,
			Url = "/admin/users",
			Title = LOC(TfConstants.AdminUsersMenuTitle)
		});

		menuItems.Add(new TucMenuItem()
		{
			Id = "tf-data-providers-link",
			Icon = TfConstants.AdminDataProvidersIcon,
			IconColor = TfConstants.AdminThemeColor,
			Match = NavLinkMatch.Prefix,
			Url = "/admin/data-providers",
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