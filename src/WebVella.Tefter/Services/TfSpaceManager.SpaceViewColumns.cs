using WebVella.Tefter.Web.Models;

namespace WebVella.Tefter;

public partial interface ITfSpaceManager
{
	public ReadOnlyCollection<ITfSpaceViewColumnType> GetAvailableSpaceViewColumnTypes();

	public List<TfSpaceViewColumn> GetSpaceViewColumnsList(
		Guid spaceViewId);

	public TfSpaceViewColumn GetSpaceViewColumn(
		Guid id);

	public TfSpaceViewColumn CreateSpaceViewColumn(
		TfSpaceViewColumn spaceViewColumn);

	public TfSpaceViewColumn UpdateSpaceViewColumn(
		TfSpaceViewColumn spaceViewColumn);

	public void DeleteSpaceViewColumn(
		Guid id);

	public void MoveSpaceViewColumnUp(
		Guid id);

	public void MoveSpaceViewColumnDown(
		Guid id);
}

public partial class TfSpaceManager : ITfSpaceManager
{
	public ReadOnlyCollection<ITfSpaceViewColumnType> GetAvailableSpaceViewColumnTypes()
	{
		var meta = _metaProvider.GetSpaceViewColumnTypesMeta();
		return meta.Select(x => x.Instance).ToList().AsReadOnly();
	}


	public List<TfSpaceViewColumn> GetSpaceViewColumnsList(
		Guid spaceViewId)
	{
		var orderSettings = new TfOrderSettings(
			nameof(TfSpace.Position),
			OrderDirection.ASC);

		var spaceViewColumns = _dboManager.GetList<TfSpaceViewColumnDbo>(
			spaceViewId,
			nameof(TfSpaceViewColumn.SpaceViewId),
			order: orderSettings);

		return spaceViewColumns.Select(x => Convert(x)).ToList();
	}


	public TfSpaceViewColumn GetSpaceViewColumn(
		Guid id)
	{
		var spaceViewColumn = _dboManager.Get<TfSpaceViewColumnDbo>(id);
		return Convert(spaceViewColumn);
	}

