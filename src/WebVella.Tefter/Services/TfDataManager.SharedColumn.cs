namespace WebVella.Tefter;

public partial interface ITfDataManager
{
	internal void DeleteSharedColumnData(
		TfSharedColumn sharedColumn);
}

public partial class TfDataManager
{
	public void DeleteSharedColumnData(
		TfSharedColumn sharedColumn)
	{
		string tableName = GetSharedColumnValueTableNameByType(sharedColumn.DbType);

		string sql = $"DELETE FROM {tableName} WHERE shared_column_id = @shared_column_id";

		_dbService.ExecuteSqlNonQueryCommand(sql, new NpgsqlParameter("@shared_column_id", sharedColumn.Id));
	}

	private static string GetSharedColumnValueTableNameByType(TfDatabaseColumnType dbColumnType)
	{
		switch (dbColumnType)
		{
			case TfDatabaseColumnType.ShortText:
				return "shared_column_short_text_value";
			case TfDatabaseColumnType.Text:
				return "shared_column_text_value";
			case TfDatabaseColumnType.Boolean:
				return "shared_column_boolean_value";
			case TfDatabaseColumnType.Guid:
				return "shared_column_guid_value";
			case TfDatabaseColumnType.ShortInteger:
				return "shared_column_short_integer_value";
			case TfDatabaseColumnType.Integer:
				return "shared_column_integer_value";
			case TfDatabaseColumnType.LongInteger:
				return "shared_column_long_integer_value";
			case TfDatabaseColumnType.Number:
				return "shared_column_number_value";
			case TfDatabaseColumnType.Date:
				return "shared_column_date_value";
			case TfDatabaseColumnType.DateTime:
				return "shared_column_datetime_value";
			default:
				throw new Exception("Not supported column type.");
		}

	}
}
