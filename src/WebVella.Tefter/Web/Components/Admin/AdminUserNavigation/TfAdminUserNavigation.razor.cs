namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.AdminUserNavigation.TfAdminUserNavigation", "WebVella.Tefter")]
public partial class TfAdminUserNavigation : TfBaseComponent, IAsyncDisposable
{
	[Inject] protected IStateSelection<TfUserState,bool> SidebarExpanded { get; set; }
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

	private async Task onAddClick()
	{
		var dialog = await DialogService.ShowDialogAsync<TfUserManageDialog>(
		new TucUser(),
		new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge,
			TrapFocus = false
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			var user = (TucUser)result.Data;
			ToastService.ShowSuccess(LOC("User successfully created!"));
			Navigator.NavigateTo(string.Format(TfConstants.AdminUserDetailsPageUrl, user.Id));
		}
	}

	private void onSearch(string search)
	{
		this.search = search;
	}

}