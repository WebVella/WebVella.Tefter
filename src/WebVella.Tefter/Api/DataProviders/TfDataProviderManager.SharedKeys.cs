﻿namespace WebVella.Tefter;

public partial interface ITfDataProviderManager
{
	/// <summary>
	/// Gets shared key instance by id
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	internal TfDataProviderSharedKey GetDataProviderSharedKey(
		Guid id);

	/// <summary>
	/// Gets shared keys list for specified provider id
	/// </summary>
	/// <param name="providerId"></param>
	/// <returns></returns>
	internal List<TfDataProviderSharedKey> GetDataProviderSharedKeys(
		Guid providerId);

	/// <summary>
	/// Creates new shared key
	/// </summary>
	/// <param name="sharedKey"></param>
	/// <returns></returns>
	public Result<TfDataProvider> CreateDataProviderSharedKey(
		TfDataProviderSharedKey sharedKey);

	/// <summary>
	/// Updates an existing shared key
	/// </summary>
	/// <param name="sharedKey"></param>
	/// <returns></returns>
	public Result<TfDataProvider> UpdateDataProviderSharedKey(
		TfDataProviderSharedKey sharedKey);

	/// <summary>
	/// Deletes and existing shared key for specified identifier
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	public Result<TfDataProvider> DeleteDataProviderSharedKey(
		Guid id);
}

public partial class TfDataProviderManager : ITfDataProviderManager
{
	/// <summary>
	/// Gets shared key instance by id
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	public TfDataProviderSharedKey GetDataProviderSharedKey(
		Guid id)
	{
		var dbo = _dboManager.Get<TfDataProviderSharedKeyDbo>(id);
		
		if (dbo == null)
			return null;

		var allColumns = GetDataProviderColumns(dbo.DataProviderId);

		return SharedKeyFromDbo(dbo, allColumns);
	}

	/// <summary>
	/// Gets shared keys list for specified provider id
	/// </summary>
	/// <param name="providerId"></param>
	/// <returns></returns>
	public List<TfDataProviderSharedKey> GetDataProviderSharedKeys(
		Guid providerId)
	{
		var orderSettings = new OrderSettings(
			nameof(TfDataProviderSharedKey.DbName),
			OrderDirection.ASC);

		var dbos = _dboManager.GetList<TfDataProviderSharedKeyDbo>(
			providerId,
			nameof(TfDataProviderColumn.DataProviderId),
			order: orderSettings
		);

		var allColumns = GetDataProviderColumns(providerId);

		return dbos
			.Select(x => SharedKeyFromDbo(x, allColumns))
			.ToList();
	}

