using NpgsqlTypes;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace WebVella.Tefter.Services;

public partial interface ITfService
{
	/// <summary>
	/// Gets join key instance by id
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	internal TfDataProviderJoinKey GetDataProviderJoinKey(
		Guid id);

	/// <summary>
	/// Gets join keys list for specified provider id
	/// </summary>
	/// <param name="providerId"></param>
	/// <returns></returns>
	internal List<TfDataProviderJoinKey> GetDataProviderJoinKeys(
		Guid providerId);

	/// <summary>
	/// Creates new join key
	/// </summary>
	/// <param name="joinKey"></param>
	/// <returns></returns>
	public TfDataProvider CreateDataProviderJoinKey(
		TfDataProviderJoinKey joinKey);

	/// <summary>
	/// Updates an existing join key
	/// </summary>
	/// <param name="joinKey"></param>
	/// <returns></returns>
	public TfDataProvider UpdateDataProviderJoinKey(
		TfDataProviderJoinKey joinKey);

	/// <summary>
	/// Deletes and existing join key for specified identifier
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	public TfDataProvider DeleteDataProviderJoinKey(
		Guid id);

	/// <summary>
	/// Updates rows with join keys different to last version
	/// This happens when new join key is added or existing one is
	/// updated after provider data is synchronized
	/// </summary>
	internal Task UpdateJoinKeysVersionAsync(CancellationToken stoppingToken);
}

