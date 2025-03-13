using NpgsqlTypes;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace WebVella.Tefter.Services;

public partial interface ITfService
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
	public TfDataProvider CreateDataProviderSharedKey(
		TfDataProviderSharedKey sharedKey);

	/// <summary>
	/// Updates an existing shared key
	/// </summary>
	/// <param name="sharedKey"></param>
	/// <returns></returns>
	public TfDataProvider UpdateDataProviderSharedKey(
		TfDataProviderSharedKey sharedKey);

	/// <summary>
	/// Deletes and existing shared key for specified identifier
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	public TfDataProvider DeleteDataProviderSharedKey(
		Guid id);
}

public partial class TfService : ITfService
{
	/// <summary>
	/// Gets shared key instance by id
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	public TfDataProviderSharedKey GetDataProviderSharedKey(
		Guid id)
	{
		try
		{
			var dbo = _dboManager.Get<TfDataProviderSharedKeyDbo>(id);

			if (dbo == null)
				return null;

			var allColumns = GetDataProviderColumns(dbo.DataProviderId);

			return SharedKeyFromDbo(dbo, allColumns);
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	/// <summary>
	/// Gets shared keys list for specified provider id
	/// </summary>
	/// <param name="providerId"></param>
	/// <returns></returns>
	public List<TfDataProviderSharedKey> GetDataProviderSharedKeys(
		Guid providerId)
	{
		try
		{
			var orderSettings = new TfOrderSettings(
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
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	/// <summary>
	/// Creates new shared key
	/// </summary>
	/// <param name="sharedKey"></param>
	/// <returns></returns>
	public TfDataProvider CreateDataProviderSharedKey(
		TfDataProviderSharedKey sharedKey)
	{
		try
		{
			if (sharedKey != null && sharedKey.Id == Guid.Empty)
				sharedKey.Id = Guid.NewGuid();

			new TfDataProviderSharedKeyValidator(this)
				.ValidateCreate(sharedKey)
				.ToValidationException()
				.ThrowIfContainsErrors();

			using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var dbo = SharedKeyToDbo(sharedKey);

				//set initial version to 1,
				//default database value for version column is 0
				//so already existing rows will be with version 0 and
				//will be updated by background job
				dbo.Version = 1;

				dbo.LastModifiedOn = DateTime.Now;

				var success = _dboManager.Insert<TfDataProviderSharedKeyDbo>(dbo);
				if (!success)
					throw new TfDboServiceException("Insert<TfDataProviderSharedKeyDbo>");

				var provider = GetDataProvider(sharedKey.DataProviderId);
				if (provider is null)
					throw new TfException("Failed to create new data provider shared key");

				string providerTableName = $"dp{provider.Index}";

				TfDatabaseBuilder dbBuilder = _dbManager.GetDatabaseBuilder();

				dbBuilder
					.WithTableBuilder(providerTableName)
					.WithColumnsBuilder()
					.AddGuidColumn($"tf_sk_{sharedKey.DbName}_id", c =>
					{
						c.WithDefaultValue(Guid.Empty);
					})
					.AddShortIntegerColumn($"tf_sk_{sharedKey.DbName}_version", c =>
					{
						c.WithDefaultValue(0);
					});

				dbBuilder
					.WithTableBuilder(providerTableName)
					.WithConstraints(constraints =>
					{
						constraints
							.AddForeignKeyConstraint($"fk_{providerTableName}_{sharedKey.DbName}_id_dict", c =>
							{
								c.WithColumns($"tf_sk_{sharedKey.DbName}_id")
								.WithForeignTable("id_dict")
								.WithForeignColumns("id");
							});
					});

				_dbManager.SaveChanges(dbBuilder);

				scope.Complete();

				return provider;
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	/// <summary>
	/// Updates an existing shared key
	/// </summary>
	/// <param name="sharedKey"></param>
	/// <returns></returns>
	public TfDataProvider UpdateDataProviderSharedKey(
		TfDataProviderSharedKey sharedKey)
	{
		try
		{
			new TfDataProviderSharedKeyValidator(this)
				.ValidateUpdate(sharedKey)
				.ToValidationException()
				.ThrowIfContainsErrors();

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
					dbo.Version = (short)(existingSharedKey.Version + 1);

				dbo.LastModifiedOn = DateTime.Now;

				var success = _dboManager.Update<TfDataProviderSharedKeyDbo>(dbo);

				if (!success)
					throw new TfDboServiceException("Update<TfDataProviderSharedKeyDbo>");

				var provider = GetDataProvider(sharedKey.DataProviderId);
				if (provider is null)
					throw new TfException("Failed to create new data provider shared key");

				scope.Complete();

				return provider;
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}


	/// <summary>
	/// Deletes and existing shared key for specified identifier
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	public TfDataProvider DeleteDataProviderSharedKey(
		Guid id)
	{
		try { 
		using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
		{
			var sharedKey = GetDataProviderSharedKey(id);

			if (sharedKey is null)
				throw new TfException("Failed to delete data provider shared key cause not found");

			var success = _dboManager.Delete<TfDataProviderSharedKeyDbo>(sharedKey.Id);

			if (!success)
				throw new TfDboServiceException("Delete<TfDataProviderSharedKeyDbo>");

			var provider = GetDataProvider(sharedKey.DataProviderId);
			if (provider is null)
				throw new TfException("Failed to create new data provider shared key");


			string providerTableName = $"dp{provider.Index}";

			TfDatabaseBuilder dbBuilder = _dbManager.GetDatabaseBuilder();

			dbBuilder
				.WithTableBuilder(providerTableName)
				.WithColumnsBuilder()
				.Remove($"tf_sk_{sharedKey.DbName}_id")
				.Remove($"tf_sk_{sharedKey.DbName}_version");

			_dbManager.SaveChanges(dbBuilder);

			scope.Complete();

			return provider;
		}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
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
				sharedKey.Columns.Select(x => x.Id));

		return new TfDataProviderSharedKeyDbo
		{
			Id = sharedKey.Id,
			DataProviderId = sharedKey.DataProviderId,
			DbName = sharedKey.DbName,
			Description = sharedKey.Description ?? string.Empty,
			ColumnIdsJson = columnIdsJson,
			Version = sharedKey.Version,
			LastModifiedOn = sharedKey.LastModifiedOn,
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

		var columnIds = JsonSerializer
			.Deserialize<List<Guid>>(dbo.ColumnIdsJson ?? "[]")
			.ToList();

		var columns = new List<TfDataProviderColumn>();
		foreach (var id in columnIds)
			columns.Add(allColumns.Single(x => x.Id == id));

		return new TfDataProviderSharedKey
		{
			Id = dbo.Id,
			DataProviderId = dbo.DataProviderId,
			DbName = dbo.DbName,
			Description = dbo.Description,
			Columns = columns,
			Version = dbo.Version,
			LastModifiedOn = dbo.LastModifiedOn
		};
	}

	#endregion

	#region <--- validation --->

	internal class TfDataProviderSharedKeyValidator
		: AbstractValidator<TfDataProviderSharedKey>
	{
		public TfDataProviderSharedKeyValidator(
			ITfService tfService)
		{
			RuleSet("general", () =>
			{
				RuleFor(sharedKey => sharedKey.Id)
					.NotEmpty()
					.WithMessage("The shared key id is required.");

				RuleFor(sharedKey => sharedKey.DataProviderId)
					.NotEmpty()
					.WithMessage("The shared key data provider id is required.");

				RuleFor(sharedKey => sharedKey.DataProviderId)
					.Must(providerId => { return tfService.GetDataProvider(providerId) != null; })
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

						var providerColumns = tfService.GetDataProviderColumns(sharedKey.DataProviderId);

						foreach (var column in columns)
						{
							var exists = providerColumns.Any(x => x.Id == column.Id);
							if (!exists)
								return false;
						}

						return true;
					})
					.WithMessage($"Some of the selected columns cannot be found in data provider columns list.");

				RuleFor(sharedKey => sharedKey.Columns)
					.Must((sharedKey, columns) =>
					{
						if (columns == null)
							return true;

						var providerColumns = tfService.GetDataProviderColumns(sharedKey.DataProviderId);

						HashSet<Guid> columnIds = new HashSet<Guid>();

						foreach (var column in columns)
						{
							var exists = columnIds.Contains(column.Id);
							if (exists)
								return false;

							columnIds.Add(column.Id);
						}

						return true;
					})
					.WithMessage($"There are same columns added more than once in the key. Its not allowed.");
			});

			RuleSet("create", () =>
			{
				RuleFor(sharedKey => sharedKey.Id)
					.Must((sharedKey, id) =>
					{
						return tfService.GetDataProviderSharedKey(id) == null;
					})
					.WithMessage("There is already existing shared key with specified identifier.");


				RuleFor(sharedKey => sharedKey.DbName)
					.Must((sharedKey, dbName) =>
					{
						if (string.IsNullOrEmpty(dbName))
							return true;

						var sharedKeys = tfService.GetDataProviderSharedKeys(sharedKey.DataProviderId);
						return !sharedKeys.Any(x => x.DbName.ToLowerInvariant().Trim() == dbName.ToLowerInvariant().Trim());
					})
					.WithMessage("There is already existing shared key with specified database name.");
			});

			RuleSet("update", () =>
			{
				RuleFor(sharedKey => sharedKey.Id)
					.Must((sharedKey, id) =>
					{
						return tfService.GetDataProviderSharedKey(id) != null;
					})
					.WithMessage("There is not existing shared key with specified identifier.");

				RuleFor(sharedKey => sharedKey.DataProviderId)
					.Must((sharedKey, providerId) =>
					{

						var existingSharedKey = tfService.GetDataProviderSharedKey(sharedKey.Id);
						if (existingSharedKey is null)
							return true;

						return existingSharedKey.DataProviderId == providerId;
					})
					.WithMessage("There data provider cannot be changed for shared key.");

				RuleFor(sharedKey => sharedKey.DbName)
					.Must((sharedKey, dbName) =>
					{

						var existingSharedKey = tfService.GetDataProviderSharedKey(sharedKey.Id);
						if (existingSharedKey is null)
							return true;

						return existingSharedKey.DbName == dbName;
					})
					.WithMessage("There database name of shared key cannot be changed.");
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
	}

	#endregion
}