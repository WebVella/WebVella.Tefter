using FluentResults;

namespace WebVella.Tefter;

public partial interface ITfDataProviderManager
{
	public Result<TfDataProvider> GetProvider(Guid id);

	public Result<TfDataProvider> GetProvider(string name);

	public Result<ReadOnlyCollection<TfDataProvider>> GetProviders();

	public Result<TfDataProvider> CreateDataProvider(TfDataProviderModel providerModel);

	public Result<TfDataProvider> UpdateDataProvider(TfDataProviderModel providerModel);
}

public partial class TfDataProviderManager : ITfDataProviderManager
{
	public Result<TfDataProvider> GetProvider(Guid id)
	{
		try
		{
			var providerDbo = _dboManager.Get<TfDataProviderDbo>(id);

			if (providerDbo == null)
				return Result.Ok();

			var providerTypeResult = GetProviderType(providerDbo.TypeId);
			if (providerTypeResult.IsFailed)
				return Result.Fail(new Error("Failed to get data provider")
					.CausedBy(providerTypeResult.Errors));

			var providerType = providerTypeResult.Value;
			if(providerType ==null)
				return Result.Fail(new Error("Unable to find provider type for specified provider instance."));

			TfDataProvider provider = new TfDataProvider
				{
					Id = providerDbo.Id,
					Name = providerDbo.Name,
					Settings = providerDbo.SettingsJson,
					CompositeKeyPrefix = providerDbo.CompositeKeyPrefix,
					Index = providerDbo.Index,
					ProviderType = providerType
				};

			return Result.Ok(provider);
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to get data providers").CausedBy(ex));
		}
	}

	public Result<TfDataProvider> GetProvider(string name)
	{
		try
		{
			var providerDbo = _dboManager.Get<TfDataProviderDbo>(name, nameof(TfDataProviderDbo.Name));

			if (providerDbo == null)
				return Result.Ok();

			var providerTypeResult = GetProviderType(providerDbo.TypeId);
			if (providerTypeResult.IsFailed)
				return Result.Fail(new Error("Failed to get data provider")
					.CausedBy(providerTypeResult.Errors));

			var providerType = providerTypeResult.Value;
			if (providerType == null)
				return Result.Fail(new Error("Unable to find provider type for specified provider instance."));

			TfDataProvider provider = new TfDataProvider
			{
				Id = providerDbo.Id,
				Name = providerDbo.Name,
				Settings = providerDbo.SettingsJson,
				CompositeKeyPrefix = providerDbo.CompositeKeyPrefix,
				Index = providerDbo.Index,
				ProviderType = providerType
			};

			return Result.Ok(provider);
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to get data providers").CausedBy(ex));
		}
	}

	public Result<ReadOnlyCollection<TfDataProvider>> GetProviders()
	{
		try
		{
			List<TfDataProvider> providers = new List<TfDataProvider>();

			var providerTypesResult = GetProviderTypes();
			if (providerTypesResult.IsFailed)
				return Result.Fail(new Error("Failed to get data providers list ")
					.CausedBy(providerTypesResult.Errors));

			var providerTypes = providerTypesResult.Value;

			var providersDbo = _dboManager.GetList<TfDataProviderDbo>();

			foreach (var dbo in providersDbo)
			{
				var providerType = providerTypes.SingleOrDefault(x => x.Id == dbo.TypeId);

				if (providerType == null)
					return Result.Fail(new Error($"Failed to get data providers, because " +
						$"provider type {dbo.TypeName} with id = '{dbo.TypeId}' is not found."));

				TfDataProvider provider = new TfDataProvider
				{
					Id = dbo.Id,
					Name = dbo.Name,
					Settings = dbo.SettingsJson,
					CompositeKeyPrefix = dbo.CompositeKeyPrefix,
					Index = dbo.Index,
					ProviderType = providerType
				};

				providers.Add(provider);
			}

			return Result.Ok(providers.AsReadOnly());
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to get data providers").CausedBy(ex));
		}
	}

