namespace WebVella.Tefter.Utility;

public static class ViewColumnTypeExt
{
	private const string _separator = "$$$";
	private const string _innerSeparator = "$_$";

	#region << ColumnType >>

	public static string GetCompatabilityHash(this ITfSpaceViewColumnTypeAddon columnType)
	{
		var sb = new StringBuilder();
		foreach (var definition in columnType.DataMappingDefinitions)
		{
			sb.Append(_separator);
			sb.Append(definition.Alias);
			sb.Append(_innerSeparator);
			var support = _getMaxSupportLevel(definition);
			sb.Append(support.GetHash());
			sb.Append(_separator);
		}

		return sb.ToString();
	}

	#endregion

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

	private record ComponentTypeSupportMaxLevel
	{
		public TfDatabaseColumnType? Number { get; set; } = null;
		public TfDatabaseColumnType? Text { get; set; } = null;
		public TfDatabaseColumnType? Boolean { get; set; } = null;
		public TfDatabaseColumnType? DateTime { get; set; } = null;
		public TfDatabaseColumnType? Guid { get; set; } = null;

		public string GetHash()
		{
			var sb = new StringBuilder();
			sb.Append(Number?.ToString());
			sb.Append(_innerSeparator);
			sb.Append(Text?.ToString());
			sb.Append(_innerSeparator);
			sb.Append(Boolean?.ToString());
			sb.Append(_innerSeparator);
			sb.Append(DateTime?.ToString());
			sb.Append(_innerSeparator);
			sb.Append(Guid?.ToString());
			sb.Append(_innerSeparator);
			return sb.ToString();
		}
	}

	private static ComponentTypeSupportMaxLevel _getMaxSupportLevel(
		this TfSpaceViewColumnDataMappingDefinition definition)
	{
		var support = new ComponentTypeSupportMaxLevel();
		var numberSupportPriority = new List<TfDatabaseColumnType>
		{
			TfDatabaseColumnType.Number,
			TfDatabaseColumnType.LongInteger,
			TfDatabaseColumnType.Integer,
			TfDatabaseColumnType.ShortInteger,
		};
		var textSupportPriority = new List<TfDatabaseColumnType>
		{
			TfDatabaseColumnType.Text, TfDatabaseColumnType.ShortText
		};
		var boolSupportPriority = new List<TfDatabaseColumnType> { TfDatabaseColumnType.Boolean };
		var dateTimeSupportPriority = new List<TfDatabaseColumnType>
		{
			TfDatabaseColumnType.DateTime, TfDatabaseColumnType.DateOnly,
		};
		var guidSupportPriority = new List<TfDatabaseColumnType> { TfDatabaseColumnType.Guid };
		//number
		foreach (var dbType in numberSupportPriority)
		{
			if (definition.SupportedDatabaseColumnTypes.Contains(dbType))
			{
				support.Number = dbType;
				break;
			}
		}

		//text
		foreach (var dbType in textSupportPriority)
		{
			if (definition.SupportedDatabaseColumnTypes.Contains(dbType))
			{
				support.Text = dbType;
				break;
			}
		}

		//boolean
		foreach (var dbType in boolSupportPriority)
		{
			if (definition.SupportedDatabaseColumnTypes.Contains(dbType))
			{
				support.Boolean = dbType;
				break;
			}
		}

		//datetime
		foreach (var dbType in dateTimeSupportPriority)
		{
			if (definition.SupportedDatabaseColumnTypes.Contains(dbType))
			{
				support.DateTime = dbType;
				break;
			}
		}

		//guid
		foreach (var dbType in guidSupportPriority)
		{
			if (definition.SupportedDatabaseColumnTypes.Contains(dbType))
			{
				support.Guid = dbType;
				break;
			}
		}


		return support;
	}

	#endregion
}