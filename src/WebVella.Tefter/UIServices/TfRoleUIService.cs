
namespace WebVella.Tefter.UIServices;

public partial interface ITfRoleUIService
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
public partial class TfRoleUIService : ITfRoleUIService
{
	#region << Ctor >>
	private static readonly AsyncLock _asyncLock = new AsyncLock();
	private readonly IJSRuntime _jsRuntime;
	private readonly AuthenticationStateProvider _authStateProvider;
	private readonly ITfService _tfService;
	private readonly NavigationManager _navigationManager;
	private TfUser? _currentUser = null;

	public TfRoleUIService(IJSRuntime jsRuntime,
		ITfService tfService,
		NavigationManager navigationManager,
		AuthenticationStateProvider authStateProvider)
	{
		_jsRuntime = jsRuntime;
		_tfService = tfService;
		_navigationManager = navigationManager;
		_authStateProvider = authStateProvider;
	}
	#endregion

	#region << Events >>
	public event EventHandler<TfRole> RoleUpdated = default!;
	public event EventHandler<TfRole> RoleCreated = default!;
	public event EventHandler<TfRole> RoleDeleted = default!;
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
