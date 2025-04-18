﻿namespace WebVella.Tefter.Services;

public partial interface ITfService
{
	TfRoleBuilder CreateRoleBuilder(TfRole role = null);
	TfRole GetRole(Guid id);
	TfRole GetRole(string name);
	ReadOnlyCollection<TfRole> GetRoles();
	TfRole SaveRole(TfRole role);
	void DeleteRole(TfRole role);
	Task<TfRole> GetRoleAsync(Guid id);
	Task<TfRole> GetRoleAsync(string name);
	Task<ReadOnlyCollection<TfRole>> GetRolesAsync();
	Task<TfRole> SaveRoleAsync(TfRole role);
	Task DeleteRoleAsync(TfRole role);
}

public partial class TfService : ITfService
{
	public TfRoleBuilder CreateRoleBuilder(TfRole role = null)
	{
		try
		{
			return new TfRoleBuilder(this, role);
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public TfRole GetRole(Guid id)
	{
		try
		{
			var roleDbo = _dboManager.Get<RoleDbo>(id, nameof(TfRole.Id));
			if (roleDbo == null)
				return null;

			return new TfRoleBuilder(this, roleDbo.Id)
					.WithName(roleDbo.Name)
					.IsSystem(roleDbo.IsSystem)
					.Build();
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}

	}

	public TfRole GetRole(string name)
	{
		try
		{
			var roleDbo = _dboManager.Get<RoleDbo>(name, nameof(TfRole.Name));
			if (roleDbo == null)
				return null;

			return new TfRoleBuilder(this, roleDbo.Id)
					.WithName(roleDbo.Name)
					.IsSystem(roleDbo.IsSystem)
					.Build();
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public ReadOnlyCollection<TfRole> GetRoles()
	{
		try
		{
			var orderSettings = new TfOrderSettings(nameof(RoleDbo.Name), OrderDirection.ASC);

			return _dboManager.GetList<RoleDbo>(null, null, orderSettings)
				.Select(x =>
					new TfRoleBuilder(this, x.Id)
						.WithName(x.Name)
						.IsSystem(x.IsSystem)
						.Build()
				)
				.ToList()
				.AsReadOnly();
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public TfRole SaveRole(TfRole role)
	{
		try
		{
			if (role == null)
				throw new ArgumentNullException(nameof(role));

			if (role.Id == Guid.Empty)
				return CreateRole(role);
			else
				return UpdateRole(role);
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	private TfRole CreateRole(TfRole role)
	{
		try
		{
			new RoleValidator(this)
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
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	private TfRole UpdateRole(TfRole role)
	{
		try
		{
			new RoleValidator(this)
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
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public void DeleteRole(TfRole role)
	{
		try
		{
			if (role == null)
				throw new ArgumentNullException(nameof(role));

			new RoleValidator(this)
				.ValidateDelete(role)
				.ToValidationException()
				.ThrowIfContainsErrors();

			var success = _dboManager.Delete<RoleDbo>(role.Id);
			if (!success)
				throw new TfDboServiceException("Delete<RoleDbo> failed");
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public async Task<TfRole> GetRoleAsync(Guid id)
	{
		try
		{
			var roleDbo = await _dboManager.GetAsync<RoleDbo>(id, nameof(TfRole.Id));

			if (roleDbo == null)
				return null;

			return new TfRoleBuilder(this, roleDbo.Id)
					.WithName(roleDbo.Name)
					.IsSystem(roleDbo.IsSystem)
					.Build();
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public async Task<TfRole> GetRoleAsync(string name)
	{
		try
		{
			var roleDbo = await _dboManager.GetAsync<RoleDbo>(name, nameof(TfRole.Name));
			if (roleDbo == null)
				return null;

			return new TfRoleBuilder(this, roleDbo.Id)
					.WithName(roleDbo.Name)
					.IsSystem(roleDbo.IsSystem)
					.Build();
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public async Task<ReadOnlyCollection<TfRole>> GetRolesAsync()
	{
		try
		{
			var orderSettings = new TfOrderSettings(nameof(RoleDbo.Name), OrderDirection.ASC);

			return (await _dboManager.GetListAsync<RoleDbo>(null, null, orderSettings))
				.Select(x =>
					new TfRoleBuilder(this, x.Id)
						.WithName(x.Name)
						.IsSystem(x.IsSystem)
						.Build()
				)
				.ToList()
				.AsReadOnly();
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public async Task<TfRole> SaveRoleAsync(TfRole role)
	{
		try
		{
			if (role == null)
				throw new ArgumentNullException(nameof(role));

			if (role.Id == Guid.Empty)
				return await CreateRoleAsync(role);
			else
				return await UpdateRoleAsync(role);
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	private async Task<TfRole> CreateRoleAsync(TfRole role)
	{
		try
		{
			new RoleValidator(this)
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
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	private async Task<TfRole> UpdateRoleAsync(TfRole role)
	{
		try
		{
			new RoleValidator(this)
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
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public async Task DeleteRoleAsync(TfRole role)
	{
		try
		{
			if (role == null)
				throw new ArgumentNullException(nameof(role));

			new RoleValidator(this)
				.ValidateDelete(role)
				.ToValidationException()
				.ThrowIfContainsErrors();

			var success = await _dboManager.DeleteAsync<RoleDbo>(role.Id);
			if (!success)
				throw new TfDboServiceException("DeleteAsync<RoleDbo> failed");
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	#region <--- validation --->

	internal class RoleValidator : AbstractValidator<TfRole>
	{
		public RoleValidator(ITfService tfService)
		{
			RuleSet("general", () =>
			{
				RuleFor(role => role.Name)
					.NotEmpty()
					.WithMessage("Role name is required.");
			});

			RuleSet("create", () =>
			{
				RuleFor(role => role.Name)
					.Must(name => { return tfService.GetRole(name) == null; })
					.WithMessage("There is already existing role with specified name.");

				RuleFor(role => role.Id)
					.Must(id => { return tfService.GetRole(id) == null; })
					.WithMessage("There is already existing role with specified identifier.");
			});

			RuleSet("update", () =>
			{
				RuleFor(role => role.Id)
					.Must((role, id) => { return tfService.GetRole(id) != null; })
					.WithMessage("There is no existing role for specified identifier.");

				RuleFor(role => role.Name)
					.Must((role, name) =>
					{
						var existingRole = tfService.GetRole(role.Name);
						return !(existingRole != null && existingRole.Id != role.Id);
					})
					.WithMessage("There is already existing role with specified name.");
			});

			RuleSet("delete", () =>
			{
				RuleFor(role => role.Id)
					.Must((role, id) => { return tfService.GetRole(id) != null; })
					.WithMessage("There is no existing role for specified identifier.");

				RuleFor(role => role.IsSystem)
					.Must((role, isSystem) => { return !role.IsSystem; })
					.WithMessage("The role is system and cannot be deleted.");

				RuleFor(role => role.Id)
					.Must(id =>
					{
						var users = tfService.GetUsers();
						return users.Count(x => x.Roles.Any(r => r.Id == id)) == 0;
					})
					.WithMessage("There is one or more existing users with this role.");
			});
		}

		public ValidationResult ValidateCreate(TfRole role)
		{
			if (role == null)
				return new ValidationResult(new[] { new ValidationFailure("", "The role instance is null.") });

			return this.Validate(role, options =>
			{
				options.IncludeRuleSets("general", "create");
			});
		}

		public ValidationResult ValidateUpdate(TfRole role)
		{
			if (role == null)
				return new ValidationResult(new[] { new ValidationFailure("", "The role instance is null.") });

			return this.Validate(role, options =>
			{
				options.IncludeRuleSets("general", "update");
			});
		}

		public ValidationResult ValidateDelete(TfRole role)
		{
			if (role == null)
				return new ValidationResult(new[] { new ValidationFailure("", "The role instance is null.") });

			return this.Validate(role, options =>
			{
				options.IncludeRuleSets("delete");
			});
		}
	}

	#endregion
}
