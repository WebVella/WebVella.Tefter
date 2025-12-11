using Nito.AsyncEx.Synchronous;
using WebVella.Tefter.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace WebVella.Tefter.Services;

public partial interface ITfService
{
	/// <summary>
	/// Gets a data provider identity by its unique identifier.
	/// </summary>
	/// <param name="id">The unique identifier of the data provider identity.</param>
	/// <returns>The <see cref="TfDataProviderIdentity"/> if found; otherwise, null.</returns>
	public TfDataProviderIdentity GetDataProviderIdentity(
	   Guid id);

	/// <summary>
	/// Gets a list of data provider identities for a specified data provider.
	/// </summary>
	/// <param name="providerId">The unique identifier of the data provider.</param>
	/// <returns>A list of <see cref="TfDataProviderIdentity"/> objects.</returns>
	public List<TfDataProviderIdentity> GetDataProviderIdentities(
		Guid providerId);

	/// <summary>
	/// Gets a list of data provider identities by the data identity key.
	/// </summary>
	/// <param name="dataIdentity">The data identity key.</param>
	/// <returns>A list of <see cref="TfDataProviderIdentity"/> objects.</returns>
	public List<TfDataProviderIdentity> GetDataProviderIdentities(
	   string dataIdentity);

	/// <summary>
	/// Creates a new data provider identity.
	/// </summary>
	/// <param name="identity">The <see cref="TfDataProviderIdentity"/> to create.</param>
	/// <returns>The updated <see cref="TfDataProvider"/> after creation.</returns>
	public TfDataProvider CreateDataProviderIdentity(
		TfDataProviderIdentity identity);

	/// <summary>
	/// Updates an existing data provider identity.
	/// </summary>
	/// <param name="identity">The <see cref="TfDataProviderIdentity"/> to update.</param>
	/// <returns>The updated <see cref="TfDataProvider"/> after update.</returns>
	public TfDataProvider UpdateDataProviderIdentity(
		TfDataProviderIdentity identity);

	/// <summary>
	/// Deletes a data provider identity by its unique identifier.
	/// </summary>
	/// <param name="id">The unique identifier of the data provider identity to delete.</param>
	/// <returns>The updated <see cref="TfDataProvider"/> after deletion.</returns>
	public TfDataProvider DeleteDataProviderIdentity(
		Guid id);
}

