namespace WebVella.Tefter.Services;

public partial interface ITfService
{
	TfSharedColumn GetSharedColumn(
	Guid id);

	List<TfSharedColumn> GetSharedColumns();

	void CreateSharedColumn(
	   TfSharedColumn column);

	void UpdateSharedColumn(
		TfSharedColumn column);

	void DeleteSharedColumn(
		Guid columnId);
}

public partial class TfService : ITfService
{
	public TfSharedColumn GetSharedColumn(
		Guid id)
	{
		return _dboManager.Get<TfSharedColumn>(id);
	}

	public List<TfSharedColumn> GetSharedColumns()
	{
		var orderSettings = new TfOrderSettings(nameof(TfDataProviderColumn.DbName), OrderDirection.ASC);
		return _dboManager.GetList<TfSharedColumn>(order: orderSettings);
	}

	public void CreateSharedColumn(
		TfSharedColumn column)
	{
		if (column != null && column.Id == Guid.Empty)
			column.Id = Guid.NewGuid();

		new TfSharedColumnValidator(this)
			.ValidateCreate(column)
			.ToValidationException()
			.ThrowIfContainsErrors();

		using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
		{
			var success = _dboManager.Insert<TfSharedColumn>(column);
			if (!success)
				throw new TfDboServiceException("Insert<TfSharedColumn> failed");

			scope.Complete();
		}
	}

	public void UpdateSharedColumn(
		TfSharedColumn column)
	{
		if (column != null && column.Id == Guid.Empty)
			column.Id = Guid.NewGuid();


		new TfSharedColumnValidator(this)
			.ValidateUpdate(column)
			.ToValidationException()
			.ThrowIfContainsErrors();

		using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
		{
			var success = _dboManager.Update<TfSharedColumn>(column);
			if (!success)
				throw new TfDboServiceException("Update<TfSharedColumn> failed");

			scope.Complete();
		}

	}

	public void DeleteSharedColumn(
		Guid columnId)
	{
		var column = GetSharedColumn(columnId);

		new TfSharedColumnValidator(this)
			.ValidateDelete(column)
			.ToValidationException()
			.ThrowIfContainsErrors();


		using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
		{
			DeleteSharedColumnData(column);

			var success = _dboManager.Delete<TfSharedColumn>(columnId);
			if (!success)
				throw new TfDboServiceException("Delete<TfSharedColumn> failed");

			scope.Complete();
		}
	}

	#region <--- validation --->

	internal class TfSharedColumnValidator
		: AbstractValidator<TfSharedColumn>
	{
		public TfSharedColumnValidator(
			ITfService tfService)
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

						return dbName.StartsWith("sk_");
					})
					.WithMessage("The shared column database name should start with 'sk_'.");

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
						.Must((column, id) => { return tfService.GetSharedColumn(id) == null; })
						.WithMessage("There is already existing data shared column with specified identifier.");

				RuleFor(column => column.DbName)
						.Must((column, dbName) =>
						{
							if (string.IsNullOrEmpty(dbName))
								return true;

							var columns = tfService.GetSharedColumns();
							return !columns.Any(x => x.DbName.ToLowerInvariant().Trim() == dbName.ToLowerInvariant().Trim());
						})
						.WithMessage("There is already existing shared column with specified database name.");
			});

			RuleSet("update", () =>
			{
				RuleFor(column => column.Id)
						.Must((column, id) =>
						{
							return tfService.GetSharedColumn(id) != null;
						})
						.WithMessage("There is not existing data shared column with specified identifier.");

				RuleFor(column => column.DbName)
						.Must((column, dbName) =>
						{
							if (string.IsNullOrEmpty(dbName))
								return true;

							var columns = tfService.GetSharedColumns();
							return !columns.Any(x => x.DbName.ToLowerInvariant().Trim() == dbName.ToLowerInvariant().Trim() && x.Id != column.Id);
						})
						.WithMessage("There is already existing shared column with specified database name.");
			});

			//no rules for delete
			RuleSet("delete", () =>
			{
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

		public ValidationResult ValidateDelete(
			TfSharedColumn column)
		{
			if (column == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The shared column object is null.") });

			return this.Validate(column, options =>
			{
				options.IncludeRuleSets("delete");
			});
		}
	}

	#endregion
}
