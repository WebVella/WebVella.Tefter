using WebVella.Tefter.Models;

namespace WebVella.Tefter.UI.Components;

public partial class TucSpaceAccessDialog : TfBaseComponent, IDialogContentComponent<Guid>
{
	[Parameter] public Guid Content { get; set; } = Guid.Empty;
	[CascadingParameter] public FluentDialog Dialog { get; set; } = null!;

	private string _error = string.Empty;
	private TfNavigationState _navState = null!;
	private TfSpace _space = null!;
	public bool _submitting = false;
	public TfRole _adminRole = null!;
	public List<TfRole> _roleOptions = null!;
	private TfRole? _selectedRole = null;
	public Guid? _removingRoleId = null;

	protected override void OnInitialized()
	{
		if (Content == Guid.Empty) throw new Exception("Content is null");
		_init();
	}

	private void _init(TfSpace? space = null)
	{
		if (space is null)
			_space = TfService.GetSpace(Content);
		else
			_space = space;

		var allRoles = TfService.GetRoles();
		_roleOptions = allRoles.Where(x => x.Id != TfConstants.ADMIN_ROLE_ID && !_space.Roles.Any(u => x.Id == u.Id)).ToList();
		_adminRole = allRoles.Single(x => x.Id == TfConstants.ADMIN_ROLE_ID);
	}

	private async Task _cancel()
	{
		await Dialog.CancelAsync();
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
			_init(TfService.GetSpace(_space.Id));
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

		try
		{
			_removingRoleId = role.Id;
			TfService.RemoveSpacesRole(new List<TfSpace> {_space }, role);
			ToastService.ShowSuccess(LOC("Space role removed"));
			_init(TfService.GetSpace(_space.Id));
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
			var space = TfService.SetSpacePrivacy(_space.Id, newValue);
			ToastService.ShowSuccess(LOC("Space access changed"));
			_init(space);
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
