namespace WebVella.Tefter;

public partial interface IDataManager
{
	internal Result<TfDataProviderDataRow> GetProviderRow(
		TfDataProvider provider,
		Guid id);
	internal Result<TfDataProviderDataRow> GetProviderRow(
		TfDataProvider provider,
		int index);

	internal Result InsertNewProviderRow(
		TfDataProvider provider,
		TfDataProviderDataRow row);

	internal Result UpdateProviderRow(
		TfDataProvider provider,
		TfDataProviderDataRow row);

	internal Result DeleteProviderRowsAfterIndex(
		TfDataProvider provider,
		int index);
}

public partial class DataManager
{
	public Result<TfDataProviderDataRow> GetProviderRow(
		TfDataProvider provider,
		Guid id)
	{
		try
		{
			string whereClause = "WHERE tf_id = @id";

			string sql = BuildSelectRowSql(provider, whereClause);

			var dt = _dbService.ExecuteSqlQueryCommand(sql, new NpgsqlParameter("@id", id));

			if (dt.Rows.Count == 0)
				return Result.Ok();

			TfDataProviderDataRow row = new TfDataProviderDataRow();
			foreach (DataColumn column in dt.Columns)
			{
				object value = dt.Rows[0][column.ColumnName];
				if (value == DBNull.Value)
					value = null;
				else
				{
					var providerColumn = provider.Columns.SingleOrDefault(x => x.DbName == column.ColumnName);
					if (providerColumn is not null && providerColumn.DbType == DatabaseColumnType.Date)
					{
						value = DateOnly.FromDateTime((DateTime)value);
					}
				}

				row[column.ColumnName] = value;
			}

			return Result.Ok(row);

		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to get data provider row").CausedBy(ex));
		}
	}

	public Result<TfDataProviderDataRow> GetProviderRow(
		TfDataProvider provider,
		int index)
	{
		try
		{
			string whereClause = "WHERE tf_row_index = @index";

			string sql = BuildSelectRowSql(provider, whereClause);

			var dt = _dbService.ExecuteSqlQueryCommand(sql, new NpgsqlParameter("@index", index));

			if (dt.Rows.Count == 0)
				return Result.Ok();

			TfDataProviderDataRow row = new TfDataProviderDataRow();
			foreach (DataColumn column in dt.Columns)
			{
				object value = dt.Rows[0][column.ColumnName];
				if (value == DBNull.Value)
					value = null;
				else
				{
					var providerColumn = provider.Columns.SingleOrDefault(x => x.DbName == column.ColumnName);
					if (providerColumn is not null && providerColumn.DbType == DatabaseColumnType.Date)
					{
						value = DateOnly.FromDateTime((DateTime)value);
					}
				}

				row[column.ColumnName] = value;
			}

			return Result.Ok(row);
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to get data provider row").CausedBy(ex));
		}
	}

	public Result InsertNewProviderRow(
		TfDataProvider provider,
		TfDataProviderDataRow row)
	{
		try
		{
			var idResult = GetId(Guid.NewGuid());
			if (!idResult.IsSuccess)
				throw new Exception("Unable to get new ID from id manager.");

			row["tf_id"] = idResult.Value;
			row["tf_created_on"] = DateTime.Now;
			row["tf_updated_on"] = DateTime.Now;
			row["tf_search"] = string.Empty;

			foreach (var sharedKey in provider.SharedKeys)
			{
				List<string> keys = new List<string>();
				foreach (var column in sharedKey.Columns)
					keys.Add(row[column.DbName].ToString());

				var skIdResult = GetId(keys.ToArray());
				if (!skIdResult.IsSuccess)
					throw new Exception("Unable to get new ID from id manager.");

				row[$"tf_sk_{sharedKey.DbName}_id"] = skIdResult.Value;
				row[$"tf_sk_{sharedKey.DbName}_version"] = sharedKey.Version;
			}

			List<NpgsqlParameter> parameters;
			var sql = BuildInsertNewRowSql(provider, row, out parameters);

			var count = _dbService.ExecuteSqlNonQueryCommand(sql, parameters);
			if (count != 1)
				throw new Exception("Failed to insert new row");
			return Result.Ok();
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to insert new data provider row").CausedBy(ex));
		}
	}

	public Result UpdateProviderRow(
		TfDataProvider provider,
		TfDataProviderDataRow row)
	{
		try
		{
			row["tf_updated_on"] = DateTime.Now;
			row["tf_search"] = string.Empty;
			foreach (var sharedKey in provider.SharedKeys)
			{
				List<string> keys = new List<string>();
				foreach (var column in sharedKey.Columns)
					keys.Add(row[column.DbName].ToString());

				var skIdResult = GetId(keys.ToArray());
				if (!skIdResult.IsSuccess)
					throw new Exception("Unable to get new ID from id manager.");

				row[$"tf_sk_{sharedKey.DbName}_id"] = skIdResult.Value;
				row[$"tf_sk_{sharedKey.DbName}_version"] = sharedKey.Version;
			}

			List<NpgsqlParameter> parameters;
			var sql = BuildUpdateRowSql(provider, row, out parameters);

			var count = _dbService.ExecuteSqlNonQueryCommand(sql, parameters);
			if (count != 1)
				throw new Exception("Failed to update row");
			return Result.Ok();
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to update data provider row").CausedBy(ex));
		}
	}

	public Result DeleteProviderRowsAfterIndex(
		TfDataProvider provider,
		int index)
	{
		try
		{
			string sql = $"DELETE FROM dp{provider.Index} WHERE tf_row_index >= @index";
			_dbService.ExecuteSqlNonQueryCommand(sql, new NpgsqlParameter("@index", index));

			return Result.Ok();
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to delete data provider row after index").CausedBy(ex));
		}
	}

