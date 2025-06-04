namespace WebVella.Tefter.Services;

public partial interface ITfService
{
	public List<TfDataIdentityConnection> GetDataIdentityConnections(
		string sourceDataIndentity = null,
		string sourceDataValue = null,
		string targetDataIdentity = null,
		string targetDataValue = null);

	public bool DataIdentityConnectionExists(
		TfDataIdentityConnection dataIdentityConnection);

	public void CreateDataIdentityConnection(
		TfDataIdentityConnection dataIdentityConnection);

	public void DeleteDataIdentityConnection(
		TfDataIdentityConnection dataIdentityConnection);
}

public partial class TfService : ITfService
{
	public List<TfDataIdentityConnection> GetDataIdentityConnections(
		string sourceDataIndentity = null,
		string sourceDataValue = null,
		string targetDataIdentity = null,
		string targetDataValue = null)
	{
		try
		{
			return _dboManager.GetList<TfDataIdentityConnection>(
				" WHERE ( @source_data_identity IS NULL OR source_data_identity = @source_data_identity) AND " +
				" ( @source_data_value IS NULL OR source_data_value = @source_data_value ) AND " +
				" ( @target_data_identity IS NULL OR target_data_identity = @target_data_identity ) AND " +
				" ( @target_data_value IS NULL OR target_data_value = @target_data_value )",
				order: null,
				new NpgsqlParameter<string>("source_data_identity", sourceDataIndentity),
				new NpgsqlParameter<string>("source_data_value", sourceDataValue),
				new NpgsqlParameter<string>("target_data_identity", targetDataIdentity),
				new NpgsqlParameter<string>("target_data_value", targetDataValue)
			);
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}

	}

