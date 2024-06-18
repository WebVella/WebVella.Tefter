namespace WebVella.Tefter.Identity;

public partial interface IIdentityManager
{
	RoleBuilder CreateRoleBuilder(Role role = null);
	Task<Role> GetRoleAsync(Guid id);
	Task<Role> GetRoleAsync(string name);
	Task<ReadOnlyCollection<Role>> GetRolesAsync();
	Task<Role> SaveRoleAsync(Role role);
}

public partial class IdentityManager : IIdentityManager
{
	public RoleBuilder CreateRoleBuilder(Role role = null)
	{
		return new RoleBuilder(this, role);
	}

	public async Task<Role> GetRoleAsync(Guid id)
	{
		var roleDbo = await _dboManager.GetAsync<RoleDbo>(id, nameof(Role.Id));
		if (roleDbo == null)
			return null;

		return new RoleBuilder(this, roleDbo.Id)
					.WithName(roleDbo.Name)
					.IsSystem(roleDbo.IsSystem)
					.Build();
	}

	public async Task<Role> GetRoleAsync(string name)
	{
		var roleDbo = await _dboManager.GetAsync<RoleDbo>(name, nameof(Role.Name));
		if (roleDbo == null)
			return null;

		return new RoleBuilder(this, roleDbo.Id)
					.WithName(roleDbo.Name)
					.IsSystem(roleDbo.IsSystem)
					.Build();
	}


	public async Task<ReadOnlyCollection<Role>> GetRolesAsync()
	{
		var orderSettings = new OrderSettings(nameof(RoleDbo.Name), OrderDirection.ASC);
		return (await _dboManager.GetListAsync<RoleDbo>(null, null, orderSettings))
			.Select(x =>
				new RoleBuilder(this, x.Id)
					.WithName(x.Name)
					.IsSystem(x.IsSystem)
					.Build()
			)
			.ToList()
			.AsReadOnly();

	}

	public async Task<Role> SaveRoleAsync(Role role)
	{
		if (role == null)
			throw new ArgumentNullException(nameof(role));

		if (role.Id == Guid.Empty)
			return await CreateRole(role);
		else
			return await UpdateRole(role);
	}

	private async Task<Role> CreateRole(Role role)
	{
		//ValidateRoleOnCreate(role);

		RoleDbo roleDbo = new RoleDbo
		{
			Id = Guid.NewGuid(),
			Name = role.Name,
			IsSystem = role.IsSystem
		};

		bool success = await _dboManager.InsertAsync<RoleDbo>(roleDbo);

		if (!success)
			throw new DatabaseException("InsertAsync<RoleDbo> operation failed.");

		return await GetRoleAsync(roleDbo.Id);
	}

	private async Task<Role> UpdateRole(Role role)
	{
		//ValidateRoleOnUpdate(role);

		RoleDbo roleDbo = new RoleDbo
		{
			Id = role.Id,
			Name = role.Name,
			IsSystem = role.IsSystem
		};

		Dictionary<string, Guid> recordKey = new Dictionary<string, Guid> { { "id", role.Id } };
		bool success = await _dboManager.UpdateAsync<RoleDbo>(roleDbo, recordKey);

		if (!success)
			throw new DatabaseException("UpdateAsync<RoleDbo> operation failed.");

		return await GetRoleAsync(roleDbo.Id);
	}
}
