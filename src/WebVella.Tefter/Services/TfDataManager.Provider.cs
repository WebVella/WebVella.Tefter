namespace WebVella.Tefter;

public partial interface ITfDataManager
{
	internal TfDataProviderDataRow GetProviderRow(
		TfDataProvider provider,
		Guid id);
	internal TfDataProviderDataRow GetProviderRow(
		TfDataProvider provider,
		int index);

	internal void InsertNewProviderRow(
		TfDataProvider provider,
		TfDataProviderDataRow row);

	internal void UpdateProviderRow(
		TfDataProvider provider,
		TfDataProviderDataRow row);

	internal void DeleteProviderRowsAfterIndex(
		TfDataProvider provider,
		int index);

	internal void UpdateValue(
		TfDataProvider provider,
		Guid rowId,
		string dbName,
		object value);

	internal void DeleteAllProviderRows(
		TfDataProvider provider);
}

public partial class TfDataManager
{
	public TfDataProviderDataRow GetProviderRow(
		TfDataProvider provider,
		Guid id)
	{
		string whereClause = "WHERE tf_id = @id";

		string sql = BuildSelectRowSql(provider, whereClause);

		var dt = _dbService.ExecuteSqlQueryCommand(sql, new NpgsqlParameter("@id", id));

		if (dt.Rows.Count == 0)
		{
			return null;
		}

		TfDataProviderDataRow row = new TfDataProviderDataRow();
		foreach (DataColumn column in dt.Columns)
		{
			object value = dt.Rows[0][column.ColumnName];
			if (value == DBNull.Value)
			{
				value = null;
			}
			else
			{
				var providerColumn = provider.Columns.SingleOrDefault(x => x.DbName == column.ColumnName);
				if (providerColumn is not null && providerColumn.DbType == TfDatabaseColumnType.Date)
				{
					value = DateOnly.FromDateTime((DateTime)value);
				}
			}

			row[column.ColumnName] = value;
		}
		return row;
	}

	public TfDataProviderDataRow GetProviderRow(
		TfDataProvider provider,
		int index)
	{
		string whereClause = "WHERE tf_row_index = @index";

		string sql = BuildSelectRowSql(provider, whereClause);

		var dt = _dbService.ExecuteSqlQueryCommand(sql, new NpgsqlParameter("@index", index));

		if (dt.Rows.Count == 0)
		{
			return null;
		}

		TfDataProviderDataRow row = new TfDataProviderDataRow();
		foreach (DataColumn column in dt.Columns)
		{
			object value = dt.Rows[0][column.ColumnName];
			if (value == DBNull.Value)
				value = null;
			else
			{
				var providerColumn = provider.Columns.SingleOrDefault(x => x.DbName == column.ColumnName);
				if (providerColumn is not null && providerColumn.DbType == TfDatabaseColumnType.Date)
				{
					value = DateOnly.FromDateTime((DateTime)value);
				}
			}

			row[column.ColumnName] = value;
		}
		return row;
	}

	public void InsertNewProviderRow(
		TfDataProvider provider,
		TfDataProviderDataRow row)
	{
		row["tf_id"] = GetId(Guid.NewGuid());
		row["tf_created_on"] = DateTime.Now;
		row["tf_updated_on"] = DateTime.Now;

		//generate search
		var searchSb = new StringBuilder();
		foreach (var column in provider.Columns)
		{
			if (column.IncludeInTableSearch)
			{
				var index = row.ColumnNames.IndexOf(column.DbName);
				if (index > 0)
				{
					object value = row[column.DbName];
					if (value is not null)
						searchSb.Append($" {value}");
				}
			}
		}
		row["tf_search"] = searchSb.ToString();

		foreach (var sharedKey in provider.SharedKeys)
		{
			List<string> keys = new List<string>();
			foreach (var column in sharedKey.Columns)
				keys.Add(row[column.DbName]?.ToString());

			row[$"tf_sk_{sharedKey.DbName}_id"] = GetId(keys.ToArray());
			row[$"tf_sk_{sharedKey.DbName}_version"] = sharedKey.Version;
		}

		List<NpgsqlParameter> parameters;
		var sql = BuildInsertNewRowSql(provider, row, out parameters);

		var count = _dbService.ExecuteSqlNonQueryCommand(sql, parameters);
		if (count != 1)
		{
			throw new Exception("Failed to insert new row");
		}
	}

	public void UpdateProviderRow(
		TfDataProvider provider,
		TfDataProviderDataRow row)
	{
		row["tf_updated_on"] = DateTime.Now;

		//generate search
		var searchSb = new StringBuilder();
		foreach (var column in provider.Columns)
		{
			if (column.IncludeInTableSearch)
			{
				var index = row.ColumnNames.IndexOf(column.DbName);
				if (index > 0)
				{
					object value = row[column.DbName];
					if (value is not null)
						searchSb.Append($" {value}");
				}
			}
		}
		row["tf_search"] = searchSb.ToString();

		foreach (var sharedKey in provider.SharedKeys)
		{
			List<string> keys = new List<string>();
			foreach (var column in sharedKey.Columns)
				keys.Add(row[column.DbName].ToString());

			row[$"tf_sk_{sharedKey.DbName}_id"] = GetId(keys.ToArray());
			row[$"tf_sk_{sharedKey.DbName}_version"] = sharedKey.Version;
		}

		List<NpgsqlParameter> parameters;
		var sql = BuildUpdateRowSql(provider, row, out parameters);

		var count = _dbService.ExecuteSqlNonQueryCommand(sql, parameters);
		if (count != 1)
		{
			throw new Exception("Failed to update row");
		}
	}

	public void DeleteProviderRowsAfterIndex(
		TfDataProvider provider,
		int index)
	{
		string sql = $"DELETE FROM dp{provider.Index} WHERE tf_row_index >= @index";
		_dbService.ExecuteSqlNonQueryCommand(sql, new NpgsqlParameter("@index", index));
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
			var parameterType = GetDbTypeForDatabaseColumnType(column.DbType);

			NpgsqlParameter parameter = new NpgsqlParameter($"@{column.DbName}", parameterType);

			if (row[column.DbName] is null)
				parameter.Value = DBNull.Value;
			else
				parameter.Value = row[column.DbName];

			parameters.Add(parameter);

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

			var sharedKeyVersion = $"tf_sk_{sharedKey.DbName}_version";
			parameters.Add(new NpgsqlParameter($"@{sharedKeyVersion}", row[sharedKeyVersion]));
			parameters.Add(new NpgsqlParameter($"@{sharedKeyVersion}", row[sharedKeyVersion]));
			sql.Append($"{sharedKeyVersion} = @{sharedKeyVersion}");
			sql.AppendLine(",");
		}

		foreach (var column in provider.Columns)
		{
			var parameterType = GetDbTypeForDatabaseColumnType(column.DbType);

			NpgsqlParameter parameter = new NpgsqlParameter($"@{column.DbName}", parameterType);

			if (row[column.DbName] is null)
				parameter.Value = DBNull.Value;
			else
				parameter.Value = row[column.DbName];

			parameters.Add(parameter);

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


	public void UpdateValue(
		TfDataProvider provider,
		Guid rowId,
		string dbName,
		object value)
	{
		var row = GetProviderRow(provider, rowId);
		row[dbName] = value;
		UpdateProviderRow(provider, row);
	}

	public void DeleteAllProviderRows(
		TfDataProvider provider)
	{
		_dbService.ExecuteSqlNonQueryCommand($"DELETE FROM dp{provider.Index}");
	}
}
