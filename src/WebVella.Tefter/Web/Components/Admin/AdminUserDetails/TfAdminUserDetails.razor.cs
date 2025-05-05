namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.AdminUserDetails.TfAdminUserDetails", "WebVella.Tefter")]
public partial class TfAdminUserDetails : TfBaseComponent
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] private AppStateUseCase UC { get; set; }
	private List<TucRole> _roleOptions
	{
		get
		{
			if (TfAppState.Value.AdminManagedUser is null
			|| TfAppState.Value.AdminManagedUser.Roles is null
			|| TfAppState.Value.AdminManagedUser.Roles.Count == 0) return TfAppState.Value.UserRoles;
			return TfAppState.Value.UserRoles.Where(x => !TfAppState.Value.AdminManagedUser.Roles.Any(u => x.Id == u.Id)).ToList();
		}
	}
	private TucRole _selectedRole = null;
	public bool _submitting = false;
	public Guid? _removingRoleId = null;

	private async Task _addRole()
	{
		if (_submitting) return;

		if (_selectedRole is null) return;
		try
		{
			_submitting = true;
			var result = await UC.AddRoleToUserAsync(_selectedRole.Id, TfAppState.Value.AdminManagedUser.Id);
			_updateUserInState(result);
			ToastService.ShowSuccess(LOC("User role added"));
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			_submitting = false;
			_selectedRole = null;
			await InvokeAsync(StateHasChanged);
		}
	}
	private async Task _removeRole(TucRole role)
	{
		if (_removingRoleId is not null) return;
		if (role is null) return;
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this role unassigned?")))
			return;
		try
		{
			_removingRoleId = role.Id;
			var result = await UC.RemoveRoleFromUserAsync(role.Id, TfAppState.Value.AdminManagedUser.Id);
			_updateUserInState(result);
			ToastService.ShowSuccess(LOC("User role removed"));
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			_removingRoleId = null;
			await InvokeAsync(StateHasChanged);
		}
	}
	private void _updateUserInState(TucUser user)
	{
		var state = TfAppState.Value with { AdminManagedUser = user };
		var recIndex = TfAppState.Value.AdminUsers.FindIndex(x => x.Id == user.Id);
		if (recIndex > 0)
		{
			var users = state.AdminUsers.ToList();
			users[recIndex] = user;
			state = state with { AdminUsers = users };
		}
		Dispatcher.Dispatch(new SetAppStateAction(component: this, state: state));
	}
}