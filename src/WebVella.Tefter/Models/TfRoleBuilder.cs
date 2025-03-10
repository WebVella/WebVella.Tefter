namespace WebVella.Tefter.Models;
public class TfRoleBuilder
{
	private readonly ITfService _tfService;
	private Guid _id;
	private string _name;
	private bool _isSystem;

	internal TfRoleBuilder(ITfService tfService, TfRole role = null)
	{
		_tfService = tfService;
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

	internal TfRoleBuilder(ITfService tfService, Guid roleId)
	{
		_tfService = tfService;
		_id = roleId;
	}

	public TfRoleBuilder WithName(string name)
	{
		_name = name;
		return this;
	}

	internal TfRoleBuilder IsSystem(bool isSystem)
	{
		_isSystem = isSystem;
		return this;
	}

	public TfRole Build()
	{
		return new TfRole
		{
			Id = _id,
			Name = _name,
			IsSystem = _isSystem
		};
	}

}
