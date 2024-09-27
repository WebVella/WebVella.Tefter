using DocumentFormat.OpenXml.Wordprocessing;
using System.Data;

namespace WebVella.Tefter;

public partial interface IDataManager
{
	internal Result<TfDataTable> QueryDataProvider(
		TfDataProvider provider,
		string search = null,
		int? page = null,
		int? pageSize = null,
		bool noRows = false);

	internal Result<TfDataTable> QueryDataProvider(
		TfDataProvider provider,
		List<Guid> tfIds);


	internal Result<TfDataTable> QuerySpaceData(
		Guid spaceDataId,
		List<TfFilterBase> additionalFilters = null,
		List<TfSort> sortOverrides = null,
		string search = null,
		int? page = null,
		int? pageSize = null,
		bool noRows = false);

	public Result<TfDataTable> QuerySpaceData(
		Guid spaceDataId,
		List<Guid> tfIds);

	internal Result<TfDataTable> SaveDataTable(
		TfDataTable table);

	internal Result DeleteDataProviderRowByTfId(
		TfDataProvider provider,
		Guid tfId);
}

public partial class DataManager
{

	public Result<TfDataTable> QueryDataProvider(
		TfDataProvider provider,
		string search = null,
		int? page = null,
		int? pageSize = null,
		bool noRows = false)
	{
		try
		{

			if (provider is null)
			{
				return Result.Fail(new ValidationError(
						nameof(provider),
						"Provider object is null"));
			}

			var sqlBuilder = new SqlBuilder(
				dbService: _dbService,
				dataProvider: provider,
				spaceData: null,
				additionalFilters: null,
				sortOrders: null,
				search: search,
				page: page,
				pageSize: pageSize);

			var (sql, parameters, usedPage, usedPageSize) = sqlBuilder.Build();

			//do not make sql request if no rows are required
			DataTable dataTable = null;
			if (noRows)
				dataTable = new DataTable();
			else
				dataTable = _dbService.ExecuteSqlQueryCommand(sql, parameters);

			return Result.Ok(ProcessSqlResult(
				sql,
				parameters,
				provider,
				new TfDataTableQuery
				{
					Search = search,
					Page = usedPage,
					PageSize = usedPageSize,
					DataProviderId = provider.Id,
					SpaceDataId = null
				},
				dataTable
			));
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to get data provider rows").CausedBy(ex));
		}
	}

	public Result<TfDataTable> QueryDataProvider(
		TfDataProvider provider,
		List<Guid> tfIds)
	{
		try
		{
			if (provider is null)
			{
				return Result.Fail(new ValidationError(
						nameof(provider),
						"Provider object is null"));
			}

			if (tfIds is null)
			{
				return Result.Fail(new ValidationError(
						nameof(tfIds),
						"List of row ids object is null"));
			}

			var sqlBuilder = new SqlBuilder(
				dbService: _dbService,
				dataProvider: provider,
				spaceData: null,
				tfIds: tfIds);

			var (sql, parameters, usedPage, usedPageSize) = sqlBuilder.Build();

			DataTable dataTable = _dbService.ExecuteSqlQueryCommand(sql, parameters);

			return Result.Ok(ProcessSqlResult(
				sql,
				parameters,
				provider,
				new TfDataTableQuery
				{
					Search = null,
					Page = null,
					PageSize = null,
					DataProviderId = provider.Id,
					SpaceDataId = null
				},
				dataTable
			));
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to get data provider rows").CausedBy(ex));
		}
	}

