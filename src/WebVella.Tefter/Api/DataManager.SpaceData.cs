//using WebVella.Tefter.Models;

//namespace WebVella.Tefter;

//public partial interface IDataManager
//{
//	internal Result<TfDataTable> QuerySpaceData(
//		TfSpaceData spaceData,
//		List<TfFilterBase> additionalFilters = null,
//		List<TfSort> sortOverrides = null,
//		string search = null,
//		int? page = null,
//		int? pageSize = null);
//}

//public partial class DataManager
//{
//	public Result<TfDataTable> QuerySpaceData(
//		TfSpaceData spaceData,
//		List<TfFilterBase> additionalFilters = null,
//		List<TfSort> sortOverrides = null,
//		string search = null,
//		int? page = null,
//		int? pageSize = null)
//	{
//		try
//		{
//			var resultPage = page;
//			var resultPageSize = pageSize;

//			List<NpgsqlParameter> parameters;

//			if (page.HasValue && page.Value < 0)
//			{
//				string countSql = BuildSelectCountRowsSql(
//					provider,
//					search,
//					out parameters);

//				var dtCount = _dbService.ExecuteSqlQueryCommand(countSql, parameters);
//				var rowsCount = (long)dtCount.Rows[0][0];

//				resultPage = 1;
//				resultPageSize = pageSize.HasValue ? pageSize.Value : TfDataTableQuery.DEFAULT_PAGE_SIZE;

//				if (rowsCount > 0)
//				{
//					page = (int)(rowsCount / resultPageSize) + 1;

//					if (rowsCount % resultPageSize == 0)
//						page--;

//					resultPage = page;
//				}
//			}

//			parameters = null;
//			string sql = BuildSelectRowsSql(
//				provider,
//				search,
//				resultPage,
//				resultPageSize,
//				out parameters);

//			var dt = _dbService.ExecuteSqlQueryCommand(sql, parameters);

//			TfDataTableQuery query = new TfDataTableQuery();
//			query.Search = search;
//			query.Page = resultPage;
//			query.PageSize = resultPageSize;
//			query.DataProviderId = provider.Id;
//			query.ExcludeSharedColumns = true;

//			TfDataTable resultTable = new TfDataTable(provider, query);

//			if (dt.Rows.Count == 0)
//				return Result.Ok(resultTable);

//			HashSet<string> dateOnlyColumns = provider.Columns
//				.Where(x=>x.DbType == DatabaseColumnType.Date)
//				.Select(x=>x.DbName)
//				.ToHashSet();

//			foreach (DataRow row in dt.Rows)
//			{
//				object[] values = new object[resultTable.Columns.Count];

//				int valuesCounter = 0;
//				foreach(var column in resultTable.Columns)
//				{
//					object value = row[column.Name];
//					if (value == DBNull.Value)
//						value = null;
//					else
//						if (dateOnlyColumns.Contains(column.Name))
//							value = DateOnly.FromDateTime((DateTime)value);

//					values[valuesCounter++] = value;
//				}
//				resultTable.Rows.Add(new TfDataRow(resultTable,values));
//			}

//			return Result.Ok(resultTable);
//		}
//		catch (Exception ex)
//		{
//			return Result.Fail(new Error("Failed to get data provider rows").CausedBy(ex));
//		}
//	}


//	private string BuildSelectRowsSql(
//		TfDataProvider provider,
//		string search,
//		int? page,
//		int? pageSize,
//		out List<NpgsqlParameter> parameters)
//	{
//		parameters = new List<NpgsqlParameter>();
//		StringBuilder sql = new StringBuilder();

//		sql.AppendLine($"SELECT * FROM dp{provider.Index} ");

//		if (!string.IsNullOrWhiteSpace(search))
//		{
//			parameters.Add(new NpgsqlParameter("@tf_search", search));
//			sql.AppendLine($" WHERE tf_search ILIKE CONCAT ('%', @tf_search , '%') ");
//		}

//		if (page.HasValue || pageSize.HasValue)
//		{
//			if (page == null && pageSize.HasValue)
//				page = TfDataTableQuery.DEFAULT_PAGE;
//			if (page.HasValue && pageSize == null)
//				pageSize = TfDataTableQuery.DEFAULT_PAGE_SIZE;

//			int offset = (page.Value - 1) * pageSize.Value;
//			int limit = pageSize.Value;
//			sql.AppendLine($"OFFSET {offset} LIMIT {limit}");
//		}

//		return sql.ToString();
//	}

//	private string BuildSelectCountRowsSql(
//		TfDataProvider provider,
//		string search,
//		out List<NpgsqlParameter> parameters)
//	{
//		parameters = new List<NpgsqlParameter>();
//		StringBuilder sql = new StringBuilder();

//		sql.AppendLine($"SELECT COUNT(*) FROM dp{provider.Index}");
//		if (!string.IsNullOrWhiteSpace(search))
//		{
//			parameters.Add(new NpgsqlParameter("@tf_search", search));
//			sql.AppendLine($"WHERE tf_search ILIKE CONCAT ('%', @tf_search, '%') ");
//			sql.AppendLine();
//		}
//		return sql.ToString();
//	}
//}
