namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.AdminUsersActions.TfAdminUsersActions", "WebVella.Tefter")]
public partial class TfAdminUsersActions : TfBaseComponent
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] private AppStateUseCase UC { get; set; }

	//private bool _settingsMenuVisible = false;
	private async Task addUser()
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
			var users = TfAppState.Value.AdminUsers.ToList();
			users.Add(user);
			var state = TfAppState.Value with { AdminManagedUser = user, AdminUsers = users };
			Dispatcher.Dispatch(new SetAppStateAction(component: this, state: state));
			await Task.Delay(1);
			Navigator.NavigateTo(string.Format(TfConstants.AdminUserDetailsPageUrl, user.Id));
		}

	}
	private void onUsersListClick()
	{
		if (TfAppState.Value.Route.HasNode(RouteDataNode.Users,1)) return;
		Navigator.NavigateTo(string.Format(TfConstants.AdminUsersPageUrl));
	}

	private void onRolesListClick()
	{
		if (TfAppState.Value.Route.HasNode(RouteDataNode.Roles,1)) return;
		Navigator.NavigateTo(string.Format(TfConstants.AdminRolesPageUrl));
	}

	private async Task addRoleClick()
	{
		var dialog = await DialogService.ShowDialogAsync<TfRoleManageDialog>(
		new TucRole(),
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
			var role = (TucRole)result.Data;
			ToastService.ShowSuccess(LOC("Role successfully updated!"));
			var roles = TfAppState.Value.UserRoles.ToList();
			roles.Add(role);
			var state = TfAppState.Value with { AdminManagedRole = role, UserRoles = roles };
			Dispatcher.Dispatch(new SetAppStateAction(component: this, state: state));
			await Task.Delay(1);
			Navigator.NavigateTo(string.Format(TfConstants.AdminRoleDetailsPageUrl, role.Id));
		}
	}


}