namespace WebVella.Tefter.Utility;

public static class ViewColumnTypeExt
{
	#region << Context >>

	public static (TfDataColumn, object?) GetColumnAndDataByAlias(this TfSpaceViewColumnBaseContext args, string alias)
	{
		TfDataTable? dt = null;
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

		TfDataColumn? column = dt.GetColumnByAlias(alias, args.SpaceViewColumn.DataMapping);
		if (column is null)
			throw new Exception("Column not found");
		return (column, dt.GetColumnData(rowId, column));
	}

	#endregion


	#region << DataTable >>

	public static TfDataColumn? GetColumnByAlias(this TfDataTable? dt, string alias,
		Dictionary<string, string> dataMapping)
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

	private static string? _getColumnNameFromAlias(this Dictionary<string, string> dataMapping, string alias)
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
		return value!.ToString();
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
		return dt._getDataStruct<T>(rowId, column.Name, defaultValue);
	}


	private static T? GetDataObjectFromJsonByAlias<T>(this TfDataTable? dt, Guid rowId, TfDataColumn column,
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

	#endregion
}