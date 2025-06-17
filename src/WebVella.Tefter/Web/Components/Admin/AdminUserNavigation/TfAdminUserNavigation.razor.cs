namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.AdminUserNavigation.TfAdminUserNavigation", "WebVella.Tefter")]
public partial class TfAdminUserNavigation : TfBaseComponent, IAsyncDisposable
{
	[Inject] protected IStateSelection<TfUserState, bool> SidebarExpanded { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }

	private string search = null;
	private int _stringLimit = 30;
	protected override void OnInitialized()
	{
		base.OnInitialized();
		SidebarExpanded.Select(x => x.SidebarExpanded);
	}

	private List<TucUser> _getUsers()
	{
		search = search?.Trim().ToLowerInvariant();
		var menuItems = new List<TucUser>();
		foreach (var user in TfAppState.Value.AdminUsers)
		{
			if (!String.IsNullOrWhiteSpace(search) &&
				!(
					user.FirstName.ToLowerInvariant().Contains(search)
					|| user.LastName.ToLowerInvariant().Contains(search)
					|| user.Email.ToLowerInvariant().Contains(search)
				)
				)
				continue;

			menuItems.Add(user);
		}

		return menuItems;
	}



	private void onSearch(string search)
	{
		this.search = search;
	}

}