using FluentResults;
using NpgsqlTypes;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;

namespace WebVella.Tefter;

public partial class DataManager
{
	class SqlBuilder
	{
		private string _tableName;
		private short _tableAliasCounter = 1;
		private string _tableAlias = "t1";

		private IDatabaseService _dbService = null;
		private List<SqlBuilderColumn> _availableColumns = new();
		private List<SqlBuilderColumn> _selectColumns = new();
		private List<SqlBuilderColumn> _filterColumns = new();
		private List<SqlBuilderColumn> _sortColumns = new();

		private TfFilterAnd _mainFilter;
		private List<TfFilterBase> _filters = new();
		private List<TfSort> _sorts = null;
		private List<TfFilterBase> _userFilters = new();
		private List<TfSort> _userSorts = null;
		private List<TfFilterBase> _presetFilters = new();
		private List<TfSort> _presetSorts = null;
		private string _search = null;
		private int? _page = null;
		private int? _pageSize = null;
		private List<Guid> _tfIds = null;

		public SqlBuilder(
			IDatabaseService dbService,
			TfDataProvider dataProvider,
			TfSpaceData spaceData = null,
			List<TfFilterBase> userFilters = null,
			List<TfSort> userSorts = null,
			List<TfFilterBase> presetFilters = null,
			List<TfSort> presetSorts = null,
			string search = null,
			int? page = null,
			int? pageSize = null)
		{
			if (dbService is null)
				throw new ArgumentNullException(nameof(dbService));

			if (dataProvider is null)
				throw new ArgumentNullException(nameof(dataProvider));

			if (spaceData is not null && spaceData.Filters is not null)
				_filters = spaceData.Filters;

			if (userFilters is not null)
				_userFilters = userFilters;

			_userSorts = userSorts;

			if (presetFilters is not null)
				_presetFilters = presetFilters;

			_presetSorts = presetSorts;

			_search = search;

			_page = page;

			_pageSize = pageSize;

			_dbService = dbService;

			_tableName = $"dp{dataProvider.Index}";

			InitColumns(dataProvider, spaceData);

			if (spaceData is not null && spaceData.Filters is not null)
				_filters = spaceData.Filters;
		}

		public SqlBuilder(
			IDatabaseService dbService,
			TfDataProvider dataProvider,
			TfSpaceData spaceData,
			List<Guid> tfIds
			)
		{
			if (dbService is null)
				throw new ArgumentNullException(nameof(dbService));

			if (dataProvider is null)
				throw new ArgumentNullException(nameof(dataProvider));

			if (tfIds is null)
				throw new ArgumentNullException(nameof(tfIds));

			_tfIds = tfIds;

			_filters = new List<TfFilterBase>();

			_userFilters = new List<TfFilterBase>();

			_userSorts = new List<TfSort>();

			_presetFilters = new List<TfFilterBase>();

			_presetSorts = new List<TfSort>();

			_search = null;

			_page = null;

			_pageSize = null;

			_dbService = dbService;

			_tableName = $"dp{dataProvider.Index}";

			InitColumns(dataProvider, spaceData);
		}

