namespace WebVella.Tefter.UI.Components;

public partial class TucAdminRoleDetailsContent : TfBaseComponent, IAsyncDisposable
{
	private TfRole? _role = null;
	private List<TfUser> _userOptions = new();
	private TfUser? _selectedUser = null;
	private List<TfUser> _roleUsers = new();

	// ReSharper disable once InconsistentNaming
	public bool _submitting = false;
	private Guid? _removingUserId = null;
	private IAsyncDisposable? _roleEventSubscriber = null;

	public async ValueTask DisposeAsync()
	{
		Navigator.LocationChanged -= On_NavigationStateChanged;
		if (_roleEventSubscriber is not null)
			await _roleEventSubscriber.DisposeAsync();
	}

	protected override async Task OnInitializedAsync()
	{
		await _init(TfAuthLayout.GetState().NavigationState);
		Navigator.LocationChanged += On_NavigationStateChanged;
		_roleEventSubscriber = await TfEventBus.SubscribeAsync<TfRoleEventPayload>(
			handler: On_RoleEventAsync,
			matchKey: (_) => true);
	}

	private void On_NavigationStateChanged(object? caller, LocationChangedEventArgs args)
	{
		InvokeAsync(async () =>
		{
			if (UriInitialized != args.Location)
				await _init(navState: TfAuthLayout.GetState().NavigationState);
		});
	}

	private async Task On_RoleEventAsync(string? key, TfRoleEventPayload? payload)
	{
		if (payload is null) return;
		if (payload.Role.Id != _role?.Id) return;
		if (key == TfAuthLayout.GetSessionId().ToString())
			await _init(navState: TfAuthLayout.GetState().NavigationState, role: payload.Role);
		else
			await TfEventBus.PublishAsync(key: key, new TfPageOutdatedAlertEventPayload());
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
					_role = await TfService.GetRoleAsync(routeData.RoleId.Value);
			}

			if (_role is null) return;
			_roleUsers = TfService.GetUsersForRole(_role.Id).ToList();
			_userOptions = (await TfService.GetUsersAsync()).Where(x => _roleUsers.All(u => x.Id != u.Id)).ToList();
		}
		finally
		{
			UriInitialized = navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _editRole()
	{
		if (_role is null) return;
		var dialog = await DialogService.ShowDialogAsync<TucRoleManageDialog>(
			_role,
			new()
			{
				PreventDismissOnOverlayClick = true,
				PreventScroll = true,
				Width = TfConstants.DialogWidthLarge,
				TrapFocus = false
			});
		var result = await dialog.Result;
		if (result is { Cancelled: false, Data: not null }) { }
	}

	private async Task _deleteRole()
	{
		if (_role is null) return;
		if (_roleUsers.Count > 0)
		{
			ToastService.ShowWarning(LOC("Please unassign all users first"));
			return;
		}

		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this role deleted?")))
			return;
		try
		{
			await TfService.DeleteRoleAsync(_role);
			var allRoles = await TfService.GetRolesAsync();
			ToastService.ShowSuccess(LOC("User removed from role"));
			await TfEventBus.PublishAsync(key: TfAuthLayout.GetSessionId(),
				payload: new TfRoleDeletedEventPayload(_role));
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
		if (_role is null) return;
		if (_submitting) return;

		if (_selectedUser is null) return;
		try
		{
			_submitting = true;
			_ = await TfService.AddUserToRoleAsync(userId: _selectedUser.Id, roleId: _role.Id);
			_roleUsers = TfService.GetUsersForRole(_role.Id).ToList();
			_userOptions = (await TfService.GetUsersAsync()).Where(x => _roleUsers.All(u => x.Id != u.Id)).ToList();
			ToastService.ShowSuccess(LOC("User added to role"));
			await TfEventBus.PublishAsync(key: TfAuthLayout.GetSessionId(),
				payload: new TfRoleUpdatedEventPayload(_role));
			await TfEventBus.PublishAsync(key: TfAuthLayout.GetSessionId(),
				payload: new TfUserUpdatedEventPayload(_selectedUser));
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
		if (_role is null) return;
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this user unassigned?")))
			return;
		try
		{
			_removingUserId = user.Id;
			_ = await TfService.RemoveUserFromRoleAsync(userId: user.Id, roleId: _role.Id);
			_roleUsers = TfService.GetUsersForRole(_role.Id).ToList();
			_userOptions = (await TfService.GetUsersAsync()).Where(x => _roleUsers.All(u => x.Id != u.Id)).ToList();
			ToastService.ShowSuccess(LOC("User removed from role"));
			await TfEventBus.PublishAsync(key: TfAuthLayout.GetSessionId(),
				payload: new TfRoleUpdatedEventPayload(_role));
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