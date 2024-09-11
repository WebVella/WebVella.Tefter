using Microsoft.FluentUI.AspNetCore.Components;

namespace WebVella.Tefter;

public partial interface IDataManager
{
	internal Result<TfDataTable> QuerySpaceData(
		Guid spaceDataId,
		List<TfFilterBase> additionalFilters = null,
		List<TfSort> sortOverrides = null,
		string search = null,
		int? page = null,
		int? pageSize = null);
}

public partial class DataManager
{
	public Result<TfDataTable> QuerySpaceData(
		Guid spaceDataId,
		List<TfFilterBase> additionalFilters = null,
		List<TfSort> sortOverrides = null,
		string search = null,
		int? page = null,
		int? pageSize = null)
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
			 	tableName: $"dp{provider.Index}",
				filters: spaceData.Filters,
				additionalFilters: additionalFilters,
				sortOrders: sortOverrides ?? spaceData.SortOrders,
				search: search,
				page: page,
				pageSize: pageSize);

			HashSet<string> addedColumns = new HashSet<string>();

			foreach (var systemColumn in provider.SystemColumns)
			{
				sqlBuilder.AddColumn(Guid.Empty, systemColumn.DbName, systemColumn.DbType);
				addedColumns.Add(systemColumn.DbName);
				continue;
			}

			foreach (var columnName in spaceData.Columns)
			{
				if (addedColumns.Contains(columnName))
					continue;
				
				addedColumns.Add(columnName);

				var column = provider.Columns.SingleOrDefault(x => x.DbName == columnName);
				if (column != null)
				{
					sqlBuilder.AddColumn(column.Id, column.DbName, column.DbType);
					addedColumns.Add(columnName);
					continue;
				}

				var sharedColumn = provider.SharedColumns.SingleOrDefault(x => x.DbName == columnName);
				if (sharedColumn != null)
				{
					sqlBuilder.AddColumn(sharedColumn.Id, sharedColumn.DbName, sharedColumn.DbType, sharedColumn.SharedKeyDbName);
					addedColumns.Add(columnName);
					continue;
				}

				//ignore missing columns
			}

			//used to calculate if page is negative and need to count data
			sqlBuilder.CalculatePaging(_dbService);

			var (sql, parameters, usedPage, usedPageSize) = sqlBuilder.Build();

			var dataTable = _dbService.ExecuteSqlQueryCommand(sql, parameters);

			return Result.Ok(ProcessSqlResult(
				provider,
				new TfDataTableQuery
				{
					Search = search,
					Page = usedPage,
					PageSize = usedPageSize,
					DataProviderId = provider.Id,
					ExcludeSharedColumns = false
				},
				dataTable,
				addedColumns.ToList()
			));
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to get data provider rows").CausedBy(ex));
		}
	}

	private TfDataTable ProcessSqlResult(
		TfDataProvider provider,
		TfDataTableQuery query,
		DataTable dataTable,
		List<string> onlyColumns )
	{
		TfDataTable resultTable = new TfDataTable(provider, query, onlyColumns);

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



}
