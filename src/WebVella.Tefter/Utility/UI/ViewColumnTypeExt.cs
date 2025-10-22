namespace WebVella.Tefter.Utility;

public static class ViewColumnTypeExt
{
	#region << DataColumn >>

	public static object? ConvertStringToColumnObject(this TfDataColumn? column, string stringValue)
	{
		if (column is null) return null;

		switch (column.DbType)
		{
			case TfDatabaseColumnType.ShortInteger:
				return TfConverters.Convert<short>(stringValue);
			case TfDatabaseColumnType.Integer:
				return TfConverters.Convert<int>(stringValue);
			case TfDatabaseColumnType.LongInteger:
				return TfConverters.Convert<long>(stringValue);
			case TfDatabaseColumnType.Number:
				return TfConverters.Convert<decimal>(stringValue);
			case TfDatabaseColumnType.Boolean:
				return TfConverters.Convert<bool>(stringValue);
			case TfDatabaseColumnType.DateOnly:
				return TfConverters.Convert<DateOnly>(stringValue);
			case TfDatabaseColumnType.DateTime:
				return TfConverters.Convert<DateTime>(stringValue);
			case TfDatabaseColumnType.ShortText:
			case TfDatabaseColumnType.Text:
				return stringValue;
			case TfDatabaseColumnType.Guid:
				return TfConverters.Convert<Guid>(stringValue);
				;
			default:
				throw new Exception("colDbType not supported");
		}
	}

	#endregion

	#region << Context >>

	public static (TfDataColumn?, object?) GetColumnAndDataByAlias(this TfSpaceViewColumnBaseContext args, string alias)
	{
		TfDataTable? dt;
		Guid rowId;
		switch (args)
		{
			case TfSpaceViewColumnReadModeContext context:
				dt = context.DataTable;
				rowId = context.RowId;
				break;
			case TfSpaceViewColumnEditModeContext context:
				dt = context.DataTable;
				rowId = context.RowId;
				break;
			case TfSpaceViewColumnExportExcelModeContext context:
				dt = context.DataTable;
				rowId = context.RowId;
				break;
			case TfSpaceViewColumnExportCsvModeContext context:
				dt = context.DataTable;
				rowId = context.RowId;
				break;
			default:
				throw new Exception("The requested mode has no value");
		}

		if (dt is null)
			throw new Exception("DataTable not provided");

		TfDataColumn? column = dt.GetColumnByAlias(alias, args.ViewColumn.DataMapping);
		if (column is null)
			return (null, null);
		return (column, dt.GetColumnData(rowId, column));
	}

	public static string GetStorageKey(this TfSpaceViewColumnBaseContext args, string suffix)
	{
		var sb = new StringBuilder();
		sb.Append(args.ViewColumn.Id);
		sb.Append("__");
		sb.Append(args.ViewColumn.TypeId);
		sb.Append("__");
		sb.Append(suffix);
		return sb.ToString();
	}

	public static (TfDataColumn, object?) GetColumnAndData(this TfSpaceViewColumnBaseContext args, string columnName)
	{
		TfDataTable? dt;
		Guid rowId;
		switch (args)
		{
			case TfSpaceViewColumnReadModeContext context:
				dt = context.DataTable;
				rowId = context.RowId;
				break;
			case TfSpaceViewColumnEditModeContext context:
				dt = context.DataTable;
				rowId = context.RowId;
				break;
			case TfSpaceViewColumnExportExcelModeContext context:
				dt = context.DataTable;
				rowId = context.RowId;
				break;
			case TfSpaceViewColumnExportCsvModeContext context:
				dt = context.DataTable;
				rowId = context.RowId;
				break;
			default:
				throw new Exception("The requested mode has no value");
		}

		if (dt is null)
			throw new Exception("DataTable not provided");

		TfDataColumn? column = dt.Columns[columnName];
		if (column is null)
			throw new Exception("Column not found");
		return (column, dt.GetColumnData(rowId, column));
	}

	public static T GetSettings<T>(this TfSpaceViewColumnBaseContext args)
	{
		if (args.ViewColumn.TypeOptionsJson is null) return Activator.CreateInstance<T>();
		var options = JsonSerializer.Deserialize<T>(args.ViewColumn.TypeOptionsJson);
		if (options is null) return Activator.CreateInstance<T>();
		return options;
	}

	public static void SetColumnTypeOptions<T>(this TfSpaceViewColumnBaseContext args, T? options)
	{
		if (options is null)
			args.ViewColumn.TypeOptionsJson = "{}";

		args.ViewColumn.TypeOptionsJson = JsonSerializer.Serialize(options);
	}

	#endregion

	#region << DataTable >>

