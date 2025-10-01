
namespace WebVella.Tefter.UIServices;

public partial interface ITfUIService
{
	//Events
	event EventHandler<TfRole> RoleUpdated;
	event EventHandler<TfRole> RoleCreated;
	event EventHandler<TfRole> RoleDeleted;

	//Roles
	ReadOnlyCollection<TfRole> GetRoles(string? search = null);
	TfRole GetRole(Guid roleId);
	TfRole CreateRole(TfRole role);
	TfRole UpdateRole(TfRole role);
	void DeleteRole(TfRole role);
}
public partial class TfUIService : ITfUIService
{
	#region << Events >>
	public event EventHandler<TfRole> RoleUpdated = null!;
	public event EventHandler<TfRole> RoleCreated = null!;
	public event EventHandler<TfRole> RoleDeleted = null!;
	#endregion

	#region << Roles >>

	public ReadOnlyCollection<TfRole> GetRoles(string? search = null) => _tfService.GetRoles(search);
	public TfRole GetRole(Guid roleId) => _tfService.GetRole(roleId);
	public TfRole CreateRole(TfRole role)
	{
		var roleSM = _tfService.CreateRole(role);
		RoleCreated?.Invoke(this, roleSM);
		return roleSM;
	}
	public TfRole UpdateRole(TfRole role)
	{
		var roleSM = _tfService.UpdateRole(role);
		RoleUpdated?.Invoke(this, roleSM);
		return roleSM;
	}
	public void DeleteRole(TfRole role)
	{
		_tfService.DeleteRole(role);
		RoleDeleted?.Invoke(this, role);
	}

	#endregion
}
