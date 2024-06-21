namespace WebVella.Tefter.Database;

public partial interface IDatabaseManager
{
	DatabaseBuilder GetDatabaseBuilder();
	DatabaseUpdateResult SaveChanges(DatabaseBuilder databaseBuilder);
}

public partial class DatabaseManager : IDatabaseManager
{
	private readonly IDatabaseService _dbService;

	public DatabaseManager(IServiceProvider serviceProvider)
	{
		_dbService = serviceProvider.GetService<IDatabaseService>();
	}

	public DatabaseBuilder GetDatabaseBuilder()
	{
		var databaseBuilder = DatabaseBuilder.New();

		using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
		{
			#region <---tables--->

			Dictionary<string, DatabaseTableBuilder> tableBuildersDict = new Dictionary<string, DatabaseTableBuilder>();

			DataTable dtTables = _dbService.ExecuteSqlQueryCommand(DatabaseSqlProvider.GetTablesMetaSql());
			foreach (DataRow row in dtTables.Rows)
			{
				DatabaseTableMeta meta = null;
				//ignore tables with comments cant deserialize to meta object 
				try { meta = JsonSerializer.Deserialize<DatabaseTableMeta>((string)row["meta"]); } catch { continue; }

				string name = (string)row["table_name"];
				tableBuildersDict[name] = databaseBuilder
					.NewTableBuilder(meta.Id, name)
					.WithApplicationId(meta.ApplicationId)
					.WithDataProviderId(meta.DataProviderId);
			}

			#endregion

			#region <--- columns --->

			DataTable dtColumns = _dbService.ExecuteSqlQueryCommand(DatabaseSqlProvider.GetColumnsMetaSql());
			foreach (DataRow row in dtColumns.Rows)
			{
				//if table is not found in tables dictionary -> skip it
				var tableName = (string)row["table_name"];
				if (!tableBuildersDict.ContainsKey(tableName))
					continue;

				var tableBuilder = tableBuildersDict[tableName];

				DatabaseColumnMeta meta = null;
				//ignore columns with comments cant deserialize to meta object 
				try { meta = JsonSerializer.Deserialize<DatabaseColumnMeta>((string)row["meta"]); } catch { continue; }

				var columnName = (string)row["column_name"];
				var defaultValue = row["column_default"] == DBNull.Value ? null : ((string)row["column_default"]);
				var isNullable = ((string)row["is_nullable"]).ToLower() == "yes";
				var dbType = (string)row["data_type"];

				var columnCollectionBuilder = tableBuilder.WithColumnsBuilder();

				switch (dbType)
				{
					case "uuid":
						{
							Guid? guidDefaultValue = (Guid?)DatabaseUtility.ConvertDatabaseDefaultValueToDbColumnDefaultValue(columnName, typeof(GuidDatabaseColumn), defaultValue);

							bool isAutoDefaultValue = defaultValue?.Trim() == Constants.DB_GUID_COLUMN_AUTO_DEFAULT_VALUE;

							var columnBuider = columnCollectionBuilder
								.AddGuidColumnBuilder(meta.Id, columnName)
								.WithDefaultValue(guidDefaultValue)
								.WithLastCommited(meta.LastCommited);

							if (isNullable)
								columnBuider.Nullable();
							else
								columnBuider.NotNullable();

							if (isAutoDefaultValue)
								columnBuider.WithAutoDefaultValue();
							else
								columnBuider.WithoutAutoDefaultValue();
						}
						break;
					case "integer":
						{
							var columnBuider = columnCollectionBuilder
								.AddAutoIncrementColumnBuilder(meta.Id, columnName)
								.WithLastCommited(meta.LastCommited)
								.WithLastCommited(meta.LastCommited);
						}
						break;
					case "boolean":
						{
							bool? columnDefaultValue = (bool?)DatabaseUtility.ConvertDatabaseDefaultValueToDbColumnDefaultValue(columnName, typeof(BooleanDatabaseColumn), defaultValue);

							var columnBuider = columnCollectionBuilder
								.AddBooleanColumnBuilder(meta.Id, columnName)
								.WithDefaultValue(columnDefaultValue)
								.WithLastCommited(meta.LastCommited);

							if (isNullable)
								columnBuider.Nullable();
							else
								columnBuider.NotNullable();
						}
						break;
					case "numeric":
						{
							decimal? columnDefaultValue = (decimal?)DatabaseUtility.ConvertDatabaseDefaultValueToDbColumnDefaultValue(columnName, typeof(NumberDatabaseColumn), defaultValue);

							var columnBuider = columnCollectionBuilder
								.AddNumberColumnBuilder(meta.Id, columnName)
								.WithDefaultValue(columnDefaultValue);

							if (isNullable)
								columnBuider.Nullable();
							else
								columnBuider.NotNullable();

							columnBuider.WithLastCommited(meta.LastCommited);
						}
						break;
					case "date":
						{
							DateOnly? columnDefaultValue = (DateOnly?)DatabaseUtility.ConvertDatabaseDefaultValueToDbColumnDefaultValue(columnName, typeof(DateDatabaseColumn), defaultValue);

							bool isAutoDefaultValue = defaultValue?.Trim() == Constants.DB_DATETIME_COLUMN_AUTO_DEFAULT_VALUE;

							var columnBuider = columnCollectionBuilder
								.AddDateColumnBuilder(meta.Id, columnName)
								.WithDefaultValue(columnDefaultValue)
								.WithLastCommited(meta.LastCommited);

							if (isNullable)
								columnBuider.Nullable();
							else
								columnBuider.NotNullable();

							if (isAutoDefaultValue)
								columnBuider.WithAutoDefaultValue();
							else
								columnBuider.WithoutAutoDefaultValue();
						}
						break;
					case "timestamp with time zone":
						{
							DateTime? columnDefaultValue = (DateTime?)DatabaseUtility.ConvertDatabaseDefaultValueToDbColumnDefaultValue(columnName, typeof(DateTimeDatabaseColumn), defaultValue);

							bool isAutoDefaultValue = defaultValue?.Trim() == Constants.DB_DATETIME_COLUMN_AUTO_DEFAULT_VALUE;

							var columnBuider = columnCollectionBuilder
								.AddDateTimeColumnBuilder(meta.Id, columnName)
								.WithDefaultValue(columnDefaultValue)
								.WithLastCommited(meta.LastCommited);

							if (isNullable)
								columnBuider.Nullable();
							else
								columnBuider.NotNullable();

							if (isAutoDefaultValue)
								columnBuider.WithAutoDefaultValue();
							else
								columnBuider.WithoutAutoDefaultValue();
						}
						break;
					case "text":
						{
							string columnDefaultValue = (string)DatabaseUtility
								.ConvertDatabaseDefaultValueToDbColumnDefaultValue(columnName, typeof(TextDatabaseColumn), defaultValue);

							var columnBuider = columnCollectionBuilder
								.AddTextColumnBuilder(meta.Id, columnName)
								.WithDefaultValue(columnDefaultValue)
								.WithLastCommited(meta.LastCommited);

							if (isNullable)
								columnBuider.Nullable();
							else
								columnBuider.NotNullable();
						}
						break;
					default:
						throw new DatabaseException($"Not supported dbType for column '{columnName}'");
				};
			}

			#endregion

			#region <--- Constraints --->

			Dictionary<string, string> constraintTableDict = new Dictionary<string, string>();
			Dictionary<string, List<string>> constraintColumnsDict = new Dictionary<string, List<string>>();
			Dictionary<string, List<string>> constraintForeignColumnsDict = new Dictionary<string, List<string>>();
			Dictionary<string, string> constraintForeignTableDict = new Dictionary<string, string>();
			Dictionary<string, char> constraintTypeDict = new Dictionary<string, char>();

			DataTable dtConstraints = _dbService.ExecuteSqlQueryCommand(DatabaseSqlProvider.GetConstraintsMetaSql());
			foreach (DataRow row in dtConstraints.Rows)
			{
				var constraintName = (string)row["constraint_name"];
				if (!constraintTableDict.ContainsKey(constraintName))
					constraintTableDict[constraintName] = (string)row["table_name"];
				if (!constraintColumnsDict.ContainsKey(constraintName))
					constraintColumnsDict[constraintName] = new List<string>();
				if (!constraintTypeDict.ContainsKey(constraintName))
					constraintTypeDict[constraintName] = (char)row["constraint_type"];
				if (!constraintForeignColumnsDict.ContainsKey(constraintName))
					constraintForeignColumnsDict[constraintName] = new List<string>();

				constraintColumnsDict[constraintName].Add((string)row["column_name"]);

				if (row["foreign_column_name"] != DBNull.Value && !string.IsNullOrEmpty((string)row["foreign_column_name"]))
					constraintForeignColumnsDict[constraintName].Add((string)row["foreign_column_name"]);

				if (row["foreign_table_name"] != DBNull.Value && !string.IsNullOrEmpty((string)row["foreign_table_name"]))
					constraintForeignTableDict[constraintName] = ((string)row["foreign_table_name"]);
			}

			foreach (var constraintName in constraintTableDict.Keys)
			{
				//if table is not found in tables dictionary -> skip it
				var tableName = constraintTableDict[constraintName];
				if (!tableBuildersDict.ContainsKey(tableName))
					continue;

				var tableBuilder = tableBuildersDict[tableName];

				var constraintsBuilder = tableBuilder.WithConstraintsBuilder();

				switch (constraintTypeDict[constraintName])
				{
					case 'p':
						{
							constraintsBuilder
								.AddPrimaryKeyConstraintBuilder(constraintName)
								.WithColumns(constraintColumnsDict[constraintName].ToArray());
						}
						break;
					case 'u':
						{
							constraintsBuilder
								.AddUniqueKeyConstraintBuilder(constraintName)
								.WithColumns(constraintColumnsDict[constraintName].ToArray());
						}
						break;
					case 'f':
						{
							constraintsBuilder
								.AddForeignKeyConstraintBuilder(constraintName)
								.WithColumns(constraintColumnsDict[constraintName].ToArray())
								.WithForeignTable(constraintForeignTableDict[constraintName])
								.WithForeignColumns(constraintForeignColumnsDict[constraintName].ToArray());
						}
						break;
					default:
						continue;
				}
			}
			#endregion

			#region <--- Indexes --->

			Dictionary<string, string> indexTableDict = new Dictionary<string, string>();
			Dictionary<string, List<string>> indexColumnsDict = new Dictionary<string, List<string>>();
			Dictionary<string, string> indexTypeDict = new Dictionary<string, string>();
			DataTable dtIndexes = _dbService.ExecuteSqlQueryCommand(DatabaseSqlProvider.GetIndexesMetaSql());
			foreach (DataRow row in dtIndexes.Rows)
			{
				var indexName = (string)row["index_name"];
				if (!indexTableDict.ContainsKey(indexName))
					indexTableDict[indexName] = (string)row["table_name"];
				if (!indexColumnsDict.ContainsKey(indexName))
					indexColumnsDict[indexName] = new List<string>();
				if (!indexTypeDict.ContainsKey(indexName))
					indexTypeDict[indexName] = (string)row["index_type"];

				indexColumnsDict[indexName].Add((string)row["column_name"]);
			}

			foreach (var indexName in indexTableDict.Keys)
			{
				//if table is not found in tables dictionary -> skip it
				var tableName = indexTableDict[indexName];
				if (!tableBuildersDict.ContainsKey(tableName))
					continue;

				var tableBuilder = tableBuildersDict[tableName];

				var indexesBuilder = tableBuilder.WithIndexesBuilder();

				switch (indexTypeDict[indexName])
				{
					case "btree":
						{
							indexesBuilder
								.AddBTreeIndexBuilder(indexName)
								.WithColumns(indexColumnsDict[indexName].ToArray());
						}
						break;
					case "gin":
						{
							indexesBuilder
								.AddGinIndexBuilder(indexName)
								.WithColumns(indexColumnsDict[indexName].ToArray());

						}
						break;
					case "gist":
						{
							indexesBuilder
								.AddGistIndexBuilder(indexName)
								.WithColumns(indexColumnsDict[indexName].ToArray());
						}
						break;
					case "hash":
						{
							indexesBuilder
								.AddHashIndexBuilder(indexName)
								.WithColumn(indexColumnsDict[indexName][0]);
						}
						break;
					default:
						continue;
				}
			}

			#endregion
		}

		return databaseBuilder;
	}

