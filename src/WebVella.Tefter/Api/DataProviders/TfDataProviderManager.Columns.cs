using Bogus.DataSets;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace WebVella.Tefter;

public partial interface ITfDataProviderManager
{
	internal TfDataProviderColumn GetDataProviderColumn(Guid id);

	internal List<TfDataProviderColumn> GetDataProviderColumns(Guid providerId);

	public Result<TfDataProvider> CreateDataProviderColumn(TfDataProviderColumn column);

	public Result<TfDataProvider> UpdateDataProviderColumn(TfDataProviderColumn column);

	public Result<TfDataProvider> DeleteDataProviderColumn(Guid id);
}

public partial class TfDataProviderManager : ITfDataProviderManager
{
	public TfDataProviderColumn GetDataProviderColumn(Guid id)
	{
		return _dboManager.Get<TfDataProviderColumn>(id);
	}

	public List<TfDataProviderColumn> GetDataProviderColumns(Guid providerId)
	{
		return _dboManager.GetList<TfDataProviderColumn>(providerId, nameof(TfDataProviderColumn.DataProviderId));
	}

	public Result<TfDataProvider> CreateDataProviderColumn(TfDataProviderColumn column)
	{
		try
		{
			if (column.Id == Guid.Empty)
				column.Id = Guid.NewGuid();

			TfDataProviderColumnValidator validator =
				new TfDataProviderColumnValidator(_dboManager, this);

			var validationResult = validator.ValidateCreate(column);
			if (!validationResult.IsValid)
				return validationResult.ToResult();

			using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{

				var success = _dboManager.Insert<TfDataProviderColumn>(column);

				if (!success)
					return Result.Fail(new DboManagerError("Insert", column));

				var providerResult = GetProvider(column.DataProviderId);
				if (providerResult.IsFailed)
					return Result.Fail(new Error("Failed to create new data provider column")
						.CausedBy(providerResult.Errors));

				var provider = providerResult.Value;
				string providerTableName = $"dp{provider.Index}";

				//DatabaseBuilder dbBuilder = _dbManager.GetDatabaseBuilder();
				//dbBuilder.WithTableBuilder(providerTableName)
				//	.WithColumns(columns =>
				//	{
				//		columns
				//			.AddGuidColumn("tf_id", c => { c.WithoutAutoDefaultValue().NotNullable(); })
				//			.AddDateTimeColumn("tf_created_on", c => { c.WithoutAutoDefaultValue().NotNullable(); })
				//			.AddDateTimeColumn("tf_updated_on", c => { c.WithoutAutoDefaultValue().NotNullable(); })
				//			.AddTextColumn("tf_search", c => { c.NotNullable().WithDefaultValue(string.Empty); });
				//	})
				//	.WithConstraints(constraints =>
				//	{
				//		constraints
				//			.AddPrimaryKeyConstraintBuilder($"pk_{providerTableName}", c => { c.WithColumns("tf_id"); });
				//	})
				//	.WithIndexes(indexes =>
				//	{
				//		indexes
				//			.AddBTreeIndex($"ix_{providerTableName}_tf_id", c => { c.WithColumns("tf_id"); })
				//			.AddGinIndex($"ix_{providerTableName}_tf_search", c => { c.WithColumns("tf_search"); });
				//	});

				//_dbManager.SaveChanges(dbBuilder);

				scope.Complete();

				return Result.Ok(provider);
			}
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to create new data provider column.").CausedBy(ex));
		}
	}

	public Result<TfDataProvider> UpdateDataProviderColumn(TfDataProviderColumn column)
	{
		try
		{
			TfDataProviderColumnValidator validator =
				new TfDataProviderColumnValidator(_dboManager, this);

			var validationResult = validator.ValidateUpdate(column);
			if (!validationResult.IsValid)
				return validationResult.ToResult();

			var success = _dboManager.Update<TfDataProviderColumn>(column);

			if (!success)
				return Result.Fail(new DboManagerError("Update", column));

			return Result.Ok(GetProvider(column.DataProviderId).Value);
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to create new data provider column.").CausedBy(ex));
		}
	}

