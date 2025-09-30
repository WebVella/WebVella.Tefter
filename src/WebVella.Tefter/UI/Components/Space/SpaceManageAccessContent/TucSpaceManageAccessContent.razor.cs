namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceManageAccessContent : TfBaseComponent, IDisposable
{
	private TfSpace _space = default!;
	private TfNavigationState _navState = default!;
	public bool _submitting = false;
	public TfRole _adminRole = default!;
	public List<TfRole> _roleOptions = default!;
	private TfRole? _selectedRole = null;
	public Guid? _removingRoleId = null;
	public void Dispose()
	{
		TfUIService.SpaceUpdated -= On_SpaceUpdated;
		TfUIService.NavigationStateChanged -= On_NavigationStateChanged;
	}

	protected override async Task OnInitializedAsync()
	{
		await _init();
		TfUIService.SpaceUpdated += On_SpaceUpdated;
		TfUIService.NavigationStateChanged += On_NavigationStateChanged;
	}


	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		if (UriInitialized != args.Uri)
			await _init(navState: args);
	}

	private async void On_SpaceUpdated(object? caller, TfSpace args)
	{
		await _init(space: args);
	}

	private async Task _init(TfNavigationState? navState = null, TfSpace? space = null)
	{
		if (navState == null)
			_navState = await TfUIService.GetNavigationStateAsync(Navigator);
		else
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
				_space = TfUIService.GetSpace(_navState.SpaceId.Value);
			}
			if (_space is null) return;
			var allRoles = TfUIService.GetRoles();
			_roleOptions =allRoles.Where(x => x.Id != TfConstants.ADMIN_ROLE_ID &&  !_space.Roles.Any(u => x.Id == u.Id)).ToList();
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
			var result = TfUIService.AddSpacesRole(_space,_selectedRole);
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
			var result = TfUIService.RemoveSpacesRole(_space,role);
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
			var result = TfUIService.SetSpacePrivacy(_space.Id, newValue);
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