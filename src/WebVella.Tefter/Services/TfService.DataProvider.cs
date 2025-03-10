using NpgsqlTypes;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace WebVella.Tefter.Services;

public partial interface ITfService
{
	#region <--- DataProvider Types --->

	ITfDataProviderType GetDataProviderType(
		Guid id);

	ReadOnlyCollection<ITfDataProviderType> GetDataProviderTypes();

	#endregion

	#region <--- DataProvider --->
	/// <summary>
	/// Gets data provider instance for specified identifier
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	public TfDataProvider GetDataProvider(
		Guid id);

	/// <summary>
	/// Gets data provider instance for specified name
	/// </summary>
	/// <param name="name"></param>
	/// <returns></returns>
	public TfDataProvider GetDataProvider(
		string name);

	/// <summary>
	/// Gets list of available data providers
	/// </summary>
	/// <returns></returns>
	public ReadOnlyCollection<TfDataProvider> GetDataProviders();

	/// <summary>
	/// Creates new data provider 
	/// </summary>
	/// <param name="providerModel"></param>
	/// <returns></returns>
	internal TfDataProvider CreateDataProvider(
		TfDataProviderModel providerModel);

	/// <summary>
	/// Update existing data provider
	/// </summary>
	/// <param name="providerModel"></param>
	/// <returns></returns>
	internal TfDataProvider UpdateDataProvider(
		TfDataProviderModel providerModel);

	/// <summary>
	/// Deletes existing data provider
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	internal void DeleteDataProvider(
		Guid id);
	#endregion

	#region <--- Columns --->
	/// <summary>
	/// Gets data provider column instance for specified identifier
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	internal TfDataProviderColumn GetDataProviderColumn(
		Guid id);

	/// <summary>
	/// Gets list of data provider columns for specified provider identifier
	/// </summary>
	/// <param name="providerId"></param>
	/// <returns></returns>
	internal List<TfDataProviderColumn> GetDataProviderColumns(
		Guid providerId);

	/// <summary>
	/// Creates new data provider column
	/// </summary>
	/// <param name="column"></param>
	/// <returns></returns>
	public TfDataProvider CreateDataProviderColumn(
		TfDataProviderColumn column);


	/// <summary>
	/// Creates new data provider column
	/// </summary>
	/// <param name="column"></param>
	/// <returns></returns>
	public TfDataProvider CreateBulkDataProviderColumn(Guid providerId,
		List<TfDataProviderColumn> columns);

	/// <summary>
	/// Updates existing data provider column
	/// </summary>
	/// <param name="column"></param>
	/// <returns></returns>
	public TfDataProvider UpdateDataProviderColumn(
		TfDataProviderColumn column);

	/// <summary>
	/// Deletes existing data provider column
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	public TfDataProvider DeleteDataProviderColumn(
		Guid id);

	ReadOnlyCollection<DatabaseColumnTypeInfo> GetDatabaseColumnTypeInfos();

	#endregion

	#region <--- Shared Keys ---->

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

	#endregion

	#region <--- Synchronization --->

	internal Task BulkSynchronize(
		TfDataProviderSynchronizeTask task);

	internal TfDataProviderSynchronizeTask GetSynchronizationTask(
		Guid taskId);

	internal TfDataProviderSynchronizeTaskExtended GetSynchronizationTaskExtended(
		Guid taskId);

	internal List<TfDataProviderSynchronizeTaskExtended> GetSynchronizationTasksExtended(
		Guid? providerId = null,
		TfSynchronizationStatus? status = null);

	internal List<TfDataProviderSynchronizeTask> GetSynchronizationTasks(
		Guid? providerId = null,
		TfSynchronizationStatus? status = null);

	internal Guid CreateSynchronizationTask(
		Guid providerId,
		TfSynchronizationPolicy synchPolicy);

	internal void UpdateSychronizationTask(
		Guid taskId,
		TfSynchronizationStatus status,
		DateTime? startedOn = null,
		DateTime? completedOn = null);

	internal List<TfDataProviderSynchronizeResultInfo> GetSynchronizationTaskResultInfos(
		Guid taskId);

	internal void CreateSynchronizationResultInfo(
		Guid syncTaskId,
		int? tfRowIndex,
		Guid? tfId,
		string info = null,
		string warning = null,
		string error = null);

	internal TfDataProviderSourceSchemaInfo GetDataProviderSourceSchemaInfo(
		Guid providerId);
	#endregion
}

public partial class TfService : ITfService
{
	#region <--- DataProvider Types --->

	private static void ScanAndRegisterDataProviderTypes()
	{
		var assemblies = AppDomain.CurrentDomain.GetAssemblies()
							.Where(a => !(a.FullName.ToLowerInvariant().StartsWith("microsoft.")
							   || a.FullName.ToLowerInvariant().StartsWith("system.")));

		foreach (var assembly in assemblies)
		{
			foreach (Type type in assembly.GetTypes())
			{
				if (type.GetInterfaces().Any(x => x == typeof(ITfDataProviderType)))
				{
					var instance = (ITfDataProviderType)Activator.CreateInstance(type);
					_providerTypes.Add(instance);
				}
			}
		}
	}

	public static List<ITfDataProviderType> _providerTypes { get; internal set; }

	public ITfDataProviderType GetDataProviderType(Guid id)
	{
		return _providerTypes.SingleOrDefault(x => x.Id == id);
	}

	public ReadOnlyCollection<ITfDataProviderType> GetDataProviderTypes()
	{
		return _providerTypes.AsReadOnly();
	}

	#endregion

	#region <--- DataProvider --->

	/// <summary>
	/// Gets data provider instance for specified identifier
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	public TfDataProvider GetDataProvider(
		Guid id)
	{
		var providerDbo = _dboManager.Get<TfDataProviderDbo>(id);

		if (providerDbo == null)
			return null;

		var providerType = GetDataProviderType(providerDbo.TypeId);
		if (providerType == null)
			throw new TfException("Unable to find provider type for specified provider instance.");

		var sharedKeys = GetDataProviderSharedKeys(id);

		var provider = DataProviderFromDbo(
				providerDbo,
				GetDataProviderSystemColumns(sharedKeys),
				GetDataProviderColumns(id),
				sharedKeys,
				providerType);

		provider.ServiceProvider = _serviceProvider;

		InitDataProviderSharedColumns(provider);

		return provider;
	}


	/// <summary>
	/// Gets data provider instance for specified name
	/// </summary>
	/// <param name="name"></param>
	/// <returns></returns>
	public TfDataProvider GetDataProvider(
		string name)
	{
		var providerDbo = _dboManager
			.Get<TfDataProviderDbo>(name, nameof(TfDataProviderDbo.Name));

		if (providerDbo == null)
			return null;

		var providerType = GetDataProviderType(providerDbo.TypeId);
		if (providerType == null)
			throw new TfException("Failed to get data provider");

		var sharedKeys = GetDataProviderSharedKeys(providerDbo.Id);

		var provider = DataProviderFromDbo(
				providerDbo,
				GetDataProviderSystemColumns(sharedKeys),
				GetDataProviderColumns(providerDbo.Id),
				sharedKeys,
				providerType);

		provider.ServiceProvider = _serviceProvider;

		InitDataProviderSharedColumns(provider);

		return provider;
	}

	/// <summary>
	/// Gets list of available data providers
	/// </summary>
	/// <returns></returns>
	public ReadOnlyCollection<TfDataProvider> GetDataProviders()
	{

		List<TfDataProvider> providers = new List<TfDataProvider>();

		var providerTypes = GetDataProviderTypes();

		var providersDbo = _dboManager.GetList<TfDataProviderDbo>();

		foreach (var dbo in providersDbo)
		{
			var providerType = providerTypes.SingleOrDefault(x => x.Id == dbo.TypeId);

			if (providerType == null)
				throw new TfException($"Failed to get data providers, because " +
					$"provider type {dbo.TypeName} with id = '{dbo.TypeId}' is not found.");

			var sharedKeys = GetDataProviderSharedKeys(dbo.Id);

			var provider = DataProviderFromDbo(
					dbo,
					GetDataProviderSystemColumns(sharedKeys),
					GetDataProviderColumns(dbo.Id),
					sharedKeys,
					providerType);

			provider.ServiceProvider = _serviceProvider;

			InitDataProviderSharedColumns(provider);

			providers.Add(provider);
		}

		return providers.AsReadOnly();
	}


	/// <summary>
	/// Creates new data provider 
	/// </summary>
	/// <param name="providerModel"></param>
	/// <returns></returns>
	public TfDataProvider CreateDataProvider(
		TfDataProviderModel providerModel)
	{
		if (providerModel != null && providerModel.Id == Guid.Empty)
			providerModel.Id = Guid.NewGuid();

		new TfDataProviderCreateValidator(this)
			.Validate(providerModel)
			.ToValidationException()
			.ThrowIfContainsErrors();

		using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
		{

			TfDataProviderDbo dataProviderDbo = DataProviderToDbo(providerModel);

			var success = _dboManager.Insert<TfDataProviderDbo>(dataProviderDbo);

			if (!success)
				throw new TfDboServiceException("Insert<TfDataProviderDbo> failed");

			var provider = GetDataProvider(providerModel.Id);
			if (provider is null)
				throw new TfException("Failed to create new data provider");

			string providerTableName = $"dp{provider.Index}";

			TfDatabaseBuilder dbBuilder = _dbManager.GetDatabaseBuilder();
			dbBuilder.NewTableBuilder(Guid.NewGuid(), providerTableName)
				.WithDataProviderId(provider.Id)
				.WithColumns(columns =>
				{
					columns
						.AddGuidColumn("tf_id", c => { c.WithoutAutoDefaultValue().NotNullable(); })
						.AddDateTimeColumn("tf_created_on", c => { c.WithoutAutoDefaultValue().NotNullable(); })
						.AddDateTimeColumn("tf_updated_on", c => { c.WithoutAutoDefaultValue().NotNullable(); })
						.AddTextColumn("tf_search", c => { c.NotNullable().WithDefaultValue(string.Empty); })
						.AddIntegerColumn("tf_row_index", c => { c.NotNullable(); });
				})
				.WithConstraints(constraints =>
				{
					constraints
						.AddPrimaryKeyConstraint($"pk_{providerTableName}", c => { c.WithColumns("tf_id"); })
						.AddForeignKeyConstraint($"fk_{providerTableName}_id_dict", c =>
						{
							c.WithColumns("tf_id")
							.WithForeignTable("id_dict")
							.WithForeignColumns("id");
						});

				})
				.WithIndexes(indexes =>
				{
					indexes
						.AddBTreeIndex($"ix_{providerTableName}_tf_id", c => { c.WithColumns("tf_id"); })
						.AddGinIndex($"ix_{providerTableName}_tf_search", c => { c.WithColumns("tf_search"); });
				});

			_dbManager.SaveChanges(dbBuilder);

			scope.Complete();

			return provider;
		}
	}