	public Result<TfDataProvider> DeleteDataProviderColumn(Guid id)
	{
		try
		{
			using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				TfDataProviderColumnValidator validator =
				new TfDataProviderColumnValidator(_dboManager, this);

				var column = GetDataProviderColumn(id);

				var validationResult = validator.ValidateDelete(column);
				if (!validationResult.IsValid)
					return validationResult.ToResult();

				var success = _dboManager.Delete<TfDataProviderColumn>(id);

				if (!success)
					return Result.Fail(new DboManagerError("Delete", id));

				var providerResult = GetProvider(id);
				if (providerResult.IsFailed)
					return Result.Fail(new Error("Failed to delete provider column.")
						.CausedBy(providerResult.Errors));

				//var provider = providerResult.Value;
				//string providerTableName = $"dp{provider.Index}";

				//DatabaseBuilder dbBuilder = _dbManager.GetDatabaseBuilder();

				//dbBuilder.WithTableBuilder(providerTableName).WithColumns(columns=>columns.Remove(column.DbName));

				//_dbManager.SaveChanges(dbBuilder);

				scope.Complete();

				return Result.Ok(GetProvider(column.DataProviderId).Value);
			}
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to create new data provider column.").CausedBy(ex));
		}
	}

	#region <--- Validator --->

	internal class TfDataProviderColumnValidator
	: AbstractValidator<TfDataProviderColumn>
	{
		private readonly IDboManager _dboManager;
		private readonly ITfDataProviderManager _providerManager;

		public TfDataProviderColumnValidator(
			IDboManager dboManager,
			ITfDataProviderManager providerManager)
		{
			_dboManager = dboManager;
			_providerManager = providerManager;

			RuleSet("general", () =>
			{
				RuleFor(column => column.Id)
					.NotEmpty()
					.WithMessage("The data provider column id is required.");

				RuleFor(column => column.DataProviderId)
					.NotEmpty()
					.WithMessage("The data provider id is required.");

				RuleFor(provider => provider.DataProviderId)
					.Must(providerId => { return providerManager.GetProvider(providerId).Value != null; })
					.WithMessage("There is no existing data provider for specified provider id.");

				RuleFor(column => column.SourceName)
					.NotEmpty()
					.WithMessage("The data provider column source name is required.");

				RuleFor(column => column.SourceType)
					.NotEmpty()
					.WithMessage("The data provider column source type is required.");

				RuleFor(column => column.DbName)
					.NotEmpty()
					.WithMessage("The data provider column database name is required.");
				
				RuleFor(column => column.DbName)
					.Must( (column,dbName) => 
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

						Match match = Regex.Match(dbName, Constants.DB_OBJECT_NAME_VALIDATION_PATTERN);
						return match.Success && match.Value == dbName.Trim();
					})
					.WithMessage($"Name can only contains underscores and lowercase alphanumeric characters. It must begin with a letter, " +
						$"not include spaces, not end with an underscore, and not contain two consecutive underscores");

			});
/*
 * public static bool IsValidDbObjectName(string name, out string error)
				{
					error = null;

					if (string.IsNullOrEmpty(name))
					{
						error = "Name is required and cannot be empty";
						return false;
					}

					if (name.Length < Constants.DB_MIN_OBJECT_NAME_LENGTH)
						error = $"The name must be at least {Constants.DB_MIN_OBJECT_NAME_LENGTH} characters long";

					if (name.Length > Constants.DB_MAX_OBJECT_NAME_LENGTH)
						error = $"The length of name must be less or equal than {Constants.DB_MAX_OBJECT_NAME_LENGTH} characters";

					Match match = Regex.Match(name, Constants.DB_OBJECT_NAME_VALIDATION_PATTERN);
					if (!match.Success || match.Value != name.Trim())
						error = $"Name can only contains underscores and lowercase alphanumeric characters. It must begin with a letter, " +
							$"not include spaces, not end with an underscore, and not contain two consecutive underscores";

					return string.IsNullOrWhiteSpace(error);
				}*/

			RuleSet("create", () =>
			{
				RuleFor(column => column.Id)
						.Must((column, id) => { return providerManager.GetDataProviderColumn(id) == null; })
						.WithMessage("There is already existing data provider column with specified identifier.");

				//RuleFor(column => column.SourceName)
				//		.Must( (column,sourceName) => {
				//			if (string.IsNullOrEmpty(sourceName))
				//				return true;

				//			var columns = providerManager.GetDataProviderColumns(column.DataProviderId);
				//			return !columns.Any(x => x.SourceName.ToLowerInvariant()?.Trim() == sourceName.ToLowerInvariant().Trim());
				//		})
				//		.WithMessage("There is already existing data provider column with specified source name.");

				RuleFor(column => column.DbName)
						.Must((column, dbName) =>
						{
							if (string.IsNullOrEmpty(dbName))
								return true;

							var columns = providerManager.GetDataProviderColumns(column.DataProviderId);
							return !columns.Any(x => x.DbName.ToLowerInvariant().Trim() == dbName.ToLowerInvariant().Trim());
						})
						.WithMessage("There is already existing data provider column with specified database name.");
			});

			RuleSet("update", () =>
			{
				RuleFor(column => column.Id)
						.Must((column, id) => { return providerManager.GetDataProviderColumn(id) != null; })
						.WithMessage("There is not existing data provider column with specified identifier.");

				RuleFor(column => column.DataProviderId)
						.Must((column, providerId) =>
						{

							var existingColumn = providerManager.GetDataProviderColumn(column.Id);
							if (existingColumn is null)
								return true;

							return existingColumn.DataProviderId != providerId;
						})
						.WithMessage("There data provider cannot be changed for data provider column.");

				RuleFor(column => column.DbName)
						.Must((column, dbName) =>
						{

							var existingColumn = providerManager.GetDataProviderColumn(column.Id);
							if (existingColumn is null)
								return true;

							return existingColumn.DbName != dbName;
						})
						.WithMessage("There database name of column cannot be changed.");

			});


			RuleSet("delete", () =>
			{
				// Add more check when available

			});

		}

		public ValidationResult ValidateCreate(TfDataProviderColumn column)
		{
			if (column == null)
				return new ValidationResult(new[] { new ValidationFailure("", 
					"The data provider column is null.") });

			return this.Validate(column, options =>
			{
				options.IncludeRuleSets("general", "create");
			});
		}

		public ValidationResult ValidateUpdate(TfDataProviderColumn column)
		{
			if (column == null)
				return new ValidationResult(new[] { new ValidationFailure("", 
					"The data provider column is null.") });

			return this.Validate(column, options =>
			{
				options.IncludeRuleSets("general", "update");
			});
		}

		public ValidationResult ValidateDelete(TfDataProviderColumn column)
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
}
