namespace WebVella.Tefter.Web.Components.AdminNavigation;
public partial class TfAdminNavigation: TfBaseComponent
{
	[Inject] protected IState<SessionState> SessionState { get; set; }

	private List<MenuItem> menuItems = new List<MenuItem>();

	protected override void OnAfterRender(bool firstRender)
	{
		if(firstRender){
			generateMenu();
			StateHasChanged();
		}
	}

	private void generateMenu(){ 
		menuItems.Clear();
		menuItems.Add(new MenuItem(){ 
			Id = "tf-dashboard-link",
			Icon = TfConstants.AdminDashboardIcon,
			IconColor = TfConstants.DefaultThemeColor,
			Match = NavLinkMatch.All,
			Url = "/admin",
			Title = TfConstants.AdminDashboardMenuTitle
		});

		menuItems.Add(new MenuItem()
		{
			Id = "tf-users-link",
			Icon = TfConstants.AdminUsersIcon,
			IconColor = TfConstants.DefaultThemeColor,
			Match = NavLinkMatch.Prefix,
			Url = "/admin/users",
			Title = TfConstants.AdminUsersMenuTitle
		});

		menuItems.Add(new MenuItem()
		{
			Id = "tf-data-providers-link",
			Icon = TfConstants.AdminDataProvidersIcon,
			IconColor = TfConstants.DefaultThemeColor,
			Match = NavLinkMatch.Prefix,
			Url = "/admin/data-providers",
			Title = TfConstants.AdminDataProvidersMenuTitle
		});
	}

	private void _addSpaceHandler(){
		ToastService.ShowToast(ToastIntent.Warning, "Will open add new space modal");
	}

}