namespace WebVella.Tefter.Services;

public partial interface ITfService
{
	public List<TfDataIdentityConnection> GetDataIdentityConnections(
		string? dataIdentity1 = "",
		string value1 = "",
		string? dataIdentity2 = "",
		string value2 = "");

	public bool DataIdentityConnectionExists(
		TfDataIdentityConnection dataIdentityConnection);

	public void CreateDataIdentityConnection(
		TfDataIdentityConnection dataIdentityConnection);

	public void DeleteDataIdentityConnection(
		TfDataIdentityConnection dataIdentityConnection);

	public void DeleteDataIdentityConnection(
		string dataIdentity,
		string identityValue);

	public void CreateDataIdentityConnections(
		string dataIdentity,
		string identityValue,
		string relatedDataIdentity,
		List<string> relatedDataIdentityValues);

	public void CreateBatchDataIdentityConnections(
		List<TfDataIdentityConnection> dataIdentityConnections,
		int batchSize = 1000);

	public void DeleteBatchDataIdentityConnections(
		List<TfDataIdentityConnection> dataIdentityConnections,
		int batchSize = 1000);
}

public partial class TfService : ITfService
{

	public List<TfDataIdentityConnection> GetDataIdentityConnections(
		string? dataIdentity1 = "",
		string value1 = "",
		string? dataIdentity2 = "",
		string value2 = "")
	{
		try
		{

			string? whereSQL = null;
			List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();

			Action action = (dataIdentity1, dataIdentity2) switch
			{
				//////////////////////////////////////////////////////////////////////////////
				("", "") => () =>
				{
					if (string.IsNullOrWhiteSpace(value1) && string.IsNullOrWhiteSpace(value2))
					{
					}
					else if (string.IsNullOrWhiteSpace(value1))
					{
						whereSQL = " WHERE value_2 = @value_2 ";
						parameters.Add(new NpgsqlParameter<string>("value_2", value2));
					}
					else if (string.IsNullOrWhiteSpace(value2))
					{
						whereSQL = " WHERE value_1 = @value_1 ";
						parameters.Add(new NpgsqlParameter<string>("value_1", value1));
					}
					else
					{
						whereSQL = " WHERE value_1 = @value_1 AND value_2 = @value_2 ";
						parameters.Add(new NpgsqlParameter<string>("value_1", value1));
						parameters.Add(new NpgsqlParameter<string>("value_2", value2));
					}
				}
				,
				//////////////////////////////////////////////////////////////////////////////
				(null, "") => () =>
				{
					if (string.IsNullOrWhiteSpace(value1) && string.IsNullOrWhiteSpace(value2))
					{
						whereSQL = " WHERE data_identity_1 IS NULL ";
					}
					else if (string.IsNullOrWhiteSpace(value1))
					{
						whereSQL = " WHERE data_identity_1 IS NULL AND value_2 = @value_2 ";
						parameters.Add(new NpgsqlParameter<string>("value_2", value2));
					}
					else if (string.IsNullOrWhiteSpace(value2))
					{
						whereSQL = " WHERE data_identity_1 IS NULL AND value_1 = @value_1 ";
						parameters.Add(new NpgsqlParameter<string>("value_1", value2));
					}
					else
					{
						whereSQL = "data_identity_1 IS NULL AND value_1 = @value_1 AND value_2 = @value_2 ";
						parameters.Add(new NpgsqlParameter<string>("value_1", value1));
						parameters.Add(new NpgsqlParameter<string>("value_2", value2));
					}
				}
				,
				//////////////////////////////////////////////////////////////////////////////
				("", null) => () =>
				{
					if (string.IsNullOrWhiteSpace(value1) && string.IsNullOrWhiteSpace(value2))
					{
						whereSQL = " WHERE data_identity_1 IS NULL ";
					}
					else if (string.IsNullOrWhiteSpace(value1))
					{
						whereSQL = " WHERE data_identity_2 IS NULL AND value_2 = @value_2 ";
						parameters.Add(new NpgsqlParameter<string>("value_2", value2));
					}
					else if (string.IsNullOrWhiteSpace(value2))
					{
						whereSQL = " WHERE data_identity_2 IS NULL AND value_1 = @value_1 ";
						parameters.Add(new NpgsqlParameter<string>("value_1", value2));
					}
					else
					{
						whereSQL = "data_identity_2 IS NULL AND value_1 = @value_1 AND value_2 = @value_2 ";
						parameters.Add(new NpgsqlParameter<string>("value_1", value1));
						parameters.Add(new NpgsqlParameter<string>("value_2", value2));
					}
				}
				,
				//////////////////////////////////////////////////////////////////////////////
				(null, null) => () =>
				{
					if (string.IsNullOrWhiteSpace(value1) && string.IsNullOrWhiteSpace(value2))
					{
						whereSQL = " WHERE data_identity_1 IS NULL AND data_identity_2 IS NULL ";
					}
					else if (string.IsNullOrWhiteSpace(value1))
					{
						whereSQL = " WHERE data_identity_1 IS NULL AND data_identity_2 IS NULL AND value_2 = @value_2 ";
						parameters.Add(new NpgsqlParameter<string>("value_2", value2));
					}
					else if (string.IsNullOrWhiteSpace(value2))
					{
						whereSQL = " WHERE data_identity_1 IS NULL AND data_identity_2 IS NULL AND value_1 = @value_1 ";
						parameters.Add(new NpgsqlParameter<string>("value_1", value2));
					}
					else
					{
						whereSQL = "data_identity_1 IS NULL AND data_identity_2 IS NULL AND value_1 = @value_1 AND value_2 = @value_2 ";
						parameters.Add(new NpgsqlParameter<string>("value_1", value1));
						parameters.Add(new NpgsqlParameter<string>("value_2", value2));
					}
				}
				,
				//////////////////////////////////////////////////////////////////////////////
				("", _) => () =>
				{
					if (string.IsNullOrWhiteSpace(value1) && string.IsNullOrWhiteSpace(value2))
					{
						whereSQL = " WHERE data_identity_2 = @data_identity_2 ";
						parameters.Add(new NpgsqlParameter<string>("data_identity_2", dataIdentity2!));
					}
					else if (string.IsNullOrWhiteSpace(value1))
					{
						whereSQL = " WHERE data_identity_2 = @data_identity_2 AND value_2 = @value_2 ";
						parameters.Add(new NpgsqlParameter<string>("value_2", value2));
						parameters.Add(new NpgsqlParameter<string>("data_identity_2", dataIdentity2!));
					}
					else if (string.IsNullOrWhiteSpace(value2))
					{
						whereSQL = " WHERE data_identity_2 = @data_identity_2 AND value_1 = @value_1 ";
						parameters.Add(new NpgsqlParameter<string>("value_1", value1));
						parameters.Add(new NpgsqlParameter<string>("data_identity_2", dataIdentity2!));
					}
					else
					{
						whereSQL = " WHERE data_identity_2 = @data_identity_2 AND value_1 = @value_1 AND value_2 = @value_2 ";
						parameters.Add(new NpgsqlParameter<string>("value_1", value1));
						parameters.Add(new NpgsqlParameter<string>("value_2", value2));
						parameters.Add(new NpgsqlParameter<string>("data_identity_2", dataIdentity2!));
					}
				}
				,
				//////////////////////////////////////////////////////////////////////////////
				(null, _) => () =>
				{
					if (string.IsNullOrWhiteSpace(value1) && string.IsNullOrWhiteSpace(value2))
					{
						whereSQL = " WHERE data_identity_1 IS NULL AND data_identity_2 = @data_identity_2 ";
						parameters.Add(new NpgsqlParameter<string>("data_identity_2", dataIdentity2!));
					}
					else if (string.IsNullOrWhiteSpace(value1))
					{
						whereSQL = " WHERE data_identity_1 IS NULL AND data_identity_2 = @data_identity_2 AND value_2 = @value_2 ";
						parameters.Add(new NpgsqlParameter<string>("value_2", value2));
						parameters.Add(new NpgsqlParameter<string>("data_identity_2", dataIdentity2!));
					}
					else if (string.IsNullOrWhiteSpace(value2))
					{
						whereSQL = " WHERE data_identity_1 IS NULL AND data_identity_2 = @data_identity_2 AND value_1 = @value_1 ";
						parameters.Add(new NpgsqlParameter<string>("value_1", value1));
						parameters.Add(new NpgsqlParameter<string>("data_identity_2", dataIdentity2!));
					}
					else
					{
						whereSQL = " WHERE  data_identity_1 IS NULL AND data_identity_2 = @data_identity_2 AND value_1 = @value_1 AND value_2 = @value_2 ";
						parameters.Add(new NpgsqlParameter<string>("value_1", value1));
						parameters.Add(new NpgsqlParameter<string>("value_2", value2));
						parameters.Add(new NpgsqlParameter<string>("data_identity_2", dataIdentity2!));
					}
				}
				,
				//////////////////////////////////////////////////////////////////////////////
				(_, "") => () =>
				{
					if (string.IsNullOrWhiteSpace(value1) && string.IsNullOrWhiteSpace(value2))
					{
						whereSQL = " WHERE data_identity_1 = @data_identity_1 ";
						parameters.Add(new NpgsqlParameter<string>("data_identity_1", dataIdentity1!));
					}
					else if (string.IsNullOrWhiteSpace(value1))
					{
						whereSQL = " WHERE data_identity_1 = @data_identity_1 AND value_2 = @value_2 ";
						parameters.Add(new NpgsqlParameter<string>("value_2", value2));
						parameters.Add(new NpgsqlParameter<string>("data_identity_1", dataIdentity1!));
					}
					else if (string.IsNullOrWhiteSpace(value2))
					{
						whereSQL = " WHERE data_identity_1 = @data_identity_1 AND value_1 = @value_1 ";
						parameters.Add(new NpgsqlParameter<string>("value_1", value1));
						parameters.Add(new NpgsqlParameter<string>("data_identity_1", dataIdentity1!));
					}
					else
					{
						whereSQL = " WHERE data_identity_1 = @data_identity_1 AND value_1 = @value_1 AND value_2 = @value_2 ";
						parameters.Add(new NpgsqlParameter<string>("value_1", value1));
						parameters.Add(new NpgsqlParameter<string>("value_2", value2));
						parameters.Add(new NpgsqlParameter<string>("data_identity_1", dataIdentity1!));
					}
				}
				,
				//////////////////////////////////////////////////////////////////////////////
				(_, null) => () =>
				{
					if (string.IsNullOrWhiteSpace(value1) && string.IsNullOrWhiteSpace(value2))
					{
						whereSQL = " WHERE data_identity_2 IS NULL AND data_identity_1 = @data_identity_1 ";
						parameters.Add(new NpgsqlParameter<string>("data_identity_1", dataIdentity1!));
					}
					else if (string.IsNullOrWhiteSpace(value1))
					{
						whereSQL = " WHERE data_identity_2 IS NULL AND data_identity_1 = @data_identity_1 AND value_2 = @value_2 ";
						parameters.Add(new NpgsqlParameter<string>("value_2", value2));
						parameters.Add(new NpgsqlParameter<string>("data_identity_1", dataIdentity1!));
					}
					else if (string.IsNullOrWhiteSpace(value2))
					{
						whereSQL = " WHERE data_identity_2 IS NULL AND data_identity_1 = @data_identity_1 AND value_1 = @value_1 ";
						parameters.Add(new NpgsqlParameter<string>("value_1", value1));
						parameters.Add(new NpgsqlParameter<string>("data_identity_1", dataIdentity1!));
					}
					else
					{
						whereSQL = " WHERE  data_identity_2 IS NULL AND data_identity_1 = @data_identity_1 AND value_1 = @value_1 AND value_2 = @value_2 ";
						parameters.Add(new NpgsqlParameter<string>("value_1", value1));
						parameters.Add(new NpgsqlParameter<string>("value_2", value2));
						parameters.Add(new NpgsqlParameter<string>("data_identity_1", dataIdentity1!));
					}
				}
				,
				//////////////////////////////////////////////////////////////////////////////
				(_, _) => () =>
				{
					if (string.IsNullOrWhiteSpace(value1) && string.IsNullOrWhiteSpace(value2))
					{
						whereSQL = " WHERE data_identity_1 = @data_identity_1 AND data_identity_2 = @data_identity_2 ";
						parameters.Add(new NpgsqlParameter<string>("data_identity_1", dataIdentity1!));
						parameters.Add(new NpgsqlParameter<string>("data_identity_2", dataIdentity2!));
					}
					else if (string.IsNullOrWhiteSpace(value1))
					{
						whereSQL = " WHERE data_identity_1 = @data_identity_1 AND data_identity_2 = @data_identity_2 " +
						" AND value_2 = @value_2 ";
						parameters.Add(new NpgsqlParameter<string>("data_identity_1", dataIdentity1!));
						parameters.Add(new NpgsqlParameter<string>("data_identity_2", dataIdentity2!));
						parameters.Add(new NpgsqlParameter<string>("value_2", value2));
					}
					else if (string.IsNullOrWhiteSpace(value2))
					{
						whereSQL = " WHERE data_identity_1 = @data_identity_1 AND data_identity_2 = @data_identity_2 " +
						" AND value_1 = @value_1 ";
						parameters.Add(new NpgsqlParameter<string>("data_identity_1", dataIdentity1!));
						parameters.Add(new NpgsqlParameter<string>("data_identity_2", dataIdentity2!));
						parameters.Add(new NpgsqlParameter<string>("value_1", value1));
					}
					else
					{
						whereSQL = " WHERE data_identity_1 = @data_identity_1 AND data_identity_2 = @data_identity_2 " +
						" AND value_1 = @value_1  AND value_2 = @value_2 ";
						parameters.Add(new NpgsqlParameter<string>("data_identity_1", dataIdentity1!));
						parameters.Add(new NpgsqlParameter<string>("data_identity_2", dataIdentity2!));
						parameters.Add(new NpgsqlParameter<string>("value_1", value1));
						parameters.Add(new NpgsqlParameter<string>("value_2", value2));
					}
				}
			};

			action();

			if (string.IsNullOrEmpty(whereSQL))
				return _dboManager.GetList<TfDataIdentityConnection>();

			return _dboManager.GetList<TfDataIdentityConnection>(
				whereSql: whereSQL,
				order: null,
				parameters: parameters.ToArray());
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

			var orderedDIC = OrderDataIndentityConnection(dataIdentityConnection);

			var resultList = GetDataIdentityConnections(
				orderedDIC.DataIdentity1,
				orderedDIC.Value1,
				orderedDIC.DataIdentity2,
				orderedDIC.Value2);

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

			var dicToBeInserted = OrderDataIndentityConnection(dataIdentityConnection);

			var success = _dboManager.Insert<TfDataIdentityConnection>(dicToBeInserted);
			if (!success)
				throw new TfDboServiceException("Insert<TfDataIdentityConnection> failed.");

		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}

	}

