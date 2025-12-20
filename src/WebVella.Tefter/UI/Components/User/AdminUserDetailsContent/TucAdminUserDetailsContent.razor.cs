namespace WebVella.Tefter.UI.Components;

public partial class TucAdminUserDetailsContent : TfBaseComponent, IAsyncDisposable
{
	[Inject] protected ITfEventBusEx TfEventBus { get; set; } = null!;
	private TfUser? _user = null;
	private List<TfRole> _roleOptions = new();
	private TfRole? _selectedRole = null;
	private bool _submitting = false;
	private Guid? _removingRoleId = null;
	private IAsyncDisposable _userUpdatedEventSubscriber = null!;

	public async ValueTask DisposeAsync()
	{
		Navigator.LocationChanged -= On_NavigationStateChanged;
		await _userUpdatedEventSubscriber.DisposeAsync();
	}

	protected override async Task OnInitializedAsync()
	{
		await _init(TfAuthLayout.GetState().NavigationState);
		Navigator.LocationChanged += On_NavigationStateChanged;
		_userUpdatedEventSubscriber = await TfEventBus.SubscribeAsync<TfUserUpdatedEventPayload>(
			handler: On_UserUpdatedEventAsync);
	}

	private async Task On_UserUpdatedEventAsync(string? key, TfUserUpdatedEventPayload? payload)
	{
		if(payload is null) return;
		await _init(navState: TfAuthLayout.GetState().NavigationState, user: payload.User);
	}

	private void On_NavigationStateChanged(object? caller, LocationChangedEventArgs args)
	{
		InvokeAsync(async () =>
		{
			if (UriInitialized != args.Location)
				await _init(navState: TfAuthLayout.GetState().NavigationState);
		});
	}

	private async Task _init(TfNavigationState navState, TfUser? user = null)
	{
		try
		{
			if (user is not null && _user is not null && user.Id == _user.Id)
			{
				_user = user;
			}
			else
			{
				if (navState.UserId is not null)
					_user = await TfService.GetUserAsync(navState.UserId.Value);
			}

			if (_user is null) return;
			_roleOptions = (await TfService.GetRolesAsync()).Where(x => _user.Roles.All(u => x.Id != u.Id)).ToList();
		}
		finally
		{
			UriInitialized = navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _editUser()
	{
		if(_user is null) return;
		var dialog = await DialogService.ShowDialogAsync<TucUserManageDialog>(
			_user,
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

	private async Task _addRole()
	{
		if(_user is null) return;
		if (_submitting) return;

		if (_selectedRole is null) return;
		try
		{
			_submitting = true;
			_user = await TfService.AddUserToRoleAsync(userId: _user.Id, roleId: _selectedRole.Id);
			_roleOptions = (await TfService.GetRolesAsync()).Where(x => _user.Roles.All(u => x.Id != u.Id)).ToList();
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
		if(_user is null) return;
		if (_removingRoleId is not null) return;
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this role unassigned?")))
			return;
		try
		{
			_removingRoleId = role.Id;
			_user = await TfService.RemoveUserFromRoleAsync(userId: _user.Id, roleId: role.Id);
			_roleOptions = (await TfService.GetRolesAsync()).Where(x => _user.Roles.All(u => x.Id != u.Id)).ToList();
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