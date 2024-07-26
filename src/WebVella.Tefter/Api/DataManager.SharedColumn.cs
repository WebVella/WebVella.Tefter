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
			string tableName = "";
			switch(sharedColumn.DbType)
			{
				case DatabaseColumnType.ShortText:
					tableName = "shared_column_short_text_value";
					break;
				case DatabaseColumnType.Text:
					tableName = "shared_column_text_value";
					break;
				case DatabaseColumnType.Boolean:
					tableName = "shared_column_boolean_value";
					break;
				case DatabaseColumnType.Guid:
					tableName = "shared_column_guid_value";
					break;
				case DatabaseColumnType.ShortInteger:
					tableName = "shared_column_short_integer_value";
					break;
				case DatabaseColumnType.Integer:
					tableName = "shared_column_integer_value";
					break;
				case DatabaseColumnType.LongInteger:
					tableName = "shared_column_long_integer_value";
					break;
				case DatabaseColumnType.Number:
					tableName = "shared_column_number_value";
					break;
				case DatabaseColumnType.Date:
					tableName = "shared_column_date_value";
					break;
				case DatabaseColumnType.DateTime:
					tableName = "shared_column_datetime_value";
					break;
				default:
					throw new Exception("Not supported column type.");
			}

			string sql = $"DELETE FROM {tableName} WHERE shared_column_id = @shared_column_id";
			_dbService.ExecuteSqlNonQueryCommand(sql, new NpgsqlParameter("@shared_column_id", sharedColumn.Id));

			return Result.Ok();
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to delete data provider row after index").CausedBy(ex));
		}
	}
}
