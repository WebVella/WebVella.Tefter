using WebVella.Tefter.Web.Models;

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
			var orderSettings = new TfOrderSettings(
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

			TfSpaceViewColumnValidator validator =
				new TfSpaceViewColumnValidator(_dboManager, this);

			var validationResult = validator.ValidateCreate(spaceViewColumn);

			if (!validationResult.IsValid)
				return validationResult.ToResult();

			using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var spaceViewColumns = GetSpaceViewColumnsList(spaceViewColumn.SpaceViewId).Value;

				if (spaceViewColumn.Position == null)
				{
					spaceViewColumn.Position = (short)(spaceViewColumns.Count + 1);
				}
				else if (spaceViewColumn.Position.Value > (spaceViewColumns.Count + 1))
				{
					spaceViewColumn.Position = (short)(spaceViewColumns.Count + 1);
				}
				else if (spaceViewColumn.Position.Value < 0)
				{
					spaceViewColumn.Position = 1;
				}

				//increment column position for columns for same and after new position
				var columnsAfter = spaceViewColumns
					.Where(x => x.Position >= spaceViewColumn.Position).ToList();

				foreach (var columnAfter in columnsAfter)
				{
					columnAfter.Position++;

					var successUpdatePosition = _dboManager.Update<TfSpaceViewColumnDbo>(Convert(columnAfter));

					if (!successUpdatePosition)
						return Result.Fail(new DboManagerError("Failed to update space view column position" +
							" during create space view column process", columnAfter));
				}


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
			TfSpaceViewColumnValidator validator =
				new TfSpaceViewColumnValidator(_dboManager, this);

			var validationResult = validator.ValidateUpdate(spaceViewColumn);

			if (!validationResult.IsValid)
				return validationResult.ToResult();

			var spaceViewColumns = GetSpaceViewColumnsList(spaceViewColumn.SpaceViewId).Value;

			var existingSpaceView = spaceViewColumns.Single(x => x.Id == spaceViewColumn.Id);

			if (spaceViewColumn.Position == null)
			{
				spaceViewColumn.Position = existingSpaceView.Position;
			}
			else if (spaceViewColumn.Position.Value > spaceViewColumns.Count)
			{
				spaceViewColumn.Position = (short)(spaceViewColumns.Count);
			}
			else if (spaceViewColumn.Position.Value < 0)
			{
				spaceViewColumn.Position = 1;
			}

			if (spaceViewColumn.Position.Value != existingSpaceView.Position)
			{

				//decrement column position for columns after old position (move up)
				var columnsAfter = spaceViewColumns
					.Where(x => x.Position > existingSpaceView.Position).ToList();

				foreach (var columnAfter in columnsAfter)
				{
					columnAfter.Position--;

					var successUpdatePosition = _dboManager.Update<TfSpaceViewColumnDbo>(Convert(columnAfter));

					if (!successUpdatePosition)
						return Result.Fail(new DboManagerError("Failed to update space view column position" +
							" during update space view column process", columnAfter));
				}

				//increment column position for columns after new position (move down)
				columnsAfter = spaceViewColumns
					.Where(x => x.Position >= spaceViewColumn.Position.Value).ToList();

				foreach (var columnAfter in columnsAfter)
				{
					columnAfter.Position++;

					var successUpdatePosition = _dboManager.Update<TfSpaceViewColumnDbo>(Convert(columnAfter));

					if (!successUpdatePosition)
						return Result.Fail(new DboManagerError("Failed to update space view column position" +
							" during update space view column process", columnAfter));
				}
			}


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
				TfSpaceViewColumnValidator validator =
					new TfSpaceViewColumnValidator(_dboManager, this);

				var spaceViewColumn = GetSpaceViewColumn(id).Value;

				var validationResult = validator.ValidateDelete(spaceViewColumn);

				if (!validationResult.IsValid)
					return validationResult.ToResult();


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
			Icon = dbo.Icon,
			OnlyIcon = dbo.OnlyIcon,
			Position = dbo.Position,
			ColumnType = columnType,
			ComponentType = componentType,
			DataMapping = JsonSerializer.Deserialize<Dictionary<string, string>>(dbo.DataMappingJson),
			CustomOptionsJson = dbo.CustomOptionsJson,
			SettingsJson = dbo.SettingsJson,
			FullTypeName = dbo.FullTypeName,
			FullComponentTypeName = dbo.FullComponentTypeName
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
			Icon = model.Icon,
			OnlyIcon= model.OnlyIcon,
			Position = model.Position.Value,
			FullTypeName = model.ColumnType.GetType().FullName,
			FullComponentTypeName = model.ComponentType.FullName,
			DataMappingJson = JsonSerializer.Serialize(model.DataMapping ?? new Dictionary<string, string>()),
			CustomOptionsJson = model.CustomOptionsJson,
			SettingsJson = model.SettingsJson,
		};
	}

	#region <--- validation --->

	internal class TfSpaceViewColumnValidator
	: AbstractValidator<TfSpaceViewColumn>
	{
		public TfSpaceViewColumnValidator(
			ITfDboManager dboManager,
			ITfSpaceManager spaceManager)
		{

			RuleSet("general", () =>
			{
				RuleFor(column => column.Id)
					.NotEmpty()
					.WithMessage("The column id is required.");

				RuleFor(column => column.QueryName)
					.NotEmpty()
					.WithMessage("The query name is required.");

				RuleFor(column => column.Title)
					.NotEmpty()
					.WithMessage("The query name is required.");

				RuleFor(column => column.ColumnType)
					.NotEmpty()
					.WithMessage("The column type is required.");

				RuleFor(column => column.ComponentType)
					.NotEmpty()
					.WithMessage("The column component type is required.");

				RuleFor(column => column.SpaceViewId)
					.NotEmpty()
					.WithMessage("The space view id is required.");

				RuleFor(column => column.QueryName)
				.Must((column, QueryName) =>
				{
					if (string.IsNullOrWhiteSpace(QueryName))
						return true;

					return QueryName.Length >= Constants.DB_MIN_OBJECT_NAME_LENGTH;
				})
				.WithMessage($"The query name must be at least {Constants.DB_MIN_OBJECT_NAME_LENGTH} characters long.");

				RuleFor(column => column.QueryName)
					.Must((column, QueryName) =>
					{
						if (string.IsNullOrWhiteSpace(QueryName))
							return true;

						return QueryName.Length <= Constants.DB_MAX_OBJECT_NAME_LENGTH;
					})
					.WithMessage($"The length of query name must be less or equal than {Constants.DB_MAX_OBJECT_NAME_LENGTH} characters");

				RuleFor(column => column.QueryName)
					.Must((column, QueryName) =>
					{
						if (string.IsNullOrWhiteSpace(QueryName))
							return true;

						//other validation will trigger
						if (QueryName.Length < Constants.DB_MIN_OBJECT_NAME_LENGTH)
							return true;

						//other validation will trigger
						if (QueryName.Length > Constants.DB_MAX_OBJECT_NAME_LENGTH)
							return true;

						Match match = Regex.Match(QueryName, Constants.DB_OBJECT_NAME_VALIDATION_PATTERN);
						return match.Success && match.Value == QueryName.Trim();
					})
					.WithMessage($"The query name can only contains underscores and lowercase alphanumeric characters. It must beggin with a letter, " +
						$"not include spaces, not end with an underscore, and not contain two consecutive underscores");

			});

			RuleSet("create", () =>
			{
				RuleFor(column => column.Id)
						.Must((column, id) => { return spaceManager.GetSpaceViewColumn(id).Value == null; })
						.WithMessage("There is already existing space view column with specified identifier.");

				RuleFor(column => column.QueryName)
						.Must((column, queryName) =>
						{
							if (string.IsNullOrEmpty(queryName))
								return true;

							var spaceViewColumns = spaceManager.GetSpaceViewColumnsList(column.SpaceViewId).Value;
							return !spaceViewColumns.Any(x => x.QueryName.ToLowerInvariant().Trim() == queryName.ToLowerInvariant().Trim());
						})
						.WithMessage("There is already existing space view column with same query name.");


			});

			RuleSet("update", () =>
			{
				RuleFor(column => column.Id)
						.Must((column, id) =>
						{
							return spaceManager.GetSpaceViewColumn(id).Value != null;
						})
						.WithMessage("There is not existing space view column with specified identifier.");

				RuleFor(column => column.QueryName)
						.Must((column, queryName) =>
						{
							if (string.IsNullOrEmpty(queryName))
								return true;

							var spaceViewColumns = spaceManager.GetSpaceViewColumnsList(column.SpaceViewId).Value;
							return !spaceViewColumns.Any(x => x.QueryName.ToLowerInvariant().Trim() == queryName.ToLowerInvariant().Trim() && column.Id != x.Id);
						})
						.WithMessage("There is already another space view column with same query name inside same view.");

			});

			RuleSet("delete", () =>
			{
			});

		}

		public ValidationResult ValidateCreate(
			TfSpaceViewColumn spaceViewColumn)
		{
			if (spaceViewColumn == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The space view column is null.") });

			return this.Validate(spaceViewColumn, options =>
			{
				options.IncludeRuleSets("general", "create");
			});
		}

		public ValidationResult ValidateUpdate(
			TfSpaceViewColumn spaceViewColumn)
		{
			if (spaceViewColumn == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The space view column is null.") });

			return this.Validate(spaceViewColumn, options =>
			{
				options.IncludeRuleSets("general", "update");
			});
		}

		public ValidationResult ValidateDelete(
			TfSpaceViewColumn spaceViewColumn)
		{
			if (spaceViewColumn == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The space view column with specified identifier is not found.") });

			return this.Validate(spaceViewColumn, options =>
			{
				options.IncludeRuleSets("delete");
			});
		}
	}

	#endregion

}
