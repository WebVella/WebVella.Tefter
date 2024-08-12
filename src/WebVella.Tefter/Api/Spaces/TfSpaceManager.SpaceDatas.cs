using WebVella.Tefter.Database.Dbo;
using static WebVella.Tefter.TfDataProviderManager;

namespace WebVella.Tefter;

public partial interface ITfSpaceManager
{
	public Result<List<TfSpaceData>> GetSpaceDataList(
		Guid spaceId);

	public Result<TfSpaceData> GetSpaceData(
		Guid id);

	public Result<TfSpaceData> CreateSpaceData(
		TfSpaceData spaceData);

	public Result<TfSpaceData> UpdateSpaceData(
		TfSpaceData spaceData);

	public Result DeleteSpaceData(
		Guid id);

	public Result MoveSpaceDataUp(
		Guid id);

	public Result MoveSpaceDataDown(
		Guid id);
}

public partial class TfSpaceManager : ITfSpaceManager
{
	public Result<List<TfSpaceData>> GetSpaceDataList(
		Guid spaceId)
	{
		try
		{
			var orderSettings = new OrderSettings(
				nameof(TfSpaceData.Position),
				OrderDirection.ASC);

			var dbos = _dboManager.GetList<TfSpaceDataDbo>(
				spaceId,
				nameof(TfSpaceData.SpaceId), order: orderSettings);

			return Result.Ok(dbos.Select(x => Convert(x)).ToList());
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to get list of space data list").CausedBy(ex));
		}
	}

	public Result<TfSpaceData> GetSpaceData(
		Guid id)
	{
		try
		{
			var dbo = _dboManager.Get<TfSpaceDataDbo>(id);
			return Result.Ok(Convert(dbo));
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to get space data by id").CausedBy(ex));
		}
	}

