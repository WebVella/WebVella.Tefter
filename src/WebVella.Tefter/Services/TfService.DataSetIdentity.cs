using Microsoft.FluentUI.AspNetCore.Components;
using WebVella.Tefter.Models;

namespace WebVella.Tefter.Services;

public partial interface ITfService
{
	public TfDatasetIdentity? GetDatasetIdentity(
	   Guid id);

	public List<TfDatasetIdentity> GetDatasetIdentities(
		Guid datasetId);

	public List<TfDatasetIdentity> GetDatasetIdentities(
	   string dataIdentity);

	public TfDatasetIdentity? CreateDatasetIdentity(
		TfDatasetIdentity? identity);

	public TfDatasetIdentity? UpdateDatasetIdentity(
		TfDatasetIdentity? identity);

	public void DeleteDatasetIdentity(
		Guid id);
}

public partial class TfService : ITfService
{
	public TfDatasetIdentity? GetDatasetIdentity(
		Guid id)
	{
		try
		{
			var dbo = _dboManager.Get<TfDatasetIdentityDbo>(id);

			if (dbo == null)
				return null;

			return DatasetIdentityFromDbo(dbo);
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}


	public List<TfDatasetIdentity> GetDatasetIdentities(
		Guid spaceDataId)
	{
		try
		{
			var orderSettings = new TfOrderSettings(
				nameof(TfDatasetIdentity.DataIdentity),
				OrderDirection.ASC);

			var dbos = _dboManager.GetList<TfDatasetIdentityDbo>(
				spaceDataId,
				nameof(TfDatasetIdentityDbo.DatasetId),
				order: orderSettings
			);

			return dbos
				.Select(x => DatasetIdentityFromDbo(x))
				.ToList();
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public List<TfDatasetIdentity> GetDatasetIdentities(
		string dataIdentity)
	{
		try
		{
			var orderSettings = new TfOrderSettings(
				nameof(TfDatasetIdentity.DataIdentity),
				OrderDirection.ASC);

			var dbos = _dboManager.GetList<TfDatasetIdentityDbo>(
				dataIdentity,
				nameof(TfDatasetIdentityDbo.DataIdentity),
				order: orderSettings
			);

			return dbos
				.Select(x => DatasetIdentityFromDbo(x))
				.ToList();
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public TfDatasetIdentity? CreateDatasetIdentity(
		TfDatasetIdentity? dataIdentity)
	{
		try
		{
			if (dataIdentity != null && dataIdentity.Id == Guid.Empty)
				dataIdentity.Id = Guid.NewGuid();

			new TfSpaceDataIdentityValidator(this)
				.ValidateCreate(dataIdentity)
				.ToValidationException()
				.ThrowIfContainsErrors();

			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var dbo = DatasetIdentityToDbo(dataIdentity);

				var success = _dboManager.Insert<TfDatasetIdentityDbo>(dbo);
				if (!success)
					throw new TfDboServiceException("Insert<TfDatasetIdentityDbo>");

				scope.Complete();

				if (dataIdentity is not null)
					return GetDatasetIdentity(dataIdentity.Id);
				else
					throw new ArgumentException(nameof(dataIdentity));
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}


	public TfDatasetIdentity? UpdateDatasetIdentity(
		TfDatasetIdentity? dataIdentity)
	{
		try
		{
			new TfSpaceDataIdentityValidator(this)
				.ValidateUpdate(dataIdentity)
				.ToValidationException()
				.ThrowIfContainsErrors();

			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var dbo = DatasetIdentityToDbo(dataIdentity);

				var success = _dboManager.Update<TfDatasetIdentityDbo>(dbo);
				if (!success)
					throw new TfDboServiceException("Update<TfDatasetIdentityDbo>");

				scope.Complete();

				if(dataIdentity is not null)
					return GetDatasetIdentity(dataIdentity.Id);
				else
					throw new ArgumentException(nameof(dataIdentity));
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}


	public void DeleteDatasetIdentity(
		Guid id)
	{
		try
		{
			var dataIdentity = GetDatasetIdentity(id);

			new TfSpaceDataIdentityValidator(this)
				.ValidateDelete(dataIdentity)
				.ToValidationException()
				.ThrowIfContainsErrors();

			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				bool success = _dboManager.Delete<TfDatasetIdentityDbo>(id);
				if (!success)
					throw new TfDboServiceException("Delete<TfDatasetIdentityDbo> failed.");

				scope.Complete();
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}


	#region <--- utility --->

	private static TfDatasetIdentityDbo DatasetIdentityToDbo(
		TfDatasetIdentity? identity)
	{
		if (identity == null)
			throw new ArgumentException(nameof(identity));

		string columnNamesJson = "[]";
		if (identity.Columns is not null)
			columnNamesJson = JsonSerializer.Serialize(identity.Columns);

		return new TfDatasetIdentityDbo
		{
			Id = identity.Id,
			DatasetId = identity.DatasetId,
			DataIdentity = identity.DataIdentity,
			ColumnNamesJson = columnNamesJson
		};
	}

	private static TfDatasetIdentity DatasetIdentityFromDbo(
		TfDatasetIdentityDbo? dbo)
	{
		if (dbo == null)
			throw new ArgumentException(nameof(dbo));

		var columns = JsonSerializer.Deserialize<List<string>>(dbo.ColumnNamesJson ?? "[]") ?? new List<string>();

		return new TfDatasetIdentity
		{
			Id = dbo.Id,
			DataIdentity = dbo.DataIdentity,
			DatasetId = dbo.DatasetId,
			Columns = columns
		};
	}

	#endregion

	#region <--- validation --->

	internal class TfSpaceDataIdentityValidator
		: AbstractValidator<TfDatasetIdentity>
	{
		private readonly ITfService _tfService;

		public TfSpaceDataIdentityValidator(
			ITfService tfService)
		{

			_tfService = tfService;

			RuleSet("general", () =>
			{
				RuleFor(datasetIdentity => datasetIdentity.Id)
					.NotEmpty()
					.WithMessage("The data provider identity id is required.");

				RuleFor(dataProviderIdentity => dataProviderIdentity.DatasetId)
					.NotEmpty()
					.WithMessage("The dataset id is required.");

				RuleFor(datasetIdentity => datasetIdentity.DatasetId)
					.Must( datasetId=> { return tfService.GetDataset(datasetId) != null; })
					.WithMessage("There is no existing dataset for specified dataset id.");

				RuleFor(datasetIdentity => datasetIdentity.DataIdentity)
					.NotEmpty()
					.WithMessage("The identity is required.");

				RuleFor(datasetIdentity => datasetIdentity.DataIdentity)
					.Must(datasetIdentity =>
					{
						if (string.IsNullOrWhiteSpace(datasetIdentity))
							return true;

						var identity = _tfService.GetDataIdentity(datasetIdentity);
						return identity != null;
					})
					.WithMessage($"Selected identity is not found.");

				RuleFor(datasetIdentity => datasetIdentity.Columns)
					.NotEmpty()
					.WithMessage("The dataset identity requires at least one column selected.");

				RuleFor(datasetIdentity => datasetIdentity.Columns)
					.Must((datasetIdentity, columns) =>
					{
						if (columns == null)
							return true;

						foreach (var column in columns)
						{
							if(column.StartsWith(TfConstants.TF_SHARED_COLUMN_PREFIX))
							{
								var sharedColumn = _tfService.GetSharedColumn(column);
								if (sharedColumn is null)
									return false;

								if (sharedColumn.DataIdentity != datasetIdentity.DataIdentity)
									return false;

								return true;
							}	

							if (column == TfConstants.TF_ROW_ID_DATA_IDENTITY)
								continue;

							var providerIndex = Int32.Parse(column.Split('_').First().Substring(2));
							var provider = _tfService.GetDataProvider(providerIndex);
							if (provider is null)
								return false;

							var providerIdentities = _tfService.GetDataProviderIdentities(provider.Id);
							if (providerIdentities == null || providerIdentities.Count == 0)
								return false;

							if(providerIdentities.Any(x=>x.DataIdentity == datasetIdentity.DataIdentity) == false)
								return false;	

							var exists = provider.Columns.Any(x => x.DbName == column);
							if (!exists)
								return false;
						}

						return true;
					})
					.WithMessage($"Some of the selected columns cannot be found in any related by specified identity provider columns list.");

				RuleFor(datasetIdentity => datasetIdentity.Columns)
					.Must((dataSetIdentity, columns) =>
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
				RuleFor(datasetIdentity => datasetIdentity.Id)
					.Must((datasetIdentity, id) =>
					{
						return tfService.GetDatasetIdentity(id) == null;
					})
					.WithMessage("There is already existing dataset identity with specified identifier.");


				RuleFor(datasetIdentity => datasetIdentity.DataIdentity)
					.Must((datasetIdentity, dataIdentity) =>
					{
						if (string.IsNullOrEmpty(dataIdentity))
							return true;

						var identities = tfService.GetDatasetIdentities(datasetIdentity.DatasetId);
						return !identities.Any(x => x.DataIdentity.ToLowerInvariant().Trim() == dataIdentity.ToLowerInvariant().Trim());
					})
					.WithMessage("There is already existing dataset identity for specified identity attached to space data.");
			});

			RuleSet("update", () =>
			{
				RuleFor(datasetIdentity => datasetIdentity.Id)
					.Must((datasetIdentity, id) =>
					{
						return tfService.GetDatasetIdentity(id) != null;
					})
					.WithMessage("There is not existing dataset identity with specified identifier.");

				RuleFor(datasetIdentity => datasetIdentity.DatasetId)
					.Must((datasetIdentity, datasetId) =>
					{
						var existingDataIdentity = tfService.GetDatasetIdentity(datasetIdentity.Id);
						if (existingDataIdentity is null)
							return true;

						return existingDataIdentity.DatasetId == datasetId;
					})
					.WithMessage("There dataset cannot be changed for dataset identity.");

				RuleFor(datasetIdentity => datasetIdentity.DataIdentity)
					.Must((datasetIdentity, dbName) =>
					{

						var existingDataIdentity = tfService.GetDatasetIdentity(datasetIdentity.Id);
						if (existingDataIdentity is null)
							return true;

						return existingDataIdentity.DataIdentity == dbName;
					})
					.WithMessage("There identity cannot be changed.");
			});
		}

		public ValidationResult ValidateCreate(
			TfDatasetIdentity? dataIdentity)
		{
			if (dataIdentity == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The dataset identity is null.") });

			return this.Validate(dataIdentity, options =>
			{
				options.IncludeRuleSets("general", "create");
			});
		}

		public ValidationResult ValidateUpdate(
			TfDatasetIdentity? dataIdentity)
		{
			if (dataIdentity == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The dataset identity is null.") });

			return this.Validate(dataIdentity, options =>
			{
				options.IncludeRuleSets("general", "update");
			});
		}

		public ValidationResult ValidateDelete(
			TfDatasetIdentity? dataIdentity)
		{
			if (dataIdentity == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The dataset identity is null.") });

			return new ValidationResult();
		}
	}

	#endregion

}
