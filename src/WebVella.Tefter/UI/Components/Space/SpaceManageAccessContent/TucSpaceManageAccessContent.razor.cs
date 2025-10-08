namespace WebVella.Tefter.UI.Components;

public partial class TucSpaceManageAccessContent : TfBaseComponent, IDisposable
{
	private TfSpace _space = null!;
	private TfNavigationState _navState = null!;
	public bool _submitting = false;
	public TfRole _adminRole = null!;
	public List<TfRole> _roleOptions = null!;
	private TfRole? _selectedRole = null;
	public Guid? _removingRoleId = null;

	public void Dispose()
	{
		TfEventProvider.SpaceUpdatedEvent -= On_SpaceUpdated;
		TfState.NavigationStateChangedEvent -= On_NavigationStateChanged;
	}

	protected override async Task OnInitializedAsync()
	{
		await _init(TfState.NavigationState);
		TfEventProvider.SpaceUpdatedEvent += On_SpaceUpdated;
		TfState.NavigationStateChangedEvent += On_NavigationStateChanged;
	}


	private async Task On_NavigationStateChanged(TfNavigationState args)
	{
		await InvokeAsync(async () =>
		{
			if (UriInitialized != args.Uri)
				await _init(navState: args);
		});
	}

	private async Task On_SpaceUpdated(TfSpaceUpdatedEvent args)
	{
		await InvokeAsync(async () =>
		{
			await _init(navState: TfState.NavigationState, space: args.Payload);
		});
	}

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

			if (_space is null) return;
			var allRoles = TfService.GetRoles();
			_roleOptions = allRoles
				.Where(x => x.Id != TfConstants.ADMIN_ROLE_ID && !_space.Roles.Any(u => x.Id == u.Id)).ToList();
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
			TfService.AddSpacesRole(new List<TfSpace> { _space }, _selectedRole);
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
		if (role is null) return;
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this role unassigned?")))
			return;
		try
		{
			_removingRoleId = role.Id;
			TfService.RemoveSpacesRole(new List<TfSpace> {_space }, role);
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
			var result = TfService.SetSpacePrivacy(_space.Id, newValue);
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