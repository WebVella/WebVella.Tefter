using WebVella.Tefter.Exceptions;

namespace WebVella.Tefter.DataProviders.MsSql.Addons;

public class MsSqlDataProvider : ITfDataProviderAddon
{

	public const string ID = "0f86e009-1db4-497f-b10a-a55f4fad455d";
	public const string NAME = "Microsoft Sql Data Provider";
	public const string DESCRIPTION = "Provide data from Microsoft SQL server query";
	public const string FLUENT_ICON_NAME = "DocumentTable";
	public Guid AddonId { get; init;} =  new Guid(ID);
	public string AddonName { get; init;} = NAME;
	public string AddonDescription { get; init;} = DESCRIPTION;
	public string AddonFluentIconName { get; init;} =  FLUENT_ICON_NAME;

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
				return new List<TfDatabaseColumnType> { TfDatabaseColumnType.DateOnly, TfDatabaseColumnType.DateTime }.AsReadOnly();
			case "DATETIME":
				return new List<TfDatabaseColumnType> { TfDatabaseColumnType.DateTime, TfDatabaseColumnType.DateOnly }.AsReadOnly();
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
		TfDataProvider provider, 
		ITfDataProviderSychronizationLog synchLog)
	{
		var result = new List<TfDataProviderDataRow>();

		MsSqlDataProviderSettings settings = null;
		try
		{
			synchLog.Log("start loading provider settings");
			settings = JsonSerializer.Deserialize<MsSqlDataProviderSettings>(provider.SettingsJson);
			synchLog.Log("complete loading provider settings");
		}
		catch (Exception ex)
		{
			synchLog.Log("failed loading provider settings", ex);
			throw ex;
		}

		if (string.IsNullOrWhiteSpace(settings.ConnectionString))
		{
			var ex = new Exception("Connection string is not specified.");
			synchLog.Log("connection string is not specified.", ex);
			throw ex;
		}

		if (string.IsNullOrWhiteSpace(settings.SqlQuery))
		{
			var ex = new Exception("Sql query is not specified.");
			synchLog.Log("sql query is not specified", ex);
			throw ex;
		}

		return ReadDataFromSql(
			settings.ConnectionString,
			settings.SqlQuery,
			provider,
			synchLog);
	}

	TfDataProviderSourceSchemaInfo ITfDataProviderAddon.GetDataProviderSourceSchema(
		TfDataProvider provider)
	{
		var settings = JsonSerializer.Deserialize<MsSqlDataProviderSettings>(provider.SettingsJson);

		if (string.IsNullOrWhiteSpace(settings.ConnectionString))
			throw new Exception("Connection string is not specified.");

		if (string.IsNullOrWhiteSpace(settings.SqlQuery))
			throw new Exception("Sql query is not specified.");

		TfDataProviderSourceSchemaInfo schemaInfo = new TfDataProviderSourceSchemaInfo();

		SqlConnection connection = new SqlConnection(settings.ConnectionString);
		try
		{
			connection.Open();

			//TODO Rumen - rewrite code bellow to use GetSchemaTable
			//DataTable dtSchema = null;

			//using (var schemaCommand = new SqlCommand(settings.SqlQuery, connection))
			//{
			//	using (var reader = schemaCommand.ExecuteReader(CommandBehavior.SchemaOnly))
			//	{
			//		dtSchema = reader.GetSchemaTable();
			//	}
			//}

			SqlCommand command = new SqlCommand($"WITH X AS ( {settings.SqlQuery} ) SELECT TOP 1 * FROM X", connection);

			SqlDataAdapter adapter = new SqlDataAdapter(command);

			DataTable dt = new DataTable();

			adapter.Fill(dt);

			foreach (DataColumn column in dt.Columns)
			{
				if (column.DataType == typeof(Guid) || column.DataType == typeof(Guid?))
				{
					schemaInfo.SourceColumnDefaultDbType[column.ColumnName] = TfDatabaseColumnType.Guid;
					schemaInfo.SourceColumnDefaultSourceType[column.ColumnName] = "GUID";
				}
				else if (column.DataType == typeof(DateOnly) || column.DataType == typeof(DateOnly?))
				{
					schemaInfo.SourceColumnDefaultDbType[column.ColumnName] = TfDatabaseColumnType.DateOnly;
					schemaInfo.SourceColumnDefaultSourceType[column.ColumnName] = "DATE";
				}
				else if (column.DataType == typeof(DateTime) || column.DataType == typeof(DateTime?))
				{
					schemaInfo.SourceColumnDefaultDbType[column.ColumnName] = TfDatabaseColumnType.DateTime;
					schemaInfo.SourceColumnDefaultSourceType[column.ColumnName] = "DATETIME";
				}
				else if (column.DataType == typeof(short) || column.DataType == typeof(short?))
				{
					schemaInfo.SourceColumnDefaultDbType[column.ColumnName] = TfDatabaseColumnType.ShortInteger;
					schemaInfo.SourceColumnDefaultSourceType[column.ColumnName] = "SHORT_INTEGER";
				}
				else if (column.DataType == typeof(int) || column.DataType == typeof(int?))
				{
					schemaInfo.SourceColumnDefaultDbType[column.ColumnName] = TfDatabaseColumnType.Integer;
					schemaInfo.SourceColumnDefaultSourceType[column.ColumnName] = "INTEGER";
				}
				else if (column.DataType == typeof(long) || column.DataType == typeof(long?))
				{
					schemaInfo.SourceColumnDefaultDbType[column.ColumnName] = TfDatabaseColumnType.LongInteger;
					schemaInfo.SourceColumnDefaultSourceType[column.ColumnName] = "LONG_INTEGER";
				}
				else if (column.DataType == typeof(decimal) || column.DataType == typeof(decimal?))
				{
					schemaInfo.SourceColumnDefaultDbType[column.ColumnName] = TfDatabaseColumnType.Number;
					schemaInfo.SourceColumnDefaultSourceType[column.ColumnName] = "NUMBER";
				}
				else if (column.DataType == typeof(string))
				{
					schemaInfo.SourceColumnDefaultDbType[column.ColumnName] = TfDatabaseColumnType.Text;
					schemaInfo.SourceColumnDefaultSourceType[column.ColumnName] = "TEXT";
				}
				else if (column.DataType == typeof(bool))
				{
					schemaInfo.SourceColumnDefaultDbType[column.ColumnName] = TfDatabaseColumnType.Boolean;
					schemaInfo.SourceColumnDefaultSourceType[column.ColumnName] = "BOOLEAN";
				}

			}
			schemaInfo.SourceTypeSupportedDbTypes = new Dictionary<string, List<TfDatabaseColumnType>>();
			foreach (var providerDataType in provider.ProviderType.GetSupportedSourceDataTypes())
			{
				var supportedDBList = provider.ProviderType.GetDatabaseColumnTypesForSourceDataType(providerDataType);
				var supportedDbType = supportedDBList.Count > 0 ? supportedDBList.First() : TfDatabaseColumnType.Text;
				schemaInfo.SourceTypeSupportedDbTypes[providerDataType] = supportedDBList.ToList();
			}

		}
		finally
		{
			connection.Close();
		}

		return schemaInfo;
	}

	private ReadOnlyCollection<TfDataProviderDataRow> ReadDataFromSql(
		string connectionString,
		string sql,
		TfDataProvider provider,
		ITfDataProviderSychronizationLog synchLog)
	{
		var result = new List<TfDataProviderDataRow>();

		SqlConnection connection = new SqlConnection(connectionString);
		try
		{
			synchLog.Log($"trying to open connection to sql server");

			connection.Open();

			synchLog.Log($"connnection to sql server opened successfully");

			SqlCommand command = new SqlCommand(sql, connection);

			SqlDataAdapter adapter = new SqlDataAdapter(command);

			DataTable dt = new DataTable();

			synchLog.Log($"start executing sql query and get data");
			
			adapter.Fill(dt);
			
			synchLog.Log($"complete getting data from sql");

			var providerColumnsWithSourceName = provider.Columns.Where(x => !string.IsNullOrWhiteSpace(x.SourceName));

			if (!providerColumnsWithSourceName.Any())
			{
				var ex =  new Exception($"Found no column with source name in provider.");
				synchLog.Log($"found no column with source name in provider", ex);
				throw ex;
			}

			HashSet<string> sourceColumns = new HashSet<string>();
			foreach (DataColumn column in dt.Columns)
			{
				sourceColumns.Add(column.ColumnName.ToLowerInvariant());
			}

			int rowCounter = 1;
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
							var ex = new Exception($"Source column '{sourceName}' is not found in query result.");
							synchLog.Log($"source column '{sourceName}' is not found in query result", ex);
							throw ex;
						}

						try
						{
							row[providerColumnWithSource.DbName] = ConvertValue(
									providerColumnWithSource,
									dr[sourceName]);
						}
						catch (Exception ex)
						{
							var value = dr[sourceName];
							synchLog.Log($"failed to process value for row index={rowCounter}, source column='{sourceName}'," +
								$" provider column='{providerColumnWithSource.DbName}', provider column type='{providerColumnWithSource.DbType}'," +
								$"  value='{value}'", ex);
							throw ex;
						}
					}
					catch (Exception ex)
					{
						throw new Exception($"Exception while processing source row" +
							$" ({sourceName}): {ex.Message}");
					}
				}
				result.Add(row);
				rowCounter++;
			}

			synchLog.Log($"successfully processed {rowCounter - 1} rows from sql server query result");
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

			case TfDatabaseColumnType.DateOnly:
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

	public List<ValidationError> Validate(string settingsJson)
	{
		throw new NotImplementedException();
	}
}
