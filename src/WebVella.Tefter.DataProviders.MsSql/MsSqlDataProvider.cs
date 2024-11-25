namespace WebVella.Tefter.DataProviders.MsSql;

public class MsSqlDataProvider : ITfDataProviderType
{
	public Guid Id => new Guid("0f86e009-1db4-497f-b10a-a55f4fad455d");

	public string Name => "Microsoft Sql Data Provider";

	public string Description => "Provide data from Microsoft SQL server query.";

	public string FluentIconName => "DocumentTable";

	public Type SettingsComponentType => typeof(DataProviderSettingsComponent);

	public ReadOnlyCollection<string> GetSupportedSourceDataTypes()
	{
		return new List<string> {
			"TEXT",
			"SHORT_TEXT",
			"BOOLEAN",
			"DATE",
			"DATETIME",
			"NUMBER",
			"SHORT_INTEGER",
			"INTEGER",
			"LONG_INTEGER",
			"GUID"
		}.AsReadOnly();
	}

	public ReadOnlyCollection<TfDatabaseColumnType> GetDatabaseColumnTypesForSourceDataType(
		string dataType)
	{
		switch (dataType)
		{
			case "TEXT":
				return new List<TfDatabaseColumnType> { TfDatabaseColumnType.Text }.AsReadOnly();
			case "SHORT_TEXT":
				return new List<TfDatabaseColumnType> { TfDatabaseColumnType.ShortText }.AsReadOnly();
			case "BOOLEAN":
				return new List<TfDatabaseColumnType> { TfDatabaseColumnType.Boolean }.AsReadOnly();
			case "NUMBER":
				return new List<TfDatabaseColumnType> { TfDatabaseColumnType.Number }.AsReadOnly();
			case "DATE":
				return new List<TfDatabaseColumnType> { TfDatabaseColumnType.Date, TfDatabaseColumnType.DateTime }.AsReadOnly();
			case "DATETIME":
				return new List<TfDatabaseColumnType> { TfDatabaseColumnType.DateTime, TfDatabaseColumnType.Date }.AsReadOnly();
			case "SHORT_INTEGER":
				return new List<TfDatabaseColumnType> { TfDatabaseColumnType.ShortInteger }.AsReadOnly();
			case "INTEGER":
				return new List<TfDatabaseColumnType> { TfDatabaseColumnType.Integer }.AsReadOnly();
			case "LONG_INTEGER":
				return new List<TfDatabaseColumnType> { TfDatabaseColumnType.LongInteger }.AsReadOnly();
			case "GUID":
				return new List<TfDatabaseColumnType> { TfDatabaseColumnType.Guid }.AsReadOnly();

		}
		return new List<TfDatabaseColumnType>().AsReadOnly();
	}


	public ReadOnlyCollection<TfDataProviderDataRow> GetRows(
		TfDataProvider provider)
	{
		var result = new List<TfDataProviderDataRow>();

		var settings = JsonSerializer.Deserialize<MsSqlDataProviderSettings>(provider.SettingsJson);

		if (string.IsNullOrWhiteSpace(settings.ConnectionString))
			throw new Exception("Connection string is not specified.");

		if (string.IsNullOrWhiteSpace(settings.SqlQuery))
			throw new Exception("Sql query is not specified.");

		return ReadDataFromSql(
			settings.ConnectionString,
			settings.SqlQuery,
			provider);
	}

	TfDataProviderSourceSchemaInfo ITfDataProviderType.GetDataProviderSourceSchema(TfDataProvider provider)
	{
		throw new NotImplementedException();
	}

	private ReadOnlyCollection<TfDataProviderDataRow> ReadDataFromSql(
		string connectionString,
		string sql,
		TfDataProvider provider)
	{
		var result = new List<TfDataProviderDataRow>();

		SqlConnection connection = new SqlConnection(connectionString);
		try
		{
			connection.Open();

			SqlCommand command = new SqlCommand(sql, connection);

			SqlDataAdapter adapter = new SqlDataAdapter(command);

			DataTable dt = new DataTable();

			adapter.Fill(dt);

			var providerColumnsWithSourceName = provider.Columns.Where(x => !string.IsNullOrWhiteSpace(x.SourceName));

			if(!providerColumnsWithSourceName.Any())
			{
				throw new Exception($"No columns with source name are specified.");
			}

			HashSet<string> sourceColumns = new HashSet<string>();
			foreach (DataColumn column in dt.Columns)
			{
				sourceColumns.Add(column.ColumnName.ToLowerInvariant());
			}

			foreach (DataRow dr in dt.Rows)
			{
				TfDataProviderDataRow row = new TfDataProviderDataRow();
				foreach (var providerColumnWithSource in providerColumnsWithSourceName)
				{
					var sourceName = providerColumnWithSource.SourceName.Trim();

					try
					{
						if (!sourceColumns.Contains(sourceName.ToLowerInvariant()))
						{
							throw new Exception($"Source column '{sourceName}' is not found in query result.");
						}

						row[providerColumnWithSource.DbName] = ConvertValue(
								providerColumnWithSource,
								dr[sourceName]);
					}
					catch (Exception ex)
					{
						throw new Exception($"Exception while processing source row" +
							$" ({sourceName}): {ex.Message}");
					}
				}
				result.Add(row);
			}
		}
		catch
		{
			connection.Close();
			throw;
		}

		return result.AsReadOnly();
	}

	private object ConvertValue(
		TfDataProviderColumn column,
		object value)
	{
		if ((value == null || value == DBNull.Value) && column.IsNullable)
			return null;

		if ((value == null || value == DBNull.Value) && !column.IsNullable)
		{
			throw new Exception($"Trying to set null value for column " +
				$"'{column.SourceName}' with is not nullable");
		}
			

		switch (column.DbType)
		{
			case TfDatabaseColumnType.ShortText:
			case TfDatabaseColumnType.Text:
				if (value is string)
				{
					return (string)value;
				}
				break;

			case TfDatabaseColumnType.Boolean:
				if (value is bool)
				{
					return (bool)value;
				}
				break;

			case TfDatabaseColumnType.Guid:
				if (value is Guid)
				{
					return (Guid)value;
				}
				break;

			case TfDatabaseColumnType.DateTime:
				if (value is DateTime)
				{
					return (DateTime)value;
				}
				break;

			case TfDatabaseColumnType.Date:
				{
					if (value is DateTime)
						return DateOnly.FromDateTime((DateTime)value);

					if (value is DateOnly)
						return (DateOnly)value;
				}
				break;

			case TfDatabaseColumnType.ShortInteger:
				if (value is short)
				{
					return (short)value;
				}
				break;

			case TfDatabaseColumnType.Integer:
				if (value is int)
				{
					return (int)value;
				}
				break;

			case TfDatabaseColumnType.LongInteger:
				if (value is long)
				{
					return (long)value;
				}
				break;

			case TfDatabaseColumnType.Number:
				if (value is decimal)
				{
					return (decimal)value;
				}
				break;
		}

		throw new Exception($"Not supported source type for column {column.SourceName}");
	}


}
