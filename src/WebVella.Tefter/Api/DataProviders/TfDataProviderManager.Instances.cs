﻿using FluentValidation;

namespace WebVella.Tefter;

public partial interface ITfDataProviderManager
{
	public Result<TfDataProvider> GetProvider(Guid id);

	public Result<TfDataProvider> GetProvider(string name);

	public Result<ReadOnlyCollection<TfDataProvider>> GetProviders();

	public Result<TfDataProvider> CreateDataProvider(TfDataProviderModel providerModel);

	public Result<TfDataProvider> UpdateDataProvider(TfDataProviderModel providerModel);
	
	public Result DeleteDataProvider(Guid id);
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
			if (providerType == null)
				return Result.Fail(new Error("Unable to find provider type for specified provider instance."));

			var provider = FromDbo(providerDbo, GetDataProviderColumns(id), providerType);

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

			var provider = FromDbo(providerDbo, GetDataProviderColumns(providerDbo.Id), providerType);

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

				var provider = FromDbo(dbo, GetDataProviderColumns(dbo.Id), providerType);
				providers.Add(provider);
			}

			return Result.Ok(providers.AsReadOnly());
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to get data providers").CausedBy(ex));
		}
	}

	public Result<TfDataProvider> CreateDataProvider(TfDataProviderModel providerModel)
	{
		try
		{
			if (providerModel.Id == Guid.Empty)
				providerModel.Id = Guid.NewGuid();

			TfDataProviderCreateValidator validator =
				new TfDataProviderCreateValidator(_dboManager, this);

			var validationResult = validator.Validate(providerModel);
			if (!validationResult.IsValid)
				return validationResult.ToResult();

			using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{

				TfDataProviderDbo dataProviderDbo = ToDbo(providerModel);

				var success = _dboManager.Insert<TfDataProviderDbo>(dataProviderDbo);

				if (!success)
					return Result.Fail(new DboManagerError("Insert", dataProviderDbo));

				var providerResult = GetProvider(providerModel.Id);
				if (providerResult.IsFailed)
					return Result.Fail(new Error("Failed to create new data provider")
						.CausedBy(providerResult.Errors));

				var provider = providerResult.Value;
				string providerTableName = $"dp{provider.Index}";

				DatabaseBuilder dbBuilder = _dbManager.GetDatabaseBuilder();
				dbBuilder.NewTableBuilder(Guid.NewGuid(), providerTableName)
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
					.WithIndexes(indexes =>
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

			TfDataProviderDbo dataProviderDbo = ToDbo(providerModel);
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

	public Result DeleteDataProvider(Guid id)
	{
		try
		{
			using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var providerResult = GetProvider(id);
				if (providerResult.IsFailed)
					return Result.Fail(new Error("Failed to delete provider.")
						.CausedBy(providerResult.Errors));

				TfDataProviderDeleteValidator validator =
					new TfDataProviderDeleteValidator(_dboManager, this);

				var validationResult = validator.ValidateDelete(providerResult.Value);
				if (!validationResult.IsValid)
					return validationResult.ToResult();

				var success = _dboManager.Delete<TfDataProviderDbo>(id);

				if (!success)
					return Result.Fail(new DboManagerError("Delete", id));

				var provider = providerResult.Value;
				string providerTableName = $"dp{provider.Index}";

				DatabaseBuilder dbBuilder = _dbManager.GetDatabaseBuilder();
				
				dbBuilder.Remove(providerTableName);

				_dbManager.SaveChanges(dbBuilder);

				scope.Complete();

				return Result.Ok();
			}
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to create new data provider.").CausedBy(ex));
		}
	}

	private static TfDataProviderDbo ToDbo(TfDataProviderModel providerModel)
	{
		if (providerModel == null)
			throw new ArgumentException(nameof(providerModel));

		return new TfDataProviderDbo
		{
			Id = providerModel.Id,
			CompositeKeyPrefix = providerModel.CompositeKeyPrefix,
			Name = providerModel.Name,
			SettingsJson = providerModel.SettingsJson,
			TypeId = providerModel.ProviderType.Id,
			TypeName = providerModel.ProviderType.GetType().FullName,
		};
	}

	private static TfDataProvider FromDbo(TfDataProviderDbo dbo,
		List<TfDataProviderColumn> columns, ITfDataProviderType providerType)
	{
		if (dbo == null)
			throw new ArgumentException(nameof(dbo));

		if (columns == null)
			throw new ArgumentException(nameof(columns));

		if (providerType == null)
			throw new ArgumentException(nameof(providerType));

		return new TfDataProvider
		{
			Id = dbo.Id,
			CompositeKeyPrefix = dbo.CompositeKeyPrefix,
			Name = dbo.Name,
			Index = dbo.Index,
			SettingsJson = dbo.SettingsJson,
			ProviderType = providerType,
			Columns = columns.AsReadOnly()
		};
	}


	#region <--- Validators --->

	internal class TfDataProviderCreateValidator
	: AbstractValidator<TfDataProviderModel>
	{
		public TfDataProviderCreateValidator(
			IDboManager dboManager,
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
					.Must(id => { return providerManager.GetProvider(id).Value == null; })
					.WithMessage("There is already existing data provider with specified identifier.");

			RuleFor(provider => provider.Name)
					.Must(name => { return providerManager.GetProvider(name).Value == null; })
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
			IDboManager dboManager,
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
					.Must((provider, id) => { return providerManager.GetProvider(id).Value != null; })
					.WithMessage("There is no existing data provider for specified identifier.");

			RuleFor(provider => provider.ProviderType)
					.Must((provider, providerType) =>
					{
						if (provider.ProviderType == null)
							return true;

						var existingObject = providerManager.GetProvider(provider.Id).Value;

						if (existingObject != null && existingObject.ProviderType.Id != provider.ProviderType.Id)
							return false;

						return true;
					})
					.WithMessage("The data provider type cannot be updated.");

			RuleFor(provider => provider.Name)
				.Must((provider, name) =>
				{
					var existingObj = providerManager.GetProvider(provider.Name).Value;
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
			IDboManager dboManager,
			ITfDataProviderManager providerManager)
		{

			/*	.AddGuidColumn("tf_id", c => { c.WithoutAutoDefaultValue().NotNullable(); })
							.AddDateTimeColumn("tf_created_on", c => { c.WithoutAutoDefaultValue().NotNullable(); })
							.AddDateTimeColumn("tf_updated_on", c => { c.WithoutAutoDefaultValue().NotNullable(); })
							.AddTextColumn("tf_search", c => { c.NotNullable().WithDefaultValue(string.Empty); });*/

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

			return this.Validate(provider);
		}
	}
	#endregion
}