	public Result<TfDataTable> QuerySpaceData(
		Guid spaceDataId,
		List<Guid> tfIds)
	{
		try
		{
			var spaceDataResult = _spaceManager.GetSpaceData(spaceDataId);
			if (!spaceDataResult.IsSuccess || spaceDataResult.Value == null)
			{
				return Result.Fail(new ValidationError(
						nameof(spaceDataId),
						"Found no space data for specified identifier."));
			}

			var spaceData = spaceDataResult.Value;

			var providerResult = _providerManager.GetProvider(spaceData.DataProviderId);
			if (!providerResult.IsSuccess || providerResult.Value == null)
			{
				return Result.Fail(new ValidationError(
						nameof(spaceDataId),
						"Found no data provider for specified space data."));
			}

			var provider = providerResult.Value;

			var sqlBuilder = new SqlBuilder(
				dbService: _dbService,
				dataProvider: provider,
				spaceData: spaceData,
				tfIds: tfIds);

			var (sql, parameters, usedPage, usedPageSize) = sqlBuilder.Build();

			DataTable dataTable = _dbService.ExecuteSqlQueryCommand(sql, parameters);

			return Result.Ok(ProcessSqlResult(
				sql,
				parameters,
				provider,
				new TfDataTableQuery
				{
					Search = null,
					Page = null,
					PageSize = null,
					DataProviderId = provider.Id,
					SpaceDataId = spaceDataId,
				},
				dataTable
			));
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to get data provider rows").CausedBy(ex));
		}
	}

	public Result<TfDataTable> QuerySpaceData(
		Guid spaceDataId,
		List<TfFilterBase> additionalFilters = null,
		List<TfSort> sortOverrides = null,
		string search = null,
		int? page = null,
		int? pageSize = null,
		bool noRows = false)
	{
		try
		{
			var spaceDataResult = _spaceManager.GetSpaceData(spaceDataId);
			if (!spaceDataResult.IsSuccess || spaceDataResult.Value == null)
			{
				return Result.Fail(new ValidationError(
						nameof(spaceDataId),
						"Found no space data for specified identifier."));
			}

			var spaceData = spaceDataResult.Value;

			var providerResult = _providerManager.GetProvider(spaceData.DataProviderId);
			if (!providerResult.IsSuccess || providerResult.Value == null)
			{
				return Result.Fail(new ValidationError(
						nameof(spaceDataId),
						"Found no data provider for specified space data."));
			}

			var provider = providerResult.Value;

			var sqlBuilder = new SqlBuilder(
				dbService: _dbService,
				dataProvider: provider,
				spaceData: spaceData,
				additionalFilters: additionalFilters,
				sortOrders: sortOverrides ?? spaceData.SortOrders,
				search: search,
				page: page,
				pageSize: pageSize);

			var (sql, parameters, usedPage, usedPageSize) = sqlBuilder.Build();

			//do not make sql request if no rows are required
			DataTable dataTable = null;
			if (noRows)
				dataTable = new DataTable();
			else
				dataTable = _dbService.ExecuteSqlQueryCommand(sql, parameters);

			return Result.Ok(ProcessSqlResult(
				sql,
				parameters,
				provider,
				new TfDataTableQuery
				{
					Search = search,
					Page = usedPage,
					PageSize = usedPageSize,
					DataProviderId = provider.Id,
					SpaceDataId = spaceData.Id
				},

				dataTable
			));
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to get data provider rows").CausedBy(ex));
		}
	}

	private TfDataTable ProcessSqlResult(
		string sql,
		List<NpgsqlParameter> sqlParameters,
		TfDataProvider provider,
		TfDataTableQuery query,
		DataTable dataTable)
	{
		List<string> columns = new List<string>();
		foreach (DataColumn column in dataTable.Columns)
			columns.Add(column.ColumnName);

		TfDataTable resultTable = new TfDataTable(provider, query, sql, sqlParameters, columns);

		if (dataTable.Rows.Count == 0)
			return resultTable;

		HashSet<string> dateOnlyColumns = new HashSet<string>();

		foreach (var column in provider.Columns)
		{
			if (column.DbType == DatabaseColumnType.Date)
				dateOnlyColumns.Add(column.DbName);
		}

		foreach (var column in provider.SharedColumns)
		{
			if (column.DbType == DatabaseColumnType.Date)
				dateOnlyColumns.Add(column.DbName);
		}


		foreach (DataRow row in dataTable.Rows)
		{
			object[] values = new object[resultTable.Columns.Count];

			int valuesCounter = 0;
			foreach (var column in resultTable.Columns)
			{
				object value = row[column.Name];

				if (value == DBNull.Value)
				{
					value = null;
				}
				else if (dateOnlyColumns.Contains(column.Name))
				{
					value = DateOnly.FromDateTime((DateTime)value);
				}

				values[valuesCounter++] = value;
			}
			resultTable.Rows.Add(new TfDataRow(resultTable, values));
		}

		return resultTable;
	}