	/// <summary>
	/// Update existing data provider
	/// </summary>
	/// <param name="providerModel"></param>
	/// <returns></returns>
	public TfDataProvider UpdateDataProvider(
		TfDataProviderModel providerModel)
	{
		new TfDataProviderUpdateValidator(this)
			.Validate(providerModel)
			.ToValidationException()
			.ThrowIfContainsErrors();

		TfDataProviderDbo dataProviderDbo = DataProviderToDbo(providerModel);

		var success = _dboManager.Update<TfDataProviderDbo>(dataProviderDbo);

		if (!success)
			throw new TfDboServiceException("Update<TfDataProviderDbo> failed.");

		return GetDataProvider(providerModel.Id);
	}

	/// <summary>
	/// Deletes existing data provider
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	public void DeleteDataProvider(
		Guid id)
	{
		using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
		{
			var provider = GetDataProvider(id);
			if (provider is null)
				new TfException("Provider is not found.");

			new TfDataProviderDeleteValidator(this)
				.Validate(provider)
				.ToValidationException()
				.ThrowIfContainsErrors();

			bool success = true;

			foreach (var column in provider.Columns)
			{
				success = _dboManager.Delete<TfDataProviderColumn>(column.Id);

				if (!success)
					throw new TfDboServiceException("Delete<TfDataProviderColumn> failed.");
			}

			foreach (var sharedKey in provider.SharedKeys)
			{
				success = _dboManager.Delete<TfDataProviderSharedKeyDbo>(sharedKey.Id);

				if (!success)
					throw new TfDboServiceException("Delete<TfDataProviderSharedKeyDbo> failed.");
			}

			success = _dboManager.Delete<TfDataProviderDbo>(id);

			if (!success)
				throw new TfDboServiceException("Delete<TfDataProviderDbo> failed.");


			string providerTableName = $"dp{provider.Index}";

			TfDatabaseBuilder dbBuilder = _dbManager.GetDatabaseBuilder();

			dbBuilder.Remove(providerTableName);

			_dbManager.SaveChanges(dbBuilder);

			scope.Complete();
		}
	}



	#region <--- utility --->

	private void InitDataProviderSharedColumns(
		TfDataProvider provider)
	{
		List<TfSharedColumn> columns = new List<TfSharedColumn>();

		if (provider.SharedKeys.Count == 0)
		{
			provider.SharedColumns = columns.AsReadOnly();
			return;
		}

		var sharedColumns = GetSharedColumns();
		foreach (var sharedColumn in sharedColumns)
		{
			var sharedKey = provider.SharedKeys.SingleOrDefault(x => x.DbName == sharedColumn.SharedKeyDbName);
			if (sharedKey is not null && !columns.Contains(sharedColumn))
				columns.Add(sharedColumn);
		}

		provider.SharedColumns = columns.AsReadOnly();
	}

	private static TfDataProviderDbo DataProviderToDbo(
		TfDataProviderModel providerModel)
	{
		if (providerModel == null)
			throw new ArgumentException(nameof(providerModel));

		return new TfDataProviderDbo
		{
			Id = providerModel.Id,
			Name = providerModel.Name,
			SettingsJson = providerModel.SettingsJson,
			TypeId = providerModel.ProviderType.Id,
			TypeName = providerModel.ProviderType.GetType().FullName,
			SynchPrimaryKeyColumnsJson = JsonSerializer.Serialize(providerModel.SynchPrimaryKeyColumns ?? new List<string>())
		};
	}

	private static TfDataProvider DataProviderFromDbo(
		TfDataProviderDbo dbo,
		List<TfDataProviderSystemColumn> systemColumns,
		List<TfDataProviderColumn> columns,
		List<TfDataProviderSharedKey> sharedKeys,
		ITfDataProviderType providerType)
	{
		if (dbo == null)
			throw new ArgumentException(nameof(dbo));

		if (columns == null)
			throw new ArgumentException(nameof(columns));

		if (sharedKeys == null)
			throw new ArgumentException(nameof(sharedKeys));

		if (providerType == null)
			throw new ArgumentException(nameof(providerType));

		return new TfDataProvider
		{
			Id = dbo.Id,
			Name = dbo.Name,
			Index = dbo.Index,
			SettingsJson = dbo.SettingsJson,
			ProviderType = providerType,
			SystemColumns = systemColumns.AsReadOnly(),
			Columns = columns.AsReadOnly(),
			SharedKeys = sharedKeys.AsReadOnly(),
			SynchPrimaryKeyColumns = JsonSerializer.Deserialize<List<string>>(dbo.SynchPrimaryKeyColumnsJson ?? "[]").AsReadOnly()
		};
	}

	#endregion

	#region <--- validation --->

	internal class TfDataProviderCreateValidator
		: AbstractValidator<TfDataProviderModel>
	{
		public TfDataProviderCreateValidator(
			ITfService tfService)
		{
			RuleFor(provider => provider.Id)
					.NotEmpty()
					.WithMessage("The data provider id is required.");

			RuleFor(provider => provider.Name)
					.NotEmpty()
					.WithMessage("The data provider name is required.");

			RuleFor(provider => provider.ProviderType)
				.NotEmpty()
				.WithMessage("The data provider type is required.");

			RuleFor(provider => provider.Id)
					.Must(id => { return tfService.GetDataProvider(id) == null; })
					.WithMessage("There is already existing data provider with specified identifier.");

			RuleFor(provider => provider.Name)
					.Must(name => { return tfService.GetDataProvider(name) == null; })
					.WithMessage("There is already existing data provider with specified name.");

		}

		public ValidationResult ValidateCreate(TfDataProviderModel provider)
		{
			if (provider == null)
				return new ValidationResult(new[] { new ValidationFailure("", "The data provider model is null.") });

			return this.Validate(provider);
		}
	}

	internal class TfDataProviderUpdateValidator
		: AbstractValidator<TfDataProviderModel>
	{
		public TfDataProviderUpdateValidator(
			ITfService tfService)
		{
			RuleFor(provider => provider.Id)
					.NotEmpty()
					.WithMessage("The data provider id is required.");

			RuleFor(provider => provider.Name)
					.NotEmpty()
					.WithMessage("The data provider name is required.");

			RuleFor(provider => provider.ProviderType)
				.NotEmpty()
				.WithMessage("The data provider type is required.");

			RuleFor(provider => provider.Id)
					.Must((provider, id) => { return tfService.GetDataProvider(id) != null; })
					.WithMessage("There is no existing data provider for specified identifier.");

			RuleFor(provider => provider.ProviderType)
					.Must((provider, providerType) =>
					{
						if (provider.ProviderType == null)
							return true;

						var existingObject = tfService.GetDataProvider(provider.Id);

						if (existingObject != null && existingObject.ProviderType.Id != provider.ProviderType.Id)
							return false;

						return true;
					})
					.WithMessage("The data provider type cannot be updated.");

			RuleFor(provider => provider.Name)
				.Must((provider, name) =>
				{
					var existingObj = tfService.GetDataProvider(provider.Name);
					return !(existingObj != null && existingObj.Id != provider.Id);
				})
				.WithMessage("There is already existing data provider with specified name.");
		}

		public ValidationResult ValidateUpdate(TfDataProviderModel provider)
		{
			if (provider == null)
				return new ValidationResult(new[] { new ValidationFailure("", "The data provider model is null.") });

			return this.Validate(provider);
		}
	}

	internal class TfDataProviderDeleteValidator
		: AbstractValidator<TfDataProvider>
	{
		public TfDataProviderDeleteValidator(
			ITfService tfService)
		{

			RuleFor(provider => provider.Columns)
					.Must((provider, columns) =>
					{
						if (columns.Count > 0)
							return false;

						return true;
					})
					.WithMessage("The data provider contains columns be deleted.");
		}

		public ValidationResult ValidateDelete(TfDataProvider provider)
		{
			if (provider == null)
				return new ValidationResult(new[] { new ValidationFailure("", "The data provider is null.") });

			return new ValidationResult();
		}
	}

	#endregion

	#endregion

	#region <--- Columns --->

	/// <summary>
	/// Gets data provider column instance for specified identifier
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	public TfDataProviderColumn GetDataProviderColumn(
		Guid id)
	{
		return _dboManager.Get<TfDataProviderColumn>(id);
	}

	/// <summary>
	/// Gets list of data provider columns for specified provider identifier
	/// </summary>
	/// <param name="providerId"></param>
	/// <returns></returns>
	public List<TfDataProviderColumn> GetDataProviderColumns(
		Guid providerId)
	{
		var orderSettings = new TfOrderSettings(nameof(TfDataProviderColumn.CreatedOn), OrderDirection.ASC);
		return _dboManager.GetList<TfDataProviderColumn>(providerId,
			nameof(TfDataProviderColumn.DataProviderId), order: orderSettings);
	}

	private List<TfDataProviderSystemColumn> GetDataProviderSystemColumns(
		List<TfDataProviderSharedKey> sharedKeys)
	{
		var systemColumns = new List<TfDataProviderSystemColumn>();

		systemColumns.Add(new TfDataProviderSystemColumn
		{
			DbName = "tf_id",
			DbType = TfDatabaseColumnType.Guid
		});

		systemColumns.Add(new TfDataProviderSystemColumn
		{
			DbName = "tf_row_index",
			DbType = TfDatabaseColumnType.Integer
		});

		systemColumns.Add(new TfDataProviderSystemColumn
		{
			DbName = "tf_created_on",
			DbType = TfDatabaseColumnType.DateTime
		});

		systemColumns.Add(new TfDataProviderSystemColumn
		{
			DbName = "tf_updated_on",
			DbType = TfDatabaseColumnType.DateTime
		});

		systemColumns.Add(new TfDataProviderSystemColumn
		{
			DbName = "tf_search",
			DbType = TfDatabaseColumnType.Text
		});

		foreach (var sharedKey in sharedKeys)
		{
			systemColumns.Add(new TfDataProviderSystemColumn
			{
				DbName = $"tf_sk_{sharedKey.DbName}_id",
				DbType = TfDatabaseColumnType.Guid
			});

			systemColumns.Add(new TfDataProviderSystemColumn
			{
				DbName = $"tf_sk_{sharedKey.DbName}_version",
				DbType = TfDatabaseColumnType.ShortInteger
			});
		}

		return systemColumns;
	}

	/// <summary>
	/// Creates new data provider column
	/// </summary>
	/// <param name="column"></param>
	/// <returns></returns>
	public TfDataProvider CreateDataProviderColumn(
		TfDataProviderColumn column)
	{
		if (column != null && column.Id == Guid.Empty)
			column.Id = Guid.NewGuid();

		new TfDataProviderColumnValidator(this)
			.ValidateCreate(column)
			.ToValidationException()
			.ThrowIfContainsErrors();

		using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
		{
			var success = _dboManager.Insert<TfDataProviderColumn>(column);
			if (!success)
				throw new TfDboServiceException("Insert<TfDataProviderColumn> failed.");

			var provider = GetDataProvider(column.DataProviderId);
			if (provider is null)
				throw new TfException("Failed to create new data provider column");

			CreateDatabaseColumn(provider, column);

			scope.Complete();

			return provider;
		}
	}


	/// <summary>
	/// Creates new data provider columns in bulk
	/// </summary>
	/// <param name="column"></param>
	/// <returns></returns>
	public TfDataProvider CreateBulkDataProviderColumn(Guid providerId,
		List<TfDataProviderColumn> columns)
	{
		using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
		{
			List<ValidationError> validationErrors = new();
			foreach (var column in columns)
			{
				column.DataProviderId = providerId;
				CreateDataProviderColumn(column);
			}
			scope.Complete();

			var provider = GetDataProvider(providerId);
			if (provider is null)
				throw new TfException("Failed to create new data provider column");

			return provider;
		}
	}