	private TfDataIdentityConnection OrderDataIndentityConnection(
		TfDataIdentityConnection dataIdentityConnection)
	{
		TfDataIdentityConnection orderedDataIdentityConnection = new TfDataIdentityConnection();

		// Ensure that DataIdentity1 is always less than DataIdentity2
		if (dataIdentityConnection.DataIdentity1.IsLessThan(dataIdentityConnection.DataIdentity2))
		{
			orderedDataIdentityConnection.DataIdentity1 = dataIdentityConnection.DataIdentity1;
			orderedDataIdentityConnection.Value1 = dataIdentityConnection.Value1;
			orderedDataIdentityConnection.DataIdentity2 = dataIdentityConnection.DataIdentity2;
			orderedDataIdentityConnection.Value2 = dataIdentityConnection.Value2;
		}
		else if (dataIdentityConnection.DataIdentity1.IsGreaterThan(dataIdentityConnection.DataIdentity2))
		{
			orderedDataIdentityConnection.DataIdentity1 = dataIdentityConnection.DataIdentity2;
			orderedDataIdentityConnection.Value1 = dataIdentityConnection.Value2;
			orderedDataIdentityConnection.DataIdentity2 = dataIdentityConnection.DataIdentity1;
			orderedDataIdentityConnection.Value2 = dataIdentityConnection.Value1;
		}
		else //when identity is same, compare values and ensure that Value1 is always less than Value2
		{
			orderedDataIdentityConnection.DataIdentity1 = dataIdentityConnection.DataIdentity1;
			orderedDataIdentityConnection.DataIdentity2 = dataIdentityConnection.DataIdentity2;

			if (dataIdentityConnection.Value1.IsLessThan(dataIdentityConnection.Value2))
			{
				orderedDataIdentityConnection.Value1 = dataIdentityConnection.Value1;
				orderedDataIdentityConnection.Value2 = dataIdentityConnection.Value2;
			}
			else
			{
				orderedDataIdentityConnection.Value1 = dataIdentityConnection.Value2;
				orderedDataIdentityConnection.Value2 = dataIdentityConnection.Value1;
			}
		}

		return orderedDataIdentityConnection;
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
					" ( ( ( @data_identity_1 IS NULL AND data_identity_1 IS NULL ) OR data_identity_1 = @data_identity_1) AND " +
					" ( @value_1 IS NULL OR value_1 = @value_1 ) AND " +
					" ( ( data_identity_2 IS NULL AND @data_identity_2 IS NULL ) OR data_identity_2 = @data_identity_2 ) AND " +
					" ( @value_2 IS NULL OR value_2 = @value_2 ) ) " +
					" OR " +
					" ( ( ( data_identity_1 IS NULL AND @data_identity_2 IS NULL ) OR data_identity_1 = @data_identity_2) AND " +
					" ( @value_2 IS NULL OR value_1 = @value_2 ) AND " +
					" ( ( data_identity_2 IS NULL AND @data_identity_1 IS NULL ) OR data_identity_2 = @data_identity_1 ) AND " +
					" ( @value_1 IS NULL OR value_2 = @value_1 ) )",
					new NpgsqlParameter<string?>("data_identity_1", dataIdentityConnection.DataIdentity1),
					new NpgsqlParameter<string>("value_1", dataIdentityConnection.Value1),
					new NpgsqlParameter<string?>("data_identity_2", dataIdentityConnection.DataIdentity2),
					new NpgsqlParameter<string>("value_2", dataIdentityConnection.Value2));