	public Result<TfDataProvider> CreateDataProvider( TfDataProviderModel providerModel)
	{
		try
		{
			if (providerModel.Id == Guid.Empty)
				providerModel.Id = Guid.NewGuid();

			TfDataProviderCreateValidator validator = 
				new TfDataProviderCreateValidator(_dboManager,this);

			var validationResult = validator.Validate(providerModel);	
			if(!validationResult.IsValid)
				return validationResult.ToResult();

			TfDataProviderDbo dataProviderDbo = new TfDataProviderDbo
			{
				Id = providerModel.Id,
				CompositeKeyPrefix = providerModel.CompositeKeyPrefix,
				Name = providerModel.Name,
				SettingsJson = providerModel.SettingsJson,
				TypeId = providerModel.ProviderType.Id,
				TypeName = providerModel.ProviderType.GetType().FullName,
			};

			using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{

				var success = _dboManager.Insert<TfDataProviderDbo>(dataProviderDbo);

				if (!success)
					return Result.Fail(new DboManagerError("Insert", dataProviderDbo));

				var providerResult = GetProvider(providerModel.Id);
				if(providerResult.IsFailed)
					return Result.Fail(new Error("Failed to create new data provider")
						.CausedBy(providerResult.Errors));

				var provider = providerResult.Value;
				string providerTableName = $"dp{provider.Index}";

				DatabaseBuilder dbBuilder = _dbManager.GetDatabaseBuilder();
				dbBuilder.NewTableBuilder(Guid.NewGuid(),providerTableName)
					.WithDataProviderId(provider.Id)
					.WithColumns(columns =>
					{
						columns
							.AddGuidColumn("tf_id", c => { c.WithoutAutoDefaultValue().NotNullable(); })
							.AddDateTimeColumn("tf_created_on", c => { c.WithoutAutoDefaultValue().NotNullable(); })
							.AddDateTimeColumn("tf_updated_on", c => { c.WithoutAutoDefaultValue().NotNullable(); })
							.AddTextColumn("tf_search", c => { c.NotNullable().WithDefaultValue(string.Empty); });
					})
					.WithConstraints(constraints =>
					{
						constraints
							.AddPrimaryKeyConstraintBuilder($"pk_{providerTableName}", c => { c.WithColumns("tf_id"); });
					})
					.WithIndexes( indexes =>
					{
						indexes
							.AddBTreeIndex($"ix_{providerTableName}_tf_id", c => { c.WithColumns("tf_id"); })
							.AddGinIndex($"ix_{providerTableName}_tf_search", c => { c.WithColumns("tf_search"); });
					});

				_dbManager.SaveChanges(dbBuilder);

				scope.Complete();

				return Result.Ok(provider);
			}
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to create new data provider.").CausedBy(ex));
		}
	}

	public Result<TfDataProvider> UpdateDataProvider(TfDataProviderModel providerModel)
	{
		try
		{
			TfDataProviderUpdateValidator validator =
				new TfDataProviderUpdateValidator(_dboManager, this);

			var validationResult = validator.Validate(providerModel);
			if (!validationResult.IsValid)
				return validationResult.ToResult();

			TfDataProviderDbo dataProviderDbo = new TfDataProviderDbo
			{
				Id = providerModel.Id,
				CompositeKeyPrefix = providerModel.CompositeKeyPrefix,
				Name = providerModel.Name,
				SettingsJson = providerModel.SettingsJson,
				TypeId = providerModel.ProviderType.Id,
				TypeName = providerModel.ProviderType.GetType().FullName,
			};

			var success = _dboManager.Update<TfDataProviderDbo>(dataProviderDbo);

			if (!success)
				return Result.Fail(new DboManagerError("Update", dataProviderDbo));

			return Result.Ok(GetProvider(providerModel.Id).Value);
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to create new data provider.").CausedBy(ex));
		}
	}
}