	/// <summary>
	/// Updates existing data provider column
	/// </summary>
	/// <param name="column"></param>
	/// <returns></returns>
	public TfDataProvider UpdateDataProviderColumn(
		TfDataProviderColumn column)
	{
		new TfDataProviderColumnValidator(this)
			.ValidateUpdate(column)
			.ToValidationException()
			.ThrowIfContainsErrors();

		var existingColumn = _dboManager.Get<TfDataProviderColumn>(column.Id);

		var success = _dboManager.Update<TfDataProviderColumn>(column);

		if (!success)
			throw new TfDboServiceException("Update<TfDataProviderColumn> failed.");

		var provider = GetDataProvider(column.DataProviderId);
		if (provider is null)
			throw new TfException("Failed to create new data provider column");

		UpdateDatabaseColumn(provider, column, existingColumn);

		return GetDataProvider(column.DataProviderId);
	}

	/// <summary>
	/// Deletes existing data provider column
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	public TfDataProvider DeleteDataProviderColumn(
		Guid id)
	{
		using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
		{
			var column = GetDataProviderColumn(id);

			new TfDataProviderColumnValidator(this)
				.ValidateDelete(column)
				.ToValidationException()
				.ThrowIfContainsErrors();

			var success = _dboManager.Delete<TfDataProviderColumn>(id);

			if (!success)
				throw new TfDboServiceException("Delete<TfDataProviderColumn> failed");

			var provider = GetDataProvider(column.DataProviderId);
			if (provider is null)
				throw new TfException("Failed to create new data provider column");

			string providerTableName = $"dp{provider.Index}";

			TfDatabaseBuilder dbBuilder = _dbManager.GetDatabaseBuilder();

			dbBuilder.WithTableBuilder(providerTableName).WithColumns(columns => columns.Remove(column.DbName));

			_dbManager.SaveChanges(dbBuilder);

			scope.Complete();

			return GetDataProvider(column.DataProviderId);
		}
	}

	#region <--- utility --->

	private void CreateDatabaseColumn(
		TfDataProvider provider,
		TfDataProviderColumn column)
	{
		string providerTableName = $"dp{provider.Index}";

		TfDatabaseBuilder dbBuilder = _dbManager.GetDatabaseBuilder();
		var tableBuilder = dbBuilder.WithTableBuilder(providerTableName);
		var columnsBuilder = tableBuilder.WithColumnsBuilder();

		switch (column.DbType)
		{
			case TfDatabaseColumnType.Boolean:
				{
					columnsBuilder.AddBooleanColumn(column.DbName, c =>
					{
						if (column.IsNullable) c.Nullable(); else c.NotNullable();

						if (column.DefaultValue is not null)
							c.WithDefaultValue(Convert.ToBoolean(column.DefaultValue));
					});

					if (column.IsSearchable || column.IsSortable)
					{
						tableBuilder.WithIndexes(indexes =>
						{
							indexes.AddBTreeIndex($"ix_{providerTableName}_{column.DbName}", c => { c.WithColumns(column.DbName); });
						});
					}

				}
				break;
			case TfDatabaseColumnType.Text:
				{
					columnsBuilder.AddTextColumn(column.DbName, c =>
					{
						if (column.IsNullable) c.Nullable(); else c.NotNullable();
						c.WithDefaultValue(column.DefaultValue);
					});

					if (column.IsSearchable || column.IsSortable)
					{
						tableBuilder.WithIndexes(indexes =>
						{
							indexes.AddGinIndexBuilder($"ix_{providerTableName}_{column.DbName}", c => { c.WithColumns(column.DbName); });
						});
					}
				}
				break;
			case TfDatabaseColumnType.ShortText:
				{
					columnsBuilder.AddShortTextColumn(column.DbName, c =>
					{
						if (column.IsNullable) c.Nullable(); else c.NotNullable();
						c.WithDefaultValue(column.DefaultValue);
					});

					if (column.IsSearchable || column.IsSortable)
					{
						tableBuilder.WithIndexes(indexes =>
						{
							indexes.AddBTreeIndexBuilder($"ix_{providerTableName}_{column.DbName}", c => { c.WithColumns(column.DbName); });
						});
					}
				}
				break;
			case TfDatabaseColumnType.Guid:
				{
					columnsBuilder.AddGuidColumn(column.DbName, c =>
					{
						if (column.IsNullable) c.Nullable(); else c.NotNullable();
						if (column.AutoDefaultValue)
							c.WithAutoDefaultValue();
						else
						{
							if (column.DefaultValue is not null)
								c.WithDefaultValue(new Guid(column.DefaultValue));
						}
					});

					if (column.IsSearchable || column.IsSortable)
					{
						tableBuilder.WithIndexes(indexes =>
						{
							indexes.AddBTreeIndex($"ix_{providerTableName}_{column.DbName}", c => { c.WithColumns(column.DbName); });
						});
					}
				}
				break;
			case TfDatabaseColumnType.Date:
				{
					columnsBuilder.AddDateColumn(column.DbName, c =>
					{
						if (column.IsNullable) c.Nullable(); else c.NotNullable();
						if (column.AutoDefaultValue)
							c.WithAutoDefaultValue();
						else
						{
							if (column.DefaultValue is not null)
							{
								var datetime = DateTime.Parse(column.DefaultValue, CultureInfo.InvariantCulture);
								DateOnly date = DateOnly.FromDateTime(datetime);
								c.WithDefaultValue(date);
							}
						}
					});

					if (column.IsSearchable || column.IsSortable)
					{
						tableBuilder.WithIndexes(indexes =>
						{
							indexes.AddBTreeIndex($"ix_{providerTableName}_{column.DbName}", c => { c.WithColumns(column.DbName); });
						});
					}
				}
				break;
			case TfDatabaseColumnType.DateTime:
				{
					columnsBuilder.AddDateTimeColumn(column.DbName, c =>
					{
						if (column.IsNullable) c.Nullable(); else c.NotNullable();
						if (column.AutoDefaultValue)
							c.WithAutoDefaultValue();
						else
						{
							if (column.DefaultValue is not null)
							{
								var datetime = DateTime.Parse(column.DefaultValue, CultureInfo.InvariantCulture);
								c.WithDefaultValue(datetime);
							}
						}
					});

					if (column.IsSearchable || column.IsSortable)
					{
						tableBuilder.WithIndexes(indexes =>
						{
							indexes.AddBTreeIndex($"ix_{providerTableName}_{column.DbName}", c => { c.WithColumns(column.DbName); });
						});
					}
				}
				break;
			case TfDatabaseColumnType.Number:
				{
					columnsBuilder.AddNumberColumn(column.DbName, c =>
					{
						if (column.IsNullable) c.Nullable(); else c.NotNullable();
						if (column.DefaultValue is not null)
						{
							var number = Convert.ToDecimal(column.DefaultValue, CultureInfo.InvariantCulture);
							c.WithDefaultValue(number);
						}
					});
					if (column.IsSearchable || column.IsSortable)
					{
						tableBuilder.WithIndexes(indexes =>
						{
							indexes.AddBTreeIndex($"ix_{providerTableName}_{column.DbName}", c => { c.WithColumns(column.DbName); });
						});
					}
				}
				break;
			case TfDatabaseColumnType.ShortInteger:
				{
					columnsBuilder.AddShortIntegerColumn(column.DbName, c =>
					{
						if (column.IsNullable) c.Nullable(); else c.NotNullable();
						if (column.DefaultValue is not null)
						{
							var number = short.Parse(column.DefaultValue);
							c.WithDefaultValue(number);
						}
					});
					if (column.IsSearchable || column.IsSortable)
					{
						tableBuilder.WithIndexes(indexes =>
						{
							indexes.AddBTreeIndex($"ix_{providerTableName}_{column.DbName}", c => { c.WithColumns(column.DbName); });
						});
					}
				}
				break;
			case TfDatabaseColumnType.Integer:
				{
					columnsBuilder.AddIntegerColumn(column.DbName, c =>
					{
						if (column.IsNullable) c.Nullable(); else c.NotNullable();
						if (column.DefaultValue is not null)
						{
							var number = int.Parse(column.DefaultValue);
							c.WithDefaultValue(number);
						}
					});
					if (column.IsSearchable || column.IsSortable)
					{
						tableBuilder.WithIndexes(indexes =>
						{
							indexes.AddBTreeIndex($"ix_{providerTableName}_{column.DbName}", c => { c.WithColumns(column.DbName); });
						});
					}
				}
				break;
			case TfDatabaseColumnType.LongInteger:
				{
					columnsBuilder.AddLongIntegerColumn(column.DbName, c =>
					{
						if (column.IsNullable) c.Nullable(); else c.NotNullable();
						if (column.DefaultValue is not null)
						{
							var number = long.Parse(column.DefaultValue);
							c.WithDefaultValue(number);
						}
					});
					if (column.IsSearchable || column.IsSortable)
					{
						tableBuilder.WithIndexes(indexes =>
						{
							indexes.AddBTreeIndex($"ix_{providerTableName}_{column.DbName}", c => { c.WithColumns(column.DbName); });
						});
					}
				}
				break;
			default:
				throw new Exception("Not supported database column type");
		}

		if (column.IsUnique)
		{
			tableBuilder.WithConstraints(constraints =>
			{
				constraints.AddUniqueKeyConstraintBuilder($"ux_{providerTableName}_{column.DbName}",
					c => { c.WithColumns(column.DbName); });
			});
		}

		var result = _dbManager.SaveChanges(dbBuilder);
		if (!result.IsSuccess)
			throw new TfException("Failed to save changes to database schema");
	}