		private void InitColumns(
			TfDataProvider dataProvider,
			TfSpaceData spaceData)
		{
			foreach (var column in dataProvider.SystemColumns)
				AddAvailableColumn(Guid.Empty, column.DbName, column.DbType, isSystem: true);

			foreach (var column in dataProvider.Columns)
				AddAvailableColumn(column.Id, column.DbName, column.DbType);

			foreach (var column in dataProvider.SharedColumns)
				AddAvailableColumn(column.Id, column.DbName, column.DbType, column.SharedKeyDbName);


			if (spaceData is null)
			{
				_selectColumns = _availableColumns.ToList();

				if (_userSorts is not null && _userSorts.Count >= 0)
				{
					foreach (var sortOrder in _userSorts)
					{
						var column = _availableColumns.FirstOrDefault(x => x.DbName == sortOrder.DbName);
						if (column is not null)
							_sortColumns.Add(column);

						//ignore missing columns
					}
				}
			}
			else
			{
				foreach (var column in _availableColumns.Where(x => x.IsSystem))
					_selectColumns.Add(column);

				bool spaceDataHasAtLeastOneValidColumn = false;
				if (spaceData.Columns.Any())
				{
					foreach (var columnName in spaceData.Columns.Distinct())
					{
						if (_selectColumns.Any(x => x.DbName == columnName))
						{
							spaceDataHasAtLeastOneValidColumn = true;
							continue;
						}

						var column = _availableColumns.SingleOrDefault(x => x.DbName == columnName);
						if (column is not null)
						{
							spaceDataHasAtLeastOneValidColumn = true;
							_selectColumns.Add(column);
						}

						//ignore missing columns
					}
				}

				//add all provider columns if there is no valid column in space data
				if (!spaceDataHasAtLeastOneValidColumn)
				{
					_selectColumns = _availableColumns.ToList();
				}

				//order of sort apply:
				//1. if user specify own sort 
				//2. if preset sort is specified
				//3. if space data sort is specified
				if (_userSorts is not null && _userSorts.Count > 0)
				{
					_sorts = _userSorts.ToList();
					foreach (var sortOrder in _userSorts)
					{
						var column = _availableColumns.FirstOrDefault(x => x.DbName == sortOrder.DbName);
						if (column is not null)
							_sortColumns.Add(column);
					}
				}
				else if (_presetSorts is not null && _presetSorts.Count > 0)
				{
					_sorts = _presetSorts.ToList();
					foreach (var sortOrder in _presetSorts)
					{
						var column = _availableColumns.FirstOrDefault(x => x.DbName == sortOrder.DbName);
						if (column is not null)
							_sortColumns.Add(column);
					}
				}
				else if (spaceData.SortOrders is not null && spaceData.SortOrders.Any())
				{
					_sorts = spaceData.SortOrders.ToList();
					foreach (var sortOrder in spaceData.SortOrders)
					{
						var column = _availableColumns.FirstOrDefault(x => x.DbName == sortOrder.DbName);
						if (column is not null)
							_sortColumns.Add(column);
					}
				}
			}

			//extract filter columns used later for validation if column exists and its alias			
			var spaceDataFilter = new TfFilterAnd(_filters.ToArray());
			var additionalFilter = new TfFilterAnd(_userFilters.ToArray());
			var presetFilter = new TfFilterAnd(_presetFilters.ToArray());
			_mainFilter = new TfFilterAnd(new[] { spaceDataFilter, additionalFilter, presetFilter });
			ExtractColumnsFromFilter(_mainFilter);
		}

		private void AddAvailableColumn(
			Guid id,
			string dbName,
			DatabaseColumnType dbType,
			string sharedKeyDbName = null,
			bool isSystem = false)
		{
			if (sharedKeyDbName == null)
			{
				_availableColumns.Add(new SqlBuilderColumn
				{
					Id = id,
					TableName = _tableName,
					TableAlias = _tableAlias,
					DbName = dbName,
					DbType = dbType,
					SharedKeyDbName = sharedKeyDbName,
					IsSystem = isSystem
				});
			}
			else
			{
				_tableAliasCounter++;

				_availableColumns.Add(new SqlBuilderColumn
				{
					Id = id,
					TableName = GetSharedColumnValueTableNameByType(dbType),
					TableAlias = $"t{_tableAliasCounter}",
					DbName = dbName,
					DbType = dbType,
					SharedKeyDbName = sharedKeyDbName,
					IsSystem = isSystem
				});
			}
		}