	public Result<TfDataTable> SaveDataTable(
		TfDataTable table)
	{
		if (table is null)
		{
			return Result.Fail(new ValidationError(
					nameof(table),
					"Table object is null"));
		}


		using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
		{
			var providerResult = _providerManager.GetProvider(table.QueryInfo.DataProviderId);
			var provider = providerResult.Value;

			if (!providerResult.IsSuccess || provider is null)
			{
				return Result.Fail(new ValidationError(
						nameof(provider),
						"Provider associated to data table query is not found"));
			}

			foreach (TfDataRow row in table.Rows)
			{
				if ((Guid?)row["tf_id"] == null)
					InsertRow(provider, row);
				else
				{
					TfDataRow existingRow = null;
					if (table.QueryInfo.SpaceDataId is null)
					{
						var existingRowResult = QueryDataProvider(provider, new List<Guid> { (Guid)row["tf_id"] });

						if (!existingRowResult.IsSuccess)
						{
							return Result.Fail(new ValidationError(
									nameof(table),
									"Failed to get existing row by id from data provider table"));
						}

						if (existingRowResult.Value.Rows.Count != 1)
						{
							return Result.Fail(new ValidationError(
									nameof(table),
									"Row for update not found in provider table"));
						}

						existingRow = existingRowResult.Value.Rows[0];
					}
					else
					{
						var existingRowResult = QuerySpaceData(
							table.QueryInfo.SpaceDataId.Value,
							new List<Guid> { (Guid)row["tf_id"] });

						if (!existingRowResult.IsSuccess)
						{
							return Result.Fail(new ValidationError(
									nameof(table),
									"Failed to get existing row by id from space data"));
						}

						if (existingRowResult.Value.Rows.Count != 1)
						{
							return Result.Fail(new ValidationError(
									nameof(table),
									"Row for update not found in space data"));
						}

						existingRow = existingRowResult.Value.Rows[0];
					}

					UpdateRow(provider, existingRow, row);
				}
			}

			scope.Complete();

			List<Guid> rowIds = new List<Guid>();
			foreach (TfDataRow row in table.Rows)
				rowIds.Add((Guid)row["tf_id"]);

			if (table.QueryInfo.SpaceDataId is null)
				return QueryDataProvider(provider, rowIds);
			else
				return QuerySpaceData(table.QueryInfo.SpaceDataId.Value, rowIds);
		}
	}

	#region <--- insert / update row --->

	private void InsertRow(
		TfDataProvider provider,
		TfDataRow row)
	{
		row["tf_id"] = GetId(Guid.NewGuid()).Value;

		row["tf_created_on"] = DateTime.Now;

		row["tf_updated_on"] = DateTime.Now;

		row["tf_row_index"] = GetProviderNextRowIndex(provider);

		//generate search
		var searchSb = new StringBuilder();

		foreach (var column in provider.Columns)
		{
			if (column.IncludeInTableSearch)
			{
				bool columnFoundInDataTable = false;
				foreach (var tableColumn in row.DataTable.Columns)
				{
					columnFoundInDataTable = tableColumn.Name == column.DbName;
					if (columnFoundInDataTable)
						break;
				}

				if (!columnFoundInDataTable)
					continue;

				object value = row[column.DbName];
				if (value is not null)
					searchSb.Append($" {value}");
			}
		}

		row["tf_search"] = searchSb.ToString();

		//process shared keys
		foreach (var sharedKey in provider.SharedKeys)
		{
			List<string> keys = new List<string>();

			foreach (var column in sharedKey.Columns)
				keys.Add(row[column.DbName]?.ToString()); //Boz: columns could be nullable


			row[$"tf_sk_{sharedKey.DbName}_id"] = GetId(keys.ToArray()).Value;

			row[$"tf_sk_{sharedKey.DbName}_version"] = sharedKey.Version;
		}

		List<NpgsqlParameter> parameters;
		var sql = BuildInsertNewRowSql(provider, row, out parameters);

		var count = _dbService.ExecuteSqlNonQueryCommand(sql, parameters);

		if (count != 1)
			throw new Exception("Failed to insert new row");

		foreach (var tableColumn in row.DataTable.Columns)
		{
			if (!tableColumn.IsShared)
				continue;

			var sharedColumn = provider.SharedColumns.Single(x => x.DbName == tableColumn.Name);

			var sharedKey = provider.SharedKeys.Single(x => x.DbName == sharedColumn.SharedKeyDbName);

			DatabaseColumnType dbType = sharedColumn.DbType;

			object value = row[tableColumn.Name];

			Guid sharedColumnId = sharedColumn.Id;

			Guid sharedKeyId = (Guid)row[$"tf_sk_{sharedKey.DbName}_id"];

			UpsertSharedColumnValue(
				value,
				 tableColumn.Name,
				sharedColumnId,
				sharedKeyId,
				dbType);
		}
	}

