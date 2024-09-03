namespace WebVella.Tefter;

public partial interface ITfSpaceManager
{
	public Result<ReadOnlyCollection<ITfSpaceViewColumnType>> GetAvailableSpaceViewColumnTypes();

	public Result<List<TfSpaceViewColumn>> GetSpaceViewColumnsList(
		Guid spaceViewId);

	public Result<TfSpaceViewColumn> GetSpaceViewColumn(
		Guid id);

	public Result<TfSpaceViewColumn> CreateSpaceViewColumn(
		TfSpaceViewColumn spaceViewColumn);

	public Result<TfSpaceViewColumn> UpdateSpaceViewColumn(
		TfSpaceViewColumn spaceViewColumn);

	public Result DeleteSpaceViewColumn(
		Guid id);

	public Result MoveSpaceViewColumnUp(
		Guid id);

	public Result MoveSpaceViewColumnDown(
		Guid id);
}

public partial class TfSpaceManager : ITfSpaceManager
{
	public Result<ReadOnlyCollection<ITfSpaceViewColumnType>> GetAvailableSpaceViewColumnTypes()
	{
		try
		{
			return Result.Ok(_spaceViewColumnTypes.AsReadOnly());
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to get list of available space view column types").CausedBy(ex));
		}
	}


	public Result<List<TfSpaceViewColumn>> GetSpaceViewColumnsList(
		Guid spaceViewId)
	{
		try
		{
			var orderSettings = new OrderSettings(
				nameof(TfSpace.Position),
				OrderDirection.ASC);

			var spaceViewColumns = _dboManager.GetList<TfSpaceViewColumnDbo>(
				spaceViewId,
				nameof(TfSpaceViewColumn.SpaceViewId),
				order: orderSettings);

			return Result.Ok(spaceViewColumns.Select(x => Convert(x)).ToList());
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to get list of space view columns").CausedBy(ex));
		}
	}


	public Result<TfSpaceViewColumn> GetSpaceViewColumn(
		Guid id)
	{
		try
		{
			var spaceViewColumn = _dboManager.Get<TfSpaceViewColumnDbo>(id);
			return Result.Ok(Convert(spaceViewColumn));
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to get space view by id").CausedBy(ex));
		}
	}