	public Result<TfSpaceData> CreateSpaceData(
		TfSpaceData spaceData)
	{
		try
		{
			using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				if (spaceData != null && spaceData.Id == Guid.Empty)
					spaceData.Id = Guid.NewGuid();

				TfSpaceDataValidator validator =
					new TfSpaceDataValidator(_dboManager, this);

				var validationResult = validator.ValidateCreate(spaceData);

				if (!validationResult.IsValid)
					return validationResult.ToResult();

				var spaceDataList = GetSpaceDataList(spaceData.SpaceId).Value;

				var dbo = Convert(spaceData);
				dbo.Position = (short)(spaceDataList.Count + 1);

				var success = _dboManager.Insert<TfSpaceDataDbo>(dbo);

				if (!success)
					return Result.Fail(new DboManagerError("Insert", spaceData));

				scope.Complete();

				return GetSpaceData(spaceData.Id);
			}
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to create space data").CausedBy(ex));
		}
	}

	public Result<TfSpaceData> UpdateSpaceData(
		TfSpaceData spaceData)
	{
		try
		{
			using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				TfSpaceDataValidator validator =
				new TfSpaceDataValidator(_dboManager, this);

				var validationResult = validator.ValidateUpdate(spaceData);

				if (!validationResult.IsValid)
					return validationResult.ToResult();

				var existingSpaceData = _dboManager.Get<TfSpaceDataDbo>(spaceData.Id);

				var dbo = Convert(spaceData);
				dbo.Position = existingSpaceData.Position;

				var success = _dboManager.Update<TfSpaceDataDbo>(dbo);

				if (!success)
					return Result.Fail(new DboManagerError("Update", spaceData));

				scope.Complete();

				return GetSpaceData(spaceData.Id);
			}
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to update space data").CausedBy(ex));
		}

	}

	public Result DeleteSpaceData(
		Guid id)
	{
		try
		{
			using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				TfSpaceDataValidator validator =
						new TfSpaceDataValidator(_dboManager, this);

				var spaceData = GetSpaceData(id).Value;

				var validationResult = validator.ValidateDelete(spaceData);

				if (!validationResult.IsValid)
					return validationResult.ToResult();

				var spaceDatasForSpace = GetSpaceDataList(spaceData.SpaceId).Value;

				//update positions for spaces after the one being deleted
				foreach (var sd in spaceDatasForSpace.Where(x => x.Position > spaceData.Position))
				{
					sd.Position--;
					var successUpdatePosition = _dboManager.Update<TfSpaceDataDbo>(Convert(sd));

					if (!successUpdatePosition)
						return Result.Fail(new DboManagerError("Failed to update space data position during delete space process", sd));
				}

				var success = _dboManager.Delete<TfSpaceDataDbo>(id);

				if (!success)
					return Result.Fail(new DboManagerError("Delete", id));

				scope.Complete();

				return Result.Ok();
			}
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to delete space data.").CausedBy(ex));
		}
	}


	public Result MoveSpaceDataUp(
		Guid id)
	{
		try
		{
			using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var spaceData = GetSpaceData(id).Value;
				if (spaceData == null)
					return Result.Fail(new ValidationError(
						nameof(id),
						"Found no space data for specified identifier."));

				var spaceDataList = GetSpaceDataList(spaceData.SpaceId).Value;

				if (spaceData.Position == 1)
					return Result.Ok();

				var prevSpaceData = spaceDataList.SingleOrDefault(x => x.Position == (spaceData.Position - 1));

				spaceData.Position = (short)(spaceData.Position - 1);

				if (prevSpaceData != null)
					prevSpaceData.Position = (short)(prevSpaceData.Position + 1);

				var success = _dboManager.Update<TfSpaceDataDbo>(Convert(spaceData));

				if (!success)
					return Result.Fail(new DboManagerError("Update", spaceData));

				if (prevSpaceData != null)
				{
					success = _dboManager.Update<TfSpaceDataDbo>(Convert(prevSpaceData));

					if (!success)
						return Result.Fail(new DboManagerError("Update", prevSpaceData));
				}

				scope.Complete();

				return Result.Ok();
			}
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to move space data up in position").CausedBy(ex));
		}
	}

	public Result MoveSpaceDataDown(
		Guid id)
	{
		try
		{
			using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var spaceData = GetSpaceData(id).Value;

				if (spaceData == null)
					return Result.Fail(new ValidationError(
						nameof(id),
						"Found no space data for specified identifier."));

				var spaceDataList = GetSpaceDataList(spaceData.SpaceId).Value;


				if (spaceData.Position == spaceDataList.Count)
					return Result.Ok();

				var nextSpaceData = spaceDataList.SingleOrDefault(x => x.Position == (spaceData.Position + 1));

				spaceData.Position = (short)(spaceData.Position + 1);

				if(nextSpaceData != null)
					nextSpaceData.Position = (short)(nextSpaceData.Position - 1);

				var success = _dboManager.Update<TfSpaceDataDbo>(Convert(spaceData));

				if (!success)
					return Result.Fail(new DboManagerError("Update", spaceData));

				if (nextSpaceData != null)
				{
					success = _dboManager.Update<TfSpaceDataDbo>(Convert(nextSpaceData));

					if (!success)
						return Result.Fail(new DboManagerError("Update", nextSpaceData));
				}

				scope.Complete();

				return Result.Ok();
			}
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to move space down in position").CausedBy(ex));
		}
	}

	private TfSpaceData Convert(TfSpaceDataDbo dbo)
	{
		if (dbo == null)
			return null;

		return new TfSpaceData
		{
			Id = dbo.Id,
			Name = dbo.Name,
			Position = dbo.Position,
			SpaceId = dbo.SpaceId,
			Filters = JsonSerializer.Deserialize<List<TfFilterBase>>(dbo.FiltersJson)
		};

	}

	private TfSpaceDataDbo Convert(TfSpaceData model)
	{
		if (model == null)
			return null;

		return new TfSpaceDataDbo
		{
			Id = model.Id,
			Name = model.Name,
			Position = model.Position,
			SpaceId = model.SpaceId,
			FiltersJson = JsonSerializer.Serialize(model.Filters ?? new List<TfFilterBase>())
		};
	}


	#region <--- validation --->

	internal class TfSpaceDataValidator
	: AbstractValidator<TfSpaceData>
	{
		public TfSpaceDataValidator(
			IDboManager dboManager,
			ITfSpaceManager spaceManager)
		{

			RuleSet("general", () =>
			{
				RuleFor(spaceData => spaceData.Id)
					.NotEmpty()
					.WithMessage("The space data id is required.");

				RuleFor(spaceData => spaceData.Name)
					.NotEmpty()
					.WithMessage("The space data name is required.");

				RuleFor(spaceData => spaceData.SpaceId)
					.NotEmpty()
					.WithMessage("The space id is required.");

				RuleFor(spaceData => spaceData.SpaceId)
					.Must(spaceId =>
					{
						return spaceManager.GetSpace(spaceId).Value != null;
					})
					.WithMessage("There is no existing space for specified space id.");

			});

			RuleSet("create", () =>
			{
				RuleFor(spaceData => spaceData.Id)
						.Must((spaceData, id) => { return spaceManager.GetSpaceData(id).Value == null; })
						.WithMessage("There is already existing space data with specified identifier.");

				RuleFor(spaceData => spaceData.Name)
						.Must((spaceData, name) =>
						{
							if (string.IsNullOrEmpty(name))
								return true;

							var spaceDataList = spaceManager.GetSpaceDataList(spaceData.SpaceId).Value;
							return !spaceDataList.Any(x => x.Name.ToLowerInvariant().Trim() == name.ToLowerInvariant().Trim());
						})
						.WithMessage("There is already existing space data with same name in same space.");
			});

			RuleSet("update", () =>
			{
				RuleFor(spaceData => spaceData.Id)
						.Must((spaceData, id) =>
						{
							return spaceManager.GetSpaceData(id).Value != null;
						})
						.WithMessage("There is not existing space data with specified identifier.");

			});

			RuleSet("delete", () =>
			{
			});

		}

		public ValidationResult ValidateCreate(
			TfSpaceData spaceData)
		{
			if (spaceData == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The space data is null.") });

			return this.Validate(spaceData, options =>
			{
				options.IncludeRuleSets("general", "create");
			});
		}

		public ValidationResult ValidateUpdate(
			TfSpaceData spaceData)
		{
			if (spaceData == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The space data is null.") });

			return this.Validate(spaceData, options =>
			{
				options.IncludeRuleSets("general", "update");
			});
		}

		public ValidationResult ValidateDelete(
			TfSpaceData spaceData)
		{
			if (spaceData == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The space data with specified identifier is not found.") });

			return this.Validate(spaceData, options =>
			{
				options.IncludeRuleSets("delete");
			});
		}
	}

	#endregion
}
