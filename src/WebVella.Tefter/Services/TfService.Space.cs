using Nito.AsyncEx.Synchronous;

namespace WebVella.Tefter.Services;

public partial interface ITfService
{
	/// <summary>
	/// Retrieves a list of all spaces.
	/// </summary>
	/// <returns>A list of <see cref="TfSpace"/> objects.</returns>
	public List<TfSpace> GetSpacesList();

	/// <summary>
	/// Retrieves a list of spaces accessible to a specific user.
	/// </summary>
	/// <param name="userId">The unique identifier of the user.</param>
	/// <returns>A list of <see cref="TfSpace"/> objects accessible to the user.</returns>
	public List<TfSpace> GetSpacesListForUser(
		Guid userId);

	/// <summary>
	/// Retrieves a specific space by its unique identifier.
	/// </summary>
	/// <param name="id">The unique identifier of the space.</param>
	/// <returns>The <see cref="TfSpace"/> object if found; otherwise, null.</returns>
	public TfSpace GetSpace(
		Guid id);

	/// <summary>
	/// Creates a new space.
	/// </summary>
	/// <param name="space">The <see cref="TfSpace"/> object to create.</param>
	/// <returns>The created <see cref="TfSpace"/> object.</returns>
	public TfSpace CreateSpace(
		TfSpace space);

	/// <summary>
	/// Updates an existing space.
	/// </summary>
	/// <param name="space">The <see cref="TfSpace"/> object with updated information.</param>
	/// <returns>The updated <see cref="TfSpace"/> object.</returns>
	public TfSpace UpdateSpace(
		TfSpace space);

	/// <summary>
	/// Deletes a space by its unique identifier.
	/// </summary>
	/// <param name="id">The unique identifier of the space to delete.</param>
	public void DeleteSpace(
		Guid id);

	/// <summary>
	/// Moves a space up in the order.
	/// </summary>
	/// <param name="id">The unique identifier of the space to move up.</param>
	public void MoveSpaceUp(
		Guid id);

	/// <summary>
	/// Moves a space down in the order.
	/// </summary>
	/// <param name="id">The unique identifier of the space to move down.</param>
	public void MoveSpaceDown(
		Guid id);

	public TfSpace SetSpacePrivacy(
		Guid spaceId,
		bool isPrivate);	

	/// <summary>
	/// Removes a role from a list of spaces.
	/// </summary>
	/// <param name="spaces">The list of <see cref="TfSpace"/> objects to remove the role from.</param>
	/// <param name="role">The <see cref="TfRole"/> to remove.</param>
	public void RemoveSpacesRole(
		List<TfSpace> spaces,
		TfRole role);

	/// <summary>
	/// Adds a role to a list of spaces.
	/// </summary>
	/// <param name="spaces">The list of <see cref="TfSpace"/> objects to add the role to.</param>
	/// <param name="role">The <see cref="TfRole"/> to add.</param>
	public void AddSpacesRole(
		List<TfSpace> spaces,
		TfRole role);
}

