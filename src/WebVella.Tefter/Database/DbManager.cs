namespace WebVella.Tefter.Database;

public partial interface IDbManager
{
	DatabaseBuilder GetDatabaseBuilder();
	DbUpdateResult SaveChanges(DatabaseBuilder databaseBuilder);
}

public partial class DbManager : IDbManager
{
	private readonly IDbService _dbService;

	public DbManager(IServiceProvider serviceProvider)
	{
		_dbService = serviceProvider.GetService<IDbService>();
	}

	public DatabaseBuilder GetDatabaseBuilder()
	{
		var databaseBuilder = DatabaseBuilder.New();

		using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
		{
			#region <---tables--->

			Dictionary<string, DbTableBuilder> tableBuildersDict = new Dictionary<string, DbTableBuilder>();

			DataTable dtTables = _dbService.ExecuteSqlQueryCommand(DbSqlProvider.GetTablesMetaSql());
			foreach (DataRow row in dtTables.Rows)
			{
				DbTableMeta meta = null;
				//ignore tables with comments cant deserialize to meta object 
				try { meta = JsonSerializer.Deserialize<DbTableMeta>((string)row["meta"]); } catch { continue; }

				string name = (string)row["table_name"];
				tableBuildersDict[name] = databaseBuilder
					.NewTableBuilder(meta.Id, name)
					.WithApplicationId(meta.ApplicationId)
					.WithDataProviderId(meta.DataProviderId);
			}

			#endregion

			#region <--- columns --->

			DataTable dtColumns = _dbService.ExecuteSqlQueryCommand(DbSqlProvider.GetColumnsMetaSql());
			foreach (DataRow row in dtColumns.Rows)
			{
				//if table is not found in tables dictionary -> skip it
				var tableName = (string)row["table_name"];
				if (!tableBuildersDict.ContainsKey(tableName))
					continue;

				var tableBuilder = tableBuildersDict[tableName];

				DbColumnMeta meta = null;
				//ignore columns with comments cant deserialize to meta object 
				try { meta = JsonSerializer.Deserialize<DbColumnMeta>((string)row["meta"]); } catch { continue; }

				var columnName = (string)row["column_name"];
				var defaultValue = row["column_default"] == DBNull.Value ? null : ((string)row["column_default"]);
				var isNullable = ((string)row["is_nullable"]).ToLower() == "yes";
				var dbType = (string)row["data_type"];

				var columnCollectionBuilder = tableBuilder.WithColumnsBuilder();

				switch (dbType)
				{
					case "uuid":
						{
							if (columnName == Constants.DB_TABLE_ID_COLUMN_NAME)
							{
								var columnBuider = columnCollectionBuilder
									.AddTableIdColumnBuilder(meta.Id)
									.WithLastCommited(meta.LastCommited);
							}
							else
							{
								Guid? guidDefaultValue = (Guid?)DbUtility.ConvertDatabaseDefaultValueToDbColumnDefaultValue(columnName, typeof(DbGuidColumn), defaultValue);

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
							bool? columnDefaultValue = (bool?)DbUtility.ConvertDatabaseDefaultValueToDbColumnDefaultValue(columnName, typeof(DbBooleanColumn), defaultValue);

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
							decimal? columnDefaultValue = (decimal?)DbUtility.ConvertDatabaseDefaultValueToDbColumnDefaultValue(columnName, typeof(DbNumberColumn), defaultValue);

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
							DateOnly? columnDefaultValue = (DateOnly?)DbUtility.ConvertDatabaseDefaultValueToDbColumnDefaultValue(columnName, typeof(DbDateColumn), defaultValue);

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
							DateTime? columnDefaultValue = (DateTime?)DbUtility.ConvertDatabaseDefaultValueToDbColumnDefaultValue(columnName, typeof(DbDateTimeColumn), defaultValue);

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
							string columnDefaultValue = (string)DbUtility
								.ConvertDatabaseDefaultValueToDbColumnDefaultValue(columnName, typeof(DbTextColumn), defaultValue);

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
						throw new DbException($"Not supported dbType for column '{columnName}'");
				};
			}

			#endregion

			#region <--- Constraints --->

			Dictionary<string, string> constraintTableDict = new Dictionary<string, string>();
			Dictionary<string, List<string>> constraintColumnsDict = new Dictionary<string, List<string>>();
			Dictionary<string, List<string>> constraintForeignColumnsDict = new Dictionary<string, List<string>>();
			Dictionary<string, string> constraintForeignTableDict = new Dictionary<string, string>();
			Dictionary<string, char> constraintTypeDict = new Dictionary<string, char>();

			DataTable dtConstraints = _dbService.ExecuteSqlQueryCommand(DbSqlProvider.GetConstraintsMetaSql());
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
			DataTable dtIndexes = _dbService.ExecuteSqlQueryCommand(DbSqlProvider.GetIndexesMetaSql());
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

	public DbUpdateResult SaveChanges(DatabaseBuilder databaseBuilder)
	{
		if (databaseBuilder == null)
			throw new ArgumentNullException(nameof(databaseBuilder));

		var updatedDatabase = databaseBuilder.Build();
		var currentDatabase = GetDatabaseBuilder().Build();

		var differences = DatabaseComparer.Compare(currentDatabase, updatedDatabase);
		var updateScript = DbSqlProvider.GenerateUpdateScript(differences);
		var cleanupScript = DbSqlProvider.GetUpdateCleanupScript();

		List<DbUpdateLogRecord> log = new List<DbUpdateLogRecord>();

		using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
		{
			var dtLogs = _dbService.ExecuteSqlQueryCommand(updateScript);

			foreach (DataRow row in dtLogs.Rows)
			{
				var createdOn = (DateTime)row["created_on"];
				var statement = (string)row["statement"];
				var success = (bool)row["success"];
				var sqlErr = row["sql_error"] == DBNull.Value ? string.Empty : (string)row["sql_error"];

				log.Add(new DbUpdateLogRecord
				{
					CreatedOn = createdOn,
					Statement = statement,
					Success = success,
					SqlError = sqlErr
				});
			}

			_dbService.ExecuteSqlNonQueryCommand(cleanupScript);

			var result = new DbUpdateResult(log);

			if (result.IsSuccess)
				scope.Complete();
			else
				throw new DbUpdateException(result);

			return result;
		}
	}
}