public partial class TfService : ITfService
{
	/// <summary>
	/// Gets join key instance by id
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	public TfDataProviderJoinKey GetDataProviderJoinKey(
		Guid id)
	{
		try
		{
			var dbo = _dboManager.Get<TfDataProviderJoinKeyDbo>(id);

			if (dbo == null)
				return null;

			var allColumns = GetDataProviderColumns(dbo.DataProviderId);

			return JoinKeyFromDbo(dbo, allColumns);
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	/// <summary>
	/// Gets join keys list for specified provider id
	/// </summary>
	/// <param name="providerId"></param>
	/// <returns></returns>
	public List<TfDataProviderJoinKey> GetDataProviderJoinKeys(
		Guid providerId)
	{
		try
		{
			var orderSettings = new TfOrderSettings(
				nameof(TfDataProviderJoinKey.DbName),
				OrderDirection.ASC);

			var dbos = _dboManager.GetList<TfDataProviderJoinKeyDbo>(
				providerId,
				nameof(TfDataProviderColumn.DataProviderId),
				order: orderSettings
			);

			var allColumns = GetDataProviderColumns(providerId);

			return dbos
				.Select(x => JoinKeyFromDbo(x, allColumns))
				.ToList();
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	/// <summary>
	/// Creates new join key
	/// </summary>
	/// <param name="joinKey"></param>
	/// <returns></returns>
	public TfDataProvider CreateDataProviderJoinKey(
		TfDataProviderJoinKey joinKey)
	{
		try
		{
			if (joinKey != null && joinKey.Id == Guid.Empty)
				joinKey.Id = Guid.NewGuid();

			new TfDataProviderJoinKeyValidator(this)
				.ValidateCreate(joinKey)
				.ToValidationException()
				.ThrowIfContainsErrors();

			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var dbo = JoinKeyToDbo(joinKey);

				//set initial version to 1,
				//default database value for version column is 0
				//so already existing rows will be with version 0 and
				//will be updated by background job
				dbo.Version = 1;

				dbo.LastModifiedOn = DateTime.Now;

				var success = _dboManager.Insert<TfDataProviderJoinKeyDbo>(dbo);
				if (!success)
					throw new TfDboServiceException("Insert<TfDataProviderJoinKeyDbo>");

				var provider = GetDataProvider(joinKey.DataProviderId);
				if (provider is null)
					throw new TfException("Failed to create new data provider join key");

				string providerTableName = $"dp{provider.Index}";

				TfDatabaseBuilder dbBuilder = _dbManager.GetDatabaseBuilder();

				dbBuilder
					.WithTableBuilder(providerTableName)
					.WithColumnsBuilder()
					.AddGuidColumn($"tf_jk_{joinKey.DbName}_id", c =>
					{
						c.WithDefaultValue(Guid.Empty);
					})
					.AddShortIntegerColumn($"tf_jk_{joinKey.DbName}_version", c =>
					{
						c.WithDefaultValue(0);
					});

				dbBuilder
					.WithTableBuilder(providerTableName)
					.WithConstraints(constraints =>
					{
						constraints
							.AddForeignKeyConstraint($"fk_{providerTableName}_{joinKey.DbName}_id_dict", c =>
							{
								c.WithColumns($"tf_jk_{joinKey.DbName}_id")
								.WithForeignTable("tf_id_dict")
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
	/// Updates an existing join key
	/// </summary>
	/// <param name="joinKey"></param>
	/// <returns></returns>
	public TfDataProvider UpdateDataProviderJoinKey(
		TfDataProviderJoinKey joinKey)
	{
		try
		{
			new TfDataProviderJoinKeyValidator(this)
				.ValidateUpdate(joinKey)
				.ToValidationException()
				.ThrowIfContainsErrors();

			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var existingJoinKey = GetDataProviderJoinKey(joinKey.Id);

				bool columnsChange = existingJoinKey.Columns.Count != joinKey.Columns.Count;
				if (!columnsChange)
				{
					for (int i = 0; i < joinKey.Columns.Count; i++)
					{
						if (existingJoinKey.Columns[i].Id != joinKey.Columns[i].Id)
						{
							columnsChange = true;
							break;
						}
					}
				}

				var dbo = JoinKeyToDbo(joinKey);

				//only increment version if columns or columns order is changed
				if (columnsChange)
					dbo.Version = (short)(existingJoinKey.Version + 1);

				dbo.LastModifiedOn = DateTime.Now;

				var success = _dboManager.Update<TfDataProviderJoinKeyDbo>(dbo);

				if (!success)
					throw new TfDboServiceException("Update<TfDataProviderJoinKeyDbo>");

				var provider = GetDataProvider(joinKey.DataProviderId);
				if (provider is null)
					throw new TfException("Failed to create new data provider join key");

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
	/// Deletes and existing join key for specified identifier
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	public TfDataProvider DeleteDataProviderJoinKey(
		Guid id)
	{
		try
		{
			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var joinKey = GetDataProviderJoinKey(id);

				if (joinKey is null)
					throw new TfException("Failed to delete data provider join key cause not found");

				var success = _dboManager.Delete<TfDataProviderJoinKeyDbo>(joinKey.Id);

				if (!success)
					throw new TfDboServiceException("Delete<TfDataProviderJoinKeyDbo>");

				var provider = GetDataProvider(joinKey.DataProviderId);
				if (provider is null)
					throw new TfException("Failed to create new data provider join key");


				string providerTableName = $"dp{provider.Index}";

				TfDatabaseBuilder dbBuilder = _dbManager.GetDatabaseBuilder();

				dbBuilder
					.WithTableBuilder(providerTableName)
					.WithColumnsBuilder()
					.Remove($"tf_jk_{joinKey.DbName}_id")
					.Remove($"tf_jk_{joinKey.DbName}_version");

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

	public Task UpdateJoinKeysVersionAsync(CancellationToken stoppingToken)
	{
		try
		{
			var dataProviders = GetDataProviders();
			foreach (var provider in dataProviders)
			{
				if (stoppingToken.IsCancellationRequested)
					return Task.CompletedTask;

				var joinKeys = GetDataProviderJoinKeys(provider.Id);

				if (joinKeys.Count == 0)
					continue;

				List<string> conditions = new List<string>();
				foreach (var joinKey in joinKeys)
				{
					conditions.Add($"tf_jk_{joinKey.DbName}_version <> {joinKey.Version}");
				}

				//select 100 rows
				string sql = "SELECT tf_id FROM " + $"dp{provider.Index}" +
					" WHERE " + string.Join(" OR ", conditions) +
					" LIMIT 100";

				while (true)
				{
					if (stoppingToken.IsCancellationRequested)
						return Task.CompletedTask;

					var dt = _dbService.ExecuteSqlQueryCommand(sql);

					List<Guid> tfIds = new List<Guid>();
					foreach (DataRow row in dt.Rows)
					{
						Guid tfId = (Guid)row["tf_id"];
						tfIds.Add(tfId);
					}

					if (tfIds.Count == 0)
						break;

					var dataTable = QueryDataProvider(provider, tfIds);

					Dictionary<string, object> values = new Dictionary<string, object>();
					foreach (TfDataRow row in dataTable.Rows)
					{
						foreach (var joinKey in provider.JoinKeys)
						{
							List<string> keys = new List<string>();
							foreach (var column in joinKey.Columns)
								keys.Add(row[column.DbName]?.ToString());

							values[$"tf_jk_{joinKey.DbName}_id"] = GetId(keys.ToArray());
							values[$"tf_jk_{joinKey.DbName}_version"] = joinKey.Version;
						}

						UpdateProviderRowJoinKeysOnly(provider, (Guid)row["tf_id"], values);
					}
				}
			}
			return Task.CompletedTask;
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	#region <--- utility --->

	private static TfDataProviderJoinKeyDbo JoinKeyToDbo(
	TfDataProviderJoinKey joinKey)
	{
		if (joinKey == null)
			throw new ArgumentException(nameof(joinKey));

		string columnIdsJson = "[]";
		if (joinKey.Columns is not null)
			columnIdsJson = JsonSerializer.Serialize(
				joinKey.Columns.Select(x => x.Id));

		return new TfDataProviderJoinKeyDbo
		{
			Id = joinKey.Id,
			DataProviderId = joinKey.DataProviderId,
			DbName = joinKey.DbName,
			Description = joinKey.Description ?? string.Empty,
			ColumnIdsJson = columnIdsJson,
			Version = joinKey.Version,
			LastModifiedOn = joinKey.LastModifiedOn,
		};
	}

	private static TfDataProviderJoinKey JoinKeyFromDbo(
		TfDataProviderJoinKeyDbo dbo,
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

		return new TfDataProviderJoinKey
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

	internal class TfDataProviderJoinKeyValidator
		: AbstractValidator<TfDataProviderJoinKey>
	{
		public TfDataProviderJoinKeyValidator(
			ITfService tfService)
		{
			RuleSet("general", () =>
			{
				RuleFor(joinKey => joinKey.Id)
					.NotEmpty()
					.WithMessage("The join key id is required.");

				RuleFor(joinKey => joinKey.DataProviderId)
					.NotEmpty()
					.WithMessage("The join key data provider id is required.");

				RuleFor(joinKey => joinKey.DataProviderId)
					.Must(providerId => { return tfService.GetDataProvider(providerId) != null; })
					.WithMessage("There is no existing data provider for specified provider id.");

				RuleFor(joinKey => joinKey.DbName)
					.NotEmpty()
					.WithMessage("The data provider column database name is required.");

				RuleFor(joinKey => joinKey.DbName)
					.Must((joinKey, dbName) =>
					{
						if (string.IsNullOrWhiteSpace(dbName))
							return true;

						return dbName.Length >= TfConstants.DB_MIN_OBJECT_NAME_LENGTH;
					})
					.WithMessage($"The database name must be at least " +
								$"{TfConstants.DB_MIN_OBJECT_NAME_LENGTH} characters long.");

				RuleFor(joinKey => joinKey.DbName)
					.Must((joinKey, dbName) =>
					{
						if (string.IsNullOrWhiteSpace(dbName))
							return true;

						return dbName.Length <= TfConstants.DB_MAX_OBJECT_NAME_LENGTH;
					})
					.WithMessage($"The length of database name must be less or equal " +
								$"than {TfConstants.DB_MAX_OBJECT_NAME_LENGTH} characters");

				RuleFor(joinKey => joinKey.DbName)
					.Must((joinKey, dbName) =>
					{
						if (string.IsNullOrWhiteSpace(dbName))
							return true;

						//other validation will trigger
						if (dbName.Length < TfConstants.DB_MIN_OBJECT_NAME_LENGTH)
							return true;

						//other validation will trigger
						if (dbName.Length > TfConstants.DB_MAX_OBJECT_NAME_LENGTH)
							return true;

						Match match = Regex.Match(dbName, TfConstants.DB_OBJECT_NAME_VALIDATION_PATTERN);
						return match.Success && match.Value == dbName.Trim();
					})
					.WithMessage($"Name can only contains underscores and lowercase alphanumeric characters." +
						$" It must begin with a letter, not include spaces, not end with an underscore," +
						$" and not contain two consecutive underscores");

				RuleFor(joinKey => joinKey.Columns)
					.NotEmpty()
					.WithMessage("The join key required at least one column.");

				RuleFor(joinKey => joinKey.Columns)
					.Must((joinKey, columns) =>
					{
						if (columns == null)
							return true;

						var providerColumns = tfService.GetDataProviderColumns(joinKey.DataProviderId);

						foreach (var column in columns)
						{
							var exists = providerColumns.Any(x => x.Id == column.Id);
							if (!exists)
								return false;
						}

						return true;
					})
					.WithMessage($"Some of the selected columns cannot be found in data provider columns list.");

				RuleFor(joinKey => joinKey.Columns)
					.Must((joinKey, columns) =>
					{
						if (columns == null)
							return true;

						var providerColumns = tfService.GetDataProviderColumns(joinKey.DataProviderId);

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
				RuleFor(joinKey => joinKey.Id)
					.Must((joinKey, id) =>
					{
						return tfService.GetDataProviderJoinKey(id) == null;
					})
					.WithMessage("There is already existing join key with specified identifier.");


				RuleFor(joinKey => joinKey.DbName)
					.Must((joinKey, dbName) =>
					{
						if (string.IsNullOrEmpty(dbName))
							return true;

						var joinKeys = tfService.GetDataProviderJoinKeys(joinKey.DataProviderId);
						return !joinKeys.Any(x => x.DbName.ToLowerInvariant().Trim() == dbName.ToLowerInvariant().Trim());
					})
					.WithMessage("There is already existing join key with specified database name.");
			});

			RuleSet("update", () =>
			{
				RuleFor(joinKey => joinKey.Id)
					.Must((joinKey, id) =>
					{
						return tfService.GetDataProviderJoinKey(id) != null;
					})
					.WithMessage("There is not existing join key with specified identifier.");

				RuleFor(joinKey => joinKey.DataProviderId)
					.Must((joinKey, providerId) =>
					{

						var existingJoinKey = tfService.GetDataProviderJoinKey(joinKey.Id);
						if (existingJoinKey is null)
							return true;

						return existingJoinKey.DataProviderId == providerId;
					})
					.WithMessage("There data provider cannot be changed for join key.");

				RuleFor(joinKey => joinKey.DbName)
					.Must((joinKey, dbName) =>
					{

						var existingJoinKey = tfService.GetDataProviderJoinKey(joinKey.Id);
						if (existingJoinKey is null)
							return true;

						return existingJoinKey.DbName == dbName;
					})
					.WithMessage("There database name of join key cannot be changed.");
			});
		}

		public ValidationResult ValidateCreate(
			TfDataProviderJoinKey joinKey)
		{
			if (joinKey == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The data provider join key is null.") });

			return this.Validate(joinKey, options =>
			{
				options.IncludeRuleSets("general", "create");
			});
		}

		public ValidationResult ValidateUpdate(
			TfDataProviderJoinKey joinKey)
		{
			if (joinKey == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The data provider join key is null.") });

			return this.Validate(joinKey, options =>
			{
				options.IncludeRuleSets("general", "update");
			});
		}
	}

	#endregion
}