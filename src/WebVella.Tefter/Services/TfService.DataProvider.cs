using DocumentFormat.OpenXml.Office2010.Excel;
using FluentValidation;
using Nito.AsyncEx.Synchronous;
using WebVella.Tefter.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace WebVella.Tefter.Services;

public partial interface ITfService
{
	/// <summary>
	/// Gets data provider instance for specified identifier
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	public TfDataProvider GetDataProvider(
		Guid id);

	/// <summary>
	/// Gets data provider instance for specified index
	/// </summary>
	/// <param name="index"></param>
	/// <returns></returns>
	public TfDataProvider GetDataProvider(
		int index);

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
	public ReadOnlyCollection<TfDataProvider> GetDataProviders(string? search = null);


	/// <summary>
	/// Gets list of available data providers with their row count
	/// </summary>
	/// <returns></returns>
	public ReadOnlyCollection<TfDataProviderInfo> GetDataProvidersInfo();

	/// <summary>
	/// Creates new data provider 
	/// </summary>
	/// <param name="providerModel"></param>
	/// <returns></returns>
	internal TfDataProvider CreateDataProvider(
		TfCreateDataProvider providerModel);

	/// <summary>
	/// Update existing data provider
	/// </summary>
	/// <param name="providerModel"></param>
	/// <returns></returns>
	internal TfDataProvider UpdateDataProvider(
		TfUpdateDataProvider providerModel);

	/// <summary>
	/// Deletes existing data provider
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	internal void DeleteDataProvider(
		Guid id);

	/// <summary>
	/// Gets the total number of rows in the data provider table for the specified data provider Id.
	/// </summary>
	/// <param name="dataProviderId">The unique identifier of the data provider.</param>
	/// <returns>The total number of rows in the data provider table.</returns>
	public long GetDataProviderRowsCount(Guid dataProviderId);

	/// <summary>
	/// Retrieves a list of data providers that are available for joining with the specified data provider.
	/// </summary>
	/// <param name="dataProviderId">The unique identifier of the data provider to check for join availability.</param>
	/// <returns>A list of data providers that are connected with the specified data provider.</returns>
	public List<TfDataProvider> GetDataProviderConnectedProviders(Guid dataProviderId);

	/// <summary>
	/// Calculates the aux data schema based on the implemented data identities. Returns only shared columns or column from other data providers
	/// </summary>
	/// <param name="providerId"></param>
	/// <returns></returns>
	TfDataProviderAuxDataSchema GetDataProviderAuxDataSchema(Guid providerId);
}