	private void UpsertSharedColumnValue(
		object value,
		string columnName,
		Guid sharedColumnId,
		Guid sharedKeyId,
		DatabaseColumnType dbType)
	{

		var parameterType = GetDbTypeForDatabaseColumnType(dbType);

		NpgsqlParameter valueParameter = new NpgsqlParameter("@value", parameterType);

		if (value is null)
			valueParameter.Value = DBNull.Value;
		else
			valueParameter.Value = value;

		NpgsqlParameter sharedColumnIdParameter = new NpgsqlParameter("@shared_column_id", sharedColumnId);

		NpgsqlParameter sharedKeyIdParameter = new NpgsqlParameter("@shared_key_id", sharedKeyId);

		var tableName = GetSharedColumnValueTableNameByType(dbType);

		var sql = $"DELETE FROM {tableName} WHERE " +
			$"shared_key_id =  @shared_key_id AND shared_column_id = @shared_column_id;";

		sql += $"{Environment.NewLine}INSERT INTO {tableName}(shared_key_id, shared_column_id, value) " +
			$"VALUES( @shared_key_id, @shared_column_id, @value );";

		_dbService.ExecuteSqlNonQueryCommand(sql,
			new List<NpgsqlParameter> {
				valueParameter,
				sharedColumnIdParameter,
				sharedKeyIdParameter
			});
	}

	private string BuildInsertNewRowSql(
		TfDataProvider provider,
		TfDataRow row,
		out List<NpgsqlParameter> parameters)
	{
		parameters = new List<NpgsqlParameter>();

		List<string> columnNames = new List<string>();

		for (int i = 0; i < row.DataTable.Columns.Count; i++)
		{
			var tableColumn = row.DataTable.Columns[i];

			//ignore shared columns here
			if (tableColumn.IsShared)
				continue;

			columnNames.Add(tableColumn.Name);

			var parameterType = GetDbTypeForDatabaseColumnType(tableColumn.DbType);

			NpgsqlParameter parameter = new NpgsqlParameter($"@{tableColumn.Name}", parameterType);

			if (row[tableColumn.Name] is null)
				parameter.Value = DBNull.Value;
			else
				parameter.Value = row[tableColumn.Name];

			parameters.Add(parameter);

		}

		StringBuilder sql = new StringBuilder();
		sql.Append($"INSERT INTO dp{provider.Index} ( ");
		sql.Append(string.Join(", ", columnNames.Select(x => $"\"{x}\"").ToArray()));
		sql.Append(" ) VALUES ( ");
		sql.Append(string.Join(", ", columnNames.Select(x => $"@{x}").ToArray()));
		sql.Append(" ) ");
		return sql.ToString();
	}

