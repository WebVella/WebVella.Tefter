namespace WebVella.Tefter.Services;

public partial interface ITfService
{
	public List<TfDataIdentity> GetDataIdentities(string? search = null);

	public TfDataIdentity GetDataIdentity(
		string dataIdentity);

	public TfDataIdentity CreateDataIdentity(
		TfDataIdentity dataIdentity);

	public TfDataIdentity UpdateDataIdentity(
		TfDataIdentity dataIdentity);

	public void DeleteDataIdentity(
		string dataIdentity);
}

public partial class TfService
{
	public List<TfDataIdentity> GetDataIdentities(string? search = null)
	{
		try
		{
			var allIdentities = _dboManager.GetList<TfDataIdentity>();
			if(String.IsNullOrWhiteSpace(search))
				return allIdentities;

			search = search.Trim().ToLowerInvariant();
			return allIdentities.Where(x=> 
				x.DataIdentity.ToLowerInvariant().Contains(search)
				).ToList();
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}

	}

	public TfDataIdentity GetDataIdentity(string dataIdentity)
	{
		try
		{
			return _dboManager.Get<TfDataIdentity>(dataIdentity, nameof(TfDataIdentity.DataIdentity));
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}

	}

	public TfDataIdentity CreateDataIdentity(
		TfDataIdentity dataIdentity)
	{
		try
		{
			new TfDataIdentityValidator(this)
				.ValidateCreate(dataIdentity)
				.ToValidationException()
				.ThrowIfContainsErrors();

			var success = _dboManager.Insert(dataIdentity);
			if (!success)
				throw new TfDboServiceException("Insert<TfDataIdentity> failed.");
	
			return GetDataIdentity(dataIdentity.DataIdentity);

		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}

	}

	public TfDataIdentity UpdateDataIdentity(
		TfDataIdentity dataIdentity)
	{
		try
		{
			new TfDataIdentityValidator(this)
				.ValidateUpdate(dataIdentity)
				.ToValidationException()
				.ThrowIfContainsErrors();

			var success = _dboManager.Update(nameof(TfDataIdentity.DataIdentity),dataIdentity);

			if (!success)
				throw new TfDboServiceException("Update<TfDataIdentity> failed.");
			return GetDataIdentity(dataIdentity.DataIdentity);
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public void DeleteDataIdentity(
		string dataIdentity )
	{
		try
		{
			using var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY);
			new TfDataIdentityValidator(this)
				.ValidateDelete(dataIdentity)
				.ToValidationException()
				.ThrowIfContainsErrors();
			bool success = _dboManager.Delete<TfDataIdentity,string>(nameof(TfDataIdentity.DataIdentity), dataIdentity);
			if (!success)
				throw new TfDboServiceException("Delete<TfBookmark> failed.");
			scope.Complete();
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}



	#region <--- validation --->

	internal class TfDataIdentityValidator : AbstractValidator<TfDataIdentity>
	{
		private readonly ITfService _tfService;
		public TfDataIdentityValidator(
			ITfService tfService)
		{
			_tfService = tfService;

			RuleSet("general", () =>
			{
				RuleFor(dataIdentity => dataIdentity.DataIdentity)
					.NotEmpty()
					.WithMessage("The data identity is required.");

				RuleFor(dataIdentity => dataIdentity.DataIdentity)
					.Must((dataIdentity, dbName) =>
					{
						if (string.IsNullOrWhiteSpace(dbName))
							return true;

						return !dbName.StartsWith("tf_");
					})
					.WithMessage("The identity name cannot start with 'tf_'.");


				RuleFor(dataIdentity => dataIdentity.DataIdentity)
					.Must((dataIdentity, dbName ) =>
					{
						if (string.IsNullOrWhiteSpace(dbName))
							return true;

						return dbName.Length >= TfConstants.DB_MIN_OBJECT_NAME_LENGTH;
					})
					.WithMessage($"The identity name must be at least " +
								$"{TfConstants.DB_MIN_OBJECT_NAME_LENGTH} characters long.");

				RuleFor(dataIdentity => dataIdentity.DataIdentity)
					.Must((dataIdentity, dbName) =>
					{
						if (string.IsNullOrWhiteSpace(dbName))
							return true;

						return dbName.Length <= TfConstants.DB_MAX_OBJECT_NAME_LENGTH;
					})
					.WithMessage($"The length of identity name must be less or equal " +
								$"than {TfConstants.DB_MAX_OBJECT_NAME_LENGTH} characters");

				RuleFor(dataIdentity => dataIdentity.DataIdentity)
					.Must((dataIdentity, dbName) =>
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
					.WithMessage($"Identity name can only contains underscores and lowercase alphanumeric characters." +
						$" It must begin with a letter, not include spaces, not end with an underscore," +
						$" and not contain two consecutive underscores");

			});

			RuleSet("create", () =>
			{
				RuleFor(dataIdentity => dataIdentity.DataIdentity)
						.Must((dataIdentity, dataIdentityKey) => { return _tfService.GetDataIdentity(dataIdentityKey) == null; })
						.WithMessage("There is already existing data identity with specified identifier.");

			});

			RuleSet("update", () =>
			{
				RuleFor(dataIdentity => dataIdentity.DataIdentity)
						.Must((dataIdentity, dataIdentityKey) =>
						{
							return _tfService.GetDataIdentity(dataIdentityKey) != null;
						})
						.WithMessage("There is not existing dataidentity with specified identifier.");

			});

			RuleSet("delete", () =>
			{
				RuleFor(dataIdentity => dataIdentity.DataIdentity)
					.Must((dataIdentity, dataIdentityKey) =>
					{
						return _tfService.GetDataProviderIdentities(dataIdentityKey).Count == 0;
					})
					.WithMessage("Data identity cannot be deleted while it is used by any data provider.");
			});

		}

		public ValidationResult ValidateCreate(
			TfDataIdentity dataIdentity)
		{
			if (dataIdentity == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The data identity is null.") });

			return this.Validate(dataIdentity, options =>
			{
				options.IncludeRuleSets("general", "create");
			});
		}

		public ValidationResult ValidateUpdate(
			TfDataIdentity dataIdentity)
		{
			if (dataIdentity == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The data identity is null.") });

			return this.Validate(dataIdentity, options =>
			{
				options.IncludeRuleSets("general", "update");
			});
		}

		public ValidationResult ValidateDelete(
			string dataIdentityKey)
		{
			var dataIdentity = _tfService.GetDataIdentity(dataIdentityKey);

			if (dataIdentity == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The data identity with specified identifier is not found.") });

			return this.Validate(dataIdentity, options =>
			{
				options.IncludeRuleSets("delete");
			});
		}
	}

	#endregion
}
