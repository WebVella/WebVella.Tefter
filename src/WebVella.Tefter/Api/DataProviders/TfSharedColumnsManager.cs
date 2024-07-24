namespace WebVella.Tefter.Api;

public partial interface ITfSharedColumnsManager
{
	Result<TfSharedColumn> GetSharedColumn(
		Guid id);

	Result<List<TfSharedColumn>> GetSharedColumns();

	Result CreateSharedColumn(
	   TfSharedColumn column);

	Result UpdateSharedColumn(
		TfSharedColumn column);

	Result DeleteSharedColumn(
		Guid columnId);
}

public partial class TfSharedColumnsManager : ITfSharedColumnsManager
{
	private readonly IDatabaseService _dbService;
	private readonly IDboManager _dboManager;

	public TfSharedColumnsManager(
		IServiceProvider serviceProvider,
		IDatabaseService dbService)
	{
		_dbService = dbService;
		_dboManager = serviceProvider.GetService<IDboManager>();
	}

	public Result<TfSharedColumn> GetSharedColumn(
		Guid id)
	{
		try
		{
			var sharedColumn = _dboManager.Get<TfSharedColumn>(id);

			return Result.Ok(sharedColumn);
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to get shared column").CausedBy(ex));
		}

		
	}

	public Result<List<TfSharedColumn>> GetSharedColumns()
	{
		try
		{
			var orderSettings = new OrderSettings(nameof(TfDataProviderColumn.DbName), OrderDirection.ASC);

			var sharedColumns = _dboManager.GetList<TfSharedColumn>(order: orderSettings);

			return Result.Ok(sharedColumns);
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to get shared columns").CausedBy(ex));
		}
		
	}

	public Result CreateSharedColumn(
		TfSharedColumn column)
	{
		try
		{
			if (column != null && column.Id == Guid.Empty)
				column.Id = Guid.NewGuid();

			TfSharedColumnValidator validator =
				new TfSharedColumnValidator(_dboManager, this);

			var validationResult = validator.ValidateCreate(column);

			if (!validationResult.IsValid)
				return validationResult.ToResult();

			using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{

				var success = _dboManager.Insert<TfSharedColumn>(column);

				if (!success)
					return Result.Fail(new DboManagerError("Insert", column));

				scope.Complete();

				return Result.Ok();
			}
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to create new shared column.").CausedBy(ex));
		}
	}

	public Result UpdateSharedColumn(
		TfSharedColumn column)
	{
		try
		{
			if (column != null && column.Id == Guid.Empty)
				column.Id = Guid.NewGuid();

			TfSharedColumnValidator validator =
				new TfSharedColumnValidator(_dboManager, this);

			var validationResult = validator.ValidateUpdate(column);

			if (!validationResult.IsValid)
				return validationResult.ToResult();

			using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{

				var success = _dboManager.Update<TfSharedColumn>(column);

				if (!success)
					return Result.Fail(new DboManagerError("Update", column));

				scope.Complete();

				return Result.Ok();
			}
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to update shared column.").CausedBy(ex));
		}
	}

	public Result DeleteSharedColumn(
		Guid columnId)
	{
		try
		{

			using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{

				var success = _dboManager.Delete<TfSharedColumn>(columnId);

				if (!success)
					return Result.Fail(new DboManagerError("Delete", columnId));

				scope.Complete();

				return Result.Ok();
			}
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to delete new shared column.").CausedBy(ex));
		}
	}

	#region <--- validation --->

