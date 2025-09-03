using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2013.PowerPoint.Roaming;
using DocumentFormat.OpenXml.Spreadsheet;
using Newtonsoft.Json.Linq;
using NpgsqlTypes;
using System.Data;
using WebVella.BlazorTrace;

namespace WebVella.Tefter.Services;

public partial interface ITfService
{
	public TfDataTable QueryDataProvider(
		Guid providerId,
		string? search = null,
		int? page = null,
		int? pageSize = null,
		bool noRows = false);
	public TfDataTable QueryDataProvider(
		TfDataProvider provider,
		string? search = null,
		int? page = null,
		int? pageSize = null,
		bool noRows = false);

	public TfDataTable QueryDataProvider(
		TfDataProvider provider,
		List<Guid> tfIds);


	public TfDataTable QuerySpaceData(
		Guid spaceDataId,
		List<TfFilterBase>? userFilters = null,
		List<TfSort>? userSorts = null,
		List<TfFilterBase>? presetFilters = null,
		List<TfSort>? presetSorts = null,
		string? search = null,
		int? page = null,
		int? pageSize = null,
		bool noRows = false,
		bool returnOnlyTfIds = false);

	public TfDataTable QuerySpaceData(
		Guid spaceDataId,
		List<Guid> tfIds);

	public TfDataTable SaveDataTable(
		TfDataTable table);

	public void DeleteDataProviderRowByTfId(
		TfDataProvider provider,
		Guid tfId);

	public void DeleteDataProviderRowsByTfId(
		Guid providerId,
		List<Guid> idList);

	internal TfDataProviderDataRow GetProviderRow(
		TfDataProvider provider,
		Guid id);
	internal TfDataProviderDataRow GetProviderRow(
		TfDataProvider provider,
		int index);

	internal void InsertNewProviderRow(
		TfDataProvider provider,
		TfDataProviderDataRow row);

	internal void UpdateProviderRow(
		TfDataProvider provider,
		TfDataProviderDataRow row);

	internal void DeleteProviderRowsAfterIndex(
		TfDataProvider provider,
		int index);

	internal void UpdateValue(
		TfDataProvider provider,
		Guid rowId,
		string dbName,
		object value);

	void DeleteAllProviderRows(
			Guid providerId);

	internal void DeleteAllProviderRows(
		TfDataProvider provider);

	internal void DeleteSharedColumnData(
		TfSharedColumn sharedColumn);

	public Dictionary<Guid, string> GetDataIdentityValuesForRowIds(
		TfDataProvider provider,
		TfDataIdentity dataIdentity,
		List<Guid> rowIds);

	public void SaveSharedColumnValues(
		Guid sharedColumnId,
		Dictionary<string, object> valuesDict,
		int batchSize = 100);
}

