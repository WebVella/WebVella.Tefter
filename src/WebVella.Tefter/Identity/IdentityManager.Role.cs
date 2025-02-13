namespace WebVella.Tefter.Identity;

public partial interface IIdentityManager
{
	RoleBuilder CreateRoleBuilder(Role role = null);
	Role GetRole(Guid id);
	Role GetRole(string name);
	ReadOnlyCollection<Role> GetRoles();
	Role SaveRole(Role role);
	void DeleteRole(Role role);
	Task<Role> GetRoleAsync(Guid id);
	Task<Role> GetRoleAsync(string name);
	Task<ReadOnlyCollection<Role>> GetRolesAsync();
	Task<Role> SaveRoleAsync(Role role);
	Task DeleteRoleAsync(Role role);

}

public partial class IdentityManager : IIdentityManager
{
	public RoleBuilder CreateRoleBuilder(Role role = null)
	{
		return new RoleBuilder(this, role);
	}

	public Role GetRole(Guid id)
	{
		var roleDbo = _dboManager.Get<RoleDbo>(id, nameof(Role.Id));
		if (roleDbo == null)
			return null;

		return new RoleBuilder(this, roleDbo.Id)
				.WithName(roleDbo.Name)
				.IsSystem(roleDbo.IsSystem)
				.Build();

	}

	public Role GetRole(string name)
	{
		var roleDbo = _dboManager.Get<RoleDbo>(name, nameof(Role.Name));
		if (roleDbo == null)
			return null;

		return new RoleBuilder(this, roleDbo.Id)
				.WithName(roleDbo.Name)
				.IsSystem(roleDbo.IsSystem)
				.Build();
	}

	public ReadOnlyCollection<Role> GetRoles()
	{
		var orderSettings = new TfOrderSettings(nameof(RoleDbo.Name), OrderDirection.ASC);

		return _dboManager.GetList<RoleDbo>(null, null, orderSettings)
			.Select(x =>
				new RoleBuilder(this, x.Id)
					.WithName(x.Name)
					.IsSystem(x.IsSystem)
					.Build()
			)
			.ToList()
			.AsReadOnly();
	}

	public Role SaveRole(Role role)
	{
		if (role == null)
			throw new ArgumentNullException(nameof(role));

		if (role.Id == Guid.Empty)
			return CreateRole(role);
		else
			return UpdateRole(role);
	}

	private Role CreateRole(Role role)
	{
		_roleValidator
			.ValidateCreate(role)
			.ToValidationException()
			.ThrowIfContainsErrors();

		RoleDbo roleDbo = new RoleDbo
		{
			Id = Guid.NewGuid(),
			Name = role.Name,
			IsSystem = role.IsSystem
		};

		bool success = _dboManager.Insert<RoleDbo>(roleDbo);
		if (!success)
			throw new TfDboServiceException("Insert<RoleDbo> failed");

		return GetRole(roleDbo.Id);
	}

	private Role UpdateRole(Role role)
	{
		_roleValidator
			.ValidateUpdate(role)
			.ToValidationException()
			.ThrowIfContainsErrors();

		RoleDbo roleDbo = new RoleDbo
		{
			Id = role.Id,
			Name = role.Name,
			IsSystem = role.IsSystem
		};

		bool success = _dboManager.Update<RoleDbo>(roleDbo);
		if (!success)
			throw new TfDboServiceException("Update<RoleDbo> failed");

		return GetRole(roleDbo.Id);
	}

	public void DeleteRole(Role role)
	{
		if (role == null)
			throw new ArgumentNullException(nameof(role));

		_roleValidator
			.ValidateDelete(role)
			.ToValidationException()
			.ThrowIfContainsErrors();

		var success = _dboManager.Delete<RoleDbo>(role.Id);
		if (!success)
			throw new TfDboServiceException("Delete<RoleDbo> failed");
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
		var orderSettings = new TfOrderSettings(nameof(RoleDbo.Name), OrderDirection.ASC);

		return  (await _dboManager.GetListAsync<RoleDbo>(null, null, orderSettings))
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
			return await CreateRoleAsync(role);
		else
			return await UpdateRoleAsync(role);
	}

	private async Task<Role> CreateRoleAsync(Role role)
	{
		_roleValidator
			.ValidateCreate(role)
			.ToValidationException()
			.ThrowIfContainsErrors();

		RoleDbo roleDbo = new RoleDbo
		{
			Id = Guid.NewGuid(),
			Name = role.Name,
			IsSystem = role.IsSystem
		};

		bool success = await _dboManager.InsertAsync<RoleDbo>(roleDbo);
		if (!success)
			throw new TfDboServiceException("InsertAsync<RoleDbo> failed");

		return await GetRoleAsync(roleDbo.Id);
	}

	private async Task<Role> UpdateRoleAsync(Role role)
	{
		_roleValidator
			.ValidateUpdate(role)
			.ToValidationException()
			.ThrowIfContainsErrors();

		RoleDbo roleDbo = new RoleDbo
		{
			Id = role.Id,
			Name = role.Name,
			IsSystem = role.IsSystem
		};

		bool success = await _dboManager.UpdateAsync<RoleDbo>(roleDbo);
		if (!success)
			throw new TfDboServiceException("UpdateAsync<RoleDbo> failed");

		return await GetRoleAsync(roleDbo.Id);
	}

	public async Task DeleteRoleAsync(Role role)
	{
		if (role == null)
			throw new ArgumentNullException(nameof(role));

		_roleValidator
			.ValidateDelete(role)
			.ToValidationException()
			.ThrowIfContainsErrors();

		var success = await _dboManager.DeleteAsync<RoleDbo>(role.Id);
		if (!success)
			throw new TfDboServiceException("DeleteAsync<RoleDbo> failed");
	}
}
