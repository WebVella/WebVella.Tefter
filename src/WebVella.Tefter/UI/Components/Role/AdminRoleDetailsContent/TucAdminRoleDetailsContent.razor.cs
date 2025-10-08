namespace WebVella.Tefter.UI.Components;
public partial class TucAdminRoleDetailsContent : TfBaseComponent, IDisposable
{
	private TfRole? _role = null;
	private List<TfUser> _userOptions = new();
	private TfUser? _selectedUser = null;
	internal List<TfUser> _roleUsers = new();
	public bool _submitting = false;
	public Guid? _removingUserId = null;
	public void Dispose()
	{
		TfEventProvider.RoleUpdatedEvent -= On_RoleUpdated;
		TfState.NavigationStateChangedEvent -= On_NavigationStateChanged;
	}

	protected override async Task OnInitializedAsync()
	{
		await _init(TfState.NavigationState);
		TfEventProvider.RoleUpdatedEvent += On_RoleUpdated;
		TfState.NavigationStateChangedEvent += On_NavigationStateChanged;
	}

	private async Task On_RoleUpdated(TfRoleUpdatedEvent args)
	{
		await InvokeAsync(async () =>
		{
			await _init(navState: TfState.NavigationState, role: args.Payload);
		});
	}

	private async Task On_NavigationStateChanged(TfNavigationState args)
	{
		await InvokeAsync(async () =>
		{
			if (UriInitialized != args.Uri)
				await _init(navState: args);
		});
	}

	private async Task _init(TfNavigationState navState, TfRole? role = null)
	{
		try
		{
			if (role is not null && role.Id == _role?.Id)
			{
				_role = role;
			}
			else
			{
				var routeData = TfState.NavigationState;
				if (routeData.RoleId is not null)
					_role = TfService.GetRole(routeData.RoleId.Value);

			}
			if(_role is null) return;
			_roleUsers = TfService.GetUsersForRole(_role.Id).ToList();
			_userOptions = TfService.GetUsers().Where(x => !_roleUsers.Any(u => x.Id == u.Id)).ToList();
		}
		finally
		{
			UriInitialized = navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _editRole()
	{
		var dialog = await DialogService.ShowDialogAsync<TucRoleManageDialog>(
		_role,
		new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge,
			TrapFocus = false
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null) { }
	}

	private async Task _deleteRole()
	{
		if (_roleUsers.Count > 0)
		{
			ToastService.ShowWarning(LOC("Please unassign all users first"));
			return;
		}

		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this role deleted?")))
			return;
		try
		{
			TfService.DeleteRole(_role);
			var allRoles = TfService.GetRoles();
			ToastService.ShowSuccess(LOC("User removed from role"));
			if (allRoles.Count > 0)
			{
				Navigator.NavigateTo(string.Format(TfConstants.AdminRoleDetailsPageUrl, allRoles[0].Id));
			}
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
	}

	private async Task _addUser()
	{
		if (_submitting) return;

		if (_selectedUser is null) return;
		try
		{
			_submitting = true;
			_ = await TfService.AddUserToRoleAsync(userId: _selectedUser.Id, roleId: _role.Id);
			_roleUsers = TfService.GetUsersForRole(_role.Id).ToList();
			_userOptions = TfService.GetUsers().Where(x => !_roleUsers.Any(u => x.Id == u.Id)).ToList();
			ToastService.ShowSuccess(LOC("User added to role"));
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			_submitting = false;
			_selectedUser = null;
			await InvokeAsync(StateHasChanged);
		}
	}
	private async Task _removeUser(TfUser user)
	{
		if (_removingUserId is not null) return;
		if (user is null) return;
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this user unassigned?")))
			return;
		try
		{
			_removingUserId = user.Id;
			_ = await TfService.RemoveUserFromRoleAsync(userId: user.Id, roleId: _role.Id);
			_roleUsers = TfService.GetUsersForRole(_role.Id).ToList();
			_userOptions = TfService.GetUsers().Where(x => !_roleUsers.Any(u => x.Id == u.Id)).ToList();
			ToastService.ShowSuccess(LOC("User removed from role"));
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			_removingUserId = null;
			await InvokeAsync(StateHasChanged);
		}
	}

	private void _onSelectedUserChange(TfUser? user)
	{
		_selectedUser = user;
		StateHasChanged();
	}
}