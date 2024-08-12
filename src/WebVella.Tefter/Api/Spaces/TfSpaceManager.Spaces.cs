namespace WebVella.Tefter;

public partial interface ITfSpaceManager
{
	public Result<List<TfSpace>> GetSpacesList();

	public Result<TfSpace> GetSpace(
		Guid id);

	public Result<TfSpace> CreateSpace(
		TfSpace space);

	public Result<TfSpace> UpdateSpace(
		TfSpace space);

	public Result DeleteSpace(
		Guid id);

	public Result MoveSpaceUp(
		Guid id);

	public Result MoveSpaceDown(
		Guid id);
}

public partial class TfSpaceManager : ITfSpaceManager
{
	public Result<List<TfSpace>> GetSpacesList()
	{
		try
		{
			var orderSettings = new OrderSettings(
				nameof(TfSpace.Position),
				OrderDirection.ASC);

			var dbos = _dboManager.GetList<TfSpaceDbo>(
				order: orderSettings);

			return Result.Ok(dbos.Select(x => Convert(x)).ToList());
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to get list of spaces").CausedBy(ex));
		}
	}

	public Result<TfSpace> GetSpace(
		Guid id)
	{
		try
		{
			var dbo = _dboManager.Get<TfSpaceDbo>(id);
			return Result.Ok(Convert(dbo));
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to get space by id").CausedBy(ex));
		}
	}

	public Result<TfSpace> CreateSpace(
		TfSpace space)
	{
		try
		{
			if (space != null && space.Id == Guid.Empty)
				space.Id = Guid.NewGuid();

			TfSpaceValidator validator =
				new TfSpaceValidator(_dboManager, this);

			var validationResult = validator.ValidateCreate(space);

			if (!validationResult.IsValid)
				return validationResult.ToResult();

			using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var spaces = GetSpacesList().Value;

				//position is ignored - space is added at last place
				var dbo = Convert(space);
				dbo.Position = spaces.Count + 1;

				var success = _dboManager.Insert<TfSpaceDbo>(dbo);

				if (!success)
					return Result.Fail(new DboManagerError("Insert", space));

				scope.Complete();

				return GetSpace(space.Id);
			}
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to create space").CausedBy(ex));
		}
	}

	public Result<TfSpace> UpdateSpace(
		TfSpace space)
	{
		try
		{
			TfSpaceValidator validator =
				new TfSpaceValidator(_dboManager, this);

			var validationResult = validator.ValidateUpdate(space);

			if (!validationResult.IsValid)
				return validationResult.ToResult();

			var existingSpace = _dboManager.Get<TfSpace>(space.Id);

			//position is not updated
			var dbo = Convert(space);
			dbo.Position = existingSpace.Position;

			var success = _dboManager.Update<TfSpaceDbo>(dbo);

			if (!success)
				return Result.Fail(new DboManagerError("Update", space));

			return GetSpace(space.Id);
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to update space").CausedBy(ex));
		}

	}

	public Result MoveSpaceUp(
		Guid id)
	{
		try
		{
			using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var spaces = GetSpacesList().Value;

				var space = spaces.SingleOrDefault(x => x.Id == id);

				if (space == null)
					Result.Fail(new ValidationError(
						nameof(id),
						"Found no space for specified identifier."));

				if (space.Position == 1)
					Result.Ok();

				var prevSpace = spaces.Single(x => x.Position == (space.Position - 1));

				space.Position = space.Position - 1;
				prevSpace.Position = prevSpace.Position + 1;

				var success = _dboManager.Update<TfSpaceDbo>(Convert(space));

				if (!success)
					return Result.Fail(new DboManagerError("Update", space));

				success = _dboManager.Update<TfSpaceDbo>(Convert(prevSpace));

				if (!success)
					return Result.Fail(new DboManagerError("Update", space));

				scope.Complete();

				return Result.Ok();
			}
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to move space up in position").CausedBy(ex));
		}
	}

	public Result MoveSpaceDown(
		Guid id)
	{
		try
		{
			using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var spaces = GetSpacesList().Value;

				var space = spaces.Single(x => x.Id == id);

				if (space == null)
					Result.Fail(new ValidationError(
						nameof(id),
						"Found no space for specified identifier."));

				if (space.Position == spaces.Count)
					Result.Ok();

				var nextSpace = spaces.Single(x => x.Position == (space.Position + 1));

				space.Position = space.Position + 1;
				nextSpace.Position = nextSpace.Position - 1;

				var success = _dboManager.Update<TfSpaceDbo>(Convert(space));

				if (!success)
					return Result.Fail(new DboManagerError("Update", space));

				success = _dboManager.Update<TfSpaceDbo>(Convert(nextSpace));

				if (!success)
					return Result.Fail(new DboManagerError("Update", space));

				scope.Complete();

				return Result.Ok();
			}
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to move space down in position").CausedBy(ex));
		}
	}

	public Result DeleteSpace(
		Guid id)
	{
		try
		{
			using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				TfSpaceValidator validator =
					new TfSpaceValidator(_dboManager, this);

				var space = GetSpace(id).Value;

				var validationResult = validator.ValidateDelete(space);

				if (!validationResult.IsValid)
					return validationResult.ToResult();

				//TODO DELETE VIEWS WHEN IMPLEMENTED

				var spaceDataListResult = GetSpaceDataList(id);
				if (!spaceDataListResult.IsSuccess)
					return Result.Fail(new Error("Failed to get space data list.")
											.CausedBy(spaceDataListResult.Errors));

				foreach (var spaceData in spaceDataListResult.Value)
				{
					var result = DeleteSpaceData(spaceData.Id);
					if (!result.IsSuccess)
						return Result.Fail(new Error("Failed to delete space data.")
												.CausedBy(result.Errors));
				}

				var spacesAfter = GetSpacesList().Value.Where(x => x.Position > space.Position).ToList();

				//update positions for spaces after the one being deleted
				foreach (var spaceAfter in spacesAfter)
				{
					spaceAfter.Position--;
					var successUpdatePosition = _dboManager.Update<TfSpaceDbo>(Convert(spaceAfter));

					if (!successUpdatePosition)
						return Result.Fail(new DboManagerError("Failed to update space position during delete space process", spaceAfter));
				}

				var success = _dboManager.Delete<TfSpaceDbo>(id);

				if (!success)
					return Result.Fail(new DboManagerError("Delete", id));

				scope.Complete();

				return Result.Ok();
			}
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to delete space.").CausedBy(ex));
		}
	}

	private TfSpace Convert(
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
			Color = dbo.Color,
			Icon = dbo.Icon,
		};

	}

	private TfSpaceDbo Convert(
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
			Color = model.Color,
			Icon = model.Icon,
		};
	}


	#region <--- validation --->

	internal class TfSpaceValidator
	: AbstractValidator<TfSpace>
	{
		public TfSpaceValidator(
			IDboManager dboManager,
			ITfSpaceManager spaceManager)
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
						.Must((space, id) => { return spaceManager.GetSpace(id) == null; })
						.WithMessage("There is already existing space with specified identifier.");

				RuleFor(space => space.Name)
						.Must((space, name) =>
						{
							if (string.IsNullOrEmpty(name))
								return true;

							var spaces = spaceManager.GetSpacesList().Value;
							return !spaces.Any(x => x.Name.ToLowerInvariant().Trim() == name.ToLowerInvariant().Trim());
						})
						.WithMessage("There is already existing space with same name.");
			});

			RuleSet("update", () =>
			{
				RuleFor(space => space.Id)
						.Must((space, id) =>
						{
							return spaceManager.GetSpace(id).Value != null;
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