	private void UpdateRow(
		TfDataProvider provider,
		TfDataRow existingRow,
		TfDataRow row)
	{
		//generate search
		var searchSb = new StringBuilder();

		foreach (var column in provider.Columns)
		{
			if (column.IncludeInTableSearch)
			{
				bool columnFoundInDataTable = false;
				foreach (var tableColumn in row.DataTable.Columns)
				{
					columnFoundInDataTable = tableColumn.Name == column.DbName;
					if (columnFoundInDataTable)
						break;
				}

				if (!columnFoundInDataTable)
					continue;

				object value = row[column.DbName];
				if (value is not null)
					searchSb.Append($" {value}");
			}
		}

		row["tf_search"] = searchSb.ToString();

		row["tf_updated_on"] = DateTime.Now;

		//process shared keys for changes
		foreach (var sharedKey in provider.SharedKeys)
		{
			List<string> keys = new List<string>();

			foreach (var column in sharedKey.Columns)
				keys.Add(row[column.DbName]?.ToString());

			var newId = GetId(keys.ToArray()).Value;

			if (newId != (Guid)row[$"tf_sk_{sharedKey.DbName}_id"])
				row[$"tf_sk_{sharedKey.DbName}_id"] = newId;
		}

		List<string> columnsWithChanges = new List<string>();
		foreach (var column in row.DataTable.Columns)
		{
			if (column.DbType == DatabaseColumnType.Guid)
			{
				if ((Guid?)row[column.Name] != (Guid?)existingRow[column.Name])
					columnsWithChanges.Add(column.Name);
			}
			else if (column.DbType == DatabaseColumnType.Date)
			{
				if ((DateOnly?)row[column.Name] != (DateOnly?)existingRow[column.Name])
					columnsWithChanges.Add(column.Name);
			}
			else if (column.DbType == DatabaseColumnType.DateTime)
			{
				if ((DateTime?)row[column.Name] != (DateTime?)existingRow[column.Name])
					columnsWithChanges.Add(column.Name);
			}
			else if (column.DbType == DatabaseColumnType.ShortInteger)
			{
				if ((short?)row[column.Name] != (short?)existingRow[column.Name])
					columnsWithChanges.Add(column.Name);
			}
			else if (column.DbType == DatabaseColumnType.Integer)
			{
				if ((int?)row[column.Name] != (int?)existingRow[column.Name])
					columnsWithChanges.Add(column.Name);
			}
			else if (column.DbType == DatabaseColumnType.LongInteger)
			{
				if ((long?)row[column.Name] != (long?)existingRow[column.Name])
					columnsWithChanges.Add(column.Name);
			}
			else if (column.DbType == DatabaseColumnType.Number)
			{
				if ((decimal?)row[column.Name] != (decimal?)existingRow[column.Name])
					columnsWithChanges.Add(column.Name);
			}
			else if (column.DbType == DatabaseColumnType.Boolean)
			{
				if ((bool?)row[column.Name] != (bool?)existingRow[column.Name])
					columnsWithChanges.Add(column.Name);
			}
			else if (column.DbType == DatabaseColumnType.Text ||
				column.DbType == DatabaseColumnType.ShortText)
			{
				if ((string?)row[column.Name] != (string?)existingRow[column.Name])
					columnsWithChanges.Add(column.Name);
			}
			else
			{
				throw new Exception("Not supported row type update");
			}
		}

		List<NpgsqlParameter> parameters;

		var sql = BuildUpdateExistingRowSql(
			columnsWithChanges,
			provider,
			row,
			out parameters);

		var count = _dbService.ExecuteSqlNonQueryCommand(sql, parameters);

		if (count != 1)
			throw new Exception("Failed to update row");

		foreach (var tableColumn in row.DataTable.Columns)
		{
			if (!tableColumn.IsShared)
				continue;

			//if no change
			if (!columnsWithChanges.Contains(tableColumn.Name))
				continue;

			var sharedColumn = provider.SharedColumns.Single(x => x.DbName == tableColumn.Name);

			var sharedKey = provider.SharedKeys.Single(x => x.DbName == sharedColumn.SharedKeyDbName);

			DatabaseColumnType dbType = sharedColumn.DbType;

			object value = row[tableColumn.Name];

			Guid sharedColumnId = sharedColumn.Id;

			Guid sharedKeyId = (Guid)row[$"tf_sk_{sharedKey.DbName}_id"];

			UpsertSharedColumnValue(
				value,
				 tableColumn.Name,
				sharedColumnId,
				sharedKeyId,
				dbType);
		}
	}