	public DatabaseUpdateResult SaveChanges(DatabaseBuilder databaseBuilder)
	{
		if (databaseBuilder == null)
			throw new ArgumentNullException(nameof(databaseBuilder));

		var updatedDatabase = databaseBuilder.Build();
		var currentDatabase = GetDatabaseBuilder().Build();

		var differences = DatabaseComparer.Compare(currentDatabase, updatedDatabase);
		var updateScript = DatabaseSqlProvider.GenerateUpdateScript(differences);
		var cleanupScript = DatabaseSqlProvider.GetUpdateCleanupScript();

		List<DatabaseUpdateLogRecord> log = new List<DatabaseUpdateLogRecord>();

		if (differences.Count == 0)
			return new DatabaseUpdateResult(log);

		using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
		{
			var dtLogs = _dbService.ExecuteSqlQueryCommand(updateScript);

			foreach (DataRow row in dtLogs.Rows)
			{
				var createdOn = (DateTime)row["created_on"];
				var statement = (string)row["statement"];
				var success = (bool)row["success"];
				var sqlErr = row["sql_error"] == DBNull.Value ? string.Empty : (string)row["sql_error"];

				log.Add(new DatabaseUpdateLogRecord
				{
					CreatedOn = createdOn,
					Statement = statement,
					Success = success,
					SqlError = sqlErr
				});
			}

			_dbService.ExecuteSqlNonQueryCommand(cleanupScript);

			var result = new DatabaseUpdateResult(log);

			if (result.IsSuccess)
				scope.Complete();
			else
				throw new DatabaseUpdateException(result);

			return result;
		}
	}
}