	public bool DataIdentityConnectionExists(
		TfDataIdentityConnection dataIdentityConnection)
	{
		try
		{
			if (dataIdentityConnection == null)
			{
				throw new ArgumentNullException(nameof(dataIdentityConnection),
					"The data identity connection object is null.");
			}

			var resultList = GetDataIdentityConnections(
				dataIdentityConnection.SourceDataIdentity,
				dataIdentityConnection.SourceDataValue,
				dataIdentityConnection.TargetDataIdentity,
				dataIdentityConnection.TargetDataValue);

			return resultList.Count == 1;
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public void CreateDataIdentityConnection(
		TfDataIdentityConnection dataIdentityConnection)
	{
		try
		{
			new TfDataIdentityConnectionValidator(this)
				.ValidateCreate(dataIdentityConnection)
				.ToValidationException()
				.ThrowIfContainsErrors();

			var success = _dboManager.Insert<TfDataIdentityConnection>(dataIdentityConnection);
			if (!success)
				throw new TfDboServiceException("Insert<TfDataIdentityConnection> failed.");

		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}

	}

	public void DeleteDataIdentityConnection(
		TfDataIdentityConnection dataIdentityConnection)
	{
		try
		{

			new TfDataIdentityConnectionValidator(this)
				.ValidateDelete(dataIdentityConnection)
				.ToValidationException()
				.ThrowIfContainsErrors();

			var count =
				_dbService.ExecuteSqlNonQueryCommand("DELETE FROM tf_data_identity_connection " +
					"WHERE source_data_identity = @source_data_identity AND " +
					"source_data_value = @source_data_value AND " +
					"target_data_identity = @target_data_identity AND " +
					"target_data_value = @target_data_value",
					new NpgsqlParameter<string>("source_data_identity", dataIdentityConnection.SourceDataIdentity),
					new NpgsqlParameter<string>("source_data_value", dataIdentityConnection.SourceDataValue),
					new NpgsqlParameter<string>("target_data_identity", dataIdentityConnection.TargetDataIdentity),
					new NpgsqlParameter<string>("target_data_value", dataIdentityConnection.TargetDataValue));

			if (count != 1)
				throw new TfDboServiceException("Faled delete of data identity connection");
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}


	#region <--- validation --->

	internal class TfDataIdentityConnectionValidator : AbstractValidator<TfDataIdentityConnection>
	{
		private readonly ITfService _tfService;
		public TfDataIdentityConnectionValidator(
			ITfService tfService)
		{
			_tfService = tfService;

			const string sha1Regex = "^[a-fA-F0-9]{40}$";

			RuleSet("general", () =>
			{
				RuleFor(dataIdentityConnection => dataIdentityConnection.SourceDataIdentity)
					.NotEmpty()
					.WithMessage("The source data identity is required.");

				RuleFor(dataIdentityConnection => dataIdentityConnection.TargetDataIdentity)
					.NotEmpty()
					.WithMessage("The target data identity is required.");

				RuleFor(dataIdentityConnection => dataIdentityConnection.SourceDataValue)
					.NotEmpty()
					.WithMessage("The source data value is required.");

				RuleFor(dataIdentityConnection => dataIdentityConnection.TargetDataValue)
					.NotEmpty()
					.WithMessage("The target data value is required.");

				RuleFor(dataIdentityConnection => dataIdentityConnection.SourceDataIdentity)
					.Must((dataIdentityConnection, sourceDataIdentity) =>
					{
						if (string.IsNullOrWhiteSpace(sourceDataIdentity))
							return true;

						return _tfService.GetDataIdentity(sourceDataIdentity) != null;
					})
					.WithMessage("The source data identity is not found.");

				RuleFor(dataIdentityConnection => dataIdentityConnection.SourceDataValue)
					.Must((dataIdentityConnection, sourceDataValue) =>
					{
						if (string.IsNullOrWhiteSpace(sourceDataValue))
							return true;

						return Regex.IsMatch(sourceDataValue, sha1Regex);
					})
					.WithMessage("The source data value is not a valid result from SHA1 encoded text.");

				RuleFor(dataIdentityConnection => dataIdentityConnection.TargetDataIdentity)
					.Must((dataIdentityConnection, targetDataIdentity) =>
					{
						if (string.IsNullOrWhiteSpace(targetDataIdentity))
							return true;

						return _tfService.GetDataIdentity(targetDataIdentity) != null;
					})
					.WithMessage("The target data identity is not found.");

				RuleFor(dataIdentityConnection => dataIdentityConnection.TargetDataValue)
					.Must((dataIdentityConnection, targetDataValue) =>
					{
						if (string.IsNullOrWhiteSpace(targetDataValue))
							return true;

						return Regex.IsMatch(targetDataValue, sha1Regex);
					})
					.WithMessage("The target data value is not a valid result from SHA1 encoded text.");
			});

			RuleSet("create", () =>
			{
				RuleFor(dataIdentityConnection => dataIdentityConnection)
				.Must((dataIdentityConnection) =>
				{
					return !_tfService.DataIdentityConnectionExists(dataIdentityConnection);
				})
				.WithMessage("Data identity connection already exists.");

			});

			RuleSet("delete", () =>
			{
				RuleFor(dataIdentityConnection => dataIdentityConnection)
				.Must((dataIdentityConnection) =>
				{
					return _tfService.DataIdentityConnectionExists(dataIdentityConnection);
				})
				.WithMessage("Data identity connection does not exist.");

			});

		}

		public ValidationResult ValidateCreate(
			TfDataIdentityConnection dataIdentity)
		{
			if (dataIdentity == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The data identity coonnection object is null.") });

			return this.Validate(dataIdentity, options =>
			{
				options.IncludeRuleSets("general", "create");
			});
		}

		public ValidationResult ValidateDelete(
			TfDataIdentityConnection dataIdentityConnection)
		{
			if (dataIdentityConnection == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The data identity coonnection object is null.") });

			return this.Validate(dataIdentityConnection, options =>
			{
				options.IncludeRuleSets("general", "delete");
			});
		}
	}

	#endregion
}
