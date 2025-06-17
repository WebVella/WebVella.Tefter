namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.AdminRoleNavigation.TfAdminRoleNavigation", "WebVella.Tefter")]
public partial class TfAdminRoleNavigation : TfBaseComponent, IAsyncDisposable
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

	private List<TucRole> _getRoles()
	{
		search = search?.Trim().ToLowerInvariant();
		var menuItems = new List<TucRole>();
		foreach (var role in TfAppState.Value.UserRoles)
		{
			if (!String.IsNullOrWhiteSpace(search) &&
				!(
					role.Name.ToLowerInvariant().Contains(search)
				)
				)
				continue;

			menuItems.Add(role);
		}

		return menuItems;
	}



	private void onSearch(string search)
	{
		this.search = search;
	}

}