		private void ExtractColumnsFromFilter(
			TfFilterBase filter)
		{
			if (_filterColumns is null)
				_filterColumns = new List<SqlBuilderColumn>();

			if (filter is null)
				return;

			var childFilters = new List<TfFilterBase>().AsReadOnly();

			if (filter is TfFilterAnd && (((TfFilterAnd)filter).Filters) != null)
				childFilters = (((TfFilterAnd)filter).Filters);

			if (filter is TfFilterOr && ((((TfFilterOr)filter).Filters) != null))
				childFilters = (((TfFilterOr)filter).Filters);


			if (childFilters.Any())
			{
				foreach (var childFilter in childFilters)
				{
					if (childFilter == null)
						continue;

					ExtractColumnsFromFilter(childFilter);
				}
			}

			if (string.IsNullOrWhiteSpace(filter.ColumnName))
				return;

			var filterColumn = _filterColumns.FirstOrDefault(x => x.DbName == filter.ColumnName);
			if (filterColumn is null)
			{
				var availableColumn = _availableColumns.FirstOrDefault(x => x.DbName == filter.ColumnName);
				if (availableColumn is not null)
					_filterColumns.Add(availableColumn);
			}
		}

		private void CalculatePaging()
		{
			if (_pageSize.HasValue && _page.HasValue && _page.Value < 0)
			{
				StringBuilder sb = new StringBuilder();

				sb.AppendLine($"SELECT COUNT(*) FROM {_tableName} {_tableAlias}");

				var (filterSql, parameters) = GenerateFiltersAndSearchSql();
				if (!string.IsNullOrWhiteSpace(filterSql.Trim()))
					sb.Append($"{Environment.NewLine}{filterSql}");

				var dtCount = _dbService.ExecuteSqlQueryCommand(sb.ToString(), parameters);
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
			CalculatePaging();

			string columns = string.Join($",{Environment.NewLine}\t",
				_selectColumns.Select(x => x.GetSelectString()).ToList());

			StringBuilder sb = new StringBuilder();
			sb.Append($"SELECT {columns} {Environment.NewLine}FROM {_tableName} {_tableAlias}");

			//joins are created for select columns, filter columns and sort columns
			var columnsToJoin = _selectColumns
					.Where(x => x.SharedKeyDbName != null)
					.Union(_filterColumns
						.Where(x => x.SharedKeyDbName != null)
					)
					.Union(_sortColumns
						.Where(x => x.SharedKeyDbName != null)
					)
					.Distinct().ToList();

			string joins = string.Join(Environment.NewLine, columnsToJoin
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
			if (_sorts != null && _sorts.Any())
			{
				StringBuilder sortSb = new StringBuilder();

				bool first = true;
				foreach (var sort in _sorts)
				{
					var column = _availableColumns.SingleOrDefault(x => x.DbName == sort.DbName);

					//ignore columns not found in data provider
					if (column is null)
						continue;

					string comma = first ? " " : ", ";
					string direction = sort.Direction == TfSortDirection.ASC ? "ASC" : "DESC";
					if (string.IsNullOrWhiteSpace(column.SharedKeyDbName))
						sortSb.Append($"{comma}{column.TableAlias}.{column.DbName} {direction}");
					else
						sortSb.Append($"{comma}{column.TableAlias}.value {direction}");
					first = false;
				}
				sb.Append(sortSb.ToString());
			}

			if (sb.Length == 0)
				return string.Empty;

			return "ORDER BY " + sb.ToString();
		}

		private (string, List<NpgsqlParameter>) GenerateFiltersAndSearchSql()
		{
			List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();

			StringBuilder sb = new StringBuilder();

			if (_tfIds is not null)
			{
				var arrayParam = new NpgsqlParameter("@ids", NpgsqlDbType.Array | NpgsqlDbType.Uuid);
				arrayParam.Value = _tfIds.ToArray();
				parameters.Add(arrayParam);
				sb.Append(Environment.NewLine);
				sb.Append($"\t{_tableAlias}.tf_id = ANY(@ids)");
			}
			else
			{
				if (!string.IsNullOrWhiteSpace(_search?.Trim()))
				{
					parameters.Add(new NpgsqlParameter("@tf_search", _search?.Trim()));
					sb.Append(Environment.NewLine);
					sb.Append($"\t( {_tableAlias}.tf_search ILIKE CONCAT ('%', @tf_search , '%') )");
				}


				var filterSql = GenerateFiltersSql(_mainFilter, parameters);
				if (!string.IsNullOrWhiteSpace(filterSql))
				{
					if (sb.Length > 0)
						sb.Append($" AND {Environment.NewLine}");

					sb.Append($"\t{filterSql} ");
				}
			}

			if (sb.Length == 0)
				return (string.Empty, parameters);

			return ("WHERE " + sb.ToString(), parameters);
		}

		private string GenerateFiltersSql(TfFilterBase filter, List<NpgsqlParameter> parameters)
		{
			StringBuilder sb = new StringBuilder();

			if (filter is TfFilterAnd)
			{
				TfFilterAnd andFilter = (TfFilterAnd)filter;
				if (andFilter.Filters != null && andFilter.Filters.Count > 0)
				{
					List<string> childFiltersSql = new List<string>();
					foreach (var childFilter in andFilter.Filters)
					{
						var childFilterSql = GenerateFiltersSql(childFilter, parameters);
						if (!string.IsNullOrWhiteSpace(childFilterSql))
							childFiltersSql.Add(childFilterSql);
					}
					if (childFiltersSql.Count > 1)
						return $" ( " + string.Join($" AND ", childFiltersSql.ToArray()) + " ) ";
					else if (childFiltersSql.Count == 1)
						return childFiltersSql[0];
				}
				return string.Empty;
			}
			else if (filter is TfFilterOr)
			{
				TfFilterOr orFilter = (TfFilterOr)filter;
				if (orFilter.Filters != null && orFilter.Filters.Count > 0)
				{
					List<string> childFiltersSql = new List<string>();
					foreach (var childFilter in orFilter.Filters)
					{
						var childFilterSql = GenerateFiltersSql(childFilter, parameters);
						if (!string.IsNullOrWhiteSpace(childFilterSql))
							childFiltersSql.Add(childFilterSql);
					}
					if (childFiltersSql.Count > 1)
						return $" ( " + string.Join($" OR ", childFiltersSql.ToArray()) + " ) ";
					else if (childFiltersSql.Count == 1)
						return childFiltersSql[0];
				}
				return string.Empty;
			}

			var column = _filterColumns.SingleOrDefault(x => x.DbName == filter.ColumnName);
			if (column is null)
				return string.Empty;

			var columnName = $"{column.TableAlias}.{filter.ColumnName}";
			if (!string.IsNullOrWhiteSpace(column.SharedKeyDbName))
				columnName = $"{column.TableAlias}.value";

			var parameterName = "filter_par_" + Guid.NewGuid().ToString().Replace("-", string.Empty);

			if (filter is TfFilterBoolean)
			{
				if (column.DbType == DatabaseColumnType.Boolean)
					return string.Empty;

				NpgsqlParameter parameter = new NpgsqlParameter(parameterName, DbType.Boolean);
				parameter.Value = GetFilterParameterValue(filter, typeof(TfFilterBoolean));
				parameters.Add(parameter);

				switch (((TfFilterBoolean)filter).ComparisonMethod)
				{
					case TfFilterBooleanComparisonMethod.Equal:
						return $" {columnName} = @{parameterName} ";
					case TfFilterBooleanComparisonMethod.NotEqual:
						return $" {columnName} <> @{parameterName} ";
					case TfFilterBooleanComparisonMethod.IsTrue:
						return $" {columnName} = TRUE ";
					case TfFilterBooleanComparisonMethod.IsFalse:
						return $" {columnName} = FALSE ";
					case TfFilterBooleanComparisonMethod.HasValue:
						return $" {columnName} IS NOT NULL ";
					case TfFilterBooleanComparisonMethod.HasNoValue:
						return $" {columnName} IS NULL ";
					default:
						throw new Exception("Not supported filter comparison method");
				}
			}
			else if (filter is TfFilterDateTime)
			{
				if (!(column.DbType == DatabaseColumnType.Date ||
					 column.DbType == DatabaseColumnType.DateTime))
				{
					return string.Empty;
				}

				NpgsqlParameter parameter = new NpgsqlParameter(parameterName, DbType.DateTime2);
				parameter.Value = GetFilterParameterValue(filter, typeof(TfFilterDateTime));
				parameters.Add(parameter);

				switch (((TfFilterDateTime)filter).ComparisonMethod)
				{
					case TfFilterDateTimeComparisonMethod.Equal:
						return $" {columnName} = @{parameterName} ";
					case TfFilterDateTimeComparisonMethod.NotEqual:
						return $" {columnName} <> @{parameterName} ";
					case TfFilterDateTimeComparisonMethod.GreaterOrEqual:
						return $" {columnName} >= @{parameterName} ";
					case TfFilterDateTimeComparisonMethod.Greater:
						return $" {columnName} > @{parameterName} ";
					case TfFilterDateTimeComparisonMethod.LowerOrEqual:
						return $" {columnName} <= @{parameterName} ";
					case TfFilterDateTimeComparisonMethod.Lower:
						return $" {columnName} < @{parameterName} ";
					case TfFilterDateTimeComparisonMethod.HasValue:
						return $" {columnName} IS NOT NULL ";
					case TfFilterDateTimeComparisonMethod.HasNoValue:
						return $" {columnName} IS NULL ";
					default:
						throw new Exception("Not supported filter comparison method");
				}
			}
			else if (filter is TfFilterGuid)
			{
				if (column.DbType != DatabaseColumnType.Guid)
					return string.Empty;

				NpgsqlParameter parameter = new NpgsqlParameter(parameterName, DbType.Guid);
				parameter.Value = GetFilterParameterValue(filter, typeof(TfFilterGuid));
				parameters.Add(parameter);

				switch (((TfFilterGuid)filter).ComparisonMethod)
				{
					case TfFilterGuidComparisonMethod.Equal:
						return $" {columnName} = @{parameterName} ";
					case TfFilterGuidComparisonMethod.NotEqual:
						return $" {columnName} <> @{parameterName} ";
					case TfFilterGuidComparisonMethod.IsEmpty:
						return $" {columnName} = @{parameterName} ";
					case TfFilterGuidComparisonMethod.IsNotEmpty:
						return $" {columnName} <> @{parameterName} ";
					case TfFilterGuidComparisonMethod.HasValue:
						return $" {columnName} IS NOT NULL ";
					case TfFilterGuidComparisonMethod.HasNoValue:
						return $" {columnName} IS NULL ";
					default:
						throw new Exception("Not supported filter comparison method");
				}
			}
			else if (filter is TfFilterNumeric)
			{
				if (!(column.DbType == DatabaseColumnType.Number ||
					 column.DbType == DatabaseColumnType.ShortInteger ||
					 column.DbType == DatabaseColumnType.Integer ||
					 column.DbType == DatabaseColumnType.LongInteger ||
					 column.DbType == DatabaseColumnType.AutoIncrement))
				{
					return string.Empty;
				}

				NpgsqlParameter parameter = new NpgsqlParameter(parameterName, DbType.VarNumeric);
				parameter.Value = GetFilterParameterValue(filter, typeof(TfFilterNumeric));
				parameters.Add(parameter);

				switch (((TfFilterNumeric)filter).ComparisonMethod)
				{
					case TfFilterNumericComparisonMethod.Equal:
						return $" {columnName} = @{parameterName} ";
					case TfFilterNumericComparisonMethod.NotEqual:
						return $" {columnName} <> @{parameterName} ";
					case TfFilterNumericComparisonMethod.GreaterOrEqual:
						return $" {columnName} >= @{parameterName} ";
					case TfFilterNumericComparisonMethod.Greater:
						return $" {columnName} > @{parameterName} ";
					case TfFilterNumericComparisonMethod.LowerOrEqual:
						return $" {columnName} <= @{parameterName} ";
					case TfFilterNumericComparisonMethod.Lower:
						return $" {columnName} < @{parameterName} ";
					case TfFilterNumericComparisonMethod.HasValue:
						return $" {columnName} IS NOT NULL ";
					case TfFilterNumericComparisonMethod.HasNoValue:
						return $" {columnName} IS NULL ";
					default:
						throw new Exception("Not supported filter comparison method");
				}
			}
			else if (filter is TfFilterText)
			{
				if (!(column.DbType == DatabaseColumnType.ShortText ||
					 column.DbType == DatabaseColumnType.Text))
				{
					return string.Empty;
				}

				DbType dbType = DbType.String;
				if (column.DbType == DatabaseColumnType.ShortText)
					dbType = DbType.StringFixedLength;

				NpgsqlParameter parameter = new NpgsqlParameter(parameterName, dbType);
				parameter.Value = GetFilterParameterValue(filter,typeof(TfFilterText));
				parameters.Add(parameter);

				switch (((TfFilterText)filter).ComparisonMethod)
				{
					case TfFilterTextComparisonMethod.Equal:
						return $" {columnName} = @{parameterName} ";
					case TfFilterTextComparisonMethod.NotEqual:
						return $" {columnName} <> @{parameterName} ";
					case TfFilterTextComparisonMethod.StartsWith:
						return $" {columnName} ILIKE CONCAT ( @{parameterName} , '%') ";
					case TfFilterTextComparisonMethod.EndsWith:
						return $" {columnName} ILIKE CONCAT ('%', @{parameterName} ) ";
					case TfFilterTextComparisonMethod.Contains:
						return $" {columnName} ILIKE CONCAT ('%', @{parameterName} , '%') ";
					case TfFilterTextComparisonMethod.Fts:
						return $" to_tsvector( {columnName} ) @@ to_tsquery('english', @{parameterName} ) ";
					case TfFilterTextComparisonMethod.HasValue:
						return $" {columnName} IS NOT NULL ";
					case TfFilterTextComparisonMethod.HasNoValue:
						return $" {columnName} IS NULL ";
					default:
						throw new Exception("Not supported filter comparison method");
				}
			}

			return sb.ToString();
		}

		private object GetFilterParameterValue(
			TfFilterBase filter, 
			Type type)
		{
			object value = null;
			
			if (type == typeof(TfFilterText))
			{
				value = ((TfFilterText)filter).Value;
			}
			else if (type == typeof(TfFilterBoolean))
			{
				if (bool.TryParse(((TfFilterBoolean)filter).Value?.ToString(), out bool outVal))
					value = outVal;
				else
					value = null;
			}
			else if (type == typeof(TfFilterDateTime))
			{
				value = ((TfFilterDateTime)filter).Value?.GetDateFromFormulaString();
			}
			else if (type == typeof(TfFilterGuid))
			{
				if (Guid.TryParse(((TfFilterGuid)filter).Value?.ToString(), out Guid outVal))
					value = outVal;
				else
					value = null;
			}
			else if (type == typeof(TfFilterNumeric))
			{
				if (decimal.TryParse(((TfFilterNumeric)filter).Value?.ToString(), out decimal outVal))
					value = outVal;
				else
					value = null;
			}
			else
			{
				throw new Exception("Type is not supported");
			}

			if (value is null)
			{
				value = DBNull.Value;
			}

			return value;
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

		public bool IsSystem { get; set; } = false;

		public string GetSelectString()
		{
			if (string.IsNullOrWhiteSpace(SharedKeyDbName))
				return $"{TableAlias}.{DbName}";
			else
				return $"{TableAlias}.value AS {DbName}";
		}
	}
}
