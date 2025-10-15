namespace WebVella.Tefter.UI.Components;
public partial class TucAdminRoleDetailsContent : TfBaseComponent, IDisposable
{
	[Inject] protected TfGlobalEventProvider TfEventProvider { get; set; } = null!;
	private TfRole? _role = null;
	private List<TfUser> _userOptions = new();
	private TfUser? _selectedUser = null;
	internal List<TfUser> _roleUsers = new();
	public bool _submitting = false;
	public Guid? _removingUserId = null;
	public void Dispose()
	{
		TfEventProvider.Dispose();
		Navigator.LocationChanged -= On_NavigationStateChanged;
	}

	protected override async Task OnInitializedAsync()
	{
		await _init(TfAuthLayout.GetState().NavigationState);
		TfEventProvider.RoleUpdatedEvent += On_RoleUpdated;
		Navigator.LocationChanged += On_NavigationStateChanged;
	}

	private async Task On_RoleUpdated(TfRoleUpdatedEvent args)
	{
		await InvokeAsync(async () =>
		{
			await _init(navState: TfAuthLayout.GetState().NavigationState, role: args.Payload);
		});
	}

	private void On_NavigationStateChanged(object? caller, LocationChangedEventArgs args)
	{
		InvokeAsync(async () =>
		{
			if (UriInitialized != args.Location)
				await _init(navState: TfAuthLayout.GetState().NavigationState);
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
				var routeData = TfAuthLayout.GetState().NavigationState;
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
		new ()
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

}