using Newtonsoft.Json;

namespace WebVella.Tefter;

public partial interface ITfDataProviderManager
{
	/// <summary>
	/// Gets data provider instance for specified identifier
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	public TfDataProvider GetProvider(
		Guid id);

	/// <summary>
	/// Gets data provider instance for specified name
	/// </summary>
	/// <param name="name"></param>
	/// <returns></returns>
	public TfDataProvider GetProvider(
		string name);

	/// <summary>
	/// Gets list of available data providers
	/// </summary>
	/// <returns></returns>
	public ReadOnlyCollection<TfDataProvider> GetProviders();

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
}

public partial class TfDataProviderManager : ITfDataProviderManager
{

	/// <summary>
	/// Gets data provider instance for specified identifier
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	public TfDataProvider GetProvider(
		Guid id)
	{
		var providerDbo = _dboManager.Get<TfDataProviderDbo>(id);

		if (providerDbo == null)
			return null;

		var providerType = GetProviderType(providerDbo.TypeId);
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
	public TfDataProvider GetProvider(
		string name)
	{
		var providerDbo = _dboManager
			.Get<TfDataProviderDbo>(name, nameof(TfDataProviderDbo.Name));

		if (providerDbo == null)
			return null;

		var providerType = GetProviderType(providerDbo.TypeId);
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
	public ReadOnlyCollection<TfDataProvider> GetProviders()
	{

		List<TfDataProvider> providers = new List<TfDataProvider>();

		var providerTypes = GetProviderTypes();

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

		new TfDataProviderCreateValidator(_dboManager, this)
			.Validate(providerModel)
			.ToValidationException()
			.ThrowIfContainsErrors();

		using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
		{

			TfDataProviderDbo dataProviderDbo = DataProviderToDbo(providerModel);

			var success = _dboManager.Insert<TfDataProviderDbo>(dataProviderDbo);

			if (!success)
				throw new TfDboServiceException("Insert<TfDataProviderDbo> failed");

			var provider = GetProvider(providerModel.Id);
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
		new TfDataProviderUpdateValidator(_dboManager, this)
			.Validate(providerModel)
			.ToValidationException()
			.ThrowIfContainsErrors();

		TfDataProviderDbo dataProviderDbo = DataProviderToDbo(providerModel);

		var success = _dboManager.Update<TfDataProviderDbo>(dataProviderDbo);

		if (!success)
			throw new TfDboServiceException("Update<TfDataProviderDbo> failed.");

		return GetProvider(providerModel.Id);
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
			var provider = GetProvider(id);
			if (provider is null)
				new TfException("Provider is not found.");

			new TfDataProviderDeleteValidator(_dboManager, this)
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

		var sharedColumns = _sharedColumnManager.GetSharedColumns();
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
			SynchPrimaryKeyColumnsJson = JsonConvert.SerializeObject(providerModel.SynchPrimaryKeyColumns ?? new List<string>())
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
			SynchPrimaryKeyColumns = JsonConvert.DeserializeObject<List<string>>(dbo.SynchPrimaryKeyColumnsJson ?? "[]").AsReadOnly()
		};
	}

	#endregion

	#region <--- validation --->

	internal class TfDataProviderCreateValidator
	: AbstractValidator<TfDataProviderModel>
	{
		public TfDataProviderCreateValidator(
			ITfDboManager dboManager,
			ITfDataProviderManager providerManager)
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
					.Must(id => { return providerManager.GetProvider(id) == null; })
					.WithMessage("There is already existing data provider with specified identifier.");

			RuleFor(provider => provider.Name)
					.Must(name => { return providerManager.GetProvider(name) == null; })
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
			ITfDboManager dboManager,
			ITfDataProviderManager providerManager)
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
					.Must((provider, id) => { return providerManager.GetProvider(id) != null; })
					.WithMessage("There is no existing data provider for specified identifier.");

			RuleFor(provider => provider.ProviderType)
					.Must((provider, providerType) =>
					{
						if (provider.ProviderType == null)
							return true;

						var existingObject = providerManager.GetProvider(provider.Id);

						if (existingObject != null && existingObject.ProviderType.Id != provider.ProviderType.Id)
							return false;

						return true;
					})
					.WithMessage("The data provider type cannot be updated.");

			RuleFor(provider => provider.Name)
				.Must((provider, name) =>
				{
					var existingObj = providerManager.GetProvider(provider.Name);
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
			ITfDboManager dboManager,
			ITfDataProviderManager providerManager)
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
}