public partial class TfService : ITfService
{
	public List<TfSpace> GetSpacesList()
	{
		try
		{
			var orderSettings = new TfOrderSettings(
				nameof(TfSpace.Position),
				OrderDirection.ASC);

			var dbos = _dboManager.GetList<TfSpaceDbo>(
				order: orderSettings);

			var spaces = dbos.Select(x => ConvertDboToModel(x)).ToList();

			var roles = GetRoles();
			var spaceRolesDict = new Dictionary<Guid, List<TfRole>>();
			foreach (var spaceRoleRelation in _dboManager.GetList<SpaceRoleDbo>())
			{
				if (!spaceRolesDict.ContainsKey(spaceRoleRelation.SpaceId))
					spaceRolesDict[spaceRoleRelation.SpaceId] = new List<TfRole>();

				spaceRolesDict[spaceRoleRelation.SpaceId].Add(roles.Single(x => x.Id == spaceRoleRelation.RoleId));
			}

			foreach (var space in spaces)
			{
				if (spaceRolesDict.ContainsKey(space.Id))
					space.Roles.AddRange(spaceRolesDict[space.Id].ToArray());
			}

			return spaces;

		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}


	public List<TfSpace> GetSpacesListForUser(
		Guid userId)
	{
		try
		{
			var user = GetUser(userId);
			if (user == null)
				throw new TfValidationException(nameof(userId), "User not found.");

			var spaces = GetSpacesList();

			//return all spaces for administrators
			var isAdmin = user.Roles.Any(x => x.Id == TfConstants.ADMIN_ROLE_ID);
			if (isAdmin)
				return spaces;

			List<TfSpace> userSpaces = new List<TfSpace>();
			foreach (var space in spaces)
			{
				if (space.IsPrivate && !space.Roles
					.Select(x => x.Id)
					.Intersect(user.Roles.Select(x => x.Id))
					.Any()) continue;

				userSpaces.Add(space);
			}

			return userSpaces;
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public TfSpace GetSpace(
		Guid id)
	{
		try
		{
			var dbo = _dboManager.Get<TfSpaceDbo>(id);
			var space = ConvertDboToModel(dbo);

			if (space is not null)
			{
				var roles = GetRoles();
				var spaceRoles = new List<TfRole>();
				foreach (var spaceRoleRelation in _dboManager.GetList<SpaceRoleDbo>(space.Id, nameof(SpaceRoleDbo.SpaceId)))
					space.Roles.Add(roles.Single(x => x.Id == spaceRoleRelation.RoleId));
			}

			return space;
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public TfSpace CreateSpace(
		TfSpace space)
	{
		try
		{
			if (space != null && space.Id == Guid.Empty)
				space.Id = Guid.NewGuid();

			new TfSpaceValidator(this)
				.ValidateCreate(space)
				.ToValidationException()
				.ThrowIfContainsErrors();

			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var spaces = GetSpacesList();

				//position is ignored - space is added at last place
				var spaceDbo = ConvertModelToDbo(space);
				spaceDbo.Position = (short)(spaces.Count + 1);

				var success = _dboManager.Insert<TfSpaceDbo>(spaceDbo);
				if (!success)
					throw new TfDboServiceException("Insert<TfSpaceDbo> failed");

				if (space.Roles is not null)
				{
					foreach (var role in space.Roles)
					{
						var spaceRoleDbo = new SpaceRoleDbo
						{
							RoleId = role.Id,
							SpaceId = spaceDbo.Id
						};

						success = _dboManager.Insert<SpaceRoleDbo>(spaceRoleDbo);
						if (!success)
							throw new TfDboServiceException("Insert<SpaceRoleDbo> failed");
					}
				}

				scope.Complete();
				
				var result = GetSpace(space.Id);
				PublishEventWithScope(new TfSpaceCreatedEvent(result));
				
				return result;
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public TfSpace UpdateSpace(
		TfSpace space)
	{
		try
		{
			new TfSpaceValidator(this)
				.ValidateUpdate(space)
				.ToValidationException()
				.ThrowIfContainsErrors();

			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var existingSpace = GetSpace(space.Id);

				//position is not updated
				var spaceDbo = ConvertModelToDbo(space);
				spaceDbo.Position = existingSpace.Position;

				var success = _dboManager.Update<TfSpaceDbo>(spaceDbo);
				if (!success)
					throw new TfDboServiceException("Update<TfSpaceDbo> failed");

				//remove old roles
				foreach (var role in existingSpace.Roles)
				{
					var dbId = new Dictionary<string, Guid> {
						{ nameof(SpaceRoleDbo.SpaceId), space.Id },
						{ nameof(SpaceRoleDbo.RoleId), role.Id }};

					success = _dboManager.Delete<SpaceRoleDbo>(dbId);

					if (!success)
						throw new TfDboServiceException("Delete<SpaceRoleDbo> failed");
				}

				//add new roles
				foreach (var role in space.Roles)
				{
					var spaceRoleDbo = new SpaceRoleDbo
					{
						RoleId = role.Id,
						SpaceId = spaceDbo.Id
					};

					success = _dboManager.Insert<SpaceRoleDbo>(spaceRoleDbo);

					if (!success)
						throw new TfDboServiceException("Insert<SpaceRoleDbo> failed");
				}

				scope.Complete();
				
				var result = GetSpace(space.Id);
				PublishEventWithScope(new TfSpaceUpdatedEvent(result));
				
				return result;
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public void MoveSpaceUp(
		Guid id)
	{
		try
		{
			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var spaces = GetSpacesList();

				var space = spaces.SingleOrDefault(x => x.Id == id);

				if (space == null)
				{
					throw new TfValidationException(nameof(id),
						"Found no space for specified identifier.");
				}

				if (space.Position == 1)
					return;

				var prevSpace = spaces.SingleOrDefault(x => x.Position == (space.Position - 1));
				space.Position = (short)(space.Position - 1);

				if (prevSpace != null)
					prevSpace.Position = (short)(prevSpace.Position + 1);

				var success = _dboManager.Update<TfSpaceDbo>(ConvertModelToDbo(space));
				if (!success)
					throw new TfDboServiceException("Update<TfSpaceDbo> failed");

				if (prevSpace != null)
				{
					success = _dboManager.Update<TfSpaceDbo>(ConvertModelToDbo(prevSpace));
					if (!success)
						throw new TfDboServiceException("Update<TfSpaceDbo> failed");
				}

				scope.Complete();
				
				var result = GetSpace(id);
				PublishEventWithScope(new TfSpaceUpdatedEvent(result));
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public void MoveSpaceDown(
		Guid id)
	{
		try
		{
			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var spaces = GetSpacesList();

				var space = spaces.SingleOrDefault(x => x.Id == id);

				if (space == null)
				{
					throw new TfValidationException(nameof(id),
						"Found no space for specified identifier.");
				}

				if (space.Position == spaces.Count)
					return;


				var nextSpace = spaces.SingleOrDefault(x => x.Position == (space.Position + 1));
				space.Position = (short)(space.Position + 1);

				if (nextSpace != null)
					nextSpace.Position = (short)(nextSpace.Position - 1);

				var success = _dboManager.Update<TfSpaceDbo>(ConvertModelToDbo(space));
				if (!success)
					throw new TfDboServiceException("Update<TfSpaceDbo> failed");

				if (nextSpace != null)
				{
					success = _dboManager.Update<TfSpaceDbo>(ConvertModelToDbo(nextSpace));
					if (!success)
						throw new TfDboServiceException("Update<TfSpaceDbo> failed");
				}

				scope.Complete();
				
				var result = GetSpace(id);
				PublishEventWithScope(new TfSpaceUpdatedEvent(result));
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public void DeleteSpace(
		Guid id)
	{
		try
		{
			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var space = GetSpace(id);

				new TfSpaceValidator(this)
					.ValidateDelete(space)
					.ToValidationException()
					.ThrowIfContainsErrors();

				bool success = false;

				foreach (var role in space.Roles)
				{
					var dbId = new Dictionary<string, Guid> {
						{ nameof(SpaceRoleDbo.SpaceId), space.Id },
						{ nameof(SpaceRoleDbo.RoleId), role.Id }};

					success = _dboManager.Delete<SpaceRoleDbo>(dbId);

					if (!success)
						throw new TfDboServiceException("Delete<SpaceRoleDbo> failed");
				}

				var spacePages = GetSpacePages(id);
				foreach (var spacePage in spacePages.OrderByDescending(x => x.Position))
				{
					DeleteSpacePage(spacePage);
				}

				success = _dboManager.Delete<TfSpaceDbo>(id);

				if (!success)
					throw new TfDboServiceException("Delete<TfSpaceDbo> failed");

				var spacesAfter = GetSpacesList()
					.Where(x => x.Position > space.Position)
					.ToList();

				//update positions for spaces after the one being deleted
				foreach (var spaceAfter in spacesAfter)
				{
					spaceAfter.Position--;

					var successUpdatePosition = _dboManager.Update<TfSpaceDbo>(ConvertModelToDbo(spaceAfter));
					if (!successUpdatePosition)
						throw new TfDboServiceException("Update<TfSpaceDbo> failed");
				}

				scope.Complete();
				
				PublishEventWithScope(new TfSpaceUpdatedEvent(space));
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}
	public TfSpace SetSpacePrivacy(
		Guid spaceId,
		bool isPrivate)
	{
		var space = GetSpace(spaceId);
		if (space is null)
			throw new Exception("Space not found");
		space.IsPrivate = isPrivate;
		var result = UpdateSpace(space);
		PublishEventWithScope(new TfSpaceUpdatedEvent(space));
		return result;
	}	

	public void AddSpacesRole(
		List<TfSpace> spaces,
		TfRole role)
	{
		try
		{
			if (spaces == null)
				throw new ArgumentNullException(nameof(spaces));

			if (role == null)
				throw new ArgumentNullException(nameof(role));

			using (TfDatabaseTransactionScope scope = _dbService.CreateTransactionScope())
			{
				foreach (var space in spaces)
				{
					var existingSpace = GetSpace(space.Id);
					if (existingSpace is null)
						throw new TfValidationException($"Space with id {space.Id} does not exist.");

					if (existingSpace.Roles.Any(x => x.Id == role.Id))
						continue;

					var success = _dboManager.Insert<SpaceRoleDbo>(
						new SpaceRoleDbo
						{
							RoleId = role.Id,
							SpaceId = space.Id
						});

					if (!success)
						throw new TfDboServiceException("Insert<SpaceRoleDbo> failed");

				}

				scope.Complete();
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}


	public void RemoveSpacesRole(
		List<TfSpace> spaces,
		TfRole role)
	{
		try
		{
			if (spaces == null)
				throw new ArgumentNullException(nameof(spaces));

			if (role == null)
				throw new ArgumentNullException(nameof(role));

			using (TfDatabaseTransactionScope scope = _dbService.CreateTransactionScope())
			{
				foreach (var space in spaces)
				{
					var existingSpace = GetSpace(space.Id);
					if (existingSpace is null)
						throw new TfValidationException($"Space with id {space.Id} does not exist.");

					if (existingSpace.Roles.Any(x => x.Id != role.Id))
						continue;

					var dbId = new Dictionary<string, Guid> {
						{ nameof(SpaceRoleDbo.SpaceId), space.Id },
						{ nameof(SpaceRoleDbo.RoleId), role.Id }};

					var success = _dboManager.Delete<SpaceRoleDbo>(dbId);

					if (!success)
						throw new TfDboServiceException("Delete<SpaceRoleDbo> failed");
				}

				scope.Complete();
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	private TfSpace ConvertDboToModel(
		TfSpaceDbo dbo)
	{
		if (dbo == null)
			return null;

		return new TfSpace
		{
			Id = dbo.Id,
			Name = dbo.Name,
			IsPrivate = dbo.IsPrivate,
			Position = dbo.Position,
			Color = (TfColor)dbo.Color,
			FluentIconName = dbo.Icon,
		};

	}

	private TfSpaceDbo ConvertModelToDbo(
		TfSpace model)
	{
		if (model == null)
			return null;

		return new TfSpaceDbo
		{
			Id = model.Id,
			Name = model.Name,
			IsPrivate = model.IsPrivate,
			Position = model.Position,
			Color = (short)model.Color,
			Icon = model.FluentIconName,
		};
	}


	#region <--- validation --->

	internal class TfSpaceValidator
	: AbstractValidator<TfSpace>
	{
		public TfSpaceValidator(
			ITfService tfService)
		{

			RuleSet("general", () =>
			{
				RuleFor(space => space.Id)
					.NotEmpty()
					.WithMessage("The space id is required.");

				RuleFor(space => space.Name)
					.NotEmpty()
					.WithMessage("The space name is required.");

			});

			RuleSet("create", () =>
			{
				RuleFor(space => space.Id)
						.Must((space, id) => { return tfService.GetSpace(id) == null; })
						.WithMessage("There is already existing space with specified identifier.");

				RuleFor(space => space.Name)
						.Must((space, name) =>
						{
							if (string.IsNullOrEmpty(name))
								return true;

							var spaces = tfService.GetSpacesList();
							return !spaces.Any(x => x.Name!.ToLowerInvariant().Trim() == name.ToLowerInvariant().Trim());
						})
						.WithMessage("There is already existing space with same name.");
			});

			RuleSet("update", () =>
			{
				RuleFor(space => space.Id)
						.Must((space, id) =>
						{
							return tfService.GetSpace(id) != null;
						})
						.WithMessage("There is not existing space with specified identifier.");

			});

			RuleSet("delete", () =>
			{
			});

		}

		public ValidationResult ValidateCreate(
			TfSpace space)
		{
			if (space == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The space is null.") });

			return this.Validate(space, options =>
			{
				options.IncludeRuleSets("general", "create");
			});
		}

		public ValidationResult ValidateUpdate(
			TfSpace space)
		{
			if (space == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The space is null.") });

			return this.Validate(space, options =>
			{
				options.IncludeRuleSets("general", "update");
			});
		}

		public ValidationResult ValidateDelete(
			TfSpace space)
		{
			if (space == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The space with specified identifier is not found.") });

			return this.Validate(space, options =>
			{
				options.IncludeRuleSets("delete");
			});
		}
	}

	#endregion
}