			if (count != 1)
				throw new TfDboServiceException("Faled delete of data identity connection");
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public void DeleteDataIdentityConnection(
		string dataIdentity,
		string identityValue)
	{
		try
		{
			new TfDataIdentityConnectionValidator(this)
				.ValidateDelete(dataIdentity, identityValue)
				.ToValidationException()
				.ThrowIfContainsErrors();

			_dbService.ExecuteSqlNonQueryCommand("DELETE FROM tf_data_identity_connection " +
					" WHERE" +
					" ( ( ( data_identity_1 IS NULL AND @data_identity IS NULL ) OR data_identity_1 = @data_identity ) AND value_1 = @value ) " +
					" OR " +
					" ( ( ( data_identity_2 IS NULL AND @data_identity IS NULL ) OR  data_identity_2 = @data_identity )  AND value_2 = @value )",
					new NpgsqlParameter<string>("data_identity", dataIdentity),
					new NpgsqlParameter<string>("value", identityValue));

		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public void CreateDataIdentityConnections(
		string dataIdentity,
		string identityValue,
		string relatedDataIdentity,
		List<string> relatedDataIdentityValues)
	{
		//TODO validation

		if(relatedDataIdentityValues == null || relatedDataIdentityValues.Count == 0)
			return;

		List<TfDataIdentityConnection> dataIdentityConnections = new();
		foreach (var relatedDataIdentityValue in relatedDataIdentityValues)
		{
			var dic = new TfDataIdentityConnection()
			{
				DataIdentity1 = dataIdentity,
				Value1 = identityValue,
				DataIdentity2 = relatedDataIdentity,
				Value2 = relatedDataIdentityValue
			};
			dataIdentityConnections.Add(dic);
		}
		CreateBatchDataIdentityConnections(dataIdentityConnections);
	}

	public void CreateBatchDataIdentityConnections(
		List<TfDataIdentityConnection> dataIdentityConnections,
		int batchSize = 1000)
	{
		if (dataIdentityConnections == null || dataIdentityConnections.Count == 0)
			return;

		try
		{
			using (var scope = _dbService.CreateTransactionScope())
			{
				foreach (var batch in dataIdentityConnections.Batch(batchSize))
				{
					int paramCounter = 1;
					StringBuilder sqlSb = new StringBuilder();
					List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();

					foreach (var dataIdentityConnection in batch)
					{
						if (dataIdentityConnection == null)
							continue;

						if (string.IsNullOrWhiteSpace(dataIdentityConnection.Value1) ||
							string.IsNullOrWhiteSpace(dataIdentityConnection.Value2))
						{
							continue;
						}

						var dicToBeInserted = OrderDataIndentityConnection(dataIdentityConnection);

						sqlSb.AppendLine($@"INSERT INTO tf_data_identity_connection (data_identity_1,value_1,data_identity_2,value_2)
                            VALUES (@data_identity_1_{paramCounter}, @value_1_{paramCounter}, @data_identity_2_{paramCounter}, @value_2_{paramCounter})
                            ON CONFLICT DO NOTHING;");

						parameters.Add(new NpgsqlParameter<string?>($"@data_identity_1_{paramCounter}", dicToBeInserted.DataIdentity1));
						parameters.Add(new NpgsqlParameter<string>($"@value_1_{paramCounter}", dicToBeInserted.Value1));
						parameters.Add(new NpgsqlParameter<string?>($"@data_identity_2_{paramCounter}", dicToBeInserted.DataIdentity2));
						parameters.Add(new NpgsqlParameter<string>($"@value_2_{paramCounter}", dicToBeInserted.Value2));

						paramCounter++;
					}

					_dbService.ExecuteSqlNonQueryCommand(sqlSb.ToString(), parameters);
				}

				scope.Complete();
			}

		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}

	}

	public void DeleteBatchDataIdentityConnections(
		List<TfDataIdentityConnection> dataIdentityConnections,
		int batchSize = 1000)
	{
		if (dataIdentityConnections == null || dataIdentityConnections.Count == 0)
			return;

		try
		{
			using (var scope = _dbService.CreateTransactionScope())
			{
				foreach (var batch in dataIdentityConnections.Batch(batchSize))
				{
					int paramCounter = 1;
					StringBuilder sqlSb = new StringBuilder();
					List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();

					foreach (var dataIdentityConnection in batch)
					{

						if (dataIdentityConnection == null)
							continue;

						if (string.IsNullOrWhiteSpace(dataIdentityConnection.DataIdentity1)
						|| string.IsNullOrWhiteSpace(dataIdentityConnection.Value1)
						|| string.IsNullOrWhiteSpace(dataIdentityConnection.DataIdentity2)
						|| string.IsNullOrWhiteSpace(dataIdentityConnection.Value2))
							continue;

						var dicToDeleted = OrderDataIndentityConnection(dataIdentityConnection);

						sqlSb.AppendLine($@"DELETE FROM tf_data_identity_connection 
							WHERE data_identity_1 = @data_identity_1_{paramCounter} AND value_1 = @value_1_{paramCounter}
								AND data_identity_2 = @data_identity_2_{paramCounter} AND value_2 = @value_2_{paramCounter};");

						parameters.Add(new NpgsqlParameter($"@data_identity_1_{paramCounter}", dicToDeleted.DataIdentity1));
						parameters.Add(new NpgsqlParameter($"@value_1_{paramCounter}", dicToDeleted.Value1));
						parameters.Add(new NpgsqlParameter($"@data_identity_2_{paramCounter}", dicToDeleted.DataIdentity2));
						parameters.Add(new NpgsqlParameter($"@value_2_{paramCounter}", dicToDeleted.Value2));

						paramCounter++;
					}

					_dbService.ExecuteSqlNonQueryCommand(sqlSb.ToString(), parameters);
				}

				scope.Complete();
			}
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

		public ValidationResult ValidateDelete(
			string dataIdentity,
			string identityValue)
		{
			if (string.IsNullOrWhiteSpace(identityValue))
				return new ValidationResult(new[] { new ValidationFailure("",
					"The identity value is not specified.") });

			return new ValidationResult();
		}
	}

	#endregion
}