	private string BuildSelectRowSql(
		TfDataProvider provider,
		string whereClause)
	{
		StringBuilder sql = new StringBuilder();

		sql.AppendLine("SELECT");

		sql.AppendLine("tf_id");
		sql.AppendLine(",");
		sql.AppendLine("tf_row_index");
		sql.AppendLine(",");

		foreach (var sharedKey in provider.SharedKeys)
		{
			sql.AppendLine($"tf_sk_{sharedKey.DbName}_id");
			sql.AppendLine(",");
			sql.AppendLine($"tf_sk_{sharedKey.DbName}_version");
			sql.AppendLine(",");
		}

		foreach (var column in provider.Columns)
		{
			sql.AppendLine(column.DbName);
			sql.AppendLine(",");
		}

		sql.AppendLine("tf_created_on");
		sql.AppendLine(",");
		sql.AppendLine("tf_updated_on");
		sql.AppendLine(",");
		sql.AppendLine("tf_search");
		sql.AppendLine();

		sql.AppendLine($"FROM dp{provider.Index}");

		sql.AppendLine();
		sql.AppendLine(whereClause);
		return sql.ToString();
	}

	private string BuildInsertNewRowSql(
		TfDataProvider provider,
		TfDataProviderDataRow row,
		out List<NpgsqlParameter> parameters)
	{
		parameters = new List<NpgsqlParameter>();
		StringBuilder sql = new StringBuilder();

		sql.AppendLine($"INSERT INTO dp{provider.Index} ( ");

		sql.AppendLine("tf_id");
		sql.AppendLine(",");
		sql.AppendLine("tf_row_index");
		sql.AppendLine(",");

		foreach (var sharedKey in provider.SharedKeys)
		{
			sql.AppendLine($"tf_sk_{sharedKey.DbName}_id");
			sql.AppendLine(",");
		}

		foreach (var column in provider.Columns)
		{
			sql.AppendLine(column.DbName);
			sql.AppendLine(",");
		}

		sql.AppendLine("tf_created_on");
		sql.AppendLine(",");
		sql.AppendLine("tf_updated_on");
		sql.AppendLine(",");
		sql.AppendLine("tf_search");
		sql.AppendLine();

		sql.AppendLine(" ) VALUES ( ");

		parameters.Add(new NpgsqlParameter("@tf_id", row["tf_id"]));
		sql.AppendLine("@tf_id");
		sql.AppendLine(",");
		parameters.Add(new NpgsqlParameter("@tf_row_index", row["tf_row_index"]));
		sql.AppendLine("@tf_row_index");
		sql.AppendLine(",");

		foreach (var sharedKey in provider.SharedKeys)
		{
			var sharedKeyName = $"tf_sk_{sharedKey.DbName}_id";
			parameters.Add(new NpgsqlParameter($"@{sharedKeyName}", row[sharedKeyName]));
			sql.AppendLine("@" + sharedKeyName);
			sql.AppendLine(",");
		}

		foreach (var column in provider.Columns)
		{
			parameters.Add(new NpgsqlParameter($"@{column.DbName}", row[column.DbName]));
			sql.AppendLine("@" + column.DbName);
			sql.AppendLine(",");
		}

		parameters.Add(new NpgsqlParameter("@tf_created_on", row["tf_created_on"]));
		sql.AppendLine("@tf_created_on");
		sql.AppendLine(",");
		parameters.Add(new NpgsqlParameter("@tf_updated_on", row["tf_updated_on"]));
		sql.AppendLine("@tf_updated_on");
		sql.AppendLine(",");
		parameters.Add(new NpgsqlParameter("@tf_search", row["tf_search"]));
		sql.AppendLine("@tf_search");
		sql.AppendLine();
		sql.AppendLine(" ) ");

		sql.AppendLine();
		return sql.ToString();
	}

	private string BuildUpdateRowSql(
		TfDataProvider provider,
		TfDataProviderDataRow row,
		out List<NpgsqlParameter> parameters)
	{
		parameters = new List<NpgsqlParameter>();
		StringBuilder sql = new StringBuilder();

		sql.AppendLine($"UPDATE dp{provider.Index} SET ");

		parameters.Add(new NpgsqlParameter("@tf_row_index", row["tf_row_index"]));
		sql.Append("tf_row_index = @tf_row_index");
		sql.AppendLine(",");

		foreach (var sharedKey in provider.SharedKeys)
		{
			var sharedKeyName = $"tf_sk_{sharedKey.DbName}_id";
			parameters.Add(new NpgsqlParameter($"@{sharedKeyName}", row[sharedKeyName]));
			sql.Append($"{sharedKeyName} = @{sharedKeyName}");
			sql.AppendLine(",");
		}

		foreach (var column in provider.Columns)
		{
			parameters.Add(new NpgsqlParameter($"@{column.DbName}", row[column.DbName]));
			sql.Append($"{column.DbName}=@{column.DbName}");
			sql.AppendLine(",");
		}

		parameters.Add(new NpgsqlParameter("@tf_updated_on", row["tf_updated_on"]));
		sql.Append("tf_updated_on = @tf_updated_on");
		sql.AppendLine(",");
		parameters.Add(new NpgsqlParameter("@tf_search", row["tf_search"]));
		sql.Append("tf_search = @tf_search");
		sql.AppendLine();

		parameters.Add(new NpgsqlParameter("@tf_id", row["tf_id"]));
		sql.AppendLine("WHERE tf_id = @tf_id ");

		sql.AppendLine();
		return sql.ToString();


	}
}