public partial class TfService : ITfService
{
	/// <summary>
	/// Gets data provider instance for specified identifier
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	public TfDataProvider GetDataProvider(
		Guid id)
	{
		try
		{
			var providerDbo = _dboManager.Get<TfDataProviderDbo>(id);

			if (providerDbo == null)
				return null;

			var providerType = _metaService.GetDataProviderType(providerDbo.TypeId);
			if (providerType == null)
				throw new TfException("Unable to find provider type for specified provider instance.");

			var identities = GetDataProviderIdentities(id);

			var provider = DataProviderFromDbo(
					providerDbo,
					GetDataProviderSystemColumns(identities),
					GetDataProviderColumns(id),
					identities,
					providerType);

			provider.ServiceProvider = _serviceProvider;

			InitDataProviderSharedColumns(provider);

			return provider;
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	/// <summary>
	/// Gets data provider instance for specified index
	/// </summary>
	/// <param name="index"></param>
	/// <returns></returns>
	public TfDataProvider GetDataProvider(
		int index)
	{
		try
		{
			var providerDbo = _dboManager
				.Get<TfDataProviderDbo>(index, nameof(TfDataProviderDbo.Index));

			if (providerDbo == null)
				return null;

			var providerType = _metaService.GetDataProviderType(providerDbo.TypeId);
			if (providerType == null)
				throw new TfException("Unable to find provider type for specified provider instance.");

			var identities = GetDataProviderIdentities(providerDbo.Id);

			var provider = DataProviderFromDbo(
					providerDbo,
					GetDataProviderSystemColumns(identities),
					GetDataProviderColumns(providerDbo.Id),
					identities,
					providerType);

			provider.ServiceProvider = _serviceProvider;

			InitDataProviderSharedColumns(provider);

			return provider;
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}


	/// <summary>
	/// Gets data provider instance for specified name
	/// </summary>
	/// <param name="name"></param>
	/// <returns></returns>
	public TfDataProvider GetDataProvider(
		string name)
	{
		try
		{
			var providerDbo = _dboManager
				.Get<TfDataProviderDbo>(name, nameof(TfDataProviderDbo.Name));

			if (providerDbo == null)
				return null;

			var providerType = _metaService.GetDataProviderType(providerDbo.TypeId);
			if (providerType == null)
				throw new TfException("Failed to get data provider");

			var identities = GetDataProviderIdentities(providerDbo.Id);

			var provider = DataProviderFromDbo(
					providerDbo,
					GetDataProviderSystemColumns(identities),
					GetDataProviderColumns(providerDbo.Id),
					identities,
					providerType);

			provider.ServiceProvider = _serviceProvider;

			InitDataProviderSharedColumns(provider);

			return provider;
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	/// <summary>
	/// Gets list of available data providers
	/// </summary>
	/// <returns></returns>
	public ReadOnlyCollection<TfDataProvider> GetDataProviders()
	{
		try
		{
			List<TfDataProvider> providers = new List<TfDataProvider>();

			var providerTypes = _metaService.GetDataProviderTypes();

			var providersDbo = _dboManager.GetList<TfDataProviderDbo>();

			foreach (var dbo in providersDbo)
			{
				var providerType = providerTypes.SingleOrDefault(x => x.AddonId == dbo.TypeId);

				if (providerType == null)
					throw new TfException($"Failed to get data providers, because " +
						$"provider type with id = '{dbo.TypeId}' is not found.");

				var identities = GetDataProviderIdentities(dbo.Id);

				var provider = DataProviderFromDbo(
						dbo,
						GetDataProviderSystemColumns(identities),
						GetDataProviderColumns(dbo.Id),
						identities,
						providerType);

				provider.ServiceProvider = _serviceProvider;

				InitDataProviderSharedColumns(provider);

				providers.Add(provider);
			}

			return providers.AsReadOnly();
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public ReadOnlyCollection<TfDataProvider> GetDataProviders(string? search = null)
	{
		try
		{
			var allItems = GetDataProviders();
			if (String.IsNullOrWhiteSpace(search))
				return allItems;
			search = search.Trim().ToLowerInvariant();
			return allItems.Where(x =>
				x.Name.ToLowerInvariant().Contains(search)
				).ToList().AsReadOnly();
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	/// <summary>
	/// Gets list of available data providers with their row count
	/// </summary>
	/// <returns></returns>
	public ReadOnlyCollection<TfDataProviderInfo> GetDataProvidersInfo()
	{
		var result = new List<TfDataProviderInfo>();
		foreach (var provider in GetDataProviders())
		{
			var info = new TfDataProviderInfo
			{
				Id = provider.Id,
				Index = provider.Index,
				Name = provider.Name,
				ProviderType = provider.ProviderType,
				RowsCount = 0,
				NextSyncOn = null
			};
			info.RowsCount = GetDataProviderRowsCount(provider.Id);
			info.NextSyncOn = GetDataProviderNextSynchronizationTime(provider.Id);
			result.Add(info);
		}

		return result.AsReadOnly();
	}

	/// <summary>
	/// Creates new data provider 
	/// </summary>
	/// <param name="createModel"></param>
	/// <returns></returns>
	public TfDataProvider CreateDataProvider(
		TfCreateDataProvider createModel)
	{
		try
		{
			if (createModel != null && createModel.Id == Guid.Empty)
				createModel.Id = Guid.NewGuid();

			new TfDataProviderCreateValidator(this)
				.Validate(createModel)
				.ToValidationException()
				.ThrowIfContainsErrors();

			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var providerIndex = createModel.Index;

				//find lowest available index
				if (createModel.Index == -1)
				{
					var existingProviderIndexes = GetDataProviders()
							.Select(x => x.Index)
							.ToHashSet();

					providerIndex = 1;
					while (existingProviderIndexes.Contains(providerIndex))
					{
						providerIndex++;
					}
				}

				TfDataProviderDbo dataProviderDbo = DataProviderToDbo(createModel);
				//set correct index
				dataProviderDbo.Index = providerIndex;

				var success = _dboManager.Insert<TfDataProviderDbo>(dataProviderDbo);

				if (!success)
					throw new TfDboServiceException("Insert<TfDataProviderDbo> failed");

				var provider = GetDataProvider(createModel.Id);
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
					.WithIndexes(indexes =>
					{
						indexes
							.AddBTreeIndex($"ix_{providerTableName}_tf_id", c => { c.WithColumns("tf_id"); })
							.AddGinIndex($"ix_{providerTableName}_tf_search", c => { c.WithColumns("tf_search"); });
					});

				_dbManager.SaveChanges(dbBuilder);

				if (!success)
					throw new TfDboServiceException("Insert<TfDataProviderIdentityDbo> failed");

				if (createModel.AutoInitialize)
				{
					//Import schema
					var columns = new List<TfDataProviderColumn>();
					var schemaInfo = GetDataProviderSourceSchemaInfo(provider.Id);
					foreach (var columnName in schemaInfo.SourceColumnDefaultDbType.Keys)
					{
						string? defaultValue = schemaInfo.SourceColumnDefaultValue.ContainsKey(columnName) ? schemaInfo.SourceColumnDefaultValue[columnName] : null;
						var column = new TfDataProviderColumn
						{
							Id = Guid.NewGuid(),
							SourceName = columnName,
							SourceType = schemaInfo.SourceColumnDefaultSourceType[columnName],
							CreatedOn = DateTime.Now,
							DataProviderId = provider.Id,
							DbName = columnName.GenerateDbNameFromText(),
							DbType = schemaInfo.SourceColumnDefaultDbType[columnName],
							DefaultValue = defaultValue
						};
						column.FixPrefix(provider.ColumnPrefix);
						columns.Add(column);
					}

					if (columns.Count > 0)
					{
						provider = CreateBulkDataProviderColumn(provider.Id, columns);

						List<string> syncPrimaryKeyColumns = new List<string>();
						foreach (var synchKeyColumn in schemaInfo.SynchPrimaryKeyColumns)
						{
							var providerColumn = provider.Columns.SingleOrDefault(x => x.SourceName == synchKeyColumn);
							if(providerColumn is not null)
								syncPrimaryKeyColumns.Add(providerColumn.DbName!);
						}

						if (syncPrimaryKeyColumns.Count > 0)
							UpdateDataProviderSynchPrimaryKeyColumns(provider.Id, syncPrimaryKeyColumns);

						//Trigger Initial Sync
						TriggerSynchronization(provider.Id);
					}

					//get unique dataset name
					var index = 0;
					var presentDataSetNamesHS = GetDatasets().Select(x => x.Name).ToHashSet();
					var datasetName = provider.Name;
					do
					{
						if (!presentDataSetNamesHS.Contains(datasetName))
							break;

						index++;
						datasetName = $"{provider.Name} {index}";
					} 
					while (true);

					TfCreateDataset createDatasetModel = new TfCreateDataset
					{
						Id = Guid.NewGuid(),
						Name = datasetName,
						Filters = new(),
						Columns = new(),//when no columns specified all columns are added to the dataset
						DataProviderId = provider.Id
					};
					CreateDataset(createDatasetModel);
				}


				scope.Complete();

				PublishEventWithScope(new TfDataProviderCreatedEvent(provider));
				
				return provider;
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}


	/// <summary>
	/// Update existing data provider
	/// </summary>
	/// <param name="providerModel"></param>
	/// <returns></returns>
	public TfDataProvider UpdateDataProvider(
		TfUpdateDataProvider updateModel)
	{
		try
		{
			new TfDataProviderUpdateValidator(this)
				.Validate(updateModel)
				.ToValidationException()
				.ThrowIfContainsErrors();

			TfDataProvider existingProvider = GetDataProvider(updateModel.Id);
			TfDataProviderDbo dataProviderDbo = DataProviderToDbo(updateModel, existingProvider);

			var success = _dboManager.Update<TfDataProviderDbo>(dataProviderDbo,
				nameof(TfDataProviderDbo.Name),
				nameof(TfDataProviderDbo.TypeId),
				nameof(TfDataProviderDbo.SettingsJson),
				nameof(TfDataProviderDbo.SynchPrimaryKeyColumnsJson),
				nameof(TfDataProviderDbo.SynchScheduleMinutes),
				nameof(TfDataProviderDbo.SynchScheduleEnabled)
				);

			if (!success)
				throw new TfDboServiceException("Update<TfDataProviderDbo> failed.");

			var result = GetDataProvider(updateModel.Id);
			PublishEventWithScope(new TfDataProviderUpdatedEvent(result));

			return result;
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	/// <summary>
	/// Deletes existing data provider
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	public void DeleteDataProvider(
		Guid id)
	{
		try
		{
			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
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

				foreach (var identity in provider.Identities)
				{
					success = _dboManager.Delete<TfDataProviderIdentityDbo>(identity.Id);

					if (!success)
						throw new TfDboServiceException("Delete<TfDataProviderIdentityDbo> failed.");
				}

				success = _dboManager.Delete<TfDataProviderDbo>(id);

				if (!success)
					throw new TfDboServiceException("Delete<TfDataProviderDbo> failed.");


				string providerTableName = $"dp{provider.Index}";

				TfDatabaseBuilder dbBuilder = _dbManager.GetDatabaseBuilder();

				dbBuilder.Remove(providerTableName);

				_dbManager.SaveChanges(dbBuilder);
				scope.Complete();
				
				PublishEventWithScope(new TfDataProviderDeletedEvent(provider));
			
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public long GetDataProviderRowsCount(Guid dataProviderId)
	{
		try
		{
			var dataProvider = GetDataProvider(dataProviderId);
			if (dataProvider is null)
				throw new Exception("DataProvider not found for specified identifier.");


			using var connection = _dbService.CreateConnection();
			var dbCommand = connection.CreateCommand($"SELECT COUNT(*) FROM dp{dataProvider.Index} ");
			return (long)dbCommand.ExecuteScalar();

		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public List<TfDataProvider> GetDataProviderConnectedProviders(Guid dataProviderId)
	{
		List<TfDataProvider> result = new List<TfDataProvider>();

		var dataProvider = GetDataProvider(dataProviderId);
		var allDataProviders = GetDataProviders();

		foreach (var provider in allDataProviders)
		{

			if (provider.Id == dataProviderId)
				continue;

			var foundSimilarJoinKey = provider
							.Identities
							.Select(x => x.DataIdentity)
							.Intersect(dataProvider.Identities.Select(x => x.DataIdentity))
							.Any();

			if (foundSimilarJoinKey)
				result.Add(provider);
		}

		return result;
	}

	public TfDataProviderAuxDataSchema GetDataProviderAuxDataSchema(Guid providerId)
	{
		var result = new TfDataProviderAuxDataSchema();
		var providers = GetDataProviders();
		TfDataProvider provider = providers.Single(x => x.Id == providerId);
		var sharedColumns = GetSharedColumns();
		var identities = GetDataIdentities();
		foreach (var identity in provider.Identities)
		{
			var resultIdentity = identities.FirstOrDefault(x => x.DataIdentity == identity.DataIdentity);
			if (resultIdentity is null) continue;

			var identitySharedColumns = sharedColumns.Where(x => x.DataIdentity == identity.DataIdentity).ToList();
			var identityProviders = providers.Where(x => x.Id != providerId && x.Identities.Any(x => x.DataIdentity == identity.DataIdentity)).ToList();

			if (identitySharedColumns.Count == 0 && identityProviders.Count == 0)
				continue;

			var resultIdentitySchema = new TfDataProviderAuxDataSchemaIdentity
			{
				DataIdentity = resultIdentity,
				Columns = new List<TfDataProviderAuxDataSchemaColumn>()
			};

			foreach (var column in identitySharedColumns)
			{
				resultIdentitySchema.Columns.Add(new TfDataProviderAuxDataSchemaColumn
				{
					DbName = column.DbName,
					DataIdentity = resultIdentity,
					SharedColumn = column,
					DataProvider = null,
					DataProviderColumn = null
				});
			}

			foreach (var identityProvider in identityProviders)
			{
				foreach (var providerColumn in identityProvider.Columns)
				{
					resultIdentitySchema.Columns.Add(new TfDataProviderAuxDataSchemaColumn
					{
						DbName = $"{identity.DataIdentity}.{providerColumn.DbName}",
						DataIdentity = resultIdentity,
						SharedColumn = null,
						DataProvider = identityProvider,
						DataProviderColumn = providerColumn
					});
				}
			}

			result.DataIdentities.Add(resultIdentitySchema);
		}
		return result;
	}


	#region <--- utility --->

	private void InitDataProviderSharedColumns(
		TfDataProvider provider)
	{
		List<TfSharedColumn> columns = new List<TfSharedColumn>();

		var sharedColumns = GetSharedColumns();
		foreach (var sharedColumn in sharedColumns)
		{
			var dataIdentity = provider.Identities.SingleOrDefault(x => x.DataIdentity == sharedColumn.DataIdentity);
			if (dataIdentity is not null && !columns.Contains(sharedColumn))
				columns.Add(sharedColumn);
		}

		provider.SharedColumns = columns.AsReadOnly();
	}

	private static TfDataProviderDbo DataProviderToDbo(
		TfCreateDataProvider providerModel)
	{
		if (providerModel == null)
			throw new ArgumentException(nameof(providerModel));

		return new TfDataProviderDbo
		{
			Id = providerModel.Id,
			Name = providerModel.Name,
			Index = providerModel.Index,
			SettingsJson = providerModel.SettingsJson,
			TypeId = providerModel.ProviderType.AddonId,
			SynchPrimaryKeyColumnsJson = JsonSerializer.Serialize(providerModel.SynchPrimaryKeyColumns ?? new List<string>()),
			SynchScheduleEnabled = providerModel.SynchScheduleEnabled,
			SynchScheduleMinutes = providerModel.SynchScheduleMinutes,
		};
	}

	private static TfDataProviderDbo DataProviderToDbo(
		TfUpdateDataProvider providerModel,
		TfDataProvider existingProvider)
	{
		if (providerModel == null)
			throw new ArgumentException(nameof(providerModel));

		return new TfDataProviderDbo
		{
			Id = providerModel.Id,
			Name = providerModel.Name,
			Index = existingProvider.Index,
			TypeId = existingProvider.ProviderType.AddonId,
			SettingsJson = providerModel.SettingsJson,
			SynchPrimaryKeyColumnsJson = JsonSerializer.Serialize(providerModel.SynchPrimaryKeyColumns ?? new List<string>()),
			SynchScheduleEnabled = providerModel.SynchScheduleEnabled,
			SynchScheduleMinutes = providerModel.SynchScheduleMinutes,
		};
	}

	private static TfDataProvider DataProviderFromDbo(
		TfDataProviderDbo dbo,
		List<TfDataProviderSystemColumn> systemColumns,
		List<TfDataProviderColumn> columns,
		List<TfDataProviderIdentity> identities,
		ITfDataProviderAddon providerType)
	{
		if (dbo == null)
			throw new ArgumentException(nameof(dbo));

		if (columns == null)
			throw new ArgumentException(nameof(columns));

		if (identities == null)
			throw new ArgumentException(nameof(identities));

		if (providerType == null)
			throw new ArgumentException(nameof(providerType));

		return new TfDataProvider
		{
			Id = dbo.Id,
			Name = dbo.Name,
			Index = dbo.Index,
			SettingsJson = dbo.SettingsJson,
			SynchScheduleEnabled = dbo.SynchScheduleEnabled,
			SynchScheduleMinutes = dbo.SynchScheduleMinutes,
			ProviderType = providerType,
			SystemColumns = systemColumns.AsReadOnly(),
			Columns = columns.AsReadOnly(),
			Identities = identities.AsReadOnly(),
			SynchPrimaryKeyColumns = JsonSerializer.Deserialize<List<string>>(dbo.SynchPrimaryKeyColumnsJson ?? "[]").AsReadOnly()
		};
	}

	#endregion

	#region <--- validation --->

	internal class TfDataProviderCreateValidator
		: AbstractValidator<TfCreateDataProvider>
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

			RuleFor(provider => provider.Index)
					.Must((provider, index) =>
					{
						if (index == -1)
							return true;

						if (index <= 0)
							return false;

						return true;
					})
					.WithMessage("The data provider index should be -1 or positive integer value.");

			RuleFor(provider => provider.Index)
				.Must((provider, index) =>
				{
					var existingProviderIndexes = tfService.GetDataProviders()
						.Select(x => x.Index)
						.ToHashSet();

					return !existingProviderIndexes.Contains(index);
				})
				.WithMessage("There is already existing data provider with same index.");

			RuleFor(provider => provider.ProviderType)
				.NotEmpty()
				.WithMessage("The data provider type is required.");

			RuleFor(provider => provider.Id)
					.Must(id => { return tfService.GetDataProvider(id) == null; })
					.WithMessage("There is already existing data provider with specified identifier.");

			RuleFor(provider => provider.Name)
					.Must(name => { return tfService.GetDataProvider(name) == null; })
					.WithMessage("There is already existing data provider with specified name.");

			RuleFor(provider => provider.SynchScheduleMinutes)
					.Must((provider, syncMinutes) =>
					{
						return syncMinutes >= 15;
					})
					.WithMessage("Minimum time between synchronizations is 15 min");
		}

		public ValidationResult ValidateCreate(TfCreateDataProvider provider)
		{
			if (provider == null)
				return new ValidationResult(new[] { new ValidationFailure("", "The data provider model is null.") });

			return this.Validate(provider);
		}
	}

	internal class TfDataProviderUpdateValidator
		: AbstractValidator<TfUpdateDataProvider>
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

			RuleFor(provider => provider.Id)
					.Must((provider, id) => { return tfService.GetDataProvider(id) != null; })
					.WithMessage("There is no existing data provider for specified identifier.");

			RuleFor(provider => provider.Name)
				.Must((provider, name) =>
				{
					var existingObj = tfService.GetDataProvider(provider.Name);
					return !(existingObj != null && existingObj.Id != provider.Id);
				})
				.WithMessage("There is already existing data provider with specified name.");

			RuleFor(provider => provider.SynchScheduleMinutes)
				.Must((provider, syncMinutes) =>
				{
					return syncMinutes >= 15;
				})
				.WithMessage("Minimum time between synchronizations is 15 min");
		}

		public ValidationResult ValidateUpdate(TfUpdateDataProvider provider)
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

			//RuleFor(provider => provider.Columns)
			//		.Must((provider, columns) =>
			//		{
			//			if (columns.Count > 0)
			//				return false;

			//			return true;
			//		})
			//		.WithMessage("The data provider contains columns be deleted.");
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

public record DatabaseColumnTypeInfo
{
	public string Name { get; init; }
	public TfDatabaseColumnType Type { get; init; }
	public bool CanBeProviderDataType { get; init; }
	public bool SupportAutoDefaultValue { get; init; }
}