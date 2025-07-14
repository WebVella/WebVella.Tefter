namespace WebVella.Tefter.UI.Components;
public partial class TucAdminRoleDetailsContent : TfBaseComponent, IDisposable
{
	[Inject] public ITfUserUIService TfUserUIService { get; set; } = default!;
	[Inject] public ITfRoleUIService TfRoleUIService { get; set; } = default!;
	[Inject] public ITfNavigationUIService TfNavigationUIService { get; set; } = default!;

	private TfUser _currentUser = default!;
	private TfRole? _role = null;
	private List<TfUser> _userOptions = new();
	private TfUser? _selectedUser = null;
	internal List<TfUser> _roleUsers = new();
	public bool _submitting = false;
	public Guid? _removingUserId = null;
	public void Dispose()
	{
		TfRoleUIService.RoleUpdated -= On_RoleUpdated;
		TfNavigationUIService.NavigationStateChanged -= On_NavigationStateChanged;
	}

	protected override async Task OnInitializedAsync()
	{
		await _init();
		TfRoleUIService.RoleUpdated += On_RoleUpdated;
		TfNavigationUIService.NavigationStateChanged += On_NavigationStateChanged;
	}

	private async void On_RoleUpdated(object? caller, TfRole args)
	{
		await _init(role: args);
	}

	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		if (UriInitialized != args.Uri)
			await _init(navState: args);
	}

	private async Task _init(TfNavigationState? navState = null, TfRole? role = null)
	{
		if (navState == null)
			navState = await TfNavigationUIService.GetNavigationStateAsync(Navigator);
		try
		{
			_currentUser = (await TfUserUIService.GetCurrentUserAsync())!;
			if (role is not null && role.Id == _role?.Id)
			{
				_role = role;
			}
			else
			{
				var routeData = Navigator.GetRouteState();
				if (routeData.RoleId is not null)
					_role = TfRoleUIService.GetRole(routeData.RoleId.Value);

			}
			if(_role is null) return;
			_roleUsers = TfUserUIService.GetUsersForRole(_role.Id).ToList();
			_userOptions = TfUserUIService.GetUsers().Where(x => !_roleUsers.Any(u => x.Id == u.Id)).ToList();
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
			TfRoleUIService.DeleteRole(_role);
			var allRoles = TfRoleUIService.GetRoles();
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
			_ = await TfUserUIService.AddRoleToUserAsync(roleId: _role.Id, userId: _selectedUser.Id);
			_roleUsers = TfUserUIService.GetUsersForRole(_role.Id).ToList();
			_userOptions = TfUserUIService.GetUsers().Where(x => !_roleUsers.Any(u => x.Id == u.Id)).ToList();
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
			_ = await TfUserUIService.RemoveRoleFromUserAsync(roleId: _role.Id, userId: user.Id);
			_roleUsers = TfUserUIService.GetUsersForRole(_role.Id).ToList();
			_userOptions = TfUserUIService.GetUsers().Where(x => !_roleUsers.Any(u => x.Id == u.Id)).ToList();
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