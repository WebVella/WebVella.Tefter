namespace WebVella.Tefter;

public partial interface IDataManager
{
	internal Result<TfDataTable> QueryDataProvider(
		TfDataProvider provider,
		string search = null,
		int? page = null,
		int? pageSize = null,
		bool noRows = false);

	internal Result<TfDataTable> QuerySpaceData(
		Guid spaceDataId,
		List<TfFilterBase> additionalFilters = null,
		List<TfSort> sortOverrides = null,
		string search = null,
		int? page = null,
		int? pageSize = null,
		bool noRows = false);

	internal Result<TfDataTable> SaveDataTable(
		TfDataTable table);
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
			if (!noRows)
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
					ExcludeSharedColumns = false
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
			if (!noRows)
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
					ExcludeSharedColumns = false
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
		return Result.Fail(new ValidationError(string.Empty,
				"Not implemented yet"));
	}
}
