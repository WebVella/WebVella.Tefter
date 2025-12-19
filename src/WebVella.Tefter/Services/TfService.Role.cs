using Nito.AsyncEx.Synchronous;

namespace WebVella.Tefter.Services;

public partial interface ITfService
{
	/// <summary>
	/// Creates a new instance of <see cref="TfRoleBuilder"/> for building roles.
	/// </summary>
	/// <param name="role">Optional role to initialize the builder with.</param>
	/// <returns>A <see cref="TfRoleBuilder"/> instance.</returns>
	TfRoleBuilder CreateRoleBuilder(
		TfRole role = null);

	/// <summary>
	/// Retrieves a role by its unique identifier.
	/// </summary>
	/// <param name="id">The unique identifier of the role.</param>
	/// <returns>The <see cref="TfRole"/> instance if found; otherwise, null.</returns>
	TfRole GetRole(
		Guid id);

	/// <summary>
	/// Retrieves a role by its name.
	/// </summary>
	/// <param name="name">The name of the role.</param>
	/// <returns>The <see cref="TfRole"/> instance if found; otherwise, null.</returns>
	TfRole GetRole(
		string name);


	ReadOnlyCollection<TfRole> GetRoles(string? search);
	/// <summary>
	/// Retrieves all roles in the system.
	/// </summary>
	/// <returns>A read-only collection of <see cref="TfRole"/> instances.</returns>
	ReadOnlyCollection<TfRole> GetRoles();

	/// <summary>
	/// Saves a role to the system. Creates a new role if it does not exist, or updates an existing one.
	/// </summary>
	/// <param name="role">The role to save.</param>
	/// <returns>The saved <see cref="TfRole"/> instance.</returns>
	TfRole SaveRole(
		TfRole role);

	/// <summary>
	/// Creates new role
	/// </summary>
	/// <param name="role"></param>
	/// <returns></returns>
	TfRole CreateRole(TfRole role);

	/// <summary>
	/// Updates existing role
	/// </summary>
	/// <param name="role"></param>
	/// <returns></returns>
	TfRole UpdateRole(TfRole role);

	/// <summary>
	/// Deletes a role from the system.
	/// </summary>
	/// <param name="role">The role to delete.</param>
	void DeleteRole(
		TfRole role);

	/// <summary>
	/// Asynchronously retrieves a role by its unique identifier.
	/// </summary>
	/// <param name="id">The unique identifier of the role.</param>
	/// <returns>A task representing the asynchronous operation, with the <see cref="TfRole"/> instance if found; otherwise, null.</returns>
	Task<TfRole> GetRoleAsync(
		Guid id);

	/// <summary>
	/// Asynchronously retrieves a role by its name.
	/// </summary>
	/// <param name="name">The name of the role.</param>
	/// <returns>A task representing the asynchronous operation, with the <see cref="TfRole"/> instance if found; otherwise, null.</returns>
	Task<TfRole> GetRoleAsync(
		string name);

	/// <summary>
	/// Asynchronously retrieves all roles in the system.
	/// </summary>
	/// <returns>A task representing the asynchronous operation, with a read-only collection of <see cref="TfRole"/> instances.</returns>
	Task<ReadOnlyCollection<TfRole>> GetRolesAsync();

	/// <summary>
	/// Asynchronously saves a role to the system. Creates a new role if it does not exist, or updates an existing one.
	/// </summary>
	/// <param name="role">The role to save.</param>
	/// <returns>A task representing the asynchronous operation, with the saved <see cref="TfRole"/> instance.</returns>
	Task<TfRole> SaveRoleAsync(
		TfRole role);

	/// <summary>
	/// Creates new role
	/// </summary>
	/// <param name="role"></param>
	/// <returns></returns>
	Task<TfRole> CreateRoleAsync(TfRole role);

	/// <summary>
	/// Updates existing role
	/// </summary>
	/// <param name="role"></param>
	/// <returns></returns>
	Task<TfRole> UpdateRoleAsync(TfRole role);

	/// <summary>
	/// Asynchronously deletes a role from the system.
	/// </summary>
	/// <param name="role">The role to delete.</param>
	/// <returns>A task representing the asynchronous operation.</returns>
	Task DeleteRoleAsync(
		TfRole role);

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

	public ReadOnlyCollection<TfRole> GetRoles(string? search)
	{
		try
		{
			var allRoles = GetRoles();
			if (String.IsNullOrWhiteSpace(search))
				return allRoles;
			search = search.Trim().ToLowerInvariant();
			return allRoles.Where(x =>
				x.Name.ToLowerInvariant().Contains(search)
				).ToList().AsReadOnly();
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
			{
				var existingRole = GetRole(role.Id);
				if (existingRole is not null)
					return UpdateRole(role);
				else
					return CreateRole(role);
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public TfRole CreateRole(TfRole role)
	{
		try
		{
			new RoleValidator(this)
				.ValidateCreate(role)
				.ToValidationException()
				.ThrowIfContainsErrors();

			Guid roleId = role.Id;
			if (role.Id == Guid.Empty)
				roleId = Guid.NewGuid();

			RoleDbo roleDbo = new RoleDbo
			{
				Id = roleId,
				Name = role.Name,
				IsSystem = role.IsSystem
			};

			bool success = _dboManager.Insert<RoleDbo>(roleDbo);
			if (!success)
				throw new TfDboServiceException("Insert<RoleDbo> failed");
			var result = GetRole(roleDbo.Id);
			_eventBus.Publish(
				key: null,
				payload: new TfRoleCreatedEventPayload(result));			
			
			return result;
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public TfRole UpdateRole(TfRole role)
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

			var result = GetRole(roleDbo.Id);
			_eventBus.Publish(
				key: null,
				payload: new TfRoleUpdatedEventPayload(result));				
			return result;
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
			_eventBus.Publish(
				key: null,
				payload: new TfRoleDeletedEventPayload(role));				
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
			{
				var existingRole = await GetRoleAsync(role.Id);

				if (existingRole is not null)
					return await UpdateRoleAsync(role);
				else
					return await CreateRoleAsync(role);
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public async Task<TfRole> CreateRoleAsync(TfRole role)
	{
		try
		{
			new RoleValidator(this)
				.ValidateCreate(role)
				.ToValidationException()
				.ThrowIfContainsErrors();

			Guid roleId = role.Id;
			if (role.Id == Guid.Empty)
				roleId = Guid.NewGuid();

			RoleDbo roleDbo = new RoleDbo
			{
				Id = roleId,
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

	public async Task<TfRole> UpdateRoleAsync(TfRole role)
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

				RuleFor(role => role.Id)
					.Must(id =>
					{
						var spaces = tfService.GetSpacesList();
						return spaces.Count(x => x.Roles.Any(r => r.Id == id)) == 0;
					})
					.WithMessage("There is one or more existing spaces with this role.");
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
