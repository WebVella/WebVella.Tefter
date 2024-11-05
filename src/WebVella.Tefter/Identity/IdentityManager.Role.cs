namespace WebVella.Tefter.Identity;

public partial interface IIdentityManager
{
	RoleBuilder CreateRoleBuilder(Role role = null);

	Result<Role> GetRole(Guid id);
	Result<Role> GetRole(string name);
	Result<ReadOnlyCollection<Role>> GetRoles();
	Result<Role> SaveRole(Role role);
	Result DeleteRole(Role role);
	Task<Result<Role>> GetRoleAsync(Guid id);
	Task<Result<Role>> GetRoleAsync(string name);
	Task<Result<ReadOnlyCollection<Role>>> GetRolesAsync();
	Task<Result<Role>> SaveRoleAsync(Role role);
	Task<Result> DeleteRoleAsync(Role role);

}

public partial class IdentityManager : IIdentityManager
{
	public RoleBuilder CreateRoleBuilder(Role role = null)
	{
		return new RoleBuilder(this, role);
	}

	public Result<Role> GetRole(Guid id)
	{
		var roleDbo = _dboManager.Get<RoleDbo>(id, nameof(Role.Id));
		if (roleDbo == null)
			return Result.Ok();

		return Result.Ok(
			new RoleBuilder(this, roleDbo.Id)
				.WithName(roleDbo.Name)
				.IsSystem(roleDbo.IsSystem)
				.Build()
			);
	}

	public Result<Role> GetRole(string name)
	{
		var roleDbo = _dboManager.Get<RoleDbo>(name, nameof(Role.Name));
		if (roleDbo == null)
			return Result.Ok();

		return Result.Ok(
			new RoleBuilder(this, roleDbo.Id)
				.WithName(roleDbo.Name)
				.IsSystem(roleDbo.IsSystem)
				.Build()
			);
	}

	public Result<ReadOnlyCollection<Role>> GetRoles()
	{
		var orderSettings = new TfOrderSettings(nameof(RoleDbo.Name), OrderDirection.ASC);

		var result = _dboManager.GetList<RoleDbo>(null, null, orderSettings)
			.Select(x =>
				new RoleBuilder(this, x.Id)
					.WithName(x.Name)
					.IsSystem(x.IsSystem)
					.Build()
			)
			.ToList()
			.AsReadOnly();

		return Result.Ok(result);
	}

	public Result<Role> SaveRole(Role role)
	{
		if (role == null)
			throw new ArgumentNullException(nameof(role));

		if (role.Id == Guid.Empty)
			return CreateRole(role);
		else
			return UpdateRole(role);
	}

	private Result<Role> CreateRole(Role role)
	{
		ValidationResult result = _roleValidator.ValidateCreate(role);

		if (!result.IsValid)
			return result.ToResult<Role>();

		RoleDbo roleDbo = new RoleDbo
		{
			Id = Guid.NewGuid(),
			Name = role.Name,
			IsSystem = role.IsSystem
		};

		bool success = _dboManager.Insert<RoleDbo>(roleDbo);
		if (!success)
			return Result.Fail(new DboManagerError("Insert", roleDbo));

		return GetRole(roleDbo.Id);
	}

	private Result<Role> UpdateRole(Role role)
	{
		ValidationResult result = _roleValidator.ValidateUpdate(role);

		if (!result.IsValid)
			return result.ToResult<Role>();

		RoleDbo roleDbo = new RoleDbo
		{
			Id = role.Id,
			Name = role.Name,
			IsSystem = role.IsSystem
		};

		bool success = _dboManager.Update<RoleDbo>(roleDbo);
		
		if (!success)
			return Result.Fail(new DboManagerError("Update", roleDbo));

		return GetRole(roleDbo.Id);
	}

	public Result DeleteRole(Role role)
	{
		if (role == null)
			throw new ArgumentNullException(nameof(role));

		ValidationResult result = _roleValidator.ValidateDelete(role);

		if (!result.IsValid)
			return result.ToResult();

		var success = _dboManager.Delete<RoleDbo>(role.Id);
		if (!success)
			return Result.Fail(new DboManagerError("Delete", role));

		return Result.Ok();
	}

	public async Task<Result<Role>> GetRoleAsync(Guid id)
	{
		var roleDbo = await _dboManager.GetAsync<RoleDbo>(id, nameof(Role.Id));
		
		if (roleDbo == null)
			return Result.Ok();

		return Result.Ok(
			new RoleBuilder(this, roleDbo.Id)
				.WithName(roleDbo.Name)
				.IsSystem(roleDbo.IsSystem)
				.Build()
			);
	}

	public async Task<Result<Role>> GetRoleAsync(string name)
	{
		var roleDbo = await _dboManager.GetAsync<RoleDbo>(name, nameof(Role.Name));
		if (roleDbo == null)
			return Result.Ok();

		return Result.Ok(
			new RoleBuilder(this, roleDbo.Id)
				.WithName(roleDbo.Name)
				.IsSystem(roleDbo.IsSystem)
				.Build()
			);
	}

	public async Task<Result<ReadOnlyCollection<Role>>> GetRolesAsync()
	{
		var orderSettings = new TfOrderSettings(nameof(RoleDbo.Name), OrderDirection.ASC);

		var result = (await _dboManager.GetListAsync<RoleDbo>(null, null, orderSettings))
			.Select(x =>
				new RoleBuilder(this, x.Id)
					.WithName(x.Name)
					.IsSystem(x.IsSystem)
					.Build()
			)
			.ToList()
			.AsReadOnly();

		return Result.Ok(result);
	}

	public async Task<Result<Role>> SaveRoleAsync(Role role)
	{
		if (role == null)
			throw new ArgumentNullException(nameof(role));

		if (role.Id == Guid.Empty)
			return await CreateRoleAsync(role);
		else
			return await UpdateRoleAsync(role);
	}

	private async Task<Result<Role>> CreateRoleAsync(Role role)
	{
		ValidationResult result = _roleValidator.ValidateCreate(role);

		if(!result.IsValid)
			return result.ToResult<Role>();
		
		RoleDbo roleDbo = new RoleDbo
		{
			Id = Guid.NewGuid(),
			Name = role.Name,
			IsSystem = role.IsSystem
		};

		bool success = await _dboManager.InsertAsync<RoleDbo>(roleDbo);

		if (!success)
			return Result.Fail(new DboManagerError("InsertAsync", roleDbo));

		return await GetRoleAsync(roleDbo.Id);
	}

	private async Task<Result<Role>> UpdateRoleAsync(Role role)
	{
		ValidationResult result = _roleValidator.ValidateUpdate(role);

		if (!result.IsValid)
			return result.ToResult<Role>();


		RoleDbo roleDbo = new RoleDbo
		{
			Id = role.Id,
			Name = role.Name,
			IsSystem = role.IsSystem
		};

		bool success = await _dboManager.UpdateAsync<RoleDbo>(roleDbo);

		if (!success)
			return Result.Fail(new DboManagerError("UpdateAsync", roleDbo));

		return await GetRoleAsync(roleDbo.Id);
	}

	public async Task<Result> DeleteRoleAsync(Role role)
	{
		if (role == null)
			throw new ArgumentNullException(nameof(role));

		ValidationResult result = _roleValidator.ValidateDelete(role);

		if (!result.IsValid)
			return result.ToResult();

		var success = await _dboManager.DeleteAsync<RoleDbo>(role.Id);
		if (!success)
			return Result.Fail(new DboManagerError("DeleteAsync", role));

		return Result.Ok();
	}
}