	public Result<TfSpaceViewColumn> CreateSpaceViewColumn(
		TfSpaceViewColumn spaceViewColumn)
	{
		try
		{
			if (spaceViewColumn != null && spaceViewColumn.Id == Guid.Empty)
				spaceViewColumn.Id = Guid.NewGuid();

			//TfSpaceViewValidator validator =
			//	new TfSpaceViewValidator(_dboManager, this);

			//var validationResult = validator.ValidateCreate(spaceView);

			//if (!validationResult.IsValid)
			//	return validationResult.ToResult();

			using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var spaceViewColumns = GetSpaceViewColumnsList(spaceViewColumn.SpaceViewId).Value;

				//position is ignored - space view column is added at last place
				spaceViewColumn.Position = (short)(spaceViewColumns.Count + 1);

				var success = _dboManager.Insert<TfSpaceViewColumnDbo>(Convert(spaceViewColumn));

				if (!success)
					return Result.Fail(new DboManagerError("Insert", spaceViewColumn));

				scope.Complete();

				return GetSpaceViewColumn(spaceViewColumn.Id);
			}
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to create space view column").CausedBy(ex));
		}
	}

	public Result<TfSpaceViewColumn> UpdateSpaceViewColumn(
		TfSpaceViewColumn spaceViewColumn)
	{
		try
		{
			//TfSpaceViewValidator validator =
			//	new TfSpaceViewValidator(_dboManager, this);

			//var validationResult = validator.ValidateUpdate(spaceViewColumn);

			//if (!validationResult.IsValid)
			//	return validationResult.ToResult();

			var existingSpaceView = _dboManager.Get<TfSpaceViewColumn>(spaceViewColumn.Id);

			//position is not updated
			spaceViewColumn.Position = existingSpaceView.Position;

			var success = _dboManager.Update<TfSpaceViewColumnDbo>(Convert(spaceViewColumn));

			if (!success)
				return Result.Fail(new DboManagerError("Update", spaceViewColumn));

			return GetSpaceViewColumn(spaceViewColumn.Id);
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to update space column").CausedBy(ex));
		}

	}

	public Result MoveSpaceViewColumnUp(
		Guid id)
	{
		try
		{
			using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var spaceViewColumn = GetSpaceViewColumn(id).Value;

				if (spaceViewColumn == null)
					return Result.Fail(new ValidationError(
						nameof(id),
						"Found no space view column for specified identifier."));

				if (spaceViewColumn.Position == 1)
					return Result.Ok();

				var spaceViewColumns = GetSpaceViewColumnsList(spaceViewColumn.SpaceViewId).Value;

				var prevSpaceViewColumn = spaceViewColumns.SingleOrDefault(x => x.Position == (spaceViewColumn.Position - 1));
				spaceViewColumn.Position = (short)(spaceViewColumn.Position - 1);

				if (prevSpaceViewColumn != null)
					prevSpaceViewColumn.Position = (short)(prevSpaceViewColumn.Position + 1);

				var success = _dboManager.Update<TfSpaceViewColumnDbo>(Convert(spaceViewColumn));

				if (!success)
					return Result.Fail(new DboManagerError("Update", spaceViewColumn));

				if (prevSpaceViewColumn != null)
				{
					success = _dboManager.Update<TfSpaceViewColumnDbo>(Convert(prevSpaceViewColumn));

					if (!success)
						return Result.Fail(new DboManagerError("Update", prevSpaceViewColumn));
				}

				scope.Complete();

				return Result.Ok();
			}
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to move space view column up in position").CausedBy(ex));
		}
	}

	public Result MoveSpaceViewColumnDown(
		Guid id)
	{
		try
		{
			using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{


				var spaceViewColumn = GetSpaceViewColumn(id).Value;

				if (spaceViewColumn == null)
					return Result.Fail(new ValidationError(
						nameof(id),
						"Found no space view column for specified identifier."));

				var spaceViewColumns = GetSpaceViewColumnsList(spaceViewColumn.SpaceViewId).Value;

				if (spaceViewColumn.Position == spaceViewColumns.Count)
					return Result.Ok();

				var nextSpaceViewColumn = spaceViewColumns.SingleOrDefault(x => x.Position == (spaceViewColumn.Position + 1));
				spaceViewColumn.Position = (short)(spaceViewColumn.Position + 1);

				if (nextSpaceViewColumn != null)
					nextSpaceViewColumn.Position = (short)(nextSpaceViewColumn.Position - 1);

				var success = _dboManager.Update<TfSpaceViewColumnDbo>(Convert(spaceViewColumn));

				if (!success)
					return Result.Fail(new DboManagerError("Update", spaceViewColumn));

				if (nextSpaceViewColumn != null)
				{
					success = _dboManager.Update<TfSpaceViewColumnDbo>(Convert(nextSpaceViewColumn));

					if (!success)
						return Result.Fail(new DboManagerError("Update", nextSpaceViewColumn));
				}

				scope.Complete();

				return Result.Ok();
			}
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to move space view column down in position").CausedBy(ex));
		}
	}

	public Result DeleteSpaceViewColumn(
		Guid id)
	{
		try
		{
			using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				//TfSpaceViewValidator validator =
				//	new TfSpaceViewValidator(_dboManager, this);

				var spaceViewColumn = GetSpaceViewColumn(id).Value;

				//var validationResult = validator.ValidateDelete(spaceView);

				//if (!validationResult.IsValid)
				//	return validationResult.ToResult();


				var columnsAfter = GetSpaceViewColumnsList(spaceViewColumn.SpaceViewId)
					.Value
					.Where(x => x.Position > spaceViewColumn.Position).ToList();

				//update positions for space view columns after the one being deleted
				foreach (var columnAfter in columnsAfter)
				{
					columnAfter.Position--;
					var successUpdatePosition = _dboManager.Update<TfSpaceViewColumnDbo>(Convert(columnAfter));

					if (!successUpdatePosition)
						return Result.Fail(new DboManagerError("Failed to update space view column position" +
							" during delete space view column process", columnAfter));
				}

				var success = _dboManager.Delete<TfSpaceViewColumnDbo>(id);

				if (!success)
					return Result.Fail(new DboManagerError("Delete", id));

				scope.Complete();

				return Result.Ok();
			}
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to delete space view column").CausedBy(ex));
		}
	}

	private TfSpaceViewColumn Convert(TfSpaceViewColumnDbo dbo)
	{
		if (dbo == null)
			return null;

		ITfSpaceViewColumnType columnType = GetSpaceViewColumnTypeByName(dbo.FullTypeName);
		Type componentType = GetSpaceViewColumnComponentType(dbo.FullComponentTypeName);

		return new TfSpaceViewColumn
		{
			Id = dbo.Id,
			SpaceViewId = dbo.SpaceViewId,
			QueryName = dbo.QueryName,
			Title = dbo.Title,
			Position = dbo.Position,
			ColumnType = columnType,
			ComponentType = componentType,
			DataMapping = JsonSerializer.Deserialize<Dictionary<string, string>>(dbo.DataMappingJson),
			CustomOptionsJson = dbo.CustomOptionsJson,
		};

	}

	private TfSpaceViewColumnDbo Convert(TfSpaceViewColumn model)
	{
		if (model == null)
			return null;

		return new TfSpaceViewColumnDbo
		{
			Id = model.Id,
			SpaceViewId = model.SpaceViewId,
			QueryName = model.QueryName,
			Title = model.Title,
			Position = model.Position,
			FullTypeName = model.ColumnType.GetType().FullName,
			FullComponentTypeName = model.ComponentType.FullName,
			DataMappingJson = JsonSerializer.Serialize(model.DataMapping ?? new Dictionary<string, string>()),
			CustomOptionsJson = model.CustomOptionsJson,
		};
	}

	//#region <--- validation --->

	//internal class TfSpaceViewValidator
	//: AbstractValidator<TfSpaceView>
	//{
	//	public TfSpaceViewValidator(
	//		IDboManager dboManager,
	//		ITfSpaceManager spaceManager)
	//	{

	//		RuleSet("general", () =>
	//		{
	//			RuleFor(spaceView => spaceView.Id)
	//				.NotEmpty()
	//				.WithMessage("The space view id is required.");

	//			RuleFor(spaceView => spaceView.Name)
	//				.NotEmpty()
	//				.WithMessage("The space view name is required.");

	//			RuleFor(spaceView => spaceView.SpaceId)
	//				.NotEmpty()
	//				.WithMessage("The space id is required.");

	//			RuleFor(spaceView => spaceView.SpaceDataId)
	//				.NotEmpty()
	//				.WithMessage("The space data id is required.");

	//			//TODO rumen more validation about space data - SpaceId and SpaceId in space view

	//		});

	//		RuleSet("create", () =>
	//		{
	//			RuleFor(spaceView => spaceView.Id)
	//					.Must((spaceView, id) => { return spaceManager.GetSpaceView(id).Value == null; })
	//					.WithMessage("There is already existing space view with specified identifier.");

	//			RuleFor(spaceView => spaceView.Name)
	//					.Must((spaceView, name) =>
	//					{
	//						if (string.IsNullOrEmpty(name))
	//							return true;

	//						var spaceViews = spaceManager.GetSpaceViewsList(spaceView.SpaceId).Value;
	//						return !spaceViews.Any(x => x.Name.ToLowerInvariant().Trim() == name.ToLowerInvariant().Trim());
	//					})
	//					.WithMessage("There is already existing space view with same name.");


	//		});

	//		RuleSet("update", () =>
	//		{
	//			RuleFor(spaceView => spaceView.Id)
	//					.Must((spaceView, id) =>
	//					{
	//						return spaceManager.GetSpaceView(id).Value != null;
	//					})
	//					.WithMessage("There is not existing space view with specified identifier.");


	//			//TODO rumen more validation about SpaceId and SpaceDataId changes


	//		});

	//		RuleSet("delete", () =>
	//		{
	//		});

	//	}

	//	public ValidationResult ValidateCreate(
	//		TfSpaceView spaceView)
	//	{
	//		if (spaceView == null)
	//			return new ValidationResult(new[] { new ValidationFailure("",
	//				"The space view is null.") });

	//		return this.Validate(spaceView, options =>
	//		{
	//			options.IncludeRuleSets("general", "create");
	//		});
	//	}

	//	public ValidationResult ValidateUpdate(
	//		TfSpaceView spaceView)
	//	{
	//		if (spaceView == null)
	//			return new ValidationResult(new[] { new ValidationFailure("",
	//				"The space view is null.") });

	//		return this.Validate(spaceView, options =>
	//		{
	//			options.IncludeRuleSets("general", "update");
	//		});
	//	}

	//	public ValidationResult ValidateDelete(
	//		TfSpaceView spaceView)
	//	{
	//		if (spaceView == null)
	//			return new ValidationResult(new[] { new ValidationFailure("",
	//				"The space view with specified identifier is not found.") });

	//		return this.Validate(spaceView, options =>
	//		{
	//			options.IncludeRuleSets("delete");
	//		});
	//	}
	//}

	//#endregion

}
