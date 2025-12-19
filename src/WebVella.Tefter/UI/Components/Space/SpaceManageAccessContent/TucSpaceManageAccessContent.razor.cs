namespace WebVella.Tefter.UI.Components;

public partial class TucSpaceManageAccessContent : TfBaseComponent, IAsyncDisposable
{
	[Inject] protected ITfEventBusEx TfEventBus { get; set; } = null!;
	private TfSpace _space = null!;
	private TfNavigationState _navState = null!;
	private bool _submitting = false;
	private TfRole _adminRole = null!;
	private List<TfRole> _roleOptions = null!;
	private TfRole? _selectedRole = null;
	private Guid? _removingRoleId = null;
	private IAsyncDisposable _spaceUpdatedEventSubscriber = null!;

	public async ValueTask DisposeAsync()
	{
		Navigator.LocationChanged -= On_NavigationStateChanged;
		await _spaceUpdatedEventSubscriber.DisposeAsync();
	}

	protected override async Task OnInitializedAsync()
	{
		await _init(TfAuthLayout.GetState().NavigationState);
		Navigator.LocationChanged += On_NavigationStateChanged;
		_spaceUpdatedEventSubscriber = await TfEventBus.SubscribeAsync<TfSpaceUpdatedEventPayload>(
			handler: On_SpaceUpdatedEventAsync);
	}


	private void On_NavigationStateChanged(object? caller, LocationChangedEventArgs args)
	{
		InvokeAsync(async () =>
		{
			if (UriInitialized != args.Location)
				await _init(navState: TfAuthLayout.GetState().NavigationState);
		});
	}

	private async Task On_SpaceUpdatedEventAsync(string? key, TfSpaceUpdatedEventPayload? payload)
		=> await _init(navState: TfAuthLayout.GetState().NavigationState, space: payload?.Space);


	private async Task _init(TfNavigationState navState, TfSpace? space = null)
	{
		_navState = navState;

		try
		{
			if (space is not null)
			{
				_space = space;
			}
			else
			{
				if (_navState.SpaceId is null) return;
				_space = TfService.GetSpace(_navState.SpaceId.Value);
			}

			var allRoles = await TfService.GetRolesAsync();
			_roleOptions = allRoles
				.Where(x => x.Id != TfConstants.ADMIN_ROLE_ID && _space.Roles.All(u => x.Id != u.Id)).ToList();
			_adminRole = allRoles.Single(x => x.Id == TfConstants.ADMIN_ROLE_ID);
		}
		finally
		{
			UriInitialized = _navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _addRole()
	{
		if (_submitting) return;

		if (_selectedRole is null) return;
		try
		{
			_submitting = true;
			TfService.AddSpacesRole([_space], _selectedRole);
			ToastService.ShowSuccess(LOC("Space role added"));
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
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this role unassigned?")))
			return;
		try
		{
			_removingRoleId = role.Id;
			TfService.RemoveSpacesRole([_space], role);
			ToastService.ShowSuccess(LOC("Space role removed"));
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

	private async Task _setPrivacy(bool newValue)
	{
		try
		{
			TfService.SetSpacePrivacy(_space.Id, newValue);
			ToastService.ShowSuccess(LOC("Space access changed"));
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