	private void UpdateDatabaseColumn(
		TfDataProvider provider,
		TfDataProviderColumn column,
		TfDataProviderColumn existingColumn)
	{
		string providerTableName = $"dp{provider.Index}";

		TfDatabaseBuilder dbBuilder = _dbManager.GetDatabaseBuilder();
		var tableBuilder = dbBuilder.WithTableBuilder(providerTableName);
		var columnsBuilder = tableBuilder.WithColumnsBuilder();

		switch (column.DbType)
		{
			case TfDatabaseColumnType.Boolean:
				{
					columnsBuilder.WithBooleanColumn(column.DbName, c =>
					{
						if (column.IsNullable) c.Nullable(); else c.NotNullable();

						if (existingColumn.DefaultValue != column.DefaultValue && column.DefaultValue is not null)
						{
							c.WithDefaultValue(Convert.ToBoolean(column.DefaultValue));
						}
					});

					string indexName = $"ix_{providerTableName}_{column.DbName}";

					if ((!existingColumn.IsSearchable && !existingColumn.IsSortable) &&
						(column.IsSearchable || column.IsSortable))
					{
						tableBuilder.WithIndexes(indexes =>
						{
							indexes.AddGinIndexBuilder(indexName, c => { c.WithColumns(column.DbName); });
						});
					}

					if ((existingColumn.IsSearchable || existingColumn.IsSortable) &&
						(!column.IsSearchable && !column.IsSortable))
					{
						tableBuilder.WithIndexes(indexes => { indexes.Remove(indexName); });
					}

				}
				break;
			case TfDatabaseColumnType.Text:
				{
					columnsBuilder.WithTextColumn(column.DbName, c =>
					{
						if (column.IsNullable) c.Nullable(); else c.NotNullable();

						if (existingColumn.DefaultValue != column.DefaultValue && column.DefaultValue is not null)
						{
							c.WithDefaultValue(column.DefaultValue);
						}
					});


					string indexName = $"ix_{providerTableName}_{column.DbName}";

					if ((!existingColumn.IsSearchable && !existingColumn.IsSortable) &&
						(column.IsSearchable || column.IsSortable))
					{
						tableBuilder.WithIndexes(indexes =>
						{
							indexes.AddGinIndexBuilder(indexName, c => { c.WithColumns(column.DbName); });
						});
					}

					if ((existingColumn.IsSearchable || existingColumn.IsSortable) &&
						(!column.IsSearchable && !column.IsSortable))
					{
						tableBuilder.WithIndexes(indexes => { indexes.Remove(indexName); });
					}

				}
				break;
			case TfDatabaseColumnType.ShortText:
				{
					columnsBuilder.WithShortTextColumn(column.DbName, c =>
					{
						if (column.IsNullable) c.Nullable(); else c.NotNullable();

						if (existingColumn.DefaultValue != column.DefaultValue && column.DefaultValue is not null)
						{
							c.WithDefaultValue(column.DefaultValue);
						}
					});

					string indexName = $"ix_{providerTableName}_{column.DbName}";

					if ((!existingColumn.IsSearchable && !existingColumn.IsSortable) &&
						(column.IsSearchable || column.IsSortable))
					{
						tableBuilder.WithIndexes(indexes =>
						{
							indexes.AddBTreeIndexBuilder(indexName, c => { c.WithColumns(column.DbName); });
						});
					}

					if ((existingColumn.IsSearchable || existingColumn.IsSortable) &&
						(!column.IsSearchable && !column.IsSortable))
					{
						tableBuilder.WithIndexes(indexes => { indexes.Remove(indexName); });
					}

				}
				break;
			case TfDatabaseColumnType.Guid:
				{
					columnsBuilder.WithGuidColumn(column.DbName, c =>
					{
						if (column.IsNullable) c.Nullable(); else c.NotNullable();
						if (column.AutoDefaultValue)
							c.WithAutoDefaultValue();
						else
						{
							if (column.DefaultValue is not null)
								c.WithDefaultValue(new Guid(column.DefaultValue));
						}
					});

					string indexName = $"ix_{providerTableName}_{column.DbName}";

					if ((!existingColumn.IsSearchable && !existingColumn.IsSortable) &&
						(column.IsSearchable || column.IsSortable))
					{
						tableBuilder.WithIndexes(indexes =>
						{
							indexes.AddBTreeIndexBuilder(indexName, c => { c.WithColumns(column.DbName); });
						});
					}

					if ((existingColumn.IsSearchable || existingColumn.IsSortable) &&
						(!column.IsSearchable && !column.IsSortable))
					{
						tableBuilder.WithIndexes(indexes => { indexes.Remove(indexName); });
					}
				}
				break;
			case TfDatabaseColumnType.Date:
				{
					columnsBuilder.WithDateColumn(column.DbName, c =>
					{
						if (column.IsNullable) c.Nullable(); else c.NotNullable();
						if (column.AutoDefaultValue)
							c.WithAutoDefaultValue();
						else
						{
							if (column.DefaultValue is not null)
							{
								var datetime = DateTime.Parse(column.DefaultValue, CultureInfo.InvariantCulture);
								DateOnly date = DateOnly.FromDateTime(datetime);
								c.WithDefaultValue(date);
							}
						}
					});

					string indexName = $"ix_{providerTableName}_{column.DbName}";

					if ((!existingColumn.IsSearchable && !existingColumn.IsSortable) &&
						(column.IsSearchable || column.IsSortable))
					{
						tableBuilder.WithIndexes(indexes =>
						{
							indexes.AddBTreeIndexBuilder(indexName, c => { c.WithColumns(column.DbName); });
						});
					}

					if ((existingColumn.IsSearchable || existingColumn.IsSortable) &&
						(!column.IsSearchable && !column.IsSortable))
					{
						tableBuilder.WithIndexes(indexes => { indexes.Remove(indexName); });
					}
				}
				break;
			case TfDatabaseColumnType.DateTime:
				{
					columnsBuilder.WithDateTimeColumn(column.DbName, c =>
					{
						if (column.IsNullable) c.Nullable(); else c.NotNullable();
						if (column.AutoDefaultValue)
							c.WithAutoDefaultValue();
						else
						{
							if (column.DefaultValue is not null)
							{
								var datetime = DateTime.Parse(column.DefaultValue, CultureInfo.InvariantCulture);
								c.WithDefaultValue(datetime);
							}
						}
					});

					string indexName = $"ix_{providerTableName}_{column.DbName}";

					if ((!existingColumn.IsSearchable && !existingColumn.IsSortable) &&
						(column.IsSearchable || column.IsSortable))
					{
						tableBuilder.WithIndexes(indexes =>
						{
							indexes.AddBTreeIndexBuilder(indexName, c => { c.WithColumns(column.DbName); });
						});
					}

					if ((existingColumn.IsSearchable || existingColumn.IsSortable) &&
						(!column.IsSearchable && !column.IsSortable))
					{
						tableBuilder.WithIndexes(indexes => { indexes.Remove(indexName); });
					}
				}
				break;
			case TfDatabaseColumnType.Number:
				{
					columnsBuilder.WithNumberColumn(column.DbName, c =>
					{
						if (column.IsNullable) c.Nullable(); else c.NotNullable();
						if (column.DefaultValue is not null)
						{
							var number = Convert.ToDecimal(column.DefaultValue, CultureInfo.InvariantCulture);
							c.WithDefaultValue(number);
						}
					});

					string indexName = $"ix_{providerTableName}_{column.DbName}";

					if ((!existingColumn.IsSearchable && !existingColumn.IsSortable) &&
						(column.IsSearchable || column.IsSortable))
					{
						tableBuilder.WithIndexes(indexes =>
						{
							indexes.AddBTreeIndexBuilder(indexName, c => { c.WithColumns(column.DbName); });
						});
					}

					if ((existingColumn.IsSearchable || existingColumn.IsSortable) &&
						(!column.IsSearchable && !column.IsSortable))
					{
						tableBuilder.WithIndexes(indexes => { indexes.Remove(indexName); });
					}
				}
				break;
			case TfDatabaseColumnType.ShortInteger:
				{
					columnsBuilder.WithShortIntegerColumn(column.DbName, c =>
					{
						if (column.IsNullable) c.Nullable(); else c.NotNullable();
						if (column.DefaultValue is not null)
						{
							var number = short.Parse(column.DefaultValue);
							c.WithDefaultValue(number);
						}
					});

					string indexName = $"ix_{providerTableName}_{column.DbName}";

					if ((!existingColumn.IsSearchable && !existingColumn.IsSortable) &&
						(column.IsSearchable || column.IsSortable))
					{
						tableBuilder.WithIndexes(indexes =>
						{
							indexes.AddBTreeIndexBuilder(indexName, c => { c.WithColumns(column.DbName); });
						});
					}

					if ((existingColumn.IsSearchable || existingColumn.IsSortable) &&
						(!column.IsSearchable && !column.IsSortable))
					{
						tableBuilder.WithIndexes(indexes => { indexes.Remove(indexName); });
					}
				}
				break;
			case TfDatabaseColumnType.Integer:
				{
					columnsBuilder.WithIntegerColumn(column.DbName, c =>
					{
						if (column.IsNullable) c.Nullable(); else c.NotNullable();
						if (column.DefaultValue is not null)
						{
							var number = int.Parse(column.DefaultValue);
							c.WithDefaultValue(number);
						}
					});

					string indexName = $"ix_{providerTableName}_{column.DbName}";

					if ((!existingColumn.IsSearchable && !existingColumn.IsSortable) &&
						(column.IsSearchable || column.IsSortable))
					{
						tableBuilder.WithIndexes(indexes =>
						{
							indexes.AddBTreeIndexBuilder(indexName, c => { c.WithColumns(column.DbName); });
						});
					}

					if ((existingColumn.IsSearchable || existingColumn.IsSortable) &&
						(!column.IsSearchable && !column.IsSortable))
					{
						tableBuilder.WithIndexes(indexes => { indexes.Remove(indexName); });
					}
				}
				break;
			case TfDatabaseColumnType.LongInteger:
				{
					columnsBuilder.WithLongIntegerColumn(column.DbName, c =>
					{
						if (column.IsNullable) c.Nullable(); else c.NotNullable();
						if (column.DefaultValue is not null)
						{
							var number = long.Parse(column.DefaultValue);
							c.WithDefaultValue(number);
						}
					});

					string indexName = $"ix_{providerTableName}_{column.DbName}";

					if ((!existingColumn.IsSearchable && !existingColumn.IsSortable) &&
						(column.IsSearchable || column.IsSortable))
					{
						tableBuilder.WithIndexes(indexes =>
						{
							indexes.AddBTreeIndexBuilder(indexName, c => { c.WithColumns(column.DbName); });
						});
					}

					if ((existingColumn.IsSearchable || existingColumn.IsSortable) &&
						(!column.IsSearchable && !column.IsSortable))
					{
						tableBuilder.WithIndexes(indexes => { indexes.Remove(indexName); });
					}
				}
				break;
			default:
				throw new Exception("Not supported database column type");
		}

		if (column.IsUnique && !existingColumn.IsUnique)
		{
			tableBuilder.WithConstraints(constraints =>
			{
				constraints.AddUniqueKeyConstraintBuilder($"ux_{providerTableName}_{column.DbName}",
					c => { c.WithColumns(column.DbName); });
			});
		}

		if (!column.IsUnique && existingColumn.IsUnique)
		{
			tableBuilder.WithConstraints(constraints =>
			{
				constraints.Remove($"ux_{providerTableName}_{column.DbName}");
			});
		}

		var result = _dbManager.SaveChanges(dbBuilder);
		if (!result.IsSuccess)
			throw new TfException("Failed to save changes to database schema");
	}
	#endregion