	private string BuildUpdateExistingRowSql(
		List<string> columnsWithChange,
		TfDataProvider provider,
		TfDataRow row,
		out List<NpgsqlParameter> parameters)
	{
		parameters = new List<NpgsqlParameter>();

		List<string> columnNames = new List<string>();

		for (int i = 0; i < row.DataTable.Columns.Count; i++)
		{
			var tableColumn = row.DataTable.Columns[i];

			//ignore shared columns here
			if (tableColumn.IsShared)
				continue;

			if (!columnsWithChange.Contains(tableColumn.Name))
				continue;

			columnNames.Add(tableColumn.Name);

			var parameterType = GetDbTypeForDatabaseColumnType(tableColumn.DbType);

			NpgsqlParameter parameter = new NpgsqlParameter($"@{tableColumn.Name}", parameterType);

			if (row[tableColumn.Name] is null)
				parameter.Value = DBNull.Value;
			else
				parameter.Value = row[tableColumn.Name];

			parameters.Add(parameter);
		}

		StringBuilder sql = new StringBuilder();

		sql.Append($"UPDATE dp{provider.Index} {Environment.NewLine} SET {Environment.NewLine}");

		sql.Append(string.Join($",{Environment.NewLine}",
			columnNames.Select(x => $"\"{x}\" = @{x}").ToArray()));

		sql.AppendLine();

		sql.Append("WHERE tf_id = @tf_id");

		parameters.Add(new NpgsqlParameter($"@tf_id", row["tf_id"]));

		return sql.ToString();
	}

	#endregion


	public Result DeleteDataProviderRowByTfId(
		TfDataProvider provider,
		Guid tfId)
	{

		try
		{
			if (provider is null)
			{
				return Result.Fail(new ValidationError(
						nameof(provider),
						"Provider object is null"));
			}

			var count = _dbService.ExecuteSqlNonQueryCommand(
				$"DELETE FROM dp{provider.Index} WHERE tf_id = @tf_id",
				new NpgsqlParameter("@tf_id", tfId));

			if(count == 0)
			{
				return Result.Fail(new ValidationError(
						nameof(provider),
						"Data row not found in provider table."));
			}

			return Result.Ok();
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to delete data provider row").CausedBy(ex));
		}
	}

	private int GetProviderNextRowIndex(
		TfDataProvider provider)
	{
		var dt = _dbService.ExecuteSqlQueryCommand($"SELECT MAX(tf_row_index) FROM dp{provider.Index}");
		if (dt.Rows[0][0] == DBNull.Value)
			return 1;

		return ((int)dt.Rows[0][0]) + 1;
	}

	private static DbType GetDbTypeForDatabaseColumnType(DatabaseColumnType dbType)
	{
		DbType resultType;
		switch (dbType)
		{
			case DatabaseColumnType.Guid:
				resultType = DbType.Guid; break;
			case DatabaseColumnType.Boolean:
				resultType = DbType.Boolean; break;
			case DatabaseColumnType.Date:
				resultType = DbType.Date; break;
			case DatabaseColumnType.DateTime:
				resultType = DbType.DateTime2; break;
			case DatabaseColumnType.ShortText:
				resultType = DbType.StringFixedLength; break;
			case DatabaseColumnType.Text:
				resultType = DbType.String; break;
			case DatabaseColumnType.ShortInteger:
				resultType = DbType.Int16; break;
			case DatabaseColumnType.Integer:
				resultType = DbType.Int32; break;
			case DatabaseColumnType.LongInteger:
				resultType = DbType.Int64; break;
			case DatabaseColumnType.Number:
				resultType = DbType.Decimal; break;
			case DatabaseColumnType.AutoIncrement:
			default:
				throw new Exception("Not supported for conversion database column type.");
		}
		return resultType;
	}
}
