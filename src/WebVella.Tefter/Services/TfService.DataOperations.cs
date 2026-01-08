using NpgsqlTypes;

namespace WebVella.Tefter.Services;

public partial interface ITfService
{
	TfDataProviderDataRow InsertProviderRow(
		Guid providerId,
		Dictionary<string, object>? rowDict = null);

	TfDataRow InsertDatasetRow(
		Guid datasetId,
		Dictionary<string, object>? rowDict = null);

	TfDataProviderDataRow UpdateProviderRow(
		Guid tfId,
		Guid providerId,
		Dictionary<string, object> rowDict);

	TfDataRow UpdateDatasetRow(
		Guid tfId,
		Guid datasetId,
		Dictionary<string, object> rowDict);
}

public partial class TfService : ITfService
{
	public TfDataProviderDataRow InsertProviderRow(
		Guid providerId,
		Dictionary<string, object>? rowDict = null)
	{
		try
		{
			var tfId = InsertRow(providerId, rowDict);
			return GetProviderRow(GetDataProvider(providerId), tfId);
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public TfDataRow InsertDatasetRow(
		Guid datasetId,
		Dictionary<string, object>? rowDict = null)
	{
		var errors = new Dictionary<string, string>();

		var dataset = GetDataset(datasetId);

		if (dataset is null)
			errors.Add("datasetId", "Dataset with given id not found");

		CheckProcessValidationErrors(errors);

		var tfId = InsertRow(dataset!.DataProviderId, rowDict);

		return QueryDataset(datasetId, new List<Guid> { tfId }).Rows[0];
	}

	public TfDataProviderDataRow UpdateProviderRow(
		Guid tfId,
		Guid providerId,
		Dictionary<string, object> rowDict)
	{
		UpdateRow(tfId, providerId, rowDict);
		return GetProviderRow(GetDataProvider(providerId), tfId);
	}

	public TfDataRow UpdateDatasetRow(
		Guid tfId,
		Guid datasetId,
		Dictionary<string, object> rowDict)
	{
		var errors = new Dictionary<string, string>();

		var dataset = GetDataset(datasetId);

		if (dataset is null)
			errors.Add("datasetId", "Dataset with given id not found");

		CheckProcessValidationErrors(errors);

		UpdateRow(tfId, dataset!.DataProviderId, rowDict);

		return QueryDataset(datasetId, new List<Guid> { tfId }).Rows[0];
	}

	#region <--- private methods --->

	private Guid InsertRow(
		Guid providerId,
		Dictionary<string, object>? rowDict = null)
	{
		using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
		{
			var errors = new Dictionary<string, string>();

			var provider = GetDataProvider(providerId);

			if (provider is null)
				errors.Add("providerId", "Data provider with given id not found");

			CheckProcessValidationErrors(errors);

			var processedRowDict = new Dictionary<string, object?>();
			foreach (var column in provider!.Columns)
			{
				if (!string.IsNullOrWhiteSpace(column.Expression))
					continue;

				var value = GetColumnValue(provider, column, null, rowDict, errors);
				processedRowDict[column.DbName!] = value;
			}

			CheckProcessValidationErrors(errors);

			processedRowDict["tf_id"] = Guid.NewGuid();
			processedRowDict["tf_created_on"] = DateTime.Now;
			processedRowDict["tf_updated_on"] = DateTime.Now;
			processedRowDict["tf_row_index"] = GetProviderNextRowIndex(provider);
			processedRowDict["tf_search"] = GetTableSearchValue(provider, processedRowDict!);

			var columnNames = processedRowDict.Keys.ToList();

			StringBuilder sqlSb = new StringBuilder();
			sqlSb.Append($"INSERT INTO dp{provider.Index} ( ");
			sqlSb.Append(string.Join(", ", columnNames.Select(x => $"\"{x}\"").ToArray()));
			sqlSb.Append(" ) VALUES ( ");
			sqlSb.Append(string.Join(", ", columnNames.Select(x => $"@{x}").ToArray()));
			sqlSb.Append(" ) ");

			var parameters = new List<NpgsqlParameter>();
			foreach (var columnName in columnNames)
				parameters.Add(new NpgsqlParameter($"@{columnName}", processedRowDict[columnName] ?? DBNull.Value));

			var count = _dbService.ExecuteSqlNonQueryCommand(sqlSb.ToString(), parameters);

			if (count != 1)
				errors.Add("", "Database operation of insert new row failed.");

			CheckProcessValidationErrors(errors);

			if (rowDict is not null)
			{
				var inputColumnNames = rowDict.Keys.ToHashSet();
				var sharedColumns = provider.SharedColumns
					.Where(x => inputColumnNames.Contains($"{x.DataIdentity}.{x.DbName}"));

				if (sharedColumns.Any())
				{
					var insertedRow = GetProviderRow(GetDataProvider(providerId), (Guid)processedRowDict["tf_id"]!);

					foreach (var sharedColumn in sharedColumns)
					{
						var dataIdentity = provider.Identities.Single(x => x.DataIdentity == sharedColumn.DataIdentity);

						var sharedColumnFullName = $"{sharedColumn.DataIdentity}.{sharedColumn.DbName}";

						object? value = ValidateProcessSharedColumnValue(
							provider, sharedColumn, rowDict[sharedColumnFullName], errors);

						CheckProcessValidationErrors(errors);

						string identityValue = (string)insertedRow[$"tf_ide_{dataIdentity.DataIdentity}"]!;

						SaveSharedColumnValue(value, sharedColumnFullName, sharedColumn.Id,
							identityValue, sharedColumn.DbType);
					}
				}
			}

			scope.Complete();

			return (Guid)processedRowDict["tf_id"]!;
		}
	}

	private void UpdateRow(
		Guid tfId,
		Guid providerId,
		Dictionary<string, object> rowDict)
	{
		using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
		{
			var errors = new Dictionary<string, string>();

			var provider = GetDataProvider(providerId);

			if (provider is null)
				errors.Add("providerId", "Data provider with given id not found");

			CheckProcessValidationErrors(errors);

			if (rowDict is null || !rowDict.Any())
				errors.Add("rowDict", "No data provided for update");

			CheckProcessValidationErrors(errors);

			var existingRow = GetProviderRow(provider!, tfId);
			if (existingRow is null)
				errors.Add("tfId", "Data row with given id not found");

			CheckProcessValidationErrors(errors);

			var processedRowDict = new Dictionary<string, object?>();
			foreach (var columnName in rowDict!.Keys)
			{
				var column = provider!.Columns.SingleOrDefault(x => x.DbName == columnName);
				if (column is null)
					continue;

				if (!string.IsNullOrWhiteSpace(column.Expression))
					continue;

				var value = GetColumnValue(provider, column!, null, rowDict, errors);
				processedRowDict[column!.DbName!] = value;
			}

			CheckProcessValidationErrors(errors);

			var searchBuildRowDict = new Dictionary<string, object?>();
			foreach (var column in provider!.Columns)
			{
				if (processedRowDict.ContainsKey(column.DbName!))
					searchBuildRowDict[column.DbName!] = processedRowDict[column.DbName!];
				else
					searchBuildRowDict[column.DbName!] = existingRow![column.DbName!];
			}

			processedRowDict["tf_search"] = GetTableSearchValue(provider, searchBuildRowDict!);
			processedRowDict["tf_updated_on"] = DateTime.Now;

			var columnNames = processedRowDict.Keys.ToList();

			StringBuilder sqlSb = new StringBuilder();
			sqlSb.Append($"UPDATE dp{provider.Index} {Environment.NewLine} SET {Environment.NewLine}");
			sqlSb.Append(string.Join($",{Environment.NewLine}",
				columnNames.Select(x => $"\"{x}\" = @{x}").ToArray()));
			sqlSb.AppendLine();
			sqlSb.Append("WHERE tf_id = @tf_id");


			var parameters = new List<NpgsqlParameter>();
			foreach (var columnName in processedRowDict.Keys)
				parameters.Add(new NpgsqlParameter($"@{columnName}", processedRowDict[columnName] ?? DBNull.Value));

			parameters.Add(new NpgsqlParameter($"@tf_id", tfId));

			var count = _dbService.ExecuteSqlNonQueryCommand(sqlSb.ToString(), parameters);

			if (count != 1)
				errors.Add("", "Database operation of update existing row failed.");

			CheckProcessValidationErrors(errors);

			var sharedColumns = provider.SharedColumns
				.Where(x => rowDict.Keys.ToHashSet().Contains($"{x.DataIdentity}.{x.DbName}"));

			if (sharedColumns.Any())
			{
				//we get the existing row again to be sure we have the latest tf_ide_... values
				existingRow = GetProviderRow(provider!, tfId);

				foreach (var sharedColumn in sharedColumns)
				{
					var dataIdentity = provider.Identities.Single(x => x.DataIdentity == sharedColumn.DataIdentity);

					var sharedColumnFullName = $"{sharedColumn.DataIdentity}.{sharedColumn.DbName}";

					object? value = ValidateProcessSharedColumnValue(
						provider, sharedColumn, rowDict[sharedColumnFullName], errors);

					CheckProcessValidationErrors(errors);

					string identityValue = (string)existingRow![$"tf_ide_{dataIdentity.DataIdentity}"]!;

					SaveSharedColumnValue(value, sharedColumnFullName, sharedColumn.Id,
						identityValue, sharedColumn.DbType);
				}
			}

			scope.Complete();
		}
	}

	private object? GetColumnValue(
		TfDataProvider provider,
		TfDataProviderColumn column,
		Guid? tfId,
		Dictionary<string, object>? rowDict,
		Dictionary<string, string> errors)
	{
		if (rowDict is not null && rowDict.ContainsKey(column.DbName!))
		{
			var valueObject = rowDict[column.DbName!];
			if (valueObject is null)
			{
				if (column.IsUnique)
					return GetColumnUniqueValue(provider, column);

				return GetColumnDefaultValue(column);
			}
			else
			{
				var processedValueObject = ValidateProcessColumnValue(provider, column, valueObject, errors);
				if (column.IsUnique)
				{
					if (!IsUniqueValue(provider, column, processedValueObject, tfId))
						errors.Add(column.DbName!, "Value must be unique");
				}
				return processedValueObject;
			}
		}

		if (column.IsUnique)
			return GetColumnUniqueValue(provider, column);

		return GetColumnDefaultValue(column);
	}

	private static object? ValidateProcessColumnValue(
		TfDataProvider provider,
		TfDataProviderColumn column,
		object? value,
		Dictionary<string, string> errors)
	{
		//if value is null we only check if collumn allows null values
		if (value is null)
		{
			if (!column.IsNullable)
				errors.Add(column.DbName!, "Value is required");

			return null;
		}


		switch (column.DbType)
		{
			case TfDatabaseColumnType.Guid:
				{
					if (value is Guid)
						return value;
					if (value is string && Guid.TryParse((string)value, out Guid result))
						return result;

					errors.Add(column.DbName!, "Value is not valid");
				}
				break;
			case TfDatabaseColumnType.ShortText:
			case TfDatabaseColumnType.Text:
				{
					if (value is string)
						return value;

					errors.Add(column.DbName!, "Value is not valid");
				}
				break;

			case TfDatabaseColumnType.ShortInteger:
			case TfDatabaseColumnType.Integer:
			case TfDatabaseColumnType.LongInteger:
			case TfDatabaseColumnType.Number:
				{
					try
					{
						return ConvertToNumericType(value, column.DbType);
					}
					catch (TfValidationException ex)
					{
						errors.Add(column.DbName!, ex.Message);
					}

					break;
				}
			case TfDatabaseColumnType.Boolean:
				{
					if (value is bool)
						return value;

					if (value is string && bool.TryParse((string)value, out bool result))
						return result;

					errors.Add(column.DbName!, "Value is not valid");
				}
				break;
			case TfDatabaseColumnType.DateTime:
				{
					if (value is DateTime)
						return value;

					if (value is string &&
					    DateTime.TryParse((string)value, CultureInfo.InvariantCulture, out DateTime result))
						return result;

					errors.Add(column.DbName!, "Value is not valid");
				}
				break;
			case TfDatabaseColumnType.DateOnly:
				{
					if (value is DateOnly)
						return value;

					if (value is DateTime)
						return DateOnly.FromDateTime((DateTime)value);

					if (value is string &&
					    DateOnly.TryParse((string)value, CultureInfo.InvariantCulture, out DateOnly result))
						return result;

					errors.Add(column.DbName!, "Value is not valid");
				}
				break;
			default:
				errors.Add(column.DbName!, "Value type is not supported");
				break;
		}

		errors.Add("", "Value type is not supported");
		return null;
	}

	private static object? ConvertToNumericType(
		object value,
		TfDatabaseColumnType targetType)
	{
		if (value == null)
			return value;

		if (value is not IConvertible)
			throw new TfValidationException("Invalid value.");

		object? convertedValue = null;
		object? convertedObject = null;

		switch (targetType)
		{
			case TfDatabaseColumnType.ShortInteger:

				if (value is IConvertible)
				{
					decimal dec = Convert.ToDecimal(value);
					if (dec >= short.MinValue && dec <= short.MaxValue)
					{
						convertedValue = Convert.ToInt16(value);
						convertedObject = Convert.ChangeType(convertedValue, value.GetType());
					}
					else
					{
						throw new TfValidationException(
							$"Provided value should be within range {short.MinValue} and {short.MaxValue}.");
					}
				}

				break;

			case TfDatabaseColumnType.Integer:

				if (value is IConvertible)
				{
					decimal dec = Convert.ToDecimal(value);
					if (dec >= int.MinValue && dec <= int.MaxValue)
					{
						convertedValue = Convert.ToInt32(value);
						convertedObject = Convert.ChangeType(convertedValue, value.GetType());
					}
					else
					{
						throw new TfValidationException(
							$"Provided value should be within range {int.MinValue} and {int.MaxValue}.");
					}
				}

				break;

			case TfDatabaseColumnType.LongInteger:

				if (value is IConvertible)
				{
					decimal dec = Convert.ToDecimal(value);
					if (dec >= long.MinValue && dec <= long.MaxValue)
					{
						convertedValue = Convert.ToInt64(value);
						convertedObject = Convert.ChangeType(convertedValue, value.GetType());
					}
					else
					{
						throw new TfValidationException(
							$"Provided value should be within range {long.MinValue} and {long.MaxValue}.");
					}
				}

				break;

			case TfDatabaseColumnType.Number:
				convertedValue = Convert.ToDecimal(value);
				convertedObject = Convert.ChangeType(convertedValue, value.GetType());
				break;

			default:
				throw new TfValidationException(nameof(targetType), "Unsupported numeric type.");
		}

		//if it is string no need to check for data loss
		if (value is string)
			return convertedValue;

		if (!value.Equals(convertedObject))
			throw new TfValidationException(
				"Provided value cannot be set to specified column because data loss occur.");

		return convertedValue;
	}

	private object GetColumnUniqueValue(
		TfDataProvider provider,
		TfDataProviderColumn column)
	{
		switch (column.DbType)
		{
			case TfDatabaseColumnType.Boolean:
				return Convert.ToBoolean(column.DefaultValue);
			case TfDatabaseColumnType.Text:
			case TfDatabaseColumnType.ShortText:
				return Guid.NewGuid().ToSha1();
			case TfDatabaseColumnType.Guid:
				return Guid.NewGuid();
			case TfDatabaseColumnType.DateOnly:
				{
					var maxValue = (DateTime)GetColumnMaxValue(provider, column);
					return maxValue.AddDays(1);
				}
			case TfDatabaseColumnType.DateTime:
				{
					var maxValue = (DateTime)GetColumnMaxValue(provider, column);
					return maxValue.AddSeconds(1);
				}
			case TfDatabaseColumnType.Number:
				{
					var maxValue = (decimal)GetColumnMaxValue(provider, column);
					return maxValue + 1;
				}
			case TfDatabaseColumnType.ShortInteger:
				{
					var maxValue = (short)GetColumnMaxValue(provider, column);
					return maxValue + 1;
				}
			case TfDatabaseColumnType.Integer:
				{
					var maxValue = (int)GetColumnMaxValue(provider, column);
					return maxValue + 1;
				}
			case TfDatabaseColumnType.LongInteger:
				{
					var maxValue = (long)GetColumnMaxValue(provider, column);
					return maxValue + 1;
				}
			default:
				throw new Exception("Not supported database column type while validate default value.");
		}
	}

	private object GetColumnMaxValue(
		TfDataProvider provider,
		TfDataProviderColumn column)
	{
		switch (column.DbType)
		{
			case TfDatabaseColumnType.DateOnly:
				{
					var dt = _dbService.ExecuteSqlQueryCommand(
						$"SELECT COALESCE(MAX(\"{column.DbName}\"), CURRENT_DATE) AS result FROM dp{provider.Index};");
					return DateOnly.FromDateTime((DateTime)dt.Rows[0]["result"]).ToDateTime();
				}
			case TfDatabaseColumnType.DateTime:
				{
					var dt = _dbService.ExecuteSqlQueryCommand(
						$"SELECT COALESCE(MAX(\"{column.DbName}\"), CURRENT_DATE) AS result FROM dp{provider.Index};");
					return (DateTime)dt.Rows[0]["result"];
				}
			case TfDatabaseColumnType.ShortInteger:
				{
					var dt = _dbService.ExecuteSqlQueryCommand(
						$"SELECT COALESCE(MAX(\"{column.DbName}\"), 0) AS result FROM dp{provider.Index};");
					return ((short)(int)dt.Rows[0]["result"]);
				}
			case TfDatabaseColumnType.Integer:
				{
					var dt = _dbService.ExecuteSqlQueryCommand(
						$"SELECT COALESCE(MAX(\"{column.DbName}\"), 0) AS result FROM dp{provider.Index};");
					return ((int)dt.Rows[0]["result"]);
				}
			case TfDatabaseColumnType.LongInteger:
				{
					var dt = _dbService.ExecuteSqlQueryCommand(
						$"SELECT COALESCE(MAX(\"{column.DbName}\"), 0) AS result FROM dp{provider.Index};");
					return ((long)dt.Rows[0]["result"]);
				}
			case TfDatabaseColumnType.Number:
				{
					var dt = _dbService.ExecuteSqlQueryCommand(
						$"SELECT COALESCE(MAX(\"{column.DbName}\"), 0) AS result FROM dp{provider.Index};");
					return ((decimal)dt.Rows[0]["result"]);
				}
			default:
				throw new Exception("Not supported database column type while getting max value.");
		}
	}

	private object? GetColumnDefaultValue(
		TfDataProviderColumn column)
	{
		if (!column.IsNullable && column.DefaultValue is null)
			throw new Exception("Column is not nullable, but default value is null.");

		if (column.DefaultValue is null)
			return null;

		switch (column.DbType)
		{
			case TfDatabaseColumnType.Boolean:
				return Convert.ToBoolean(column.DefaultValue);
			case TfDatabaseColumnType.Text:
			case TfDatabaseColumnType.ShortText:
				return column.DefaultValue;
			case TfDatabaseColumnType.Guid:
				{
					if (column.AutoDefaultValue)
					{
						return Guid.NewGuid();
					}

					return Guid.Parse(column.DefaultValue);
				}
			case TfDatabaseColumnType.DateOnly:
				{
					if (column.AutoDefaultValue == true)
					{
						return DateOnly.FromDateTime(DateTime.Now).ToDateTime();
					}

					return DateOnly.Parse(column.DefaultValue, CultureInfo.InvariantCulture).ToDateTime();
				}
			case TfDatabaseColumnType.DateTime:
				{
					if (column.AutoDefaultValue == true)
					{
						return DateTime.Now;
					}

					return DateOnly.Parse(column.DefaultValue, CultureInfo.InvariantCulture);
				}
			case TfDatabaseColumnType.Number:
				return Convert.ToDecimal(column.DefaultValue, CultureInfo.InvariantCulture);
			case TfDatabaseColumnType.ShortInteger:
				return Convert.ToInt16(column.DefaultValue);
			case TfDatabaseColumnType.Integer:
				return Convert.ToInt32(column.DefaultValue);
			case TfDatabaseColumnType.LongInteger:
				return Convert.ToInt64(column.DefaultValue);
			default:
				throw new Exception("Not supported database column type while validate default value.");
		}
	}

	private bool IsUniqueValue(
		TfDataProvider provider,
		TfDataProviderColumn column,
		object? value,
		Guid? tfId = null)
	{
		var parameters = new List<NpgsqlParameter>();

		var sql =
			$"SELECT COUNT(*) FROM dp{provider.Index} WHERE \"{column.DbName}\" = @value AND ( @tf_id IS NULL OR @tf_id <> tf_id ) ";
		if (value is null)
			sql =
				$"SELECT COUNT(*) FROM dp{provider.Index} WHERE \"{column.DbName}\" IS NULL AND ( @tf_id IS NULL OR @tf_id <> tf_id ) ";

		NpgsqlParameter tfIdParameter = new NpgsqlParameter("tf_id", NpgsqlDbType.Uuid);
		if (tfId is not null)
			tfIdParameter.Value = tfId.Value;
		else
			tfIdParameter.Value = DBNull.Value;

		parameters.Add(tfIdParameter);

		if (value is not null)
		{
			switch (column.DbType)
			{
				case TfDatabaseColumnType.Boolean:
					parameters.Add(new NpgsqlParameter<bool>("value", (bool)value));
					break;
				case TfDatabaseColumnType.Text:
				case TfDatabaseColumnType.ShortText:
					parameters.Add(new NpgsqlParameter<string>("value", (string)value));
					break;
				case TfDatabaseColumnType.Guid:
					parameters.Add(new NpgsqlParameter<Guid>("value", (Guid)value));
					break;
				case TfDatabaseColumnType.DateOnly:
					parameters.Add(new NpgsqlParameter<DateOnly>("value", (DateOnly)value));
					break;
				case TfDatabaseColumnType.DateTime:
					parameters.Add(new NpgsqlParameter<DateTime>("value", (DateTime)value));
					break;
				case TfDatabaseColumnType.Number:
					parameters.Add(new NpgsqlParameter<decimal>("value", (decimal)value));
					break;
				case TfDatabaseColumnType.ShortInteger:
					parameters.Add(new NpgsqlParameter<short>("value", (short)value));
					break;
				case TfDatabaseColumnType.Integer:
					parameters.Add(new NpgsqlParameter<int>("value", (int)value));
					break;
				case TfDatabaseColumnType.LongInteger:
					parameters.Add(new NpgsqlParameter<long>("value", (long)value));
					break;
				default:
					throw new Exception("Not supported database column type while validate default value.");
			}
		}

		var dt = _dbService.ExecuteSqlQueryCommand(sql, parameters);
		return dt.Rows.Count == 1 && ((long)dt.Rows[0][0]) == 0;
	}

	private object? GetTableSearchValue(
		TfDataProvider provider,
		Dictionary<string, object>? rowDict)
	{
		if (rowDict is null || !rowDict.Any())
			return string.Empty;

		var searchSb = new StringBuilder();

		foreach (var column in provider.Columns)
		{
			if (column.IncludeInTableSearch)
			{
				object? value = rowDict[column.DbName!];
				if (value is not null)
					searchSb.Append($" {value} ");
			}
		}

		return searchSb.ToString().Trim();
	}

	private object? ValidateProcessSharedColumnValue(
		TfDataProvider provider,
		TfSharedColumn sharedColumn,
		object? value,
		Dictionary<string, string> errors)
	{
		//shared column is always nullable
		if (value is null)
			return null;

		var sharedColumnFullName = $"{sharedColumn.DataIdentity}.{sharedColumn.DbName}";

		switch (sharedColumn.DbType)
		{
			case TfDatabaseColumnType.Guid:
				{
					if (value is Guid)
						return value;
					if (value is string && Guid.TryParse((string)value, out Guid result))
						return result;

					errors.Add(sharedColumnFullName, "Value is not valid");
				}
				break;
			case TfDatabaseColumnType.ShortText:
			case TfDatabaseColumnType.Text:
				{
					if (value is string)
						return value;

					errors.Add(sharedColumnFullName, "Value is not valid");
				}
				break;

			case TfDatabaseColumnType.ShortInteger:
			case TfDatabaseColumnType.Integer:
			case TfDatabaseColumnType.LongInteger:
			case TfDatabaseColumnType.Number:
				{
					try
					{
						return ConvertToNumericType(value, sharedColumn.DbType);
					}
					catch (TfValidationException ex)
					{
						errors.Add(sharedColumnFullName, ex.Message);
					}

					break;
				}
			case TfDatabaseColumnType.Boolean:
				{
					if (value is bool)
						return value;

					if (value is string && bool.TryParse((string)value, out bool result))
						return result;

					errors.Add(sharedColumnFullName, "Value is not valid");
				}
				break;
			case TfDatabaseColumnType.DateTime:
				{
					if (value is DateTime)
						return value;

					if (value is string &&
					    DateTime.TryParse((string)value, CultureInfo.InvariantCulture, out DateTime result))
						return result;

					errors.Add(sharedColumnFullName, "Value is not valid");
				}
				break;
			case TfDatabaseColumnType.DateOnly:
				{
					if (value is DateOnly)
						return value;

					if (value is DateTime)
						return DateOnly.FromDateTime((DateTime)value);

					if (value is string &&
					    DateOnly.TryParse((string)value, CultureInfo.InvariantCulture, out DateOnly result))
						return result;

					errors.Add(sharedColumnFullName, "Value is not valid");
				}
				break;
			default:
				errors.Add(sharedColumnFullName, "Value type is not supported");
				break;
		}

		errors.Add("", "Value type is not supported");
		return null;
	}

	private void SaveSharedColumnValue(
		object? value,
		string columnName,
		Guid sharedColumnId,
		string identityValue,
		TfDatabaseColumnType dbType)
	{
		var parameterType = GetDbTypeForDatabaseColumnType(dbType);

		NpgsqlParameter valueParameter = new NpgsqlParameter("@value", parameterType);

		if (value is null)
			valueParameter.Value = DBNull.Value;
		else
			valueParameter.Value = value;

		NpgsqlParameter sharedColumnIdParameter = new NpgsqlParameter("@shared_column_id", sharedColumnId);

		NpgsqlParameter dataIdentityValueParameter = new NpgsqlParameter("@data_identity_value", identityValue);

		var tableName = GetSharedColumnValueTableNameByType(dbType);

		var sql = $"DELETE FROM {tableName} WHERE " +
		          $"data_identity_value =  @data_identity_value AND shared_column_id = @shared_column_id;";

		sql += $"{Environment.NewLine}INSERT INTO {tableName}(data_identity_value, shared_column_id, value) " +
		       $"VALUES( @data_identity_value, @shared_column_id, @value );";

		_dbService.ExecuteSqlNonQueryCommand(sql,
			new List<NpgsqlParameter> { valueParameter, sharedColumnIdParameter, dataIdentityValueParameter });
	}

	private void CheckProcessValidationErrors(Dictionary<string, string> validationErrors)
	{
		if (validationErrors.Count > 0)
		{
			var validationResult = new ValidationResult();
			foreach (var key in validationErrors.Keys)
			{
				validationResult.Errors.Add(new ValidationFailure(key, validationErrors[key]));
			}

			validationResult
				.ToValidationException()
				.ThrowIfContainsErrors();
		}
	}

	#endregion
}