	public static ReadOnlyCollection<DatabaseColumnTypeInfo> GetDatabaseColumnTypeInfosList()
	{
		List<DatabaseColumnTypeInfo> databaseColumnTypeInfos =
			new List<DatabaseColumnTypeInfo>();

		databaseColumnTypeInfos.Add(
			new DatabaseColumnTypeInfo
			{
				Name = "AUTO INCREMENT",
				Type = TfDatabaseColumnType.AutoIncrement,
				CanBeProviderDataType = false,
				SupportAutoDefaultValue = false
			});

		databaseColumnTypeInfos.Add(
			new DatabaseColumnTypeInfo
			{
				Name = "GUID",
				Type = TfDatabaseColumnType.Guid,
				CanBeProviderDataType = true,
				SupportAutoDefaultValue = true
			});

		databaseColumnTypeInfos.Add(
			new DatabaseColumnTypeInfo
			{
				Name = "DATE",
				Type = TfDatabaseColumnType.Date,
				CanBeProviderDataType = true,
				SupportAutoDefaultValue = true
			});

		databaseColumnTypeInfos.Add(
			new DatabaseColumnTypeInfo
			{
				Name = "DATE AND TIME",
				Type = TfDatabaseColumnType.DateTime,
				CanBeProviderDataType = true,
				SupportAutoDefaultValue = true
			});


		databaseColumnTypeInfos.Add(
			new DatabaseColumnTypeInfo
			{
				Name = "BOOLEAN",
				Type = TfDatabaseColumnType.Boolean,
				CanBeProviderDataType = true,
				SupportAutoDefaultValue = false
			});

		databaseColumnTypeInfos.Add(
			new DatabaseColumnTypeInfo
			{
				Name = "TEXT",
				Type = TfDatabaseColumnType.Text,
				CanBeProviderDataType = true,
				SupportAutoDefaultValue = false
			});

		databaseColumnTypeInfos.Add(
			new DatabaseColumnTypeInfo
			{
				Name = "SHORT TEXT",
				Type = TfDatabaseColumnType.ShortText,
				CanBeProviderDataType = true,
				SupportAutoDefaultValue = false
			});

		databaseColumnTypeInfos.Add(
			new DatabaseColumnTypeInfo
			{
				Name = "SHORT INTEGER (16bit)",
				Type = TfDatabaseColumnType.ShortInteger,
				CanBeProviderDataType = true,
				SupportAutoDefaultValue = false
			});

		databaseColumnTypeInfos.Add(
			new DatabaseColumnTypeInfo
			{
				Name = "INTEGER (32bit)",
				Type = TfDatabaseColumnType.Integer,
				CanBeProviderDataType = true,
				SupportAutoDefaultValue = false
			});

		databaseColumnTypeInfos.Add(
			new DatabaseColumnTypeInfo
			{
				Name = "LONG INTEGER (64bit)",
				Type = TfDatabaseColumnType.LongInteger,
				CanBeProviderDataType = true,
				SupportAutoDefaultValue = false
			});

		databaseColumnTypeInfos.Add(
			new DatabaseColumnTypeInfo
			{
				Name = "DECIMAL",
				Type = TfDatabaseColumnType.Number,
				CanBeProviderDataType = true,
				SupportAutoDefaultValue = false
			});

		return databaseColumnTypeInfos.AsReadOnly();
	}
	public ReadOnlyCollection<DatabaseColumnTypeInfo> GetDatabaseColumnTypeInfos()
	{
		return GetDatabaseColumnTypeInfosList();
	}

	#region <--- validation --->