public partial class TfService : ITfService
{
	public TfDataTable QueryDataProvider(
		Guid providerId,
		string? search = null,
		int? page = null,
		int? pageSize = null,
		bool noRows = false)
	{
		try
		{
			var provider = GetDataProvider(providerId);
			if (provider == null)
				throw new Exception("Provider not found");

			return QueryDataProvider(
				provider: provider,
				search: search,
				page: page,
				pageSize: pageSize,
				noRows: noRows
			);
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}
	public TfDataTable QueryDataProvider(
		TfDataProvider provider,
		string? search = null,
		int? page = null,
		int? pageSize = null,
		bool noRows = false)
	{
		try
		{
			if (provider is null)
			{
				throw new ArgumentNullException(nameof(provider));
			}

			var allDataProviders = GetDataProviders();

			var sqlBuilder = new SqlBuilder(
				tfService: this,
				dbService: _dbService,
				dataProvider: provider,
				dataProviders: allDataProviders,
				spaceData: null,
				userFilters: null,
				userSorts: null,
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

			return ProcessSqlResult(
				sql,
				parameters,
				provider,
				sqlBuilder.JoinData,
				sqlBuilder.SharedColumnsData,
				new TfDataTableQuery
				{
					Search = search,
					Page = usedPage,
					PageSize = usedPageSize,
					DataProviderId = provider.Id,
					SpaceDataId = null
				},
				dataTable
			);
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public TfDataTable QueryDataProvider(
		TfDataProvider provider,
		List<Guid> tfIds)
	{
		try
		{
			if (provider is null)
			{
				throw new ArgumentNullException(nameof(provider));
			}

			if (tfIds is null)
			{
				throw new ArgumentNullException(nameof(tfIds));
			}

			var allDataProviders = GetDataProviders();

			var sqlBuilder = new SqlBuilder(
				tfService: this,
				dbService: _dbService,
				dataProvider: provider,
				dataProviders: allDataProviders,
				spaceData: null,
				tfIds: tfIds);

			var (sql, parameters, usedPage, usedPageSize) = sqlBuilder.Build();

			DataTable dataTable = _dbService.ExecuteSqlQueryCommand(sql, parameters);

			return ProcessSqlResult(
				sql,
				parameters,
				provider,
				sqlBuilder.JoinData,
				sqlBuilder.SharedColumnsData,
				new TfDataTableQuery
				{
					Search = null,
					Page = null,
					PageSize = null,
					DataProviderId = provider.Id,
					SpaceDataId = null
				},
				dataTable
			);
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public TfDataTable QuerySpaceData(
		Guid spaceDataId,
		List<Guid> tfIds)
	{
		try
		{
			var spaceData = GetSpaceData(spaceDataId);
			if (spaceData is null)
			{
				throw new TfException("Found no existing space data for specified id.");
			}


			var provider = GetDataProvider(spaceData.DataProviderId);
			if (provider is null)
			{
				throw new TfException("There is not existing provider for specified space data.");
			}

			var allDataProviders = GetDataProviders();

			var sqlBuilder = new SqlBuilder(
				tfService: this,
				dbService: _dbService,
				dataProviders: allDataProviders,
				dataProvider: provider,
				spaceData: spaceData,
				tfIds: tfIds);

			var (sql, parameters, usedPage, usedPageSize) = sqlBuilder.Build();

			DataTable dataTable = _dbService.ExecuteSqlQueryCommand(sql, parameters);

			return ProcessSqlResult(
				sql,
				parameters,
				provider,
				sqlBuilder.JoinData,
				sqlBuilder.SharedColumnsData,
				new TfDataTableQuery
				{
					Search = null,
					Page = null,
					PageSize = null,
					DataProviderId = provider.Id,
					SpaceDataId = spaceDataId,
				},
				dataTable
			);
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public TfDataTable QuerySpaceData(
		Guid spaceDataId,
		List<TfFilterBase> userFilters = null,
		List<TfSort> userSorts = null,
		List<TfFilterBase> presetFilters = null,
		List<TfSort> presetSorts = null,
		string search = null,
		int? page = null,
		int? pageSize = null,
		bool noRows = false,
		bool returnOnlyTfIds = false)
	{
		try
		{
			var spaceData = GetSpaceData(spaceDataId);
			if (spaceData is null)
			{
				throw new TfException("Found no existing space data for specified id.");
			}


			var provider = GetDataProvider(spaceData.DataProviderId);
			if (provider is null)
			{
				throw new TfException("There is not existing provider for specified space data.");
			}

			var allDataProviders = GetDataProviders();

			var sqlBuilder = new SqlBuilder(
				tfService: this,
				dbService: _dbService,
				dataProvider: provider,
				dataProviders: allDataProviders,
				spaceData: spaceData,
				userFilters: userFilters,
				userSorts: userSorts,
				presetFilters: presetFilters,
				presetSorts: presetSorts,
				search: search,
				page: page,
				pageSize: pageSize,
				returnOnlyTfIds: returnOnlyTfIds);

			var (sql, parameters, usedPage, usedPageSize) = sqlBuilder.Build();

			//do not make sql request if no rows are required
			DataTable dataTable = null;
			if (noRows)
				dataTable = new DataTable();
			else
				dataTable = _dbService.ExecuteSqlQueryCommand(sql, parameters);

			return ProcessSqlResult(
				sql,
				parameters,
				provider,
				sqlBuilder.JoinData,
				sqlBuilder.SharedColumnsData,
				new TfDataTableQuery
				{
					Search = search,
					Page = usedPage,
					PageSize = usedPageSize,
					DataProviderId = provider.Id,
					SpaceDataId = spaceData.Id
				},

				dataTable
			);
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	private TfDataTable ProcessSqlResult(
		string sql,
		List<NpgsqlParameter> sqlParameters,
		TfDataProvider provider,
		List<SqlBuilderJoinData> joinData,
		List<SqlBuilderSharedColumnData> sharedColumnsData,
		TfDataTableQuery query,
		DataTable dataTable)
	{
		List<string> columns = new List<string>();
		foreach (DataColumn column in dataTable.Columns)
		{
			columns.Add(column.ColumnName);
		}

		TfDataTable resultTable = new TfDataTable(provider, joinData,
			sharedColumnsData, query, sql, sqlParameters, columns);

		HashSet<string> dateOnlyColumns = new HashSet<string>();

		foreach (var column in resultTable.Columns)
		{
			if (column.DbType == TfDatabaseColumnType.DateOnly)
				dateOnlyColumns.Add(column.Name);
		}

		Dictionary<string, TfDataProviderColumn> joinedColumns = new();

		foreach (var data in joinData)
		{
			foreach (var column in data.Columns)
			{
				var providerColumn = data.Provider.Columns.Single(x => x.DbName == column.DbName);
				joinedColumns.Add($"jp$dp{data.Provider.Index}${data.DataIdentity}", providerColumn);
			}
		}

		foreach (DataRow row in dataTable.Rows)
		{
			var joinColumnValuesDict = GetRowJoinedValues(row, joinData);
			object[] values = new object[resultTable.Columns.Count];
			int valuesCounter = 0;
			foreach (var column in resultTable.Columns)
			{
				var columnName = column.Name;

				if (column.IsJoinColumn)
				{
					values[valuesCounter++] = joinColumnValuesDict[column.Name];
					continue;
				}
				else if (column.IsShared)
				{
					var segments = columnName.Split(".");
					columnName = segments[1];
				}

				object value = row[columnName];

				if (value == DBNull.Value)
				{
					value = null;
				}
				else if (dateOnlyColumns.Contains(columnName))
				{
					value = DateOnly.FromDateTime((DateTime)value);
				}

				values[valuesCounter++] = value;
			}
			resultTable.Rows.Add(new TfDataRow(resultTable, values));
		}

		return resultTable;
	}

	private Dictionary<string, object> GetRowJoinedValues(
		DataRow row,
		List<SqlBuilderJoinData> joinData)
	{
		Dictionary<string, object> joinedValues = new Dictionary<string, object>();

		foreach (var data in joinData)
		{
			var sourceColumnName = $"jp$dp{data.Provider.Index}${data.DataIdentity}";
			JArray jArr = JArray.Parse((string)row[sourceColumnName]);

			foreach (var sqlColumn in data.Columns)
			{
				var providerColumn = data.Provider.Columns.Single(x => x.DbName == sqlColumn.DbName);
				object value = ExtractJoinedRecordsValue(jArr, providerColumn);
				joinedValues[$"{data.DataIdentity}.{sqlColumn.DbName}"] = value;
			}
		}
		return joinedValues;
	}

	private object ExtractJoinedRecordsValue(
		JArray jArr,
		TfDataProviderColumn providerColumn)
	{
		switch (providerColumn.DbType)
		{
			case TfDatabaseColumnType.Text:
			case TfDatabaseColumnType.ShortText:
				{
					List<string> result = new List<string>();
					foreach (var jObj in jArr)
					{
						string stringValue = ((JToken)jObj[providerColumn.DbName]).ToObject<string>();
						result.Add(stringValue);
					}
					return result;
				}
			case TfDatabaseColumnType.DateTime:
				{
					List<DateTime?> result = new List<DateTime?>();
					foreach (var jObj in jArr)
					{
						string stringValue = ((JToken)jObj[providerColumn.DbName]).ToObject<string>();
						if (string.IsNullOrWhiteSpace(stringValue))
							result.Add(null);
						else
							result.Add(DateTime.Parse(stringValue));
					}
					return result;
				}
			case TfDatabaseColumnType.DateOnly:
				{
					List<DateOnly?> result = new List<DateOnly?>();
					foreach (var jObj in jArr)
					{
						string stringValue = ((JToken)jObj[providerColumn.DbName]).ToObject<string>();
						if (string.IsNullOrWhiteSpace(stringValue))
							result.Add(null);
						else
							result.Add(DateOnly.Parse(stringValue));
					}
					return result;
				}
			case TfDatabaseColumnType.Number:
				{
					List<decimal?> result = new List<decimal?>();
					foreach (var jObj in jArr)
					{
						var value = ((JToken)jObj[providerColumn.DbName]).Value<decimal?>();
						result.Add(value);
					}
					return result;
				}
			case TfDatabaseColumnType.ShortInteger:
				{
					List<short?> result = new List<short?>();
					foreach (var jObj in jArr)
					{
						var value = ((JToken)jObj[providerColumn.DbName]).Value<short?>();
						result.Add(value);
					}
					return result;
				}
			case TfDatabaseColumnType.Integer:
				{
					List<int?> result = new List<int?>();
					foreach (var jObj in jArr)
					{
						var value = ((JToken)jObj[providerColumn.DbName]).Value<int?>();
						result.Add(value);
					}
					return result;
				}
			case TfDatabaseColumnType.LongInteger:
				{
					List<long?> result = new List<long?>();
					foreach (var jObj in jArr)
					{
						var value = ((JToken)jObj[providerColumn.DbName]).Value<long?>();
						result.Add(value);
					}
					return result;
				}
			case TfDatabaseColumnType.AutoIncrement:
				{
					List<int?> result = new List<int?>();
					foreach (var jObj in jArr)
					{
						var value = ((JToken)jObj[providerColumn.DbName]).Value<int?>();
						result.Add(value);
					}
					return result;
				}
			case TfDatabaseColumnType.Guid:
				{
					List<Guid?> result = new List<Guid?>();
					foreach (var jObj in jArr)
					{
						var value = ((JToken)jObj[providerColumn.DbName]).Value<Guid?>();
						result.Add(value);
					}
					return result;
				}
			case TfDatabaseColumnType.Boolean:
				{
					List<bool?> result = new List<bool?>();
					foreach (var jObj in jArr)
					{
						var value = ((JToken)jObj[providerColumn.DbName]).Value<bool?>();
						result.Add(value);
					}
					return result;
				}
			default:
				throw new Exception("Not supported db type");

		}
	}

	public TfDataTable SaveDataTable(
		TfDataTable table)
	{
		try
		{
			if (table is null)
			{
				throw new TfException("Table object is null");
			}


			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var provider = GetDataProvider(table.QueryInfo.DataProviderId);
				if (provider is null)
				{
					throw new TfValidationException(
							nameof(provider),
							"Provider associated to data table query is not found");
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
							var dt = QueryDataProvider(provider, new List<Guid> { (Guid)row["tf_id"] });
							if (dt.Rows.Count != 1)
							{
								throw new TfValidationException(
									nameof(table),
									"Row for update not found in provider table");
							}

							existingRow = dt.Rows[0];
						}
						else
						{
							var dt = QuerySpaceData(
								table.QueryInfo.SpaceDataId.Value,
								new List<Guid> { (Guid)row["tf_id"] });

							if (dt.Rows.Count != 1)
							{
								throw new TfValidationException(
									nameof(table),
									"Row for update not found in provider table");
							}

							existingRow = dt.Rows[0];
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
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	#region <--- insert / update row --->

	private void InsertRow(
		TfDataProvider provider,
		TfDataRow row)
	{
		row["tf_id"] = GetId(Guid.NewGuid());

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


		List<NpgsqlParameter> parameters;
		var sql = BuildInsertNewRowSql(provider, row, out parameters);

		var count = _dbService.ExecuteSqlNonQueryCommand(sql, parameters);

		if (count != 1)
			throw new Exception("Failed to insert new row");

		//we need the row after update because identity columns value is calculated in database
		var insertedRow = GetProviderRow(provider, (Guid)row["tf_id"]);

		foreach (var tableColumn in row.DataTable.Columns)
		{
			if (!tableColumn.IsShared)
				continue;

			//join columns will not be updated
			//this is not supported at the moment, 
			//but in case we support it at later stage
			if (tableColumn.IsJoinColumn)
				continue;


			var sharedColumn = provider.SharedColumns.Single(x => $"{x.DataIdentity}.{x.DbName}" == tableColumn.Name);

			var dataIdentity = provider.Identities.Single(x => x.DataIdentity == sharedColumn.DataIdentity);

			TfDatabaseColumnType dbType = sharedColumn.DbType;

			object value = row[tableColumn.Name];

			Guid sharedColumnId = sharedColumn.Id;

			string identityValue = null;
			if (dataIdentity.DataIdentity == TfConstants.TF_ROW_ID_DATA_IDENTITY)
				identityValue = (string)insertedRow["tf_row_id"];
			else
				identityValue = (string)insertedRow[$"tf_ide_{dataIdentity.DataIdentity}"];

			UpsertSharedColumnValue(
				value,
				tableColumn.Name,
				sharedColumnId,
				identityValue,
				dbType);
		}
	}

	public void SaveSharedColumnValues(
		Guid sharedColumnId,
		Dictionary<string, object> valuesDict,
		int batchSize = 100 )
	{
		if(batchSize <= 0)
			throw new ArgumentOutOfRangeException(nameof(batchSize), "Batch size must be greater than zero");

		if(valuesDict is null)
			throw new ArgumentNullException(nameof(valuesDict));

		var sharedColumn = GetSharedColumn(sharedColumnId);

		if (sharedColumn is null)
			throw new Exception("Shared column not found");

		var tableName = GetSharedColumnValueTableNameByType(sharedColumn.DbType);

		using (var scope = _dbService.CreateTransactionScope())
		{
			foreach (IEnumerable<string> keysBatch in valuesDict.Keys.Batch(batchSize))
			{
				int paramCounter = 1;

				List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();

				StringBuilder sqlSb = new StringBuilder();

				foreach (var identityValue in keysBatch )
				{
					var value = valuesDict[identityValue];

					sqlSb.AppendLine($"DELETE FROM {tableName} WHERE data_identity_value = @data_identity_value_{paramCounter}" +
						$" AND shared_column_id = @shared_column_id_{paramCounter};");

					sqlSb.AppendLine($"INSERT INTO {tableName}(data_identity_value, shared_column_id, value) " +
						$"VALUES( @data_identity_value_{paramCounter}, @shared_column_id_{paramCounter}, @value_{paramCounter} );");

					var valueParameter = new NpgsqlParameter($"@value_{paramCounter}", GetDbTypeForDatabaseColumnType(sharedColumn.DbType));
					if (value is null)
						valueParameter.Value = DBNull.Value;
					else
						valueParameter.Value = value;

					parameters.Add(valueParameter);
					parameters.Add(new NpgsqlParameter($"@shared_column_id_{paramCounter}", sharedColumnId));
					parameters.Add(new NpgsqlParameter($"@data_identity_value_{paramCounter}", identityValue));

					paramCounter++;
				}

				_dbService.ExecuteSqlNonQueryCommand(sqlSb.ToString(), parameters);
			}

			scope.Complete();
		}
	}

	private void UpsertSharedColumnValue(
		object value,
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
			new List<NpgsqlParameter> {
				valueParameter,
				sharedColumnIdParameter,
				dataIdentityValueParameter
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

			//ignore shared, identity and joined columns here
			if (tableColumn.IsShared || tableColumn.IsJoinColumn || tableColumn.IsIdentityColumn)
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

		string providerTableName = $"dp{provider.Index}";

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


		List<string> columnsWithChanges = new List<string>();
		foreach (var column in row.DataTable.Columns)
		{
			//join,shared and identity columns will not be updated
			if (column.IsShared || column.IsJoinColumn || column.IsIdentityColumn)
				continue;

			if (column.DbType == TfDatabaseColumnType.Guid)
			{
				if ((Guid?)row[column.Name] != (Guid?)existingRow[column.Name])
					columnsWithChanges.Add(column.Name);
			}
			else if (column.DbType == TfDatabaseColumnType.DateOnly)
			{
				if ((DateOnly?)row[column.Name] != (DateOnly?)existingRow[column.Name])
					columnsWithChanges.Add(column.Name);
			}
			else if (column.DbType == TfDatabaseColumnType.DateTime)
			{
				if ((DateTime?)row[column.Name] != (DateTime?)existingRow[column.Name])
					columnsWithChanges.Add(column.Name);
			}
			else if (column.DbType == TfDatabaseColumnType.ShortInteger)
			{
				if ((short?)row[column.Name] != (short?)existingRow[column.Name])
					columnsWithChanges.Add(column.Name);
			}
			else if (column.DbType == TfDatabaseColumnType.Integer)
			{
				if ((int?)row[column.Name] != (int?)existingRow[column.Name])
					columnsWithChanges.Add(column.Name);
			}
			else if (column.DbType == TfDatabaseColumnType.LongInteger)
			{
				if ((long?)row[column.Name] != (long?)existingRow[column.Name])
					columnsWithChanges.Add(column.Name);
			}
			else if (column.DbType == TfDatabaseColumnType.Number)
			{
				if ((decimal?)row[column.Name] != (decimal?)existingRow[column.Name])
					columnsWithChanges.Add(column.Name);
			}
			else if (column.DbType == TfDatabaseColumnType.Boolean)
			{
				if ((bool?)row[column.Name] != (bool?)existingRow[column.Name])
					columnsWithChanges.Add(column.Name);
			}
			else if (column.DbType == TfDatabaseColumnType.Text ||
				column.DbType == TfDatabaseColumnType.ShortText)
			{
				if ((string)row[column.Name] != (string)existingRow[column.Name])
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

		//we need the row after update because identity columns value is calculated in database
		var updatedRow = GetProviderRow(provider, (Guid)row["tf_id"]);

		foreach (var tableColumn in row.DataTable.Columns)
		{
			if (!tableColumn.IsShared)
				continue;

			//join columns will not be updated
			//this is not supported at the moment, 
			//but in case we support it at later stage
			if (tableColumn.IsJoinColumn)
				continue;

			var sharedColumn = provider.SharedColumns.Single(x => $"{x.DataIdentity}.{x.DbName}" == tableColumn.Name);

			var identity = provider.Identities.Single(x => x.DataIdentity == sharedColumn.DataIdentity);

			TfDatabaseColumnType dbType = sharedColumn.DbType;

			object value = row[tableColumn.Name];

			Guid sharedColumnId = sharedColumn.Id;

			var identityColumnName = $"tf_ide_{identity.DataIdentity}";
			if(identity.DataIdentity == TfConstants.TF_ROW_ID_DATA_IDENTITY)
				identityColumnName = "tf_row_id";

			string identityValue = (string)updatedRow[identityColumnName];

			UpsertSharedColumnValue(
				value,
				tableColumn.Name,
				sharedColumnId,
				identityValue,
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

			//ignore shared,identity and joined columns here
			if (tableColumn.IsIdentityColumn || tableColumn.IsShared || tableColumn.IsJoinColumn)
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

	public void DeleteDataProviderRowByTfId(
		TfDataProvider provider,
		Guid tfId)
	{
		try
		{
			if (provider is null)
				throw new TfException("Provider object is null");

			var count = _dbService.ExecuteSqlNonQueryCommand(
				$"DELETE FROM dp{provider.Index} WHERE tf_id = @tf_id",
				new NpgsqlParameter("@tf_id", tfId));

			if (count == 0)
				throw new TfException("Data row not found in provider table.");
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public void DeleteDataProviderRowsByTfId(
			Guid providerId,
			List<Guid> idList)
	{
		try
		{
			if (idList.Count == 0)
				return;
			var provider = GetDataProvider(providerId);
			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				foreach (var tfId in idList)
				{
					DeleteDataProviderRowByTfId(provider, tfId);
				}
				scope.Complete();
			}

		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
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

	private static DbType GetDbTypeForDatabaseColumnType(TfDatabaseColumnType dbType)
	{
		DbType resultType;
		switch (dbType)
		{
			case TfDatabaseColumnType.Guid:
				resultType = DbType.Guid; break;
			case TfDatabaseColumnType.Boolean:
				resultType = DbType.Boolean; break;
			case TfDatabaseColumnType.DateOnly:
				resultType = DbType.Date; break;
			case TfDatabaseColumnType.DateTime:
				resultType = DbType.DateTime2; break;
			case TfDatabaseColumnType.ShortText:
				resultType = DbType.StringFixedLength; break;
			case TfDatabaseColumnType.Text:
				resultType = DbType.String; break;
			case TfDatabaseColumnType.ShortInteger:
				resultType = DbType.Int16; break;
			case TfDatabaseColumnType.Integer:
				resultType = DbType.Int32; break;
			case TfDatabaseColumnType.LongInteger:
				resultType = DbType.Int64; break;
			case TfDatabaseColumnType.Number:
				resultType = DbType.Decimal; break;
			case TfDatabaseColumnType.AutoIncrement:
			default:
				throw new Exception("Not supported for conversion database column type.");
		}
		return resultType;
	}

	public TfDataProviderDataRow GetProviderRow(
		TfDataProvider provider,
		Guid id)
	{
		try
		{
			string whereClause = "WHERE tf_id = @id";

			string sql = BuildSelectRowSql(provider, whereClause);

			var dt = _dbService.ExecuteSqlQueryCommand(sql, new NpgsqlParameter("@id", id));

			if (dt.Rows.Count == 0)
			{
				return null;
			}

			TfDataProviderDataRow row = new TfDataProviderDataRow();
			foreach (DataColumn column in dt.Columns)
			{
				object value = dt.Rows[0][column.ColumnName];
				if (value == DBNull.Value)
				{
					value = null;
				}
				else
				{
					var providerColumn = provider.Columns.SingleOrDefault(x => x.DbName == column.ColumnName);
					if (providerColumn is not null && providerColumn.DbType == TfDatabaseColumnType.DateOnly)
					{
						value = DateOnly.FromDateTime((DateTime)value);
					}
				}

				row[column.ColumnName] = value;
			}
			return row;
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public TfDataProviderDataRow GetProviderRow(
		TfDataProvider provider,
		int index)
	{
		try
		{
			string whereClause = "WHERE tf_row_index = @index";

			string sql = BuildSelectRowSql(provider, whereClause);

			var dt = _dbService.ExecuteSqlQueryCommand(sql, new NpgsqlParameter("@index", index));

			if (dt.Rows.Count == 0)
			{
				return null;
			}

			TfDataProviderDataRow row = new TfDataProviderDataRow();
			foreach (DataColumn column in dt.Columns)
			{
				object value = dt.Rows[0][column.ColumnName];
				if (value == DBNull.Value)
					value = null;
				else
				{
					var providerColumn = provider.Columns.SingleOrDefault(x => x.DbName == column.ColumnName);
					if (providerColumn is not null && providerColumn.DbType == TfDatabaseColumnType.DateOnly)
					{
						value = DateOnly.FromDateTime((DateTime)value);
					}
				}

				row[column.ColumnName] = value;
			}
			return row;
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public void InsertNewProviderRow(
		TfDataProvider provider,
		TfDataProviderDataRow row)
	{
		try
		{
			row["tf_id"] = GetId(Guid.NewGuid());
			row["tf_created_on"] = DateTime.Now;
			row["tf_updated_on"] = DateTime.Now;

			//generate search
			var searchSb = new StringBuilder();
			foreach (var column in provider.Columns)
			{
				if (column.IncludeInTableSearch)
				{
					var index = row.ColumnNames.IndexOf(column.DbName);
					if (index > 0)
					{
						object value = row[column.DbName];
						if (value is not null)
							searchSb.Append($" {value}");
					}
				}
			}
			row["tf_search"] = searchSb.ToString();

			foreach (var joinKey in provider.JoinKeys)
			{
				List<string> keys = new List<string>();
				foreach (var column in joinKey.Columns)
					keys.Add(row[column.DbName]?.ToString());

				row[$"tf_jk_{joinKey.DbName}_id"] = GetId(keys.ToArray());
				row[$"tf_jk_{joinKey.DbName}_version"] = joinKey.Version;
			}

			List<NpgsqlParameter> parameters;
			var sql = BuildInsertNewRowSql(provider, row, out parameters);

			var count = _dbService.ExecuteSqlNonQueryCommand(sql, parameters);
			if (count != 1)
			{
				throw new Exception("Failed to insert new row");
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public void UpdateProviderRow(
		TfDataProvider provider,
		TfDataProviderDataRow row)
	{
		try
		{
			row["tf_updated_on"] = DateTime.Now;

			//generate search
			var searchSb = new StringBuilder();
			foreach (var column in provider.Columns)
			{
				if (column.IncludeInTableSearch)
				{
					var index = row.ColumnNames.IndexOf(column.DbName);
					if (index > 0)
					{
						object value = row[column.DbName];
						if (value is not null)
							searchSb.Append($" {value}");
					}
				}
			}
			row["tf_search"] = searchSb.ToString();

			foreach (var joinKey in provider.JoinKeys)
			{
				List<string> keys = new List<string>();
				foreach (var column in joinKey.Columns)
					keys.Add(row[column.DbName].ToString());

				row[$"tf_jk_{joinKey.DbName}_id"] = GetId(keys.ToArray());
				row[$"tf_jk_{joinKey.DbName}_version"] = joinKey.Version;
			}

			List<NpgsqlParameter> parameters;
			var sql = BuildUpdateRowSql(provider, row, out parameters);

			var count = _dbService.ExecuteSqlNonQueryCommand(sql, parameters);
			if (count != 1)
			{
				throw new Exception("Failed to update row");
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	//private void UpdateProviderRowJoinKeysOnly(
	//	TfDataProvider provider,
	//	Guid tfId,
	//	Dictionary<string, object> values)
	//{
	//	try
	//	{
	//		List<NpgsqlParameter> parameters;
	//		var sql = BuildUpdateRowJoinKeysOnlySql(provider, tfId, values, out parameters);

	//		var count = _dbService.ExecuteSqlNonQueryCommand(sql, parameters);
	//		if (count != 1)
	//		{
	//			throw new Exception("Failed to update row");
	//		}
	//	}
	//	catch (Exception ex)
	//	{
	//		throw ProcessException(ex);
	//	}
	//}

	public void DeleteProviderRowsAfterIndex(
		TfDataProvider provider,
		int index)
	{
		try
		{
			string sql = $"DELETE FROM dp{provider.Index} WHERE tf_row_index >= @index";
			_dbService.ExecuteSqlNonQueryCommand(sql, new NpgsqlParameter("@index", index));
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	private string BuildSelectRowSql(
		TfDataProvider provider,
		string whereClause)
	{
		StringBuilder sql = new StringBuilder();

		sql.AppendLine("SELECT");

		sql.AppendLine("tf_id");
		sql.AppendLine(",");
		sql.AppendLine("tf_row_index");
		sql.AppendLine(",");

		foreach (var identity in provider.Identities)
		{
			if (identity.DataIdentity == TfConstants.TF_ROW_ID_DATA_IDENTITY)
			{
				sql.AppendLine(TfConstants.TF_ROW_ID_DATA_IDENTITY);
				sql.AppendLine(",");
				continue;
			}
			sql.AppendLine($"tf_ide_{identity.DataIdentity}");
			sql.AppendLine(",");
		}

		foreach (var column in provider.Columns)
		{
			sql.AppendLine(column.DbName);
			sql.AppendLine(",");
		}

		sql.AppendLine("tf_created_on");
		sql.AppendLine(",");
		sql.AppendLine("tf_updated_on");
		sql.AppendLine(",");
		sql.AppendLine("tf_search");
		sql.AppendLine();

		sql.AppendLine($"FROM dp{provider.Index}");

		sql.AppendLine();
		sql.AppendLine(whereClause);
		return sql.ToString();
	}

	private string BuildInsertNewRowSql(
		TfDataProvider provider,
		TfDataProviderDataRow row,
		out List<NpgsqlParameter> parameters)
	{
		parameters = new List<NpgsqlParameter>();
		StringBuilder sql = new StringBuilder();

		sql.AppendLine($"INSERT INTO dp{provider.Index} ( ");

		sql.AppendLine("tf_id");
		sql.AppendLine(",");
		sql.AppendLine("tf_row_index");
		sql.AppendLine(",");

		//note we do not set value for identity columns and tf_row_id

		foreach (var column in provider.Columns)
		{
			sql.AppendLine(column.DbName);
			sql.AppendLine(",");
		}

		sql.AppendLine("tf_created_on");
		sql.AppendLine(",");
		sql.AppendLine("tf_updated_on");
		sql.AppendLine(",");
		sql.AppendLine("tf_search");
		sql.AppendLine();

		sql.AppendLine(" ) VALUES ( ");

		parameters.Add(new NpgsqlParameter("@tf_id", row["tf_id"]));
		sql.AppendLine("@tf_id");
		sql.AppendLine(",");
		parameters.Add(new NpgsqlParameter("@tf_row_index", row["tf_row_index"]));
		sql.AppendLine("@tf_row_index");
		sql.AppendLine(",");

		foreach (var joinKey in provider.JoinKeys)
		{
			var joinKeyName = $"tf_jk_{joinKey.DbName}_id";
			parameters.Add(new NpgsqlParameter($"@{joinKeyName}", row[joinKeyName]));
			sql.AppendLine("@" + joinKeyName);
			sql.AppendLine(",");
		}

		foreach (var column in provider.Columns)
		{
			var parameterType = GetDbTypeForDatabaseColumnType(column.DbType);

			NpgsqlParameter parameter = new NpgsqlParameter($"@{column.DbName}", parameterType);

			if (row[column.DbName] is null)
				parameter.Value = DBNull.Value;
			else
				parameter.Value = row[column.DbName];

			parameters.Add(parameter);

			sql.AppendLine("@" + column.DbName);
			sql.AppendLine(",");
		}

		parameters.Add(new NpgsqlParameter("@tf_created_on", row["tf_created_on"]));
		sql.AppendLine("@tf_created_on");
		sql.AppendLine(",");
		parameters.Add(new NpgsqlParameter("@tf_updated_on", row["tf_updated_on"]));
		sql.AppendLine("@tf_updated_on");
		sql.AppendLine(",");
		parameters.Add(new NpgsqlParameter("@tf_search", row["tf_search"]));
		sql.AppendLine("@tf_search");
		sql.AppendLine();
		sql.AppendLine(" ) ");

		sql.AppendLine();
		return sql.ToString();
	}

	private string BuildUpdateRowSql(
		TfDataProvider provider,
		TfDataProviderDataRow row,
		out List<NpgsqlParameter> parameters)
	{
		parameters = new List<NpgsqlParameter>();
		StringBuilder sql = new StringBuilder();

		sql.AppendLine($"UPDATE dp{provider.Index} SET ");

		parameters.Add(new NpgsqlParameter("@tf_row_index", row["tf_row_index"]));
		sql.Append("tf_row_index = @tf_row_index");
		sql.AppendLine(",");

		//note we do not set value for identity columns and tf_row_id

		foreach (var column in provider.Columns)
		{
			var parameterType = GetDbTypeForDatabaseColumnType(column.DbType);

			NpgsqlParameter parameter = new NpgsqlParameter($"@{column.DbName}", parameterType);

			if (row[column.DbName] is null)
				parameter.Value = DBNull.Value;
			else
				parameter.Value = row[column.DbName];

			parameters.Add(parameter);

			sql.Append($"{column.DbName}=@{column.DbName}");
			sql.AppendLine(",");
		}

		parameters.Add(new NpgsqlParameter("@tf_updated_on", row["tf_updated_on"]));
		sql.Append("tf_updated_on = @tf_updated_on");
		sql.AppendLine(",");
		parameters.Add(new NpgsqlParameter("@tf_search", row["tf_search"]));
		sql.Append("tf_search = @tf_search");
		sql.AppendLine();

		parameters.Add(new NpgsqlParameter("@tf_id", row["tf_id"]));
		sql.AppendLine("WHERE tf_id = @tf_id ");

		sql.AppendLine();
		return sql.ToString();
	}

	//private string BuildUpdateRowJoinKeysOnlySql(
	//	TfDataProvider provider,
	//	Guid tfId,
	//	Dictionary<string, object> values,
	//	out List<NpgsqlParameter> parameters)
	//{
	//	parameters = new List<NpgsqlParameter>();
	//	StringBuilder sql = new StringBuilder();

	//	sql.AppendLine($"UPDATE dp{provider.Index} SET ");

	//	foreach (var joinKey in provider.JoinKeys)
	//	{
	//		var joinKeyName = $"tf_jk_{joinKey.DbName}_id";
	//		parameters.Add(new NpgsqlParameter($"@{joinKeyName}", (Guid)values[joinKeyName]));
	//		sql.Append($"{joinKeyName} = @{joinKeyName}");
	//		sql.AppendLine(",");

	//		var joinKeyVersion = $"tf_jk_{joinKey.DbName}_version";
	//		parameters.Add(new NpgsqlParameter($"@{joinKeyVersion}", (short)values[joinKeyVersion]));
	//		sql.Append($"{joinKeyVersion} = @{joinKeyVersion}");
	//		sql.AppendLine(",");
	//	}

	//	sql.AppendLine("tf_id = @tf_id");
	//	parameters.Add(new NpgsqlParameter("@tf_id", tfId));
	//	sql.AppendLine("WHERE tf_id = @tf_id ");

	//	sql.AppendLine();
	//	return sql.ToString();
	//}

	public void UpdateValue(
		TfDataProvider provider,
		Guid rowId,
		string dbName,
		object value)
	{
		try
		{
			var row = GetProviderRow(provider, rowId);
			row[dbName] = value;
			UpdateProviderRow(provider, row);
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public void DeleteAllProviderRows(
		Guid providerId)
	{
		try
		{
			var provider = GetDataProvider(providerId);
			if (provider is null)
				throw new Exception("Provider not found");
			DeleteAllProviderRows(provider);
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public void DeleteAllProviderRows(
		TfDataProvider provider)
	{
		try
		{
			_dbService.ExecuteSqlNonQueryCommand($"DELETE FROM dp{provider.Index}");
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public void DeleteSharedColumnData(
		TfSharedColumn sharedColumn)
	{
		try
		{
			string tableName = GetSharedColumnValueTableNameByType(sharedColumn.DbType);

			string sql = $"DELETE FROM {tableName} WHERE shared_column_id = @shared_column_id";

			_dbService.ExecuteSqlNonQueryCommand(sql, new NpgsqlParameter("@shared_column_id", sharedColumn.Id));
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	private static string GetSharedColumnValueTableNameByType(TfDatabaseColumnType dbColumnType)
	{
		switch (dbColumnType)
		{
			case TfDatabaseColumnType.ShortText:
				return "tf_shared_column_short_text_value";
			case TfDatabaseColumnType.Text:
				return "tf_shared_column_text_value";
			case TfDatabaseColumnType.Boolean:
				return "tf_shared_column_boolean_value";
			case TfDatabaseColumnType.Guid:
				return "tf_shared_column_guid_value";
			case TfDatabaseColumnType.ShortInteger:
				return "tf_shared_column_short_integer_value";
			case TfDatabaseColumnType.Integer:
				return "tf_shared_column_integer_value";
			case TfDatabaseColumnType.LongInteger:
				return "tf_shared_column_long_integer_value";
			case TfDatabaseColumnType.Number:
				return "tf_shared_column_number_value";
			case TfDatabaseColumnType.DateOnly:
				return "tf_shared_column_date_value";
			case TfDatabaseColumnType.DateTime:
				return "tf_shared_column_datetime_value";
			default:
				throw new Exception("Not supported column type.");
		}

	}

	public Dictionary<Guid, string> GetDataIdentityValuesForRowIds(
		TfDataProvider provider,
		TfDataIdentity dataIdentity,
		List<Guid> rowIds)
	{
		if (provider is null)
			throw new Exception("Provider object is null");

		if (dataIdentity is null)
			throw new Exception("Data identity object is null");

		if (!provider.Identities.Any(x => x.DataIdentity == dataIdentity.DataIdentity))
			throw new Exception("Data identity not found in provider.");

		var result = new Dictionary<Guid, string>();

		if (rowIds is null || rowIds.Count == 0)
			return result;

		var identityColumnName = dataIdentity.DataIdentity == TfConstants.TF_ROW_ID_DATA_IDENTITY ?
			TfConstants.TF_ROW_ID_DATA_IDENTITY : $"tf_ide_{dataIdentity.DataIdentity}";

		var sql = $"SELECT tf_id, {identityColumnName} FROM dp{provider.Index} WHERE tf_id = ANY(@ids)";

		var idsParameter = new NpgsqlParameter("@ids", NpgsqlDbType.Array | NpgsqlDbType.Uuid);
		idsParameter.Value = rowIds.ToArray();

		DataTable dataTable = _dbService.ExecuteSqlQueryCommand(sql, idsParameter);

		foreach (DataRow row in dataTable.Rows)
		{
			Guid id = (Guid)row["tf_id"];
			string value = (string)row[identityColumnName];
			result.Add(id, value);
		}

		return result;
	}
}