	public static TfDataColumn? GetColumnByAlias(this TfDataTable? dt, string alias,
		Dictionary<string, string?> dataMapping)
	{
		if (dt is null) return null;
		var colName = dataMapping._getColumnNameFromAlias(alias);
		if (String.IsNullOrWhiteSpace(colName)) return null;

		return dt.Columns[colName];
	}

	public static object? GetColumnData(this TfDataTable? dt, Guid rowId, TfDataColumn? column)
	{
		if (column is null) return null;

		switch (column.DbType)
		{
			case TfDatabaseColumnType.ShortInteger:
				return dt._getDataStructByAlias<short>(rowId, column);
			case TfDatabaseColumnType.Integer:
				return dt._getDataStructByAlias<int>(rowId, column);
			case TfDatabaseColumnType.LongInteger:
				return dt._getDataStructByAlias<long>(rowId, column);
			case TfDatabaseColumnType.Number:
				return dt._getDataStructByAlias<decimal>(rowId, column);
			case TfDatabaseColumnType.Boolean:
				return dt._getDataStructByAlias<bool>(rowId, column);
			case TfDatabaseColumnType.DateOnly:
				return dt._getDataStructByAlias<DateOnly>(rowId, column);
			case TfDatabaseColumnType.DateTime:
				return dt._getDataStructByAlias<DateTime>(rowId, column);
			case TfDatabaseColumnType.ShortText:
			case TfDatabaseColumnType.Text:
				return dt._getDataStringByAlias(rowId, column);
			case TfDatabaseColumnType.Guid:
				return dt._getDataStructByAlias<Guid>(rowId, column);
			default:
				throw new Exception("colDbType not supported");
		}
	}

	#endregion

	#region << DataRow >>

	public static Dictionary<string, Tuple<TfColor?, TfColor?>> GenerateColoring(this TfDataTable dt,
		Guid rowId,
		List<TfColoringRule> rules, Dictionary<string, string> queryNameToColumnNameDict)
	{
		var dict = new Dictionary<string, Tuple<TfColor?, TfColor?>>();
		TfDataRow? row = dt.Rows[rowId];
		if(row is null) return dict;
		if (rules.Count == 0) return dict;
		if (queryNameToColumnNameDict.Keys.Count == 0) return dict;

		var tfId = (Guid)row[TfConstants.TEFTER_ITEM_ID_PROP_NAME]!;

		Tuple<TfColor?, TfColor?>? rowColorRule = null;

		foreach (var rule in rules)
		{
			var ruleMatch = true;

			foreach (var filter in rule.Filters)
			{
				//logical filters cannot be applied
				if (filter.Name == new TfFilterAnd().GetColumnName()
				    || filter.Name == new TfFilterOr().GetColumnName())
				{
					ruleMatch = false;
					break;
				}
				
				//Filter could not be applied as one column is missing
				if (!queryNameToColumnNameDict.ContainsKey(filter.Name))
				{
					ruleMatch = false;
					break;
				}

				//Filter could not be applied as one column is missing
				var columnName = queryNameToColumnNameDict[filter.Name];
				var column = dt.Columns[columnName];
				if(column is null)
				{
					ruleMatch = false;
					break;
				}

				if (!filter._filterIsMatched(column,dt[rowId,columnName]))
				{
					ruleMatch = false;
				}
			}
			if (!ruleMatch) continue;

			if (rule.Columns.Count == 0)
			{
				rowColorRule = new Tuple<TfColor?, TfColor?>(rule.ForegroundColor, rule.BackgroundColor);
				continue;
			}

			foreach (var column in rule.Columns)
			{
				dict[column] = new Tuple<TfColor?, TfColor?>(rule.ForegroundColor, rule.BackgroundColor);
			}
		}

		if (rowColorRule is not null)
			dict[tfId.ToString()] = rowColorRule;
		return dict;
	}

	#endregion

	#region << Private >>

	private static string? _getColumnNameFromAlias(this Dictionary<string, string?> dataMapping, string alias)
	{
		if (dataMapping.ContainsKey(alias))
			return dataMapping[alias];

		return null;
	}

	private static object? _getDataString(this TfDataTable? dt, Guid rowId, string? colDbName,
		string? defaultValue = null)
	{
		if (String.IsNullOrWhiteSpace(colDbName))
			return defaultValue;

		if (dt is null || dt.Rows.Count == 0 || rowId == Guid.Empty) return defaultValue;

		if (dt.Rows[rowId] is null) return defaultValue;
		if (dt.Columns[colDbName] is null) return defaultValue;

		object? value = dt[rowId, colDbName];
		if (value is null) return defaultValue;
		if (value.GetType().ImplementsInterface(typeof(IList))) return (List<string>)value;
		return value.ToString();
	}

