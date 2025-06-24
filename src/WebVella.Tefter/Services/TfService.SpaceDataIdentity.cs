using Microsoft.FluentUI.AspNetCore.Components;
using WebVella.Tefter.Models;

namespace WebVella.Tefter.Services;

public partial interface ITfService
{
	public TfSpaceDataIdentity GetSpaceDataIdentity(
	   Guid id);

	public List<TfSpaceDataIdentity> GetSpaceDataIdentities(
		Guid spaceDataId);

	public List<TfSpaceDataIdentity> GetSpaceDataIdentities(
	   string dataIdentity);

	public TfSpaceDataIdentity CreateSpaceDataIdentity(
		TfSpaceDataIdentity identity);

	public TfSpaceDataIdentity UpdateSpaceDataIdentity(
		TfSpaceDataIdentity identity);

	public void DeleteSpaceDataIdentity(
		Guid id);
}

public partial class TfService : ITfService
{
	public TfSpaceDataIdentity GetSpaceDataIdentity(
		Guid id)
	{
		try
		{
			var dbo = _dboManager.Get<TfSpaceDataIdentityDbo>(id);

			if (dbo == null)
				return null;

			return SpaceDataIdentityFromDbo(dbo);
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}


	public List<TfSpaceDataIdentity> GetSpaceDataIdentities(
		Guid spaceDataId)
	{
		try
		{
			var orderSettings = new TfOrderSettings(
				nameof(TfSpaceDataIdentity.DataIdentity),
				OrderDirection.ASC);

			var dbos = _dboManager.GetList<TfSpaceDataIdentityDbo>(
				spaceDataId,
				nameof(TfSpaceDataIdentityDbo.SpaceDataId),
				order: orderSettings
			);


			return dbos
				.Select(x => SpaceDataIdentityFromDbo(x))
				.ToList();
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public List<TfSpaceDataIdentity> GetSpaceDataIdentities(
		string dataIdentity)
	{
		try
		{
			var orderSettings = new TfOrderSettings(
				nameof(TfSpaceDataIdentity.DataIdentity),
				OrderDirection.ASC);

			var dbos = _dboManager.GetList<TfSpaceDataIdentityDbo>(
				dataIdentity,
				nameof(TfSpaceDataIdentityDbo.DataIdentity),
				order: orderSettings
			);


			return dbos
				.Select(x => SpaceDataIdentityFromDbo(x))
				.ToList();
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public TfSpaceDataIdentity CreateSpaceDataIdentity(
		TfSpaceDataIdentity dataIdentity)
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
				var dbo = SpaceDataIdentityToDbo(dataIdentity);

				var success = _dboManager.Insert<TfSpaceDataIdentityDbo>(dbo);
				if (!success)
					throw new TfDboServiceException("Insert<TfSpaceDataIdentityDbo>");

				scope.Complete();

				return GetSpaceDataIdentity(dataIdentity.Id);
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}


	public TfSpaceDataIdentity UpdateSpaceDataIdentity(
		TfSpaceDataIdentity dataIdentity)
	{
		try
		{
			new TfSpaceDataIdentityValidator(this)
				.ValidateUpdate(dataIdentity)
				.ToValidationException()
				.ThrowIfContainsErrors();

			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var dbo = SpaceDataIdentityToDbo(dataIdentity);

				var success = _dboManager.Update<TfSpaceDataIdentityDbo>(dbo);
				if (!success)
					throw new TfDboServiceException("Update<TfSpaceDataIdentityDbo>");

				scope.Complete();

				return GetSpaceDataIdentity(dataIdentity.Id);
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}


	public void DeleteSpaceDataIdentity(
		Guid id)
	{
		try
		{
			var dataIdentity = GetSpaceDataIdentity(id);

			new TfSpaceDataIdentityValidator(this)
				.ValidateDelete(dataIdentity)
				.ToValidationException()
				.ThrowIfContainsErrors();

			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				bool success = _dboManager.Delete<TfSpaceDataIdentityDbo>(id);
				if (!success)
					throw new TfDboServiceException("Delete<TfSpaceDataIdentityDbo> failed.");

				scope.Complete();
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}


	#region <--- utility --->

	private static TfSpaceDataIdentityDbo SpaceDataIdentityToDbo(
		TfSpaceDataIdentity identity)
	{
		if (identity == null)
			throw new ArgumentException(nameof(identity));

		string columnNamesJson = "[]";
		if (identity.Columns is not null)
			columnNamesJson = JsonSerializer.Serialize(identity.Columns);

		return new TfSpaceDataIdentityDbo
		{
			Id = identity.Id,
			SpaceDataId = identity.SpaceDataId,
			DataIdentity = identity.DataIdentity,
			ColumnNamesJson = columnNamesJson
		};
	}

	private static TfSpaceDataIdentity SpaceDataIdentityFromDbo(
		TfSpaceDataIdentityDbo dbo)
	{
		if (dbo == null)
			throw new ArgumentException(nameof(dbo));

		return new TfSpaceDataIdentity
		{
			Id = dbo.Id,
			DataIdentity = dbo.DataIdentity,
			SpaceDataId = dbo.SpaceDataId,
			Columns = JsonSerializer.Deserialize<List<string>>(dbo.ColumnNamesJson ?? "[]")
		};
	}

	#endregion

	#region <--- validation --->

	internal class TfSpaceDataIdentityValidator
		: AbstractValidator<TfSpaceDataIdentity>
	{
		private readonly ITfService _tfService;

		public TfSpaceDataIdentityValidator(
			ITfService tfService)
		{

			_tfService = tfService;

			RuleSet("general", () =>
			{
				RuleFor(spaceDataIdentity => spaceDataIdentity.Id)
					.NotEmpty()
					.WithMessage("The data provider identity id is required.");

				RuleFor(dataProviderIdentity => dataProviderIdentity.SpaceDataId)
					.NotEmpty()
					.WithMessage("The space data id is required.");

				RuleFor(spaceDataIdentity => spaceDataIdentity.SpaceDataId)
					.Must(spaceDataId=> { return tfService.GetSpaceData(spaceDataId) != null; })
					.WithMessage("There is no existing space data for specified space data id.");

				RuleFor(spaceDataIdentity => spaceDataIdentity.DataIdentity)
					.NotEmpty()
					.WithMessage("The identity is required.");

				RuleFor(spaceDataIdentity => spaceDataIdentity.DataIdentity)
					.Must(spaceDataIdentity =>
					{
						if (string.IsNullOrWhiteSpace(spaceDataIdentity))
							return true;

						var identity = _tfService.GetDataIdentity(spaceDataIdentity);
						return identity != null;
					})
					.WithMessage($"Selected identity is not found.");

				RuleFor(spaceDataIdentity => spaceDataIdentity.Columns)
					.NotEmpty()
					.WithMessage("The space data identity requires at least one column selected.");

				RuleFor(spaceDataIdentity => spaceDataIdentity.Columns)
					.Must((spaceDataIdentity, columns) =>
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

								if (sharedColumn.DataIdentity != spaceDataIdentity.DataIdentity)
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

							if(providerIdentities.Any(x=>x.DataIdentity == spaceDataIdentity.DataIdentity) == false)
								return false;	

							var exists = provider.Columns.Any(x => x.DbName == column);
							if (!exists)
								return false;
						}

						return true;
					})
					.WithMessage($"Some of the selected columns cannot be found in any related by specified identity provider columns list.");

				RuleFor(spaceDataIdentity => spaceDataIdentity.Columns)
					.Must((spaceDataIdentity, columns) =>
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
				RuleFor(spaceDataIdentity => spaceDataIdentity.Id)
					.Must((dataProviderIdentity, id) =>
					{
						return tfService.GetSpaceDataIdentity(id) == null;
					})
					.WithMessage("There is already existing space data identity with specified identifier.");


				RuleFor(spaceDataIdentity => spaceDataIdentity.DataIdentity)
					.Must((spaceDataIdentity, dataIdentity) =>
					{
						if (string.IsNullOrEmpty(dataIdentity))
							return true;

						var identities = tfService.GetSpaceDataIdentities(spaceDataIdentity.SpaceDataId);
						return !identities.Any(x => x.DataIdentity.ToLowerInvariant().Trim() == dataIdentity.ToLowerInvariant().Trim());
					})
					.WithMessage("There is already existing space data identity for specified identity attached to space data.");
			});

			RuleSet("update", () =>
			{
				RuleFor(spaceDataIdentity => spaceDataIdentity.Id)
					.Must((dataIdentity, id) =>
					{
						return tfService.GetSpaceDataIdentity(id) != null;
					})
					.WithMessage("There is not existing space data identity with specified identifier.");

				RuleFor(spaceDataIdentity => spaceDataIdentity.SpaceDataId)
					.Must((spaceDataIdentity, spaceDataId) =>
					{
						var existingDataIdentity = tfService.GetSpaceDataIdentity(spaceDataIdentity.Id);
						if (existingDataIdentity is null)
							return true;

						return existingDataIdentity.SpaceDataId == spaceDataId;
					})
					.WithMessage("There space data cannot be changed for space data identity.");

				RuleFor(dataIdentity => dataIdentity.DataIdentity)
					.Must((dataIdentity, dbName) =>
					{

						var existingDataIdentity = tfService.GetSpaceDataIdentity(dataIdentity.Id);
						if (existingDataIdentity is null)
							return true;

						return existingDataIdentity.DataIdentity == dbName;
					})
					.WithMessage("There identity cannot be changed.");
			});
		}

		public ValidationResult ValidateCreate(
			TfSpaceDataIdentity dataIdentity)
		{
			if (dataIdentity == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The space data identity is null.") });

			return this.Validate(dataIdentity, options =>
			{
				options.IncludeRuleSets("general", "create");
			});
		}

		public ValidationResult ValidateUpdate(
			TfSpaceDataIdentity dataIdentity)
		{
			if (dataIdentity == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The space data identity is null.") });

			return this.Validate(dataIdentity, options =>
			{
				options.IncludeRuleSets("general", "update");
			});
		}

		public ValidationResult ValidateDelete(
			TfSpaceDataIdentity dataIdentity)
		{
			if (dataIdentity == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The space data identity is null.") });

			return new ValidationResult();
		}
	}

	#endregion

}