	internal class TfSharedColumnValidator
		: AbstractValidator<TfSharedColumn>
	{
		public TfSharedColumnValidator(
			IDboManager dboManager,
			ITfSharedColumnsManager sharedColumnManager)
		{
			RuleSet("general", () =>
			{
				RuleFor(column => column.Id)
					.NotEmpty()
					.WithMessage("The shared column id is required.");

				RuleFor(column => column.SharedKeyDbName)
				.NotEmpty()
				.WithMessage("The shared column associated shared key database name is required.");

				RuleFor(column => column.DbName)
					.NotEmpty()
					.WithMessage("The data provider column database name is required.");

				RuleFor(column => column.DbName)
					.Must((column, dbName) =>
					{
						if (string.IsNullOrWhiteSpace(dbName))
							return true;

						return !dbName.StartsWith("sk_");
					})
					.WithMessage("The shared column database name cannot start with 'tf_'.");

				RuleFor(column => column.DbName)
					.Must((column, dbName) =>
					{
						if (string.IsNullOrWhiteSpace(dbName))
							return true;

						return dbName.Length >= Constants.DB_MIN_OBJECT_NAME_LENGTH;
					})
					.WithMessage($"The shared column database name must be at least {Constants.DB_MIN_OBJECT_NAME_LENGTH} characters long.");

				RuleFor(column => column.DbName)
					.Must((column, dbName) =>
					{
						if (string.IsNullOrWhiteSpace(dbName))
							return true;

						return dbName.Length <= Constants.DB_MAX_OBJECT_NAME_LENGTH;
					})
					.WithMessage($"The length of shared column database name must be less or equal than {Constants.DB_MAX_OBJECT_NAME_LENGTH} characters");

				RuleFor(column => column.DbName)
					.Must((column, dbName) =>
					{
						if (string.IsNullOrWhiteSpace(dbName))
							return true;

						//other validation will trigger
						if (dbName.Length < Constants.DB_MIN_OBJECT_NAME_LENGTH)
							return true;

						//other validation will trigger
						if (dbName.Length > Constants.DB_MAX_OBJECT_NAME_LENGTH)
							return true;

						Match match = Regex.Match(dbName, Constants.DB_OBJECT_NAME_VALIDATION_PATTERN);
						return match.Success && match.Value == dbName.Trim();
					})
					.WithMessage($"The shared column database name can only contains underscores and lowercase alphanumeric characters. It must begin with a letter, " +
						$"not include spaces, not end with an underscore, and not contain two consecutive underscores");

			});

			RuleSet("create", () =>
			{
				RuleFor(column => column.Id)
						.Must((column, id) => { return sharedColumnManager.GetSharedColumn(id).Value == null; })
						.WithMessage("There is already existing data shared column with specified identifier.");

				RuleFor(column => column.DbName)
						.Must((column, dbName) =>
						{
							if (string.IsNullOrEmpty(dbName))
								return true;

							var columns = sharedColumnManager.GetSharedColumns().Value;
							return !columns.Any(x => x.DbName.ToLowerInvariant().Trim() == dbName.ToLowerInvariant().Trim());
						})
						.WithMessage("There is already existing shared column with specified database name.");
			});

			RuleSet("update", () =>
			{
				RuleFor(column => column.Id)
						.Must((column, id) =>
						{
							return sharedColumnManager.GetSharedColumn(id).Value != null;
						})
						.WithMessage("There is not existing data shared column with specified identifier.");

				RuleFor(column => column.DbName)
						.Must((column, dbName) =>
						{
							if (string.IsNullOrEmpty(dbName))
								return true;

							var columns = sharedColumnManager.GetSharedColumns().Value;
							return !columns.Any(x => x.DbName.ToLowerInvariant().Trim() == dbName.ToLowerInvariant().Trim() && x.Id != column.Id);
						})
						.WithMessage("There is already existing shared column with specified database name.");
			});
		}

		public ValidationResult ValidateCreate(
			TfSharedColumn column)
		{
			if (column == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The shared column object is null.") });

			return this.Validate(column, options =>
			{
				options.IncludeRuleSets("general", "create");
			});
		}

		public ValidationResult ValidateUpdate(
			TfSharedColumn column)
		{
			if (column == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The shared column object is null.") });

			return this.Validate(column, options =>
			{
				options.IncludeRuleSets("general", "update");
			});
		}
	}

	#endregion

}