	private static object? _getDataStruct<T>(this TfDataTable? dt, Guid rowId, string colDbName,
		T? defaultValue = null) where T : struct
	{
		if (String.IsNullOrWhiteSpace(colDbName))
			return defaultValue;

		if (dt is null || dt.Rows.Count == 0 || rowId == Guid.Empty) return defaultValue;

		if (dt.Rows[rowId] is null) return defaultValue;
		if (dt.Columns[colDbName] is null) return defaultValue;

		object? value = dt.Rows[rowId]![colDbName];
		if (value is null) return defaultValue;

		if (typeof(T) == typeof(String)) return (T)value;
		else if (value is T) return (T)value;
		else if (value.GetType().ImplementsInterface(typeof(IList))) return (List<T?>)value;
		return TfConverters.Convert<T>(value.ToString()!);
	}

	private static object? _getDataStringByAlias(this TfDataTable? dt, Guid rowId, TfDataColumn column,
		string? defaultValue = null)
	{
		return dt._getDataString(rowId, column.Name, defaultValue);
	}

	private static object? _getDataStructByAlias<T>(this TfDataTable? dt, Guid rowId, TfDataColumn column,
		T? defaultValue = null)
		where T : struct
	{
		return dt._getDataStruct(rowId, column.Name, defaultValue);
	}

	private static T? _getDataObjectFromJsonByAlias<T>(this TfDataTable? dt, Guid rowId, TfDataColumn column,
		T? defaultValue = null) where T : class
	{
		if (dt is null || dt.Rows.Count == 0 || rowId == Guid.Empty) return defaultValue;

		if (dt.Rows[rowId] is null) return defaultValue;

		object? value = dt[rowId, column.Name];
		if (value is null) return null;
		if (value is string && String.IsNullOrWhiteSpace((string)value)) return null;
		if (value is T) return (T)value;

		try
		{
			return JsonSerializer.Deserialize<T>(value.ToString()!);
		}
		catch { throw new Exception("Value cannot be parsed"); }
	}

	private static bool _filterIsMatched(this TfFilterQuery filterQuery, TfDataColumn column, object? value)
	{
		switch (column.DbType)
		{
			case TfDatabaseColumnType.Boolean:
				return filterQuery._matchBooleanFilter(value);
			case TfDatabaseColumnType.DateOnly:
			case TfDatabaseColumnType.DateTime:
				return filterQuery._matchDateTimeFilter(value);			
			case TfDatabaseColumnType.Guid:
				return filterQuery._matchGuidFilter(value);				
			case TfDatabaseColumnType.ShortInteger:
			case TfDatabaseColumnType.Integer:
			case TfDatabaseColumnType.LongInteger:
			case TfDatabaseColumnType.Number:
				return filterQuery._matchNumericFilter(value);			
			case TfDatabaseColumnType.ShortText:
			case TfDatabaseColumnType.Text:
				return filterQuery._matchTextFilter(value);				
		}

		return false;
	}

	private static bool _matchBooleanFilter(this TfFilterQuery filterQuery, object? value)
	{
		var matchMethod = filterQuery.Method.TryParseEnum<TfFilterBooleanComparisonMethod>();
		if(matchMethod is null)
			throw new Exception("Comparison method cannot be determined");

		bool? columnValue = value.ParseToBool();		
		bool? filterValue = filterQuery.Value.ParseToBool();

		
		switch (matchMethod)
		{
			case TfFilterBooleanComparisonMethod.Equal:
				return columnValue == filterValue;
			case TfFilterBooleanComparisonMethod.NotEqual:
				return columnValue != filterValue;
			case TfFilterBooleanComparisonMethod.IsTrue:
				return columnValue is true;
			case TfFilterBooleanComparisonMethod.IsFalse:
				return columnValue is false;
			case TfFilterBooleanComparisonMethod.HasValue:
				return columnValue is not null;
			case TfFilterBooleanComparisonMethod.HasNoValue:
				return columnValue is null;
			default:
				throw new Exception("Not supported filter comparison method");
		}		
	}
	
