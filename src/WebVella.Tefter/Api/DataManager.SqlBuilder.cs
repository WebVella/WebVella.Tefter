namespace WebVella.Tefter;

public partial class DataManager
{
	class SqlBuilder
	{
		private string _tableName;
		private short _tableAliasCounter = 1;
		private string _tableAlias = "t1";

		private List<SqlBuilderColumn> _columns = new();
		private List<TfFilterBase> _filters = null;
		private List<TfFilterBase> _additionalFilters = null;
		private List<TfSort> _sortOrders = null;
		private string _search = null;
		private int? _page = null;
		private int? _pageSize = null;


		public SqlBuilder(
			string tableName,
			List<TfFilterBase> filters = null,
			List<TfFilterBase> additionalFilters = null,
			List<TfSort> sortOrders = null,
			string search = null,
			int? page = null,
			int? pageSize = null)
		{
			if (string.IsNullOrWhiteSpace(tableName))
				throw new ArgumentNullException(nameof(tableName));

			_tableName = tableName;
			_filters = filters;
			_additionalFilters = additionalFilters;
			_sortOrders = sortOrders;
			_search = search;
			_page = page;
			_pageSize = pageSize;
		}

		public void AddColumn(
			Guid id,
			string dbName,
			DatabaseColumnType dbType,
			string sharedKeyDbName = null)
		{
			if (sharedKeyDbName == null)
			{
				_columns.Add(new SqlBuilderColumn
				{
					Id = id,
					TableName = _tableName,
					TableAlias = _tableAlias,
					DbName = dbName,
					DbType = dbType,
					SharedKeyDbName = sharedKeyDbName
				});
			}
			else
			{
				_tableAliasCounter++;

				_columns.Add(new SqlBuilderColumn
				{
					Id = id,
					TableName = GetSharedColumnValueTableNameByType(dbType),
					TableAlias = $"t{_tableAliasCounter}",
					DbName = dbName,
					DbType = dbType,
					SharedKeyDbName = sharedKeyDbName
				});
			}
		}

		public void CalculatePaging(IDatabaseService dbService)
		{
			if (_pageSize.HasValue && _page.HasValue && _page.Value < 0)
			{
				StringBuilder sb = new StringBuilder();

				sb.AppendLine($"SELECT COUNT(*) FROM {_tableName} {_tableAlias}");

				var (filterSql, parameters) = GenerateFiltersAndSearchSql();
				if (!string.IsNullOrWhiteSpace(filterSql.Trim()))
					sb.Append($"{Environment.NewLine}{filterSql}");

				var dtCount = dbService.ExecuteSqlQueryCommand(sb.ToString(), parameters);
				var rowsCount = (long)dtCount.Rows[0][0];

				_page = 1;
				if (rowsCount > 0)
				{
					_page = (int)(rowsCount / _pageSize.Value) + 1;

					if (rowsCount % _pageSize.Value == 0)
						_page--;
				}
			}
		}

		public (string, List<NpgsqlParameter>, int?, int?) Build()
		{
			string columns = string.Join($",{Environment.NewLine}\t",
				_columns.Select(x => x.GetSelectString()).ToList());

			StringBuilder sb = new StringBuilder();
			sb.Append($"SELECT {columns} {Environment.NewLine}FROM {_tableName} {_tableAlias}");

			string joins = string.Join(Environment.NewLine, _columns
					.Where(x => x.SharedKeyDbName != null)
					.Select(x => $"	LEFT OUTER JOIN {x.TableName} {x.TableAlias} ON " +
					$"{x.TableAlias}.shared_key_id = {_tableAlias}.tf_sk_{x.SharedKeyDbName}_id AND " +
					$"{x.TableAlias}.shared_column_id = '{x.Id}'").ToList());

			if (!string.IsNullOrEmpty(joins.Trim()))
				sb.Append($"{Environment.NewLine}{joins}");

			var (filterSql, parameters) = GenerateFiltersAndSearchSql();
			if (!string.IsNullOrWhiteSpace(filterSql.Trim()))
				sb.Append($"{Environment.NewLine}{filterSql}");

			var sortSql = GenerateSortSql();
			if (!string.IsNullOrWhiteSpace(sortSql.Trim()))
				sb.Append($"{Environment.NewLine}{sortSql}");

			if (_page.HasValue && _pageSize.HasValue)
			{
				sb.Append(Environment.NewLine);
				sb.Append($"OFFSET {(_page.Value - 1) * _pageSize.Value} LIMIT {_pageSize.Value}");
			}

			return (sb.ToString(), parameters, _page, _pageSize);
		}

		private string GenerateSortSql()
		{
			StringBuilder sb = new StringBuilder();

			//sort implementation
			if (_sortOrders != null && _sortOrders.Any())
			{
				StringBuilder sortSb = new StringBuilder();
				sortSb.Append("ORDER BY ");
				bool first = true;
				foreach (var sort in _sortOrders)
				{
					string comma = first ? " " : ", ";
					string direction = sort.Direction == TfSortDirection.ASC ? "ASC" : "DESC";
					var column = _columns.SingleOrDefault(x => x.DbName == sort.DbName);
					var aliasAndDbName = column == null ? $"{column.TableAlias}.{column.DbName}" : $"{_tableAlias}.{column.DbName}";
					sortSb.Append($"{comma}{aliasAndDbName} {direction}");
					first = false;
				}
				sb.Append(Environment.NewLine);
				sb.Append(sortSb.ToString());
			}

			return sb.ToString();
		}

		private (string, List<NpgsqlParameter>) GenerateFiltersAndSearchSql()
		{
			List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();

			StringBuilder sb = new StringBuilder();

			//TODO filters
			if (!string.IsNullOrWhiteSpace(_search))
			{
				parameters.Add(new NpgsqlParameter("@tf_search", _search));
				sb.AppendLine($"WHERE {_tableAlias}.tf_search ILIKE CONCAT ('%', @tf_search , '%') ");
			}

			return (sb.ToString(), parameters);
		}
	}

	record SqlBuilderColumn
	{
		public Guid Id { get; set; }
		public string DbName { get; set; }
		public string SharedKeyDbName { get; set; }
		public DatabaseColumnType DbType { get; set; }
		public string TableName { get; set; }
		public string TableAlias { get; set; }

		public string GetSelectString()
		{
			if (string.IsNullOrWhiteSpace(SharedKeyDbName))
				return $"{TableAlias}.{DbName}";
			else
				return $"{TableAlias}.value AS {DbName}";
		}
	}
}
