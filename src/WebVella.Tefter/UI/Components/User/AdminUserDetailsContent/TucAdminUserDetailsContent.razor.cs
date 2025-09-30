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
		TfUIService.UserUpdated -= On_UserUpdated;
		TfUIService.NavigationStateChanged -= On_NavigationStateChanged;
	}

	protected override async Task OnInitializedAsync()
	{
		await _init();
		TfUIService.UserUpdated += On_UserUpdated;
		TfUIService.NavigationStateChanged += On_NavigationStateChanged;
	}

	private async void On_UserUpdated(object? caller, TfUser user)
	{
		await _init(user: user);
	}

	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		if (UriInitialized != args.Uri)
			await _init(navState: args);
	}

	private async Task _init(TfNavigationState? navState = null, TfUser? user = null)
	{
		if (navState == null)
			navState = await TfUIService.GetNavigationStateAsync(Navigator);
		try
		{
			if (user is not null && user.Id == _user.Id)
			{
				_user = user;
			}
			else
			{
				if (navState.UserId is not null)
					_user = TfUIService.GetUser(navState.UserId.Value);
			}
			if(_user is null) return;
			_roleOptions = TfUIService.GetRoles().Where(x => !_user.Roles.Any(u => x.Id == u.Id)).ToList();
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
			_user = await TfUIService.AddRoleToUserAsync(roleId: _selectedRole.Id, userId: _user.Id);
			_roleOptions = TfUIService.GetRoles().Where(x => !_user.Roles.Any(u => x.Id == u.Id)).ToList();
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
			_user = await TfUIService.RemoveRoleFromUserAsync(roleId: role.Id, userId: _user.Id);
			_roleOptions = TfUIService.GetRoles().Where(x => !_user.Roles.Any(u => x.Id == u.Id)).ToList();
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