	private static bool _matchDateTimeFilter(this TfFilterQuery filterQuery, object? value)
	{
		var matchMethod = filterQuery.Method.TryParseEnum<TfFilterDateTimeComparisonMethod>();
		if(matchMethod is null)
			throw new Exception("Comparison method cannot be determined");

		DateTime? columnValue = value.ParseToDateTime();				
		DateTime? filterValue = filterQuery.Value.ParseToDateTime();

		
		switch (matchMethod)
		{
			case TfFilterDateTimeComparisonMethod.Equal:
				return  columnValue.Equals(filterValue);
			case TfFilterDateTimeComparisonMethod.NotEqual:
				return !columnValue.Equals(filterValue);
			case TfFilterDateTimeComparisonMethod.GreaterOrEqual:
				return  columnValue > filterValue || columnValue.Equals(filterValue);
			case TfFilterDateTimeComparisonMethod.Greater:
				return columnValue > filterValue;
			case TfFilterDateTimeComparisonMethod.LowerOrEqual:
				return columnValue <= filterValue || columnValue.Equals(filterValue);
			case TfFilterDateTimeComparisonMethod.Lower:
				return columnValue <= filterValue;
			case TfFilterDateTimeComparisonMethod.HasValue:
				return columnValue is not null;
			case TfFilterDateTimeComparisonMethod.HasNoValue:
				return columnValue is null;
			default:
				throw new Exception("Not supported filter comparison method");
		}		
	}	
	
	private static bool _matchGuidFilter(this TfFilterQuery filterQuery, object? value)
	{
		var matchMethod = filterQuery.Method.TryParseEnum<TfFilterGuidComparisonMethod>();
		if(matchMethod is null)
			throw new Exception("Comparison method cannot be determined");
		
		Guid? columnValue = value.ParseToGuid();				
		Guid? filterValue = filterQuery.Value.ParseToGuid();		
		
		switch (matchMethod)
		{
			case TfFilterGuidComparisonMethod.Equal:
				return columnValue == filterValue;
			case TfFilterGuidComparisonMethod.NotEqual:
				return columnValue != filterValue;
			case TfFilterGuidComparisonMethod.IsEmpty:
				return columnValue == Guid.Empty;
			case TfFilterGuidComparisonMethod.IsNotEmpty:
				return columnValue == Guid.Empty;
			case TfFilterGuidComparisonMethod.HasValue:
				return columnValue is not null;
			case TfFilterGuidComparisonMethod.HasNoValue:
				return columnValue is null;
			default:
				throw new Exception("Not supported filter comparison method");
		}		
	}		

	private static bool _matchNumericFilter(this TfFilterQuery filterQuery, object? value)
	{
		var matchMethod = filterQuery.Method.TryParseEnum<TfFilterNumericComparisonMethod>();
		if(matchMethod is null)
			throw new Exception("Comparison method cannot be determined");
		
		decimal? columnValue = value.ParseToDecimal();				
		decimal? filterValue = filterQuery.Value.ParseToDecimal();		
		switch (matchMethod)
		{
			case TfFilterNumericComparisonMethod.Equal:
				return columnValue == filterValue;
			case TfFilterNumericComparisonMethod.NotEqual:
				return columnValue != filterValue;
			case TfFilterNumericComparisonMethod.GreaterOrEqual:
				return columnValue >= filterValue;
			case TfFilterNumericComparisonMethod.Greater:
				return columnValue > filterValue;
			case TfFilterNumericComparisonMethod.LowerOrEqual:
				return columnValue <= filterValue;
			case TfFilterNumericComparisonMethod.Lower:
				return columnValue < filterValue;
			case TfFilterNumericComparisonMethod.HasValue:
				return  columnValue is not null;
			case TfFilterNumericComparisonMethod.HasNoValue:
				return columnValue is null;
			default:
				throw new Exception("Not supported filter comparison method");
		}		
	}		
	private static bool _matchTextFilter(this TfFilterQuery filterQuery, object? value)
	{
		var matchMethod = filterQuery.Method.TryParseEnum<TfFilterTextComparisonMethod>();
		if(matchMethod is null)
			throw new Exception("Comparison method cannot be determined");
		
		string? columnValue = value.ParseToString()?.ToLowerInvariant();				
		string? filterValue = filterQuery.Value.ParseToString()?.ToLowerInvariant();		
		switch (matchMethod)
		{
			case TfFilterTextComparisonMethod.Equal:
				return columnValue == filterValue;
			case TfFilterTextComparisonMethod.NotEqual:
				return columnValue != filterValue;
			case TfFilterTextComparisonMethod.StartsWith:
				return columnValue is not null && filterValue is not null && columnValue.StartsWith(filterValue);
			case TfFilterTextComparisonMethod.EndsWith:
				return columnValue is not null && filterValue is not null && columnValue.EndsWith(filterValue);
			case TfFilterTextComparisonMethod.Contains:
				return columnValue is not null && filterValue is not null && columnValue.Contains(filterValue);
			case TfFilterTextComparisonMethod.Fts:
				return columnValue is not null && filterValue is not null && columnValue.Contains(filterValue);
			case TfFilterTextComparisonMethod.HasValue:
				return columnValue is not null;
			case TfFilterTextComparisonMethod.HasNoValue:
				return columnValue is null;
			default:
				throw new Exception("Not supported filter comparison method");
		}		
	}		
	
	#endregion
}