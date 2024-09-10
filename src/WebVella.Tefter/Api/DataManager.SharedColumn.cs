namespace WebVella.Tefter;

public partial interface IDataManager
{
	internal Result DeleteSharedColumnData(
		TfSharedColumn sharedColumn);
}

public partial class DataManager
{
	public Result DeleteSharedColumnData(
		TfSharedColumn sharedColumn)
	{
		try
		{
			string tableName = GetSharedColumnValueTableNameByType(sharedColumn.DbType);

			string sql = $"DELETE FROM {tableName} WHERE shared_column_id = @shared_column_id";

			_dbService.ExecuteSqlNonQueryCommand(sql, new NpgsqlParameter("@shared_column_id", sharedColumn.Id));

			return Result.Ok();
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to delete data provider row after index").CausedBy(ex));
		}
	}

	private static string GetSharedColumnValueTableNameByType(DatabaseColumnType dbColumnType)
	{
		switch (dbColumnType)
		{
			case DatabaseColumnType.ShortText:
				return "shared_column_short_text_value";
			case DatabaseColumnType.Text:
				return "shared_column_text_value";
			case DatabaseColumnType.Boolean:
				return "shared_column_boolean_value";
			case DatabaseColumnType.Guid:
				return "shared_column_guid_value";
			case DatabaseColumnType.ShortInteger:
				return "shared_column_short_integer_value";
			case DatabaseColumnType.Integer:
				return "shared_column_integer_value";
			case DatabaseColumnType.LongInteger:
				return "shared_column_long_integer_value";
			case DatabaseColumnType.Number:
				return "shared_column_number_value";
			case DatabaseColumnType.Date:
				return "shared_column_date_value";
			case DatabaseColumnType.DateTime:
				return "shared_column_datetime_value";
			default:
				throw new Exception("Not supported column type.");
		}

	}
}