	public TfSpaceViewColumn CreateSpaceViewColumn(
		TfSpaceViewColumn spaceViewColumn)
	{
		if (spaceViewColumn != null && spaceViewColumn.Id == Guid.Empty)
			spaceViewColumn.Id = Guid.NewGuid();

		new TfSpaceViewColumnValidator(_dboManager, this)
			.ValidateCreate(spaceViewColumn)
			.ToValidationException()
			.ThrowIfContainsErrors();

		using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
		{
			var spaceViewColumns = GetSpaceViewColumnsList(spaceViewColumn.SpaceViewId);

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
					throw new TfDboServiceException("Update<TfSpaceViewColumnDbo> failed");
			}


			var success = _dboManager.Insert<TfSpaceViewColumnDbo>(Convert(spaceViewColumn));
			if (!success)
				throw new TfDboServiceException("Insert<TfSpaceViewColumnDbo> failed");

			scope.Complete();

			return GetSpaceViewColumn(spaceViewColumn.Id);
		}
	}

	public TfSpaceViewColumn UpdateSpaceViewColumn(
		TfSpaceViewColumn spaceViewColumn)
	{
		new TfSpaceViewColumnValidator(_dboManager, this)
			.ValidateUpdate(spaceViewColumn)
			.ToValidationException()
			.ThrowIfContainsErrors();

		var spaceViewColumns = GetSpaceViewColumnsList(spaceViewColumn.SpaceViewId);

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
					throw new TfDboServiceException("Update<TfSpaceViewColumnDbo> failed");
			}

			//increment column position for columns after new position (move down)
			columnsAfter = spaceViewColumns
				.Where(x => x.Position >= spaceViewColumn.Position.Value).ToList();

			foreach (var columnAfter in columnsAfter)
			{
				columnAfter.Position++;

				var successUpdatePosition = _dboManager.Update<TfSpaceViewColumnDbo>(Convert(columnAfter));
				if (!successUpdatePosition)
					throw new TfDboServiceException("Update<TfSpaceViewColumnDbo> failed");
			}
		}


		var success = _dboManager.Update<TfSpaceViewColumnDbo>(Convert(spaceViewColumn));
		if (!success)
			throw new TfDboServiceException("Update<TfSpaceViewColumnDbo> failed");

		return GetSpaceViewColumn(spaceViewColumn.Id);
	}

	public void MoveSpaceViewColumnUp(
		Guid id)
	{
		using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
		{
			var spaceViewColumn = GetSpaceViewColumn(id);

			if (spaceViewColumn == null)
			{
				throw new TfValidationException(nameof(id),
					"Found no space view column for specified identifier.");
			}

			if (spaceViewColumn.Position == 1)
				return;

			var spaceViewColumns = GetSpaceViewColumnsList(spaceViewColumn.SpaceViewId);

			var prevSpaceViewColumn = spaceViewColumns.SingleOrDefault(x => x.Position == (spaceViewColumn.Position - 1));
			spaceViewColumn.Position = (short)(spaceViewColumn.Position - 1);

			if (prevSpaceViewColumn != null)
				prevSpaceViewColumn.Position = (short)(prevSpaceViewColumn.Position + 1);

			var success = _dboManager.Update<TfSpaceViewColumnDbo>(Convert(spaceViewColumn));

			if (!success)
				throw new TfDboServiceException("Update<TfSpaceViewColumnDbo> failed");

			if (prevSpaceViewColumn != null)
			{
				success = _dboManager.Update<TfSpaceViewColumnDbo>(Convert(prevSpaceViewColumn));

				if (!success)
					throw new TfDboServiceException("Update<TfSpaceViewColumnDbo> failed");
			}

			scope.Complete();
		}
	}

	public void MoveSpaceViewColumnDown(
		Guid id)
	{
		using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
		{


			var spaceViewColumn = GetSpaceViewColumn(id);

			if (spaceViewColumn == null)
			{
				throw new TfValidationException(nameof(id),
					"Found no space view column for specified identifier.");
			}

			var spaceViewColumns = GetSpaceViewColumnsList(spaceViewColumn.SpaceViewId);

			if (spaceViewColumn.Position == spaceViewColumns.Count)
				return;

			var nextSpaceViewColumn = spaceViewColumns.SingleOrDefault(x => x.Position == (spaceViewColumn.Position + 1));
			spaceViewColumn.Position = (short)(spaceViewColumn.Position + 1);

			if (nextSpaceViewColumn != null)
				nextSpaceViewColumn.Position = (short)(nextSpaceViewColumn.Position - 1);

			var success = _dboManager.Update<TfSpaceViewColumnDbo>(Convert(spaceViewColumn));
			if (!success)
				throw new TfDboServiceException("Update<TfSpaceViewColumnDbo> failed");

			if (nextSpaceViewColumn != null)
			{
				success = _dboManager.Update<TfSpaceViewColumnDbo>(Convert(nextSpaceViewColumn));
				if (!success)
					throw new TfDboServiceException("Update<TfSpaceViewColumnDbo> failed");
			}

			scope.Complete();
		}
	}

	public void DeleteSpaceViewColumn(
		Guid id)
	{
		using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
		{
			var spaceViewColumn = GetSpaceViewColumn(id);


			new TfSpaceViewColumnValidator(_dboManager, this)
				.ValidateDelete(spaceViewColumn)
				.ToValidationException()
				.ThrowIfContainsErrors();

			var columnsAfter = GetSpaceViewColumnsList(spaceViewColumn.SpaceViewId)
				.Where(x => x.Position > spaceViewColumn.Position).ToList();

			//update positions for space view columns after the one being deleted
			foreach (var columnAfter in columnsAfter)
			{
				columnAfter.Position--;
				var successUpdatePosition = _dboManager.Update<TfSpaceViewColumnDbo>(Convert(columnAfter));
				if (!successUpdatePosition)
					throw new TfDboServiceException("Update<TfSpaceViewColumnDbo> failed");
			}

			var success = _dboManager.Delete<TfSpaceViewColumnDbo>(id);

			if (!success)
				throw new TfDboServiceException("Delete<TfSpaceViewColumnDbo> failed");

			scope.Complete();
		}
	}

	private TfSpaceViewColumn Convert(TfSpaceViewColumnDbo dbo)
	{
		if (dbo == null)
			return null;

		ITfSpaceViewColumnType columnType = _metaProvider.GetSpaceViewColumnTypeByName(dbo.FullTypeName);
		Type componentType = _metaProvider.GetSpaceViewColumnComponentType(dbo.FullComponentTypeName);

		return new TfSpaceViewColumn
		{
			Id = dbo.Id,
			SpaceViewId = dbo.SpaceViewId,
			QueryName = dbo.QueryName,
			Title = dbo.Title,
			Icon = dbo.Icon ?? string.Empty,
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
			Icon = model.Icon ?? string.Empty,
			OnlyIcon = model.OnlyIcon,
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
						.Must((column, id) => { return spaceManager.GetSpaceViewColumn(id) == null; })
						.WithMessage("There is already existing space view column with specified identifier.");

				RuleFor(column => column.QueryName)
						.Must((column, queryName) =>
						{
							if (string.IsNullOrEmpty(queryName))
								return true;

							var spaceViewColumns = spaceManager.GetSpaceViewColumnsList(column.SpaceViewId);
							return !spaceViewColumns.Any(x => x.QueryName.ToLowerInvariant().Trim() == queryName.ToLowerInvariant().Trim());
						})
						.WithMessage("There is already existing space view column with same query name.");


			});

			RuleSet("update", () =>
			{
				RuleFor(column => column.Id)
						.Must((column, id) =>
						{
							return spaceManager.GetSpaceViewColumn(id) != null;
						})
						.WithMessage("There is not existing space view column with specified identifier.");

				RuleFor(column => column.QueryName)
						.Must((column, queryName) =>
						{
							if (string.IsNullOrEmpty(queryName))
								return true;

							var spaceViewColumns = spaceManager.GetSpaceViewColumnsList(column.SpaceViewId);
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