public partial class TfService : ITfService
{
	public TfDataProviderIdentity GetDataProviderIdentity(
		Guid id)
	{
		try
		{
			var dbo = _dboManager.Get<TfDataProviderIdentityDbo>(id);

			if (dbo == null)
				return null;

			return DataProviderIdentityFromDbo(dbo);
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}


	public List<TfDataProviderIdentity> GetDataProviderIdentities(
		Guid providerId)
	{
		try
		{
			var orderSettings = new TfOrderSettings(
				nameof(TfDataProviderIdentity.DataIdentity),
				OrderDirection.ASC);

			var dbos = _dboManager.GetList<TfDataProviderIdentityDbo>(
				providerId,
				nameof(TfDataProviderIdentityDbo.DataProviderId),
				order: orderSettings
			);


			return dbos
				.Select(x => DataProviderIdentityFromDbo(x))
				.ToList();
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public List<TfDataProviderIdentity> GetDataProviderIdentities(
		string dataIdentity)
	{
		try
		{
			var orderSettings = new TfOrderSettings(
				nameof(TfDataProviderIdentity.DataIdentity),
				OrderDirection.ASC);

			var dbos = _dboManager.GetList<TfDataProviderIdentityDbo>(
				dataIdentity,
				nameof(TfDataProviderIdentityDbo.DataIdentity),
				order: orderSettings
			);


			return dbos
				.Select(x => DataProviderIdentityFromDbo(x))
				.ToList();
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public TfDataProvider CreateDataProviderIdentity(
		TfDataProviderIdentity dataIdentity)
	{
		try
		{
			if (dataIdentity != null && dataIdentity.Id == Guid.Empty)
				dataIdentity.Id = Guid.NewGuid();

			new TfDataProviderIdentityValidator(this)
				.ValidateCreate(dataIdentity)
				.ToValidationException()
				.ThrowIfContainsErrors();

			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var dbo = DataProviderIdentityToDbo(dataIdentity);

				var success = _dboManager.Insert<TfDataProviderIdentityDbo>(dbo);
				if (!success)
					throw new TfDboServiceException("Insert<TfDataProviderIdentityDbo>");

				var provider = GetDataProvider(dataIdentity.DataProviderId);
				if (provider is null)
					throw new TfException("Failed to create new data provider identity");

				string providerTableName = $"dp{provider.Index}";
				string identityColumnName = $"tf_ide_{dataIdentity.DataIdentity}";
				string identityColumnIndexName = $"ix_{providerTableName}_{identityColumnName}";

				TfDatabaseBuilder dbBuilder = _dbManager.GetDatabaseBuilder();

				dbBuilder
					.WithTableBuilder(providerTableName)
					.WithColumns(c =>
					{
						c.AddShortTextColumn(identityColumnName, col =>
						{
							col.AsSha1ExpressionFromColumns(dataIdentity.Columns.ToArray());
						});
					});

				dbBuilder
					.WithTableBuilder(providerTableName)
					.WithIndexes(indexes =>
					{
						indexes
							.AddBTreeIndex(identityColumnIndexName, i =>
							{
								i.WithColumns(identityColumnName);
							});
					});

				_dbManager.SaveChanges(dbBuilder);

				scope.Complete();

				var result = GetDataProvider(dataIdentity.DataProviderId);
				PublishEventWithScope(new TfDataProviderUpdatedEvent(result));
				return result;
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}


	public TfDataProvider UpdateDataProviderIdentity(
		TfDataProviderIdentity dataIdentity)
	{
		try
		{
			new TfDataProviderIdentityValidator(this)
				.ValidateUpdate(dataIdentity)
				.ToValidationException()
				.ThrowIfContainsErrors();

			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var dbo = DataProviderIdentityToDbo(dataIdentity);

				var success = _dboManager.Update<TfDataProviderIdentityDbo>(dbo);
				if (!success)
					throw new TfDboServiceException("Update<TfDataProviderIdentityDbo>");

				var provider = GetDataProvider(dataIdentity.DataProviderId);
				if (provider is null)
					throw new TfException("Failed to create new data provider identity");

				string providerTableName = $"dp{provider.Index}";
				string identityColumnName = $"tf_ide_{dataIdentity.DataIdentity}";
				string identityColumnIndexName = $"ix_{providerTableName}_{identityColumnName}";

				TfDatabaseBuilder dbBuilder = _dbManager.GetDatabaseBuilder();

				//first remove the old identity column
				//it will remove related indexes
				dbBuilder
					.WithTableBuilder(providerTableName)
					.WithColumns(c => { c.Remove(identityColumnName); });

				_dbManager.SaveChanges(dbBuilder);

				dbBuilder = _dbManager.GetDatabaseBuilder();

				//now add the new identity column and new index
				dbBuilder
					.WithTableBuilder(providerTableName)
					.WithColumns(c =>
					{
						c.AddShortTextColumn(identityColumnName, col =>
						{
							col.AsSha1ExpressionFromColumns(dataIdentity.Columns.ToArray());
						});
					});

				dbBuilder
					.WithTableBuilder(providerTableName)
					.WithIndexes(indexes =>
					{
						indexes
							.AddBTreeIndex(identityColumnIndexName, i =>
							{
								i.WithColumns(identityColumnName);
							});
					});

				_dbManager.SaveChanges(dbBuilder);

				scope.Complete();
				var result = GetDataProvider(dataIdentity.DataProviderId);
				PublishEventWithScope(new TfDataProviderUpdatedEvent(result));
				return result;				
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}


	public TfDataProvider DeleteDataProviderIdentity(
		Guid id)
	{
		try
		{
			var dataIdentity = GetDataProviderIdentity(id);

			new TfDataProviderIdentityValidator(this)
				.ValidateDelete(dataIdentity)
				.ToValidationException()
				.ThrowIfContainsErrors();

			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var provider = GetDataProvider(dataIdentity.DataProviderId);
				if (provider is null)
					throw new TfException("Failed to create new data provider identity");

				string providerTableName = $"dp{provider.Index}";
				string identityColumnName = $"tf_ide_{dataIdentity.DataIdentity}";
				string identityColumnIndexName = $"ix_{providerTableName}_{identityColumnName}";

				TfDatabaseBuilder dbBuilder = _dbManager.GetDatabaseBuilder();

				dbBuilder
					.WithTableBuilder(providerTableName)
					.WithColumnsBuilder(c => { c.Remove(identityColumnName); });

				dbBuilder
					.WithTableBuilder(providerTableName)
					.WithIndexesBuilder(i => { i.Remove(identityColumnIndexName); });

				_dbManager.SaveChanges(dbBuilder);

				bool success = _dboManager.Delete<TfDataProviderIdentityDbo>(id);
				if (!success)
					throw new TfDboServiceException("Delete<TfDataProviderIdentityDbo> failed.");

				scope.Complete();
				
				var result = GetDataProvider(dataIdentity.DataProviderId);
				PublishEventWithScope(new TfDataProviderUpdatedEvent(result));
				return result;
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}


	#region <--- utility --->

	private static TfDataProviderIdentityDbo DataProviderIdentityToDbo(
		TfDataProviderIdentity identity)
	{
		if (identity == null)
			throw new ArgumentException(nameof(identity));

		string columnNamesJson = "[]";
		if (identity.Columns is not null)
			columnNamesJson = JsonSerializer.Serialize(identity.Columns);

		return new TfDataProviderIdentityDbo
		{
			Id = identity.Id,
			DataProviderId = identity.DataProviderId,
			DataIdentity = identity.DataIdentity,
			ColumnNamesJson = columnNamesJson
		};
	}

	private static TfDataProviderIdentity DataProviderIdentityFromDbo(
		TfDataProviderIdentityDbo dbo)
	{
		if (dbo == null)
			throw new ArgumentException(nameof(dbo));

		return new TfDataProviderIdentity
		{
			Id = dbo.Id,
			DataIdentity = dbo.DataIdentity,
			DataProviderId = dbo.DataProviderId,
			Columns = JsonSerializer.Deserialize<List<string>>(dbo.ColumnNamesJson ?? "[]")
		};
	}

	#endregion

	#region <--- validation --->

	internal class TfDataProviderIdentityValidator
		: AbstractValidator<TfDataProviderIdentity>
	{
		private readonly ITfService _tfService;

		public TfDataProviderIdentityValidator(
			ITfService tfService)
		{

			_tfService = tfService;

			RuleSet("general", () =>
			{
				RuleFor(dataProviderIdentity => dataProviderIdentity.Id)
					.NotEmpty()
					.WithMessage("The data provider identity id is required.");

				RuleFor(dataProviderIdentity => dataProviderIdentity.DataProviderId)
					.NotEmpty()
					.WithMessage("The data provider id is required.");

				RuleFor(dataProviderIdentity => dataProviderIdentity.DataProviderId)
					.Must(providerId => { return tfService.GetDataProvider(providerId) != null; })
					.WithMessage("There is no existing data provider for specified provider id.");

				RuleFor(dataProviderIdentity => dataProviderIdentity.DataIdentity)
					.NotEmpty()
					.WithMessage("The data provider identity is required.");

				RuleFor(dataProviderIdentity => dataProviderIdentity.DataIdentity)
					.Must(dataProviderIdentity =>
					{
						if (string.IsNullOrWhiteSpace(dataProviderIdentity))
							return true;

						var identity = _tfService.GetDataIdentity(dataProviderIdentity);
						return identity != null;
					})
					.WithMessage($"Selected data identity is not found.");

				RuleFor(dataProviderIdentity => dataProviderIdentity.Columns)
					.NotEmpty()
					.WithMessage("The data identity requires at least one column selected.");

				RuleFor(dataProviderIdentity => dataProviderIdentity.Columns)
					.Must((dataProviderIdentity, columns) =>
					{
						if (columns == null)
							return true;

						var provider = _tfService.GetDataProvider(dataProviderIdentity.DataProviderId);
						if (provider is null)
							return true;

						foreach (var column in columns)
						{
							var exists = provider.SystemColumns.Any(x => x.DbName == column);
							if (!exists)
							{
								exists = provider.Columns.Any(x => x.DbName == column);
								if (!exists)
									return false;
							}
						}

						return true;
					})
					.WithMessage($"Some of the selected columns cannot be found in data provider columns list.");

				RuleFor(dataProviderIdentity => dataProviderIdentity.Columns)
					.Must((dataProviderIdentity, columns) =>
					{
						if (columns == null)
							return true;

						HashSet<string> columnNames = new HashSet<string>();

						foreach (var column in columns)
						{
							var exists = columnNames.Contains(column);
							if (exists)
								return false;

							columnNames.Add(column);
						}

						return true;
					})
					.WithMessage($"There are same columns added more than once in the identity. Its not allowed.");
			});

			RuleSet("create", () =>
			{
				RuleFor(dataProviderIdentity => dataProviderIdentity.Id)
					.Must((dataProviderIdentity, id) =>
					{
						return tfService.GetDataProviderIdentity(id) == null;
					})
					.WithMessage("There is already existing identity with specified identifier.");


				RuleFor(dataProviderIdentity => dataProviderIdentity.DataIdentity)
					.Must((dataProviderIdentity, dataIdentity) =>
					{
						if (string.IsNullOrEmpty(dataIdentity))
							return true;

						var identities = tfService.GetDataProviderIdentities(dataProviderIdentity.DataProviderId);
						return !identities.Any(x => x.DataIdentity.ToLowerInvariant().Trim() == dataIdentity.ToLowerInvariant().Trim());
					})
					.WithMessage("There is already existing data provider identity for specified identity attached to data provider.");
			});

			RuleSet("update", () =>
			{
				RuleFor(dataIdentity => dataIdentity.Id)
					.Must((dataIdentity, id) =>
					{
						return tfService.GetDataProviderIdentity(id) != null;
					})
					.WithMessage("There is not existing identity with specified identifier.");

				RuleFor(dataIdentity => dataIdentity.DataProviderId)
					.Must((dataIdentity, providerId) =>
					{

						var existingDataIdentity = tfService.GetDataProviderIdentity(dataIdentity.Id);
						if (existingDataIdentity is null)
							return true;

						return existingDataIdentity.DataProviderId == providerId;
					})
					.WithMessage("There data provider cannot be changed for provider data identity.");

				RuleFor(dataIdentity => dataIdentity.DataIdentity)
					.Must((dataIdentity, dbName) =>
					{

						var existingDataIdentity = tfService.GetDataProviderIdentity(dataIdentity.Id);
						if (existingDataIdentity is null)
							return true;

						return existingDataIdentity.DataIdentity == dbName;
					})
					.WithMessage("The identity cannot be changed.");
			});
		}

		public ValidationResult ValidateCreate(
			TfDataProviderIdentity dataIdentity)
		{
			if (dataIdentity == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The data provider identity is null.") });

			return this.Validate(dataIdentity, options =>
			{
				options.IncludeRuleSets("general", "create");
			});
		}

		public ValidationResult ValidateUpdate(
			TfDataProviderIdentity dataIdentity)
		{
			if (dataIdentity == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The data provider identity is null.") });

			if (dataIdentity.DataIdentity == TfConstants.TEFTER_DEFAULT_OBJECT_NAME)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The default data provider identity cannot be edited.") });

			return this.Validate(dataIdentity, options =>
			{
				options.IncludeRuleSets("general", "update");
			});
		}

		public ValidationResult ValidateDelete(
			TfDataProviderIdentity dataIdentity)
		{
			if (dataIdentity == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The data provider identity is null.") });

			if (dataIdentity.DataIdentity == TfConstants.TEFTER_DEFAULT_OBJECT_NAME)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The default data provider identity cannot be deleted.") });

			return new ValidationResult();
		}
	}

	#endregion
}