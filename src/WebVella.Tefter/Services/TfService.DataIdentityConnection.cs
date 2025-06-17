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
		string dataIndentity1 = null,
		string value1 = null,
		string dataIdentity2 = null,
		string value2 = null)
	{
		try
		{
			return _dboManager.GetList<TfDataIdentityConnection>(
				" WHERE" +
				" ( ( @data_identity_1 IS NULL OR data_identity_1 = @data_identity_1) AND " +
				" ( @value_1 IS NULL OR value_1 = @value_1 ) AND " +
				" ( @data_identity_2 IS NULL OR data_identity_2 = @data_identity_2 ) AND " +
				" ( @value_2 IS NULL OR value_2 = @value_2 ) ) " +
				" OR " +
				" ( ( @data_identity_2 IS NULL OR data_identity_1 = @data_identity_2) AND " +
				" ( @value_2 IS NULL OR value_1 = @value_2 ) AND " +
				" ( @data_identity_1 IS NULL OR data_identity_2 = @data_identity_1 ) AND " +
				" ( @value_1 IS NULL OR value_2 = @value_1 ) )",
				order: null,
				new NpgsqlParameter<string>("data_identity_1", dataIndentity1),
				new NpgsqlParameter<string>("value_1", value1),
				new NpgsqlParameter<string>("data_identity_2", dataIdentity2),
				new NpgsqlParameter<string>("value_2", value2)
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
				dataIdentityConnection.DataIdentity1,
				dataIdentityConnection.Value1,
				dataIdentityConnection.DataIdentity2,
				dataIdentityConnection.Value2);

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

			TfDataIdentityConnection dicToBeInserted = new TfDataIdentityConnection();

			// Ensure that DataIdentity1 is always less than DataIdentity2
			if (dataIdentityConnection.DataIdentity1.IsLessThan(dataIdentityConnection.DataIdentity2))
			{
				dicToBeInserted.DataIdentity1 = dataIdentityConnection.DataIdentity1;
				dicToBeInserted.Value1 = dataIdentityConnection.Value1;
				dicToBeInserted.DataIdentity2 = dataIdentityConnection.DataIdentity2;
				dicToBeInserted.Value2 = dataIdentityConnection.Value2;
			}
			else if (dataIdentityConnection.DataIdentity1.IsGreaterThan(dataIdentityConnection.DataIdentity2))
			{
				dicToBeInserted.DataIdentity1 = dataIdentityConnection.DataIdentity2;
				dicToBeInserted.Value1 = dataIdentityConnection.Value2;
				dicToBeInserted.DataIdentity2 = dataIdentityConnection.DataIdentity1;
				dicToBeInserted.Value2 = dataIdentityConnection.Value1;
			}
			else //when identity is same, compare values and ensure that Value1 is always less than Value2
			{
				dicToBeInserted.DataIdentity1 = dataIdentityConnection.DataIdentity1;
				dicToBeInserted.DataIdentity2 = dataIdentityConnection.DataIdentity2;

				if (dataIdentityConnection.Value1.IsLessThan(dataIdentityConnection.Value2))
				{
					dicToBeInserted.Value1 = dataIdentityConnection.Value1;
					dicToBeInserted.Value2 = dataIdentityConnection.Value2;
				}
				else
				{
					dicToBeInserted.Value1 = dataIdentityConnection.Value2;
					dicToBeInserted.Value2 = dataIdentityConnection.Value1;
				}
			}

			var success = _dboManager.Insert<TfDataIdentityConnection>(dicToBeInserted);
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
					" WHERE" +
					" ( ( @data_identity_1 IS NULL OR data_identity_1 = @data_identity_1) AND " +
					" ( @value_1 IS NULL OR value_1 = @value_1 ) AND " +
					" ( @data_identity_2 IS NULL OR data_identity_2 = @data_identity_2 ) AND " +
					" ( @value_2 IS NULL OR value_2 = @value_2 ) ) " +
					" OR " +
					" ( ( @data_identity_2 IS NULL OR data_identity_1 = @data_identity_2) AND " +
					" ( @value_2 IS NULL OR value_1 = @value_2 ) AND " +
					" ( @data_identity_1 IS NULL OR data_identity_2 = @data_identity_1 ) AND " +
					" ( @value_1 IS NULL OR value_2 = @value_1 ) )",
					new NpgsqlParameter<string>("data_identity_1", dataIdentityConnection.DataIdentity1),
					new NpgsqlParameter<string>("value_1", dataIdentityConnection.Value1),
					new NpgsqlParameter<string>("data_identity_2", dataIdentityConnection.DataIdentity2),
					new NpgsqlParameter<string>("value_2", dataIdentityConnection.Value2));

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
				RuleFor(dataIdentityConnection => dataIdentityConnection.DataIdentity1)
					.NotEmpty()
					.WithMessage("The source data identity is required.");

				RuleFor(dataIdentityConnection => dataIdentityConnection.DataIdentity2)
					.NotEmpty()
					.WithMessage("The target data identity is required.");

				RuleFor(dataIdentityConnection => dataIdentityConnection.Value1)
					.NotEmpty()
					.WithMessage("The source data value is required.");

				RuleFor(dataIdentityConnection => dataIdentityConnection.Value2)
					.NotEmpty()
					.WithMessage("The target data value is required.");

				RuleFor(dataIdentityConnection => dataIdentityConnection.DataIdentity1)
					.Must((dataIdentityConnection, sourceDataIdentity) =>
					{
						if (string.IsNullOrWhiteSpace(sourceDataIdentity))
							return true;

						return _tfService.GetDataIdentity(sourceDataIdentity) != null;
					})
					.WithMessage("The source data identity is not found.");

				RuleFor(dataIdentityConnection => dataIdentityConnection.Value1)
					.Must((dataIdentityConnection, sourceDataValue) =>
					{
						if (string.IsNullOrWhiteSpace(sourceDataValue))
							return true;

						return Regex.IsMatch(sourceDataValue, sha1Regex);
					})
					.WithMessage("The source data value is not a valid result from SHA1 encoded text.");

				RuleFor(dataIdentityConnection => dataIdentityConnection.DataIdentity2)
					.Must((dataIdentityConnection, targetDataIdentity) =>
					{
						if (string.IsNullOrWhiteSpace(targetDataIdentity))
							return true;

						return _tfService.GetDataIdentity(targetDataIdentity) != null;
					})
					.WithMessage("The target data identity is not found.");

				RuleFor(dataIdentityConnection => dataIdentityConnection.Value2)
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
