namespace WebVella.Tefter.UI.Components;
public partial class TucAdminUserDetailsContent : TfBaseComponent, IDisposable
{
	private TfUser? _user = null;
	private List<TfRole> _roleOptions = new();
	private TfRole? _selectedRole = null;
	public bool _submitting = false;
	public Guid? _removingRoleId = null;
	public void Dispose()
	{
		TfEventProvider.UserUpdatedGlobalEvent -= On_UserUpdated;
		TfUIService.NavigationStateChanged -= On_NavigationStateChanged;
	}

	protected override async Task OnInitializedAsync()
	{
		await _init(TfAuthLayout.NavigationState);
		TfEventProvider.UserUpdatedGlobalEvent += On_UserUpdated;
		TfUIService.NavigationStateChanged += On_NavigationStateChanged;
	}

	private async void On_UserUpdated(TfUserUpdatedEvent args)
	{
		await _init(navState:TfAuthLayout.NavigationState, user: args.Payload);
	}

	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		if (UriInitialized != args.Uri)
			await _init(navState: args);
	}

	private async Task _init(TfNavigationState navState, TfUser? user = null)
	{
		try
		{
			if (user is not null && user.Id == _user.Id)
			{
				_user = user;
			}
			else
			{
				if (navState.UserId is not null)
					_user = TfService.GetUser(navState.UserId.Value);
			}
			if(_user is null) return;
			_roleOptions = TfService.GetRoles().Where(x => !_user.Roles.Any(u => x.Id == u.Id)).ToList();
		}
		finally
		{
			UriInitialized = navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _editUser()
	{
		var dialog = await DialogService.ShowDialogAsync<TucUserManageDialog>(
		_user,
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
			var user = (TfUser)result.Data;
		}
	}

	private async Task _addRole()
	{
		if (_submitting) return;

		if (_selectedRole is null) return;
		try
		{
			_submitting = true;
			_user = await TfService.AddUserToRoleAsync(userId: _user.Id, roleId: _selectedRole.Id);
			_roleOptions = TfService.GetRoles().Where(x => !_user.Roles.Any(u => x.Id == u.Id)).ToList();
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
	private async Task _removeRole(TfRole role)
	{
		if (_removingRoleId is not null) return;
		if (role is null) return;
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this role unassigned?")))
			return;
		try
		{
			_removingRoleId = role.Id;
			_user = await TfService.RemoveUserFromRoleAsync( userId: _user.Id, roleId: role.Id);
			_roleOptions = TfService.GetRoles().Where(x => !_user.Roles.Any(u => x.Id == u.Id)).ToList();
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
}