	internal class TfDataProviderColumnValidator
	: AbstractValidator<TfDataProviderColumn>
	{
		public TfDataProviderColumnValidator(
			ITfService tfService)
		{
			RuleSet("general", () =>
			{
				RuleFor(column => column.Id)
					.NotEmpty()
					.WithMessage("The data provider column id is required.");

				RuleFor(column => column.DataProviderId)
					.NotEmpty()
					.WithMessage("The data provider id is required.");

				RuleFor(column => column.DataProviderId)
					.Must(providerId =>
					{
						return tfService.GetDataProvider(providerId) != null;
					})
					.WithMessage("There is no existing data provider for specified provider id.");

				RuleFor(column => column.SourceType)
					.NotEmpty()
					.WithMessage("The data provider column source type is required.");

				RuleFor(column => column.SourceType)
					.Must((column, sourceType) =>
					{
						if (string.IsNullOrWhiteSpace(sourceType))
							return true;

						var provider = tfService.GetDataProvider(column.DataProviderId);
						if (provider is null)
							return true;

						var supportedSourceTypes = provider.ProviderType.GetSupportedSourceDataTypes();

						return supportedSourceTypes.Any(x => x == sourceType);

					})
					.WithMessage($"Selected source type is not in the list of provider supported source types.");

				RuleFor(column => column.SourceType)
					.Must((column, sourceType) =>
					{
						if (string.IsNullOrWhiteSpace(sourceType))
							return true;

						var provider = tfService.GetDataProvider(column.DataProviderId);
						if (provider is null)
							return true;

						var supportedDatabaseColumnTypes =
							provider.ProviderType.GetDatabaseColumnTypesForSourceDataType(sourceType);

						return supportedDatabaseColumnTypes.Any();

					})
					.WithMessage($"Selected source type does not provide any supported provider data type.");

				RuleFor(column => column.SourceType)
					.Must((column, sourceType) =>
					{
						if (string.IsNullOrWhiteSpace(sourceType))
							return true;

						var provider = tfService.GetDataProvider(column.DataProviderId);
						if (provider is null)
							return true;

						var supportedDatabaseColumnTypes =
							provider.ProviderType.GetDatabaseColumnTypesForSourceDataType(sourceType);

						return supportedDatabaseColumnTypes.Any(x => x == column.DbType);

					})
					.WithMessage($"The selected source type is not supported for use with selected provider data type.");

				RuleFor(column => column.DbName)
					.NotEmpty()
					.WithMessage("The data provider column database name is required.");

				RuleFor(column => column.DbName)
					.Must((column, dbName) =>
					{
						if (string.IsNullOrWhiteSpace(dbName))
							return true;

						return !dbName.StartsWith("tf_");
					})
					.WithMessage("The data provider column database name cannot start with 'tf_'.");

				RuleFor(column => column.DbName)
					.Must((column, dbName) =>
					{
						if (string.IsNullOrWhiteSpace(dbName))
							return true;

						return dbName.Length >= Constants.DB_MIN_OBJECT_NAME_LENGTH;
					})
					.WithMessage($"The database name must be at least {Constants.DB_MIN_OBJECT_NAME_LENGTH} characters long.");

				RuleFor(column => column.DbName)
					.Must((column, dbName) =>
					{
						if (string.IsNullOrWhiteSpace(dbName))
							return true;

						return dbName.Length <= Constants.DB_MAX_OBJECT_NAME_LENGTH;
					})
					.WithMessage($"The length of database name must be less or equal than {Constants.DB_MAX_OBJECT_NAME_LENGTH} characters");

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
					.WithMessage($"Name can only contains underscores and lowercase alphanumeric characters. It must begin with a letter, " +
						$"not include spaces, not end with an underscore, and not contain two consecutive underscores");

				RuleFor(column => column.DefaultValue)
					.Must((column, defaultValue) =>
					{
						if (!column.IsNullable && defaultValue is null)
							return false;

						return true;
					})
					.WithMessage($"Column is marked as not nullable, but no default value is specified.");

				RuleFor(column => column.DefaultValue)
					.Must((column, defaultValue) =>
					{
						if (defaultValue == null)
							return true;

						try
						{
							switch (column.DbType)
							{
								case TfDatabaseColumnType.Boolean:
									{
										if (column.DefaultValue is not null)
										{
											var booleanValue = Convert.ToBoolean(column.DefaultValue);
										}
									}
									break;
								case TfDatabaseColumnType.Text:
								case TfDatabaseColumnType.ShortText:
									break;
								case TfDatabaseColumnType.Guid:
									{
										if (column.AutoDefaultValue == false && column.DefaultValue is not null)
										{
											var guid = Guid.Parse(column.DefaultValue);
										}
									}
									break;
								case TfDatabaseColumnType.Date:
									{
										if (column.AutoDefaultValue == false && column.DefaultValue is not null)
										{
											var date = DateOnly.Parse(column.DefaultValue, CultureInfo.InvariantCulture);
										}
									}
									break;
								case TfDatabaseColumnType.DateTime:
									{
										if (column.AutoDefaultValue == false && column.DefaultValue is not null)
										{
											var datetime = DateTime.Parse(column.DefaultValue, CultureInfo.InvariantCulture);
										}
									}
									break;
								case TfDatabaseColumnType.Number:
									{
										if (column.DefaultValue is not null)
										{
											var number = Convert.ToDecimal(column.DefaultValue, CultureInfo.InvariantCulture);
										}
									}
									break;
								case TfDatabaseColumnType.ShortInteger:
									{
										if (column.DefaultValue is not null)
										{
											short number = Convert.ToInt16(column.DefaultValue);
										}

									}
									break;
								case TfDatabaseColumnType.Integer:
									{
										if (column.DefaultValue is not null)
										{
											int number = Convert.ToInt32(column.DefaultValue);
										}

									}
									break;
								case TfDatabaseColumnType.LongInteger:
									{
										if (column.DefaultValue is not null)
										{
											long number = Convert.ToInt64(column.DefaultValue);
										}

									}
									break;
								default:
									throw new Exception("Not supported database column type while validate default value.");
							}
							return true;
						}
						catch
						{
							return false;
						}
					})
					.WithMessage($"Column is marked not nullable. Default value is required. Specified default value is empty or not correct for selected provider data type.");

			});

			RuleSet("create", () =>
			{
				RuleFor(column => column.Id)
						.Must((column, id) => { return tfService.GetDataProviderColumn(id) == null; })
						.WithMessage("There is already existing data provider column with specified identifier.");

				RuleFor(column => column.DbName)
						.Must((column, dbName) =>
						{
							if (string.IsNullOrEmpty(dbName))
								return true;

							var columns = tfService.GetDataProviderColumns(column.DataProviderId);
							return !columns.Any(x => x.DbName.ToLowerInvariant().Trim() == dbName.ToLowerInvariant().Trim());
						})
						.WithMessage("There is already existing data provider column with specified database name.");
			});

			RuleSet("update", () =>
			{
				RuleFor(column => column.Id)
						.Must((column, id) =>
						{
							return tfService.GetDataProviderColumn(id) != null;
						})
						.WithMessage("There is not existing data provider column with specified identifier.");

				RuleFor(column => column.DataProviderId)
						.Must((column, providerId) =>
						{

							var existingColumn = tfService.GetDataProviderColumn(column.Id);
							if (existingColumn is null)
								return true;

							return existingColumn.DataProviderId == providerId;
						})
						.WithMessage("There data provider cannot be changed for data provider column.");

				RuleFor(column => column.DbName)
						.Must((column, dbName) =>
						{

							var existingColumn = tfService.GetDataProviderColumn(column.Id);
							if (existingColumn is null)
								return true;

							return existingColumn.DbName == dbName;
						})
						.WithMessage("There database name of column cannot be changed.");

				RuleFor(column => column.DbType)
					.Must((column, dbType) =>
					{

						var existingColumn = tfService.GetDataProviderColumn(column.Id);
						if (existingColumn is null)
							return true;

						return existingColumn.DbType == dbType;
					})
					.WithMessage("There database type of column cannot be changed.");

			});

			RuleSet("delete", () =>
			{
				RuleFor(column => column.Id)
						.Must((column, id) =>
						{
							var sharedKeys = tfService.GetDataProviderSharedKeys(column.DataProviderId);
							var found = sharedKeys.Any(x => x.Columns.Any(c => c.Id == id));
							return !found;
						})
						.WithMessage("There data provider column cannot be deleted, because it is part of shared key.");
			});

		}

		public ValidationResult ValidateCreate(
			TfDataProviderColumn column)
		{
			if (column == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The data provider column is null.") });

			return this.Validate(column, options =>
			{
				options.IncludeRuleSets("general", "create");
			});
		}

		public ValidationResult ValidateUpdate(
			TfDataProviderColumn column)
		{
			if (column == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The data provider column is null.") });

			return this.Validate(column, options =>
			{
				options.IncludeRuleSets("general", "update");
			});
		}

		public ValidationResult ValidateDelete(
			TfDataProviderColumn column)
		{
			if (column == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The data provider column with specified identifier is not found.") });

			return this.Validate(column, options =>
			{
				options.IncludeRuleSets("delete");
			});
		}
	}

	#endregion

	#endregion

	#region <--- Shared Keys --->

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

	/// <summary>
	/// Creates new shared key
	/// </summary>
	/// <param name="sharedKey"></param>
	/// <returns></returns>
	public TfDataProvider CreateDataProviderSharedKey(
		TfDataProviderSharedKey sharedKey)
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

	/// <summary>
	/// Updates an existing shared key
	/// </summary>
	/// <param name="sharedKey"></param>
	/// <returns></returns>
	public TfDataProvider UpdateDataProviderSharedKey(
		TfDataProviderSharedKey sharedKey)
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


	/// <summary>
	/// Deletes and existing shared key for specified identifier
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	public TfDataProvider DeleteDataProviderSharedKey(
		Guid id)
	{
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

	#endregion

	#region <--- Synchronization --->

	#region <--- Synchronization Tasks --->

	public TfDataProviderSynchronizeTask GetSynchronizationTask(
		Guid taskId)
	{
		var dbo = _dboManager.Get<TfDataProviderSynchronizeTaskDbo>(taskId);

		if (dbo == null)
			return null;

		return new TfDataProviderSynchronizeTask
		{
			Id = dbo.Id,
			DataProviderId = dbo.DataProviderId,
			CompletedOn = dbo.CompletedOn,
			CreatedOn = dbo.CreatedOn,
			Policy = JsonSerializer.Deserialize<TfSynchronizationPolicy>(dbo.PolicyJson),
			StartedOn = dbo.StartedOn,
			Status = dbo.Status,
		};
	}

	public List<TfDataProviderSynchronizeTask> GetSynchronizationTasks(
		Guid? providerId = null,
		TfSynchronizationStatus? status = null)
	{
		var orderSettings = new TfOrderSettings(
		nameof(TfDataProviderSynchronizeTaskDbo.CreatedOn),
		OrderDirection.ASC);

		List<TfDataProviderSynchronizeTaskDbo> dbos = null;
		if (providerId is not null && status is not null)
		{
			dbos = _dboManager.GetList<TfDataProviderSynchronizeTaskDbo>(
				"WHERE data_provider_id = @data_provider_id AND status = @status",
				orderSettings,
				new NpgsqlParameter("@data_provider_id", providerId.Value),
				new NpgsqlParameter("@status", (short)status.Value));

		}
		else if (providerId is not null)
		{
			dbos = _dboManager.GetList<TfDataProviderSynchronizeTaskDbo>(
				"WHERE data_provider_id = @data_provider_id ",
				orderSettings,
				new NpgsqlParameter("@data_provider_id", providerId.Value));

		}
		else if (status is not null)
		{
			dbos = _dboManager.GetList<TfDataProviderSynchronizeTaskDbo>(
				"WHERE status = @status",
				orderSettings,
				new NpgsqlParameter("@status", (short)status.Value));
		}
		else
		{
			dbos = _dboManager.GetList<TfDataProviderSynchronizeTaskDbo>(order: orderSettings);
		}

		var result = new List<TfDataProviderSynchronizeTask>();

		foreach (var dbo in dbos)
		{
			var task = new TfDataProviderSynchronizeTask
			{
				Id = dbo.Id,
				DataProviderId = dbo.DataProviderId,
				CompletedOn = dbo.CompletedOn,
				CreatedOn = dbo.CreatedOn,
				Policy = JsonSerializer.Deserialize<TfSynchronizationPolicy>(dbo.PolicyJson),
				StartedOn = dbo.StartedOn,
				Status = dbo.Status,
			};
			result.Add(task);
		}

		return result;
	}

	public TfDataProviderSynchronizeTaskExtended GetSynchronizationTaskExtended(
		Guid taskId)
	{
		TfDataProviderSynchronizeTaskExtended dbo =
			_dboManager.GetBySql<TfDataProviderSynchronizeTaskExtended>(
@"SELECT
	st.id,
	st.data_provider_id,
	st.policy_json,
	st.status,
	st.created_on,
	st.started_on,
	st.completed_on,
	(  SELECT COUNT(id) FROM data_provider_synchronize_result_info sri WHERE sri.task_id = st.id AND sri.info IS NOT NULL ) AS info_count,
	(  SELECT COUNT(id) FROM data_provider_synchronize_result_info sri WHERE sri.task_id = st.id AND sri.warning IS NOT NULL ) AS warning_count,
	(  SELECT COUNT(id) FROM data_provider_synchronize_result_info sri WHERE sri.task_id = st.id AND sri.error IS NOT NULL ) AS error_count
FROM data_provider_synchronize_task st
WHERE st.id = @task_id 
GROUP BY
	st.id,
	st.data_provider_id,
	st.policy_json,
	st.status,
	st.created_on,
	st.started_on,
	st.completed_on
ORDER BY st.created_on DESC",
				new NpgsqlParameter("@task_id", taskId));

		return dbo;
	}

	public List<TfDataProviderSynchronizeTaskExtended> GetSynchronizationTasksExtended(
		Guid? providerId = null,
		TfSynchronizationStatus? status = null)
	{
		List<TfDataProviderSynchronizeTaskExtended> dbos = null;
		if (providerId is not null && status is not null)
		{
			dbos = _dboManager.GetListBySql<TfDataProviderSynchronizeTaskExtended>(
@"SELECT
	st.id,
	st.data_provider_id,
	st.policy_json,
	st.status,
	st.created_on,
	st.started_on,
	st.completed_on,
	(  SELECT COUNT(id) FROM data_provider_synchronize_result_info sri WHERE sri.task_id = st.id AND sri.info IS NOT NULL ) AS info_count,
	(  SELECT COUNT(id) FROM data_provider_synchronize_result_info sri WHERE sri.task_id = st.id AND sri.warning IS NOT NULL ) AS warning_count,
	(  SELECT COUNT(id) FROM data_provider_synchronize_result_info sri WHERE sri.task_id = st.id AND sri.error IS NOT NULL ) AS error_count
FROM data_provider_synchronize_task st
WHERE data_provider_id = @data_provider_id AND status = @status
GROUP BY 
	st.id, 
	st.data_provider_id, 
	st.policy_json, 
	st.status, 
	st.created_on,
	st.started_on,
	st.completed_on
ORDER BY st.created_on DESC",
				new NpgsqlParameter("@data_provider_id", providerId.Value),
				new NpgsqlParameter("@status", (short)status.Value));

		}
		else if (providerId is not null)
		{
			dbos = _dboManager.GetListBySql<TfDataProviderSynchronizeTaskExtended>(
@"SELECT
	st.id,
	st.data_provider_id,
	st.policy_json,
	st.status,
	st.created_on,
	st.started_on,
	st.completed_on,
	(  SELECT COUNT(id) FROM data_provider_synchronize_result_info sri WHERE sri.task_id = st.id AND sri.info IS NOT NULL ) AS info_count,
	(  SELECT COUNT(id) FROM data_provider_synchronize_result_info sri WHERE sri.task_id = st.id AND sri.warning IS NOT NULL ) AS warning_count,
	(  SELECT COUNT(id) FROM data_provider_synchronize_result_info sri WHERE sri.task_id = st.id AND sri.error IS NOT NULL ) AS error_count
FROM data_provider_synchronize_task st
WHERE data_provider_id = @data_provider_id 
GROUP BY
	st.id,
	st.data_provider_id,
	st.policy_json,
	st.status,
	st.created_on,
	st.started_on,
	st.completed_on
ORDER BY st.created_on DESC",
				new NpgsqlParameter("@data_provider_id", providerId.Value));

		}
		else if (status is not null)
		{
			dbos = _dboManager.GetListBySql<TfDataProviderSynchronizeTaskExtended>(
@"SELECT 
	st.id, 
	st.data_provider_id, 
	st.policy_json, 
	st.status, 
	st.created_on,
	st.started_on,
	st.completed_on,
	COUNT( sri_info.id ) AS info_count,
	COUNT( sri_warning.id ) AS warning_count,
	COUNT( sri_error.id ) AS error_count
FROM data_provider_synchronize_task st
	LEFT OUTER JOIN data_provider_synchronize_result_info sri_info ON sri_info.task_id = st.id AND sri_info.info IS NOT NULL
	LEFT OUTER JOIN data_provider_synchronize_result_info sri_warning ON sri_warning.task_id = st.id AND sri_warning.warning IS NOT NULL
	LEFT OUTER JOIN data_provider_synchronize_result_info sri_error ON sri_error.task_id = st.id AND sri_error.error IS NOT NULL
WHERE status = @status
GROUP BY 
	st.id, 
	st.data_provider_id, 
	st.policy_json, 
	st.status, 
	st.created_on,
	st.started_on,
	st.completed_on
ORDER BY st.created_on DESC",
				new NpgsqlParameter("@status", (short)status.Value));
		}
		else
		{
			dbos = _dboManager.GetListBySql<TfDataProviderSynchronizeTaskExtended>(
@"SELECT
	st.id,
	st.data_provider_id,
	st.policy_json,
	st.status,
	st.created_on,
	st.started_on,
	st.completed_on,
	(  SELECT COUNT(id) FROM data_provider_synchronize_result_info sri WHERE sri.task_id = st.id AND sri.info IS NOT NULL ) AS info_count,
	(  SELECT COUNT(id) FROM data_provider_synchronize_result_info sri WHERE sri.task_id = st.id AND sri.warning IS NOT NULL ) AS warning_count,
	(  SELECT COUNT(id) FROM data_provider_synchronize_result_info sri WHERE sri.task_id = st.id AND sri.error IS NOT NULL ) AS error_count
FROM data_provider_synchronize_task st
GROUP BY 
	st.id, 
	st.data_provider_id, 
	st.policy_json, 
	st.status, 
	st.created_on,
	st.started_on,
	st.completed_on
ORDER BY st.created_on DESC");
		}

		return dbos;
	}

	public Guid CreateSynchronizationTask(
		Guid providerId,
		TfSynchronizationPolicy synchPolicy)
	{
		var task = new TfDataProviderSynchronizeTaskDbo
		{
			Id = Guid.NewGuid(),
			DataProviderId = providerId,
			PolicyJson = JsonSerializer.Serialize(synchPolicy),
			Status = TfSynchronizationStatus.Pending,
			CreatedOn = DateTime.Now,
			CompletedOn = null,
			StartedOn = DateTime.Now
		};

		var success = _dboManager.Insert<TfDataProviderSynchronizeTaskDbo>(task);
		if (!success)
			throw new TfDatabaseException("Failed to insert synchronization task.");

		return task.Id;
	}

	public void UpdateSychronizationTask(
		Guid taskId,
		TfSynchronizationStatus status,
		DateTime? startedOn = null,
		DateTime? completedOn = null)
	{
		var dbo = _dboManager.Get<TfDataProviderSynchronizeTaskDbo>(taskId);
		if (dbo == null)
			throw new Exception("Synchronization task was not found.");

		dbo.Status = status;
		if (startedOn is not null)
			dbo.StartedOn = startedOn;
		if (completedOn is not null)
			dbo.CompletedOn = completedOn;

		var success = _dboManager.Update<TfDataProviderSynchronizeTaskDbo>(dbo);
		if (!success)
			throw new TfDatabaseException("Failed to update synchronization task in database.");
	}

	#endregion

	public TfDataProviderSourceSchemaInfo GetDataProviderSourceSchemaInfo(
		Guid providerId)
	{
		var result = new TfDataProviderSourceSchemaInfo();
		var provider = GetDataProvider(providerId);
		if (provider is null)
			throw new TfException("GetProvider failed");

		return provider.ProviderType.GetDataProviderSourceSchema(provider);
	}

	#region <--- Synchronization Result Info --->

	public List<TfDataProviderSynchronizeResultInfo> GetSynchronizationTaskResultInfos(
		Guid taskId)
	{
		var orderSettings = new TfOrderSettings(
		nameof(TfDataProviderSynchronizeTaskDbo.CreatedOn),
		OrderDirection.ASC);

		var dbos = _dboManager.GetList<TfDataProviderSynchronizeResultInfoDbo>(
				"WHERE task_id = @task_id",
				orderSettings,
				new NpgsqlParameter("@task_id", taskId));

		var result = new List<TfDataProviderSynchronizeResultInfo>();

		foreach (var dbo in dbos)
		{
			var task = new TfDataProviderSynchronizeResultInfo
			{
				Id = dbo.Id,
				TaskId = dbo.TaskId,
				TfId = dbo.TfId,
				TfRowIndex = dbo.TfRowIndex,
				CreatedOn = dbo.CreatedOn,
				Info = dbo.Info,
				Error = dbo.Error,
				Warning = dbo.Warning
			};
			result.Add(task);
		}

		return result;
	}

	public void CreateSynchronizationResultInfo(
		Guid taskId,
		int? tfRowIndex,
		Guid? tfId,
		string info = null,
		string warning = null,
		string error = null)
	{
		var dbo = new TfDataProviderSynchronizeResultInfoDbo
		{
			Id = Guid.NewGuid(),
			TaskId = taskId,
			CreatedOn = DateTime.Now,
			Info = info,
			Error = error,
			Warning = warning,
			TfId = tfId,
			TfRowIndex = tfRowIndex
		};

		var success = _dboManager.Insert<TfDataProviderSynchronizeResultInfoDbo>(dbo);
		if (!success)
			throw new TfDatabaseException("Failed to insert synchronization task result info.");
	}

	#endregion

	public async Task BulkSynchronize(
		TfDataProviderSynchronizeTask task)
	{
		await Task.Delay(1);

		var provider = GetDataProvider(task.DataProviderId);

		if (provider is null)
			throw new TfException("Unable to get provider.");

		using (var scope = _dbService.CreateTransactionScope(Constants.DB_SYNC_OPERATION_LOCK_KEY))
		{
			var existingData = GetExistingData(provider);
			var newData = GetNewData(provider);

			if (newData.Count() == 0)
			{
				DeleteAllProviderRows(provider);
				scope.Complete();
				return;
			}

			var tableName = $"dp{provider.Index}";

			var preparedSharedKeyIds = BulkPrepareSharedKeyValueIds(provider, newData);

			var (columnList, paramsDict, newTfIds) = PrepareQueryArrayParameters(provider, preparedSharedKeyIds, existingData, newData);

			BulkPrepareAndUpdateTfIds(newTfIds, paramsDict["tf_id"]);

			//drop cloned table if exists
			_dbService.ExecuteSqlNonQueryCommand($"DROP TABLE IF EXISTS {tableName}_sync CASCADE;");

			//clone table
			var result = _dbManager.CloneTableForSynch(tableName);

			if (!result.IsSuccess)
			{
				throw new Exception("Failed to create duplicate structure of provider database table.");
			}

			StringBuilder sql = new StringBuilder();
			sql.Append($"INSERT INTO {tableName}_sync ( ");
			sql.Append(string.Join(", ", columnList.Select(x => $"\"{x}\"").ToArray()));
			sql.Append(" ) SELECT * FROM UNNEST ( ");
			sql.Append(string.Join(", ", columnList.Select(x => $"@{x}").ToArray()));
			sql.Append(" ); ");
			sql.AppendLine();

			sql.AppendLine($"DROP TABLE IF EXISTS {tableName} CASCADE;");
			sql.AppendLine($"ALTER TABLE {tableName}_sync RENAME TO {tableName};");

			DataTable dtConstraints = _dbService.ExecuteSqlQueryCommand(TfDatabaseSqlProvider.GetConstraintsMetaSql());
			foreach (DataRow row in dtConstraints.Rows)
			{
				var constraintName = (string)row["constraint_name"];

				if (constraintName.EndsWith("_sync") && (string)row["table_name"] == $"{tableName}_sync")
				{
					var constraintNewName = constraintName.Substring(0, constraintName.Length - 5);
					sql.AppendLine($"ALTER TABLE {tableName} RENAME CONSTRAINT {constraintName} TO {constraintNewName};");
				}
			}

			DataTable dtIndexes = _dbService.ExecuteSqlQueryCommand(TfDatabaseSqlProvider.GetIndexesMetaSql());
			foreach (DataRow row in dtIndexes.Rows)
			{
				var indexName = (string)row["index_name"];
				if (indexName.EndsWith("_sync") && (string)row["table_name"] == $"{tableName}_sync")
				{
					var indexNewName = indexName.Substring(0, indexName.Length - 5);
					sql.AppendLine($"ALTER INDEX {indexName} RENAME TO {indexNewName};");
				}
			}

			List<NpgsqlParameter> paramList = new List<NpgsqlParameter>();
			foreach (var column in columnList)
			{
				paramList.Add(paramsDict[column]);
			}

			_dbService.ExecuteSqlNonQueryCommand(sql.ToString(), paramList);

			scope.Complete();
		}
	}

	private (List<string> columnNames, Dictionary<string, NpgsqlParameter>, Dictionary<string, Guid>) PrepareQueryArrayParameters(
		TfDataProvider provider,
		Dictionary<string, Guid> sharedKeyBulkIdDict,
		Dictionary<string, DataRow> existingData,
		ReadOnlyCollection<TfDataProviderDataRow> newData)
	{

		List<string> columnNames = new List<string>();

		Dictionary<string, NpgsqlParameter> paramsDict =
			new Dictionary<string, NpgsqlParameter>();

		Dictionary<string, Guid> newTfIds = new Dictionary<string, Guid>();

		//data column names and parameters
		foreach (var column in provider.Columns)
		{
			if (string.IsNullOrWhiteSpace(column.SourceName))
				continue;

			columnNames.Add(column.DbName);

			switch (column.DbType)
			{
				case TfDatabaseColumnType.Boolean:
					{
						var parameter = new NpgsqlParameter($"@{column.DbName}", NpgsqlDbType.Array | NpgsqlDbType.Boolean);
						parameter.Value = new List<bool?>();
						paramsDict.Add(column.DbName, parameter);
					}
					break;
				case TfDatabaseColumnType.Guid:
					{
						var parameter = new NpgsqlParameter($"@{column.DbName}", NpgsqlDbType.Array | NpgsqlDbType.Uuid);
						parameter.Value = new List<Guid?>();
						paramsDict.Add(column.DbName, parameter);
					}
					break;
				case TfDatabaseColumnType.Text:
					{
						var parameter = new NpgsqlParameter($"@{column.DbName}", NpgsqlDbType.Array | NpgsqlDbType.Text);
						parameter.Value = new List<string>();
						paramsDict.Add(column.DbName, parameter);
					}
					break;
				case TfDatabaseColumnType.ShortText:
					{
						var parameter = new NpgsqlParameter($"@{column.DbName}", NpgsqlDbType.Array | NpgsqlDbType.Varchar);
						parameter.Value = new List<string>();
						paramsDict.Add(column.DbName, parameter);
					}
					break;
				case TfDatabaseColumnType.Date:
				case TfDatabaseColumnType.DateTime:
					{
						var parameter = new NpgsqlParameter($"@{column.DbName}", NpgsqlDbType.Array | NpgsqlDbType.Date);
						parameter.Value = new List<DateTime?>();
						paramsDict.Add(column.DbName, parameter);
					}
					break;
				case TfDatabaseColumnType.ShortInteger:
					{
						var parameter = new NpgsqlParameter($"@{column.DbName}", NpgsqlDbType.Array | NpgsqlDbType.Smallint);
						parameter.Value = new List<short?>();
						paramsDict.Add(column.DbName, parameter);
					}
					break;
				case TfDatabaseColumnType.Integer:
					{
						var parameter = new NpgsqlParameter($"@{column.DbName}", NpgsqlDbType.Array | NpgsqlDbType.Integer);
						parameter.Value = new List<int?>();
						paramsDict.Add(column.DbName, parameter);
					}
					break;
				case TfDatabaseColumnType.LongInteger:
					{
						var parameter = new NpgsqlParameter($"@{column.DbName}", NpgsqlDbType.Array | NpgsqlDbType.Bigint);
						parameter.Value = new List<long?>();
						paramsDict.Add(column.DbName, parameter);
					}
					break;
				case TfDatabaseColumnType.Number:
					{
						var parameter = new NpgsqlParameter($"@{column.DbName}", NpgsqlDbType.Array | NpgsqlDbType.Numeric);
						parameter.Value = new List<decimal?>();
						paramsDict.Add(column.DbName, parameter);
					}
					break;
				default:
					throw new Exception("Not supported database type");
			}
		}

		//shared keys column names and parameters
		foreach (var sharedKey in provider.SharedKeys)
		{
			columnNames.Add($"tf_sk_{sharedKey.DbName}_id");
			columnNames.Add($"tf_sk_{sharedKey.DbName}_version");

			{
				var parameter = new NpgsqlParameter($"tf_sk_{sharedKey.DbName}_id", NpgsqlDbType.Array | NpgsqlDbType.Uuid);
				parameter.Value = new List<Guid>();
				paramsDict.Add($"tf_sk_{sharedKey.DbName}_id", parameter);
			}

			{
				var parameter = new NpgsqlParameter($"tf_sk_{sharedKey.DbName}_version", NpgsqlDbType.Array | NpgsqlDbType.Smallint);
				parameter.Value = new List<short>();
				paramsDict.Add($"tf_sk_{sharedKey.DbName}_version", parameter);
			}
		}


		//add system columns names and parameters
		{
			columnNames.Add($"tf_id");
			columnNames.Add($"tf_created_on");
			columnNames.Add($"tf_updated_on");
			columnNames.Add($"tf_row_index");

			{
				var parameter = new NpgsqlParameter($"@tf_id", NpgsqlDbType.Array | NpgsqlDbType.Uuid);
				parameter.Value = new List<Guid>();
				paramsDict.Add("tf_id", parameter);
			}

			{
				var parameter = new NpgsqlParameter($"@tf_created_on", NpgsqlDbType.Array | NpgsqlDbType.Date);
				parameter.Value = new List<DateTime>();
				paramsDict.Add("tf_created_on", parameter);
			}

			{
				var parameter = new NpgsqlParameter($"@tf_updated_on", NpgsqlDbType.Array | NpgsqlDbType.Date);
				parameter.Value = new List<DateTime>();
				paramsDict.Add("tf_updated_on", parameter);
			}

			{
				var parameter = new NpgsqlParameter($"@tf_row_index", NpgsqlDbType.Array | NpgsqlDbType.Integer);
				parameter.Value = new List<int>();
				paramsDict.Add("tf_row_index", parameter);
			}
		}


		int currentRowIndex = 0;
		foreach (var row in newData)
		{
			currentRowIndex++;

			foreach (var column in provider.Columns)
			{
				switch (column.DbType)
				{
					case TfDatabaseColumnType.Boolean:
						{
							((List<bool?>)paramsDict[column.DbName].Value).Add((bool?)row[column.DbName]);
						}
						break;
					case TfDatabaseColumnType.Guid:
						{
							((List<Guid?>)paramsDict[column.DbName].Value).Add((Guid?)row[column.DbName]);
						}
						break;
					case TfDatabaseColumnType.Text:
					case TfDatabaseColumnType.ShortText:
						{
							((List<string>)paramsDict[column.DbName].Value).Add((string)row[column.DbName]);
						}
						break;
					case TfDatabaseColumnType.Date:
					case TfDatabaseColumnType.DateTime:
						{
							DateTime? value = null;
							if (row[column.DbName] is DateOnly)
							{
								value = ((DateOnly)row[column.DbName]).ToDateTime();
							}
							else if (row[column.DbName] is DateOnly?)
							{
								if (row[column.DbName] == null)
								{
									value = null;
								}
								else
								{
									value = ((DateOnly)row[column.DbName]).ToDateTime();
								}
							}
							else if (row[column.DbName] is DateTime? || row[column.DbName] is DateTime)
							{
								value = (DateTime?)row[column.DbName];
							}
							else
							{
								throw new Exception("Some source rows contains non DateTime or DateOnly objects for column 'column.DbName' of type Date\\DateTime.");
							}

							((List<DateTime?>)paramsDict[column.DbName].Value).Add(value);
						}
						break;
					case TfDatabaseColumnType.ShortInteger:
						{
							((List<short?>)paramsDict[column.DbName].Value).Add((short?)row[column.DbName]);
						}
						break;
					case TfDatabaseColumnType.Integer:
						{
							((List<int?>)paramsDict[column.DbName].Value).Add((int?)row[column.DbName]);

						}
						break;
					case TfDatabaseColumnType.LongInteger:
						{
							((List<long?>)paramsDict[column.DbName].Value).Add((long?)row[column.DbName]);
						}
						break;
					case TfDatabaseColumnType.Number:
						{
							((List<decimal?>)paramsDict[column.DbName].Value).Add((decimal)row[column.DbName]);
						}
						break;
					default:
						throw new Exception("Not supported database type");
				}
			}

			var key = GetDataRowPrimaryKeyValueAsString(provider, row, currentRowIndex);

			foreach (var sharedKey in provider.SharedKeys)
			{
				List<string> keys = new List<string>();
				foreach (var column in sharedKey.Columns)
					keys.Add(row[column.DbName]?.ToString());

				var combinedKey = CombineKey(keys);

				var skIdValue = sharedKeyBulkIdDict[combinedKey];

				((List<Guid>)paramsDict[$"tf_sk_{sharedKey.DbName}_id"].Value)
					.Add(skIdValue);

				((List<short>)paramsDict[$"tf_sk_{sharedKey.DbName}_version"].Value)
					.Add(sharedKey.Version);
			}

			if (existingData.ContainsKey(key))
			{
				((List<Guid>)paramsDict["tf_id"].Value).Add((Guid)existingData[key]["tf_id"]);
				((List<DateTime>)paramsDict["tf_created_on"].Value).Add((DateTime)existingData[key]["tf_created_on"]);
				((List<DateTime>)paramsDict["tf_updated_on"].Value).Add((DateTime)DateTime.Now);
				((List<int>)paramsDict["tf_row_index"].Value).Add(currentRowIndex);
			}
			else
			{
				var tfId = Guid.NewGuid();
				newTfIds[tfId.ToString()] = tfId;
				((List<Guid>)paramsDict["tf_id"].Value).Add((Guid)tfId);
				((List<DateTime>)paramsDict["tf_created_on"].Value).Add((DateTime)DateTime.Now);
				((List<DateTime>)paramsDict["tf_updated_on"].Value).Add((DateTime)DateTime.Now);
				((List<int>)paramsDict["tf_row_index"].Value).Add(currentRowIndex);
			}
		}

		return (columnNames, paramsDict, newTfIds);
	}

	private Dictionary<string, Guid> BulkPrepareSharedKeyValueIds(
		TfDataProvider provider,
		ReadOnlyCollection<TfDataProviderDataRow> newData)
	{
		Dictionary<string, Guid> sharedKeyBulkIdDict = new();

		foreach (var row in newData)
		{
			foreach (var sharedKey in provider.SharedKeys)
			{
				List<string> keys = new List<string>();
				foreach (var column in sharedKey.Columns)
					keys.Add(row[column.DbName]?.ToString());

				var key = CombineKey(keys);

				if (sharedKeyBulkIdDict.ContainsKey(key))
					continue;

				sharedKeyBulkIdDict[key] = Guid.Empty;
			}
		}

		BulkFillIds(sharedKeyBulkIdDict);

		return sharedKeyBulkIdDict;
	}

	private void BulkPrepareAndUpdateTfIds(
		Dictionary<string, Guid> newTfIds,
		NpgsqlParameter tfIdsParam)
	{
		BulkFillIds(newTfIds);

		var tfIdList = (List<Guid>)tfIdsParam.Value;
		List<Guid> newtfIdList = new List<Guid>();
		foreach (var id in tfIdList)
		{
			if (newTfIds.ContainsKey(id.ToString()))
			{
				newtfIdList.Add(newTfIds[id.ToString()]);
			}
			else
			{
				newtfIdList.Add(id);
			}
		}
		tfIdsParam.Value = newtfIdList;
	}

	private Dictionary<string, DataRow> GetExistingData(
		TfDataProvider provider)
	{
		List<string> columnsToSelect = new List<string>();

		foreach (var column in provider.SystemColumns)
		{
			columnsToSelect.Add(column.DbName);
		}

		foreach (var sharedKey in provider.SharedKeys)
		{
			string sharedKeyIdColumnDbName = $"tf_sk_{sharedKey.DbName}_id";
			string sharedKeyVersionColumnDbName = $"tf_sk_{sharedKey.DbName}_version";
			columnsToSelect.Add(sharedKeyIdColumnDbName);
			columnsToSelect.Add(sharedKeyVersionColumnDbName);
		}

		if (provider.SynchPrimaryKeyColumns != null &&
			provider.SynchPrimaryKeyColumns.Count > 0)
		{
			foreach (var column in provider.SynchPrimaryKeyColumns)
			{
				columnsToSelect.Add(column);
			}
		}

		var columnsString = string.Join(", ", columnsToSelect.Select(x => $"\"{x}\"").ToArray());

		var dt = _dbService.ExecuteSqlQueryCommand($"SELECT {columnsString} FROM dp{provider.Index}");

		Dictionary<string, DataRow> result = new Dictionary<string, DataRow>();

		foreach (DataRow dr in dt.Rows)
		{
			var key = GetDataRowPrimaryKeyValueAsString(provider, dr);
			if (!result.ContainsKey(key))
				result.Add(key, dr);
		}

		return result;
	}

	private ReadOnlyCollection<TfDataProviderDataRow> GetNewData(
		TfDataProvider provider)
	{
		var newData = provider.GetRows();

		if (newData.Count == 0)
		{
			return newData;
		}

		var requiredColumns = provider.Columns.Where(x => x.SourceName is not null && x.IsNullable == false);

		foreach (var requiredColum in requiredColumns)
		{
			var found = newData[0].ColumnNames.Contains(requiredColum.DbName);
			if (!found)
			{
				throw new Exception($"Required column '{requiredColum.DbName}'(Source Name='{requiredColum.SourceName})' " +
					$" is not found in source columns list.");
			}
		}

		var primaryKeyValidationSet = new HashSet<string>();

		int rowIndex = 0;

		foreach (var row in newData)
		{
			rowIndex++;

			var key = GetDataRowPrimaryKeyValueAsString(provider, row, rowIndex);

			if (primaryKeyValidationSet.Contains(key))
			{
				throw new Exception("Provider data contains rows with " +
					"duplicated value for specified synchronization key.");
			}

			primaryKeyValidationSet.Add(key);

			Dictionary<string, HashSet<object>> uniqueValidationDict =
				new Dictionary<string, HashSet<object>>();

			foreach (var column in provider.Columns)
			{
				if (string.IsNullOrWhiteSpace(column.SourceName))
					continue;

				if (!column.IsNullable && row[column.DbName] == null)
				{
					throw new Exception($"The column '{column.DbName}'(Source Name='{column.SourceName})' " +
						$" is specified as non nullable, but provider data contains records with null for this column");
				}

				if (column.IsUnique)
				{
					if (!uniqueValidationDict.ContainsKey(column.DbName))
					{
						uniqueValidationDict[column.DbName] = new HashSet<object>();
					}

					if (uniqueValidationDict[column.DbName].Contains(row[column.DbName]))
					{
						throw new Exception($"The column '{column.DbName}'(Source Name='{column.SourceName})' ]" +
												$" is specified as unique, but provider data contains records" +
												$" with duplicate value for this column");
					}

					uniqueValidationDict[column.DbName].Add(row[column.DbName]);
				}
			}
		}

		return newData;
	}

	private string GetDataRowPrimaryKeyValueAsString(
		TfDataProvider provider,
		DataRow dr)
	{
		if (provider.SynchPrimaryKeyColumns != null &&
					provider.SynchPrimaryKeyColumns.Count > 0)
		{
			List<string> columnKeyValues = new List<string>();
			foreach (var column in provider.SynchPrimaryKeyColumns)
			{
				var value = dr[column];
				if (value == DBNull.Value)
					value = null;
				columnKeyValues.Add(value?.ToString());
			}

			return string.Join(Constants.SHARED_KEY_SEPARATOR, columnKeyValues) ?? string.Empty;
		}
		else
		{
			return dr["tf_row_index"].ToString();
		}
	}

	private string GetDataRowPrimaryKeyValueAsString(
		TfDataProvider provider,
		TfDataProviderDataRow dr,
		int rowIndex)
	{
		if (provider.SynchPrimaryKeyColumns != null &&
			provider.SynchPrimaryKeyColumns.Count > 0)
		{
			List<string> columnKeyValues = new List<string>();
			foreach (var column in provider.SynchPrimaryKeyColumns)
			{
				columnKeyValues.Add(dr[column]?.ToString());
			}

			return string.Join(Constants.SHARED_KEY_SEPARATOR, columnKeyValues) ?? string.Empty;
		}
		else
		{
			return rowIndex.ToString();
		}
	}

	#endregion

}

public record DatabaseColumnTypeInfo
{
	public string Name { get; init; }
	public TfDatabaseColumnType Type { get; init; }
	public bool CanBeProviderDataType { get; init; }
	public bool SupportAutoDefaultValue { get; init; }
}