namespace WebVella.Tefter.Identity;

public class RoleBuilder
{
	private readonly IIdentityManager _identityManager;
	private Guid _id;
	private string _name;
	private bool _isSystem;

	internal RoleBuilder(IIdentityManager identityManager, Role role = null)
	{
		_identityManager = identityManager;
		if (role == null)
		{
			_id = Guid.Empty;
		}
		else
		{
			_id = role.Id;
			_name = role.Name;
			_isSystem = role.IsSystem;
		}
	}

	internal RoleBuilder(IIdentityManager identityManager, Guid roleId )
	{
		_identityManager = identityManager;
		_id = roleId;
	}

	public RoleBuilder WithName(string name)
	{
		_name = name;
		return this;
	}

	internal RoleBuilder IsSystem(bool isSystem)
	{
		_isSystem = isSystem;
		return this;
	}

	public Role Build()
	{
		return new Role
		{
			Id = _id,
			Name = _name,
			IsSystem = _isSystem
		};
	}

}