	/// <summary>
	/// Creates new shared key
	/// </summary>
	/// <param name="sharedKey"></param>
	/// <returns></returns>
	public Result<TfDataProvider> CreateDataProviderSharedKey(
		TfDataProviderSharedKey sharedKey)
	{
		try
		{
			if (sharedKey != null && sharedKey.Id == Guid.Empty)
				sharedKey.Id = Guid.NewGuid();

			TfDataProviderSharedKeyValidator validator =
					new TfDataProviderSharedKeyValidator(_dboManager, this);

			var validationResult = validator.ValidateCreate(sharedKey);

			if (!validationResult.IsValid)
				return validationResult.ToResult();

			using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var dbo = SharedKeyToDbo(sharedKey);

				//set initial version to 0
				dbo.Version = 0;

				dbo.LastModifiedOn = DateTime.Now;

				var success = _dboManager.Insert<TfDataProviderSharedKeyDbo>(dbo);

				if (!success)
					return Result.Fail(new DboManagerError("Insert", dbo));

				var providerResult = GetProvider(sharedKey.DataProviderId);

				if (providerResult.IsFailed)
					return Result.Fail(new Error("Failed to create new data provider shared key")
						.CausedBy(providerResult.Errors));

				var provider = providerResult.Value;

				string providerTableName = $"dp{provider.Index}";

				string newColumnName = $"tf_sk_{sharedKey.DbName}";

				DatabaseBuilder dbBuilder = _dbManager.GetDatabaseBuilder();

				dbBuilder
					.WithTableBuilder(providerTableName)
					.WithColumnsBuilder()
					.AddGuidColumn(newColumnName, c =>
					{
						c.WithDefaultValue(Guid.Empty);
					});

				_dbManager.SaveChanges(dbBuilder);

				scope.Complete();

				return Result.Ok(provider);
			}
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to create new data provider shared key.").CausedBy(ex));
		}
	}

	/// <summary>
	/// Updates an existing shared key
	/// </summary>
	/// <param name="sharedKey"></param>
	/// <returns></returns>
	public Result<TfDataProvider> UpdateDataProviderSharedKey(
		TfDataProviderSharedKey sharedKey)
	{
		try
		{
			TfDataProviderSharedKeyValidator validator =
					new TfDataProviderSharedKeyValidator(_dboManager, this);

			var validationResult = validator.ValidateUpdate(sharedKey);

			if (!validationResult.IsValid)
				return validationResult.ToResult();

			using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var existingSharedKey = GetDataProviderSharedKey(sharedKey.Id);

				bool columnsChange = existingSharedKey.Columns.Count != sharedKey.Columns.Count;
				if (!columnsChange)
				{
					for (int i = 0; i < sharedKey.Columns.Count; i++)
					{
						if (existingSharedKey.Columns[i].Id != sharedKey.Columns[i].Id)
						{
							columnsChange = true;
							break;
						}
					}
				}

				var dbo = SharedKeyToDbo(sharedKey);

				//only increment version if columns or columns order is changed
				if (columnsChange)
					dbo.Version = existingSharedKey.Version + 1;

				dbo.LastModifiedOn = DateTime.Now;

				var success = _dboManager.Update<TfDataProviderSharedKeyDbo>(dbo);

				if (!success)
					return Result.Fail(new DboManagerError("Update", dbo));

				var providerResult = GetProvider(sharedKey.DataProviderId);

				if (providerResult.IsFailed)
					return Result.Fail(new Error("Failed to create new data provider shared key")
						.CausedBy(providerResult.Errors));

				var provider = providerResult.Value;

				scope.Complete();

				return Result.Ok(provider);
			}
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to update data provider shared key.").CausedBy(ex));
		}
	}


	/// <summary>
	/// Deletes and existing shared key for specified identifier
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	public Result<TfDataProvider> DeleteDataProviderSharedKey(
		Guid id)
	{
		try
		{
			using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var sharedKey = GetDataProviderSharedKey(id);

				if (sharedKey is null)
					return Result.Fail("Failed to delete data provider shared key cause not found");

				var success = _dboManager.Delete<TfDataProviderSharedKeyDbo>(sharedKey.Id);

				if (!success)
					return Result.Fail(new DboManagerError("Update", sharedKey));


				var providerResult = GetProvider(sharedKey.DataProviderId);

				if (providerResult.IsFailed)
					return Result.Fail(new Error("Failed to delete data provider shared key")
						.CausedBy(providerResult.Errors));

				var provider = providerResult.Value;

				string providerTableName = $"dp{provider.Index}";

				string sharedKeyDbColumnName = $"tf_sk_{sharedKey.DbName}";

				DatabaseBuilder dbBuilder = _dbManager.GetDatabaseBuilder();

				dbBuilder
					.WithTableBuilder(providerTableName)
					.WithColumnsBuilder()
					.Remove(sharedKeyDbColumnName);

				_dbManager.SaveChanges(dbBuilder);

				scope.Complete();

				return Result.Ok(provider);
			}
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to delete data provider shared key.").CausedBy(ex));
		}
	}

	#region <--- utility --->

	private static TfDataProviderSharedKeyDbo SharedKeyToDbo(
	TfDataProviderSharedKey sharedKey)
	{
		if (sharedKey == null)
			throw new ArgumentException(nameof(sharedKey));

		string columnIdsJson = "[]";
		if (sharedKey.Columns is not null)
			columnIdsJson = JsonSerializer.Serialize(
				sharedKey.Columns.Select(x=>x.Id));

		return new TfDataProviderSharedKeyDbo
		{
			Id = sharedKey.Id,
			DataProviderId = sharedKey.DataProviderId,
			DbName = sharedKey.DbName,
			Description = sharedKey.Description,
			ColumnIdsJson = columnIdsJson
		};
	}

	private static TfDataProviderSharedKey SharedKeyFromDbo(
		TfDataProviderSharedKeyDbo dbo,
		List<TfDataProviderColumn> allColumns)
	{
		if (dbo == null)
			throw new ArgumentException(nameof(dbo));

		if (allColumns == null)
			throw new ArgumentException(nameof(allColumns));

		var columnIdsHS = JsonSerializer
			.Deserialize<List<Guid>>(dbo.ColumnIdsJson ?? "[]")
			.ToHashSet();

		var columns = allColumns
			.Where(x => columnIdsHS.Contains(x.Id))
			.ToList();

		return new TfDataProviderSharedKey
		{
			Id = dbo.Id,
			DataProviderId = dbo.DataProviderId,
			DbName = dbo.DbName,
			Description = dbo.Description,
			Columns = columns
		};
	}

	#endregion

	#region <--- validation --->

	internal class TfDataProviderSharedKeyValidator
		: AbstractValidator<TfDataProviderSharedKey>
	{
		private readonly IDboManager _dboManager;
		private readonly ITfDataProviderManager _providerManager;

		public TfDataProviderSharedKeyValidator(
			IDboManager dboManager,
			ITfDataProviderManager providerManager)
		{
			_dboManager = dboManager;
			_providerManager = providerManager;

			RuleSet("general", () =>
			{
				RuleFor(sharedKey => sharedKey.Id)
					.NotEmpty()
					.WithMessage("The shared key id is required.");

				RuleFor(sharedKey => sharedKey.DataProviderId)
					.NotEmpty()
					.WithMessage("The shared key data provider id is required.");

				RuleFor(sharedKey => sharedKey.DataProviderId)
					.Must(providerId => { return providerManager.GetProvider(providerId).Value != null; })
					.WithMessage("There is no existing data provider for specified provider id.");

				RuleFor(sharedKey => sharedKey.DbName)
					.NotEmpty()
					.WithMessage("The data provider column database name is required.");

				RuleFor(sharedKey => sharedKey.DbName)
					.Must((sharedKey, dbName) =>
					{
						if (string.IsNullOrWhiteSpace(dbName))
							return true;

						return dbName.Length >= Constants.DB_MIN_OBJECT_NAME_LENGTH;
					})
					.WithMessage($"The database name must be at least " +
								$"{Constants.DB_MIN_OBJECT_NAME_LENGTH} characters long.");

				RuleFor(sharedKey => sharedKey.DbName)
					.Must((sharedKey, dbName) =>
					{
						if (string.IsNullOrWhiteSpace(dbName))
							return true;

						return dbName.Length <= Constants.DB_MAX_OBJECT_NAME_LENGTH;
					})
					.WithMessage($"The length of database name must be less or equal " +
								$"than {Constants.DB_MAX_OBJECT_NAME_LENGTH} characters");

				RuleFor(sharedKey => sharedKey.DbName)
					.Must((sharedKey, dbName) =>
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
					.WithMessage($"Name can only contains underscores and lowercase alphanumeric characters." +
						$" It must begin with a letter, not include spaces, not end with an underscore," +
						$" and not contain two consecutive underscores");

				RuleFor(sharedKey => sharedKey.Columns)
					.NotEmpty()
					.WithMessage("The shared key required at least one column.");

				RuleFor(sharedKey => sharedKey.Columns)
					.Must((sharedKey, columns) =>
					{
						if (columns == null)
							return true;

						var providerColumns = _providerManager.GetDataProviderColumns(sharedKey.DataProviderId);

						foreach (var column in columns)
						{
							var exists = providerColumns.Any(x => x.Id == column.Id);
							if (!exists)
								return false;
						}

						return true;
					})
					.WithMessage($"Some of the selected columns cannot be found in data provider columns list.");
			});

			RuleSet("create", () =>
			{
				RuleFor(sharedKey => sharedKey.Id)
					.Must((sharedKey, id) =>
					{
						return providerManager.GetDataProviderSharedKey(id) == null;
					})
					.WithMessage("There is already existing shared key with specified identifier.");


				RuleFor(sharedKey => sharedKey.DbName)
					.Must((sharedKey, dbName) =>
					{
						if (string.IsNullOrEmpty(dbName))
							return true;

						var sharedKeys = providerManager.GetDataProviderSharedKeys(sharedKey.DataProviderId);
						return !sharedKeys.Any(x => x.DbName.ToLowerInvariant().Trim() == dbName.ToLowerInvariant().Trim());
					})
					.WithMessage("There is already existing shared key with specified database name.");
			});

			RuleSet("update", () =>
			{
				RuleFor(sharedKey => sharedKey.Id)
					.Must((sharedKey, id) =>
					{
						return providerManager.GetDataProviderSharedKey(id) != null;
					})
					.WithMessage("There is not existing shared key with specified identifier.");

				RuleFor(sharedKey => sharedKey.DataProviderId)
					.Must((sharedKey, providerId) =>
					{

						var existingSharedKey = providerManager.GetDataProviderSharedKey(sharedKey.Id);
						if (existingSharedKey is null)
							return true;

						return existingSharedKey.DataProviderId == providerId;
					})
					.WithMessage("There data provider cannot be changed for shared key.");

				RuleFor(sharedKey => sharedKey.DbName)
					.Must((sharedKey, dbName) =>
					{

						var existingSharedKey = providerManager.GetDataProviderSharedKey(sharedKey.Id);
						if (existingSharedKey is null)
							return true;

						return existingSharedKey.DbName == dbName;
					})
					.WithMessage("There database name of shared key cannot be changed.");
			});


			RuleSet("delete", () =>
			{
			});

		}

		public ValidationResult ValidateCreate(
			TfDataProviderSharedKey sharedKey)
		{
			if (sharedKey == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The data provider shared key is null.") });

			return this.Validate(sharedKey, options =>
			{
				options.IncludeRuleSets("general", "create");
			});
		}

		public ValidationResult ValidateUpdate(
			TfDataProviderSharedKey sharedKey)
		{
			if (sharedKey == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The data provider shared key is null.") });

			return this.Validate(sharedKey, options =>
			{
				options.IncludeRuleSets("general", "update");
			});
		}

		public ValidationResult ValidateDelete(
			TfDataProviderSharedKey sharedKey)
		{
			if (sharedKey == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The data provider shared key with specified identifier is not found.") });

			return this.Validate(sharedKey, options =>
			{
				options.IncludeRuleSets("delete");
			});
		}
	}

	#endregion
}
