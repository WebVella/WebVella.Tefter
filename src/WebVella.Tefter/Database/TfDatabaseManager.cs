namespace WebVella.Tefter.Database;

public partial interface ITfDatabaseManager
{
	TfDatabaseBuilder GetDatabaseBuilder();
	TfDatabaseUpdateResult SaveChanges(TfDatabaseBuilder databaseBuilder);
	internal TfDatabaseUpdateResult CloneTableForSynch(string tableToClone);
}

public partial class TfDatabaseManager : ITfDatabaseManager
{
	private readonly ITfDatabaseService _dbService;

	public TfDatabaseManager(IServiceProvider serviceProvider)
	{
		_dbService = serviceProvider.GetService<ITfDatabaseService>();
	}

	public TfDatabaseBuilder GetDatabaseBuilder()
	{
		var databaseBuilder = TfDatabaseBuilder.New();

		using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
		{
			#region <---tables--->

			Dictionary<string, TfDatabaseTableBuilder> tableBuildersDict = new Dictionary<string, TfDatabaseTableBuilder>();

			DataTable dtTables = _dbService.ExecuteSqlQueryCommand(TfDatabaseSqlProvider.GetTablesMetaSql());
			foreach (DataRow row in dtTables.Rows)
			{
				TfDatabaseTableMeta meta = null;
				//ignore tables with comments cant deserialize to meta object 
				try { meta = JsonSerializer.Deserialize<TfDatabaseTableMeta>((string)row["meta"]); } catch { continue; }

				string name = (string)row["table_name"];
				tableBuildersDict[name] = databaseBuilder
					.NewTableBuilder(meta.Id, name)
					.WithApplicationId(meta.ApplicationId)
					.WithDataProviderId(meta.DataProviderId);
			}

			#endregion

			#region <--- columns --->

			DataTable dtColumns = _dbService.ExecuteSqlQueryCommand(TfDatabaseSqlProvider.GetColumnsMetaSql());
			foreach (DataRow row in dtColumns.Rows)
			{
				//if table is not found in tables dictionary -> skip it
				var tableName = (string)row["table_name"];
				if (!tableBuildersDict.ContainsKey(tableName))
					continue;

				var tableBuilder = tableBuildersDict[tableName];

				TfDatabaseColumnMeta meta = null;
				//ignore columns with comments cant deserialize to meta object 
				try { meta = JsonSerializer.Deserialize<TfDatabaseColumnMeta>((string)row["meta"]); } catch { continue; }

				var columnName = (string)row["column_name"];
				var defaultValue = row["column_default"] == DBNull.Value ? null : ((string)row["column_default"]);
				var isNullable = ((string)row["is_nullable"]).ToLower() == "yes";
				var dbType = (string)row["data_type"];
				var isGenerated = (string)row["is_generated"] == "ALWAYS";
				var generationExpression = row["generation_expression"] == DBNull.Value ? null : (string)row["generation_expression"];

				var columnCollectionBuilder = tableBuilder.WithColumnsBuilder();

				switch (meta.ColumnType)
				{
					case TfDatabaseColumnType.Guid:
						{
							if (isGenerated && !string.IsNullOrWhiteSpace(generationExpression))
							{
								columnCollectionBuilder
									.AddGuidColumnBuilder(meta.Id, columnName)
									.AsExpression(generationExpression)
									.WithLastCommited(meta.LastCommited);
							}
							else
							{
								Guid? guidDefaultValue = (Guid?)TfDatabaseUtility.ConvertDatabaseDefaultValueToDbColumnDefaultValue(columnName, typeof(TfGuidDatabaseColumn), defaultValue);

								bool isAutoDefaultValue = defaultValue?.Trim() == TfConstants.DB_GUID_COLUMN_AUTO_DEFAULT_VALUE;

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
					case TfDatabaseColumnType.Boolean:
						{
							if (isGenerated && !string.IsNullOrWhiteSpace(generationExpression))
							{
								columnCollectionBuilder
									.AddBooleanColumnBuilder(meta.Id, columnName)
									.AsExpression(generationExpression)
									.WithLastCommited(meta.LastCommited);
							}
							else
							{
								bool? columnDefaultValue = (bool?)TfDatabaseUtility.ConvertDatabaseDefaultValueToDbColumnDefaultValue(columnName, typeof(TfBooleanDatabaseColumn), defaultValue);

								var columnBuider = columnCollectionBuilder
									.AddBooleanColumnBuilder(meta.Id, columnName)
									.WithDefaultValue(columnDefaultValue)
									.WithLastCommited(meta.LastCommited);

								if (isNullable)
									columnBuider.Nullable();
								else
									columnBuider.NotNullable();
							}
						}
						break;
					case TfDatabaseColumnType.Number:
						{
							if (isGenerated && !string.IsNullOrWhiteSpace(generationExpression))
							{
								columnCollectionBuilder
									.AddNumberColumnBuilder(meta.Id, columnName)
									.AsExpression(generationExpression)
									.WithLastCommited(meta.LastCommited);
							}
							else
							{
								decimal? columnDefaultValue = (decimal?)TfDatabaseUtility.ConvertDatabaseDefaultValueToDbColumnDefaultValue(columnName, typeof(TfNumberDatabaseColumn), defaultValue);

								var columnBuider = columnCollectionBuilder
									.AddNumberColumnBuilder(meta.Id, columnName)
									.WithDefaultValue(columnDefaultValue);

								if (isNullable)
									columnBuider.Nullable();
								else
									columnBuider.NotNullable();

								columnBuider.WithLastCommited(meta.LastCommited);
							}
						}
						break;
					case TfDatabaseColumnType.DateOnly:
						{
							if (isGenerated && !string.IsNullOrWhiteSpace(generationExpression))
							{
								columnCollectionBuilder
									.AddDateColumnBuilder(meta.Id, columnName)
									.AsExpression(generationExpression)
									.WithLastCommited(meta.LastCommited);
							}
							else
							{
								DateOnly? columnDefaultValue = (DateOnly?)TfDatabaseUtility.ConvertDatabaseDefaultValueToDbColumnDefaultValue(columnName, typeof(TfDateDatabaseColumn), defaultValue);

								bool isAutoDefaultValue = defaultValue?.Trim() == TfConstants.DB_DATETIME_COLUMN_AUTO_DEFAULT_VALUE;

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
						}
						break;
					case TfDatabaseColumnType.DateTime:
						{
							if (isGenerated && !string.IsNullOrWhiteSpace(generationExpression))
							{
								columnCollectionBuilder
									.AddDateTimeColumnBuilder(meta.Id, columnName)
									.AsExpression(generationExpression)
									.WithLastCommited(meta.LastCommited);
							}
							else
							{
								DateTime? columnDefaultValue = (DateTime?)TfDatabaseUtility.ConvertDatabaseDefaultValueToDbColumnDefaultValue(columnName, typeof(TfDateTimeDatabaseColumn), defaultValue);

								bool isAutoDefaultValue = defaultValue?.Trim() == TfConstants.DB_DATETIME_COLUMN_AUTO_DEFAULT_VALUE;

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
						}
						break;
					case TfDatabaseColumnType.Text:
						{
							if (isGenerated && !string.IsNullOrWhiteSpace(generationExpression))
							{
								columnCollectionBuilder
									.AddTextColumnBuilder(meta.Id, columnName)
									.AsExpression(generationExpression)
									.WithLastCommited(meta.LastCommited);
							}
							else
							{
								string columnDefaultValue = (string)TfDatabaseUtility
								.ConvertDatabaseDefaultValueToDbColumnDefaultValue(columnName, typeof(TfTextDatabaseColumn), defaultValue);

								var columnBuider = columnCollectionBuilder
									.AddTextColumnBuilder(meta.Id, columnName)
									.WithDefaultValue(columnDefaultValue)
									.WithLastCommited(meta.LastCommited);

								if (isNullable)
									columnBuider.Nullable();
								else
									columnBuider.NotNullable();
							}
						}
						break;
					case TfDatabaseColumnType.ShortText:
						{
							if (isGenerated && !string.IsNullOrWhiteSpace(generationExpression))
							{
								columnCollectionBuilder
									.AddShortTextColumnBuilder(meta.Id, columnName)
									.AsExpression(generationExpression)
									.WithLastCommited(meta.LastCommited);
							}
							else
							{
								string columnDefaultValue = (string)TfDatabaseUtility
									.ConvertDatabaseDefaultValueToDbColumnDefaultValue(columnName, typeof(TfShortTextDatabaseColumn), defaultValue);

								bool isAutoDefaultValue = defaultValue?.Trim() == TfConstants.DB_SHORT_TEXT_COLUMN_AUTO_SHA1_DEFAULT_VALUE;

								var columnBuider = columnCollectionBuilder
									.AddShortTextColumnBuilder(meta.Id, columnName)
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
						}
						break;
					case TfDatabaseColumnType.ShortInteger:
						{
							if (isGenerated && !string.IsNullOrWhiteSpace(generationExpression))
							{
								columnCollectionBuilder
									.AddShortIntegerColumnBuilder(meta.Id, columnName)
									.AsExpression(generationExpression)
									.WithLastCommited(meta.LastCommited);
							}
							else
							{
								short? columnDefaultValue = (short?)TfDatabaseUtility.ConvertDatabaseDefaultValueToDbColumnDefaultValue(columnName,
									typeof(TfShortIntegerDatabaseColumn), defaultValue);

								bool isAutoDefaultValue = defaultValue?.Trim() == TfConstants.DB_SHORT_INT_COLUMN_AUTO_DEFAULT_VALUE;

								var columnBuider = columnCollectionBuilder
									.AddShortIntegerColumnBuilder(meta.Id, columnName)
									.WithDefaultValue(columnDefaultValue);

								if (isNullable)
									columnBuider.Nullable();
								else
									columnBuider.NotNullable();

								if (isAutoDefaultValue)
									columnBuider.WithAutoDefaultValue();
								else
									columnBuider.WithoutAutoDefaultValue();

								columnBuider.WithLastCommited(meta.LastCommited);
							}
						}
						break;
					case TfDatabaseColumnType.Integer:
						{
							if (isGenerated && !string.IsNullOrWhiteSpace(generationExpression))
							{
								columnCollectionBuilder
									.AddIntegerColumnBuilder(meta.Id, columnName)
									.AsExpression(generationExpression)
									.WithLastCommited(meta.LastCommited);
							}
							else
							{
								int? columnDefaultValue = (int?)TfDatabaseUtility.ConvertDatabaseDefaultValueToDbColumnDefaultValue(columnName,
									typeof(TfIntegerDatabaseColumn), defaultValue);

								bool isAutoDefaultValue = defaultValue?.Trim() == TfConstants.DB_INT_COLUMN_AUTO_DEFAULT_VALUE;

								var columnBuider = columnCollectionBuilder
									.AddIntegerColumnBuilder(meta.Id, columnName)
									.WithDefaultValue(columnDefaultValue);

								if (isNullable)
									columnBuider.Nullable();
								else
									columnBuider.NotNullable();

								if (isAutoDefaultValue)
									columnBuider.WithAutoDefaultValue();
								else
									columnBuider.WithoutAutoDefaultValue();

								columnBuider.WithLastCommited(meta.LastCommited);
							}
						}
						break;
					case TfDatabaseColumnType.LongInteger:
						{
							if (isGenerated && !string.IsNullOrWhiteSpace(generationExpression))
							{
								columnCollectionBuilder
									.AddLongIntegerColumnBuilder(meta.Id, columnName)
									.AsExpression(generationExpression)
									.WithLastCommited(meta.LastCommited);
							}
							else
							{
								long? columnDefaultValue = (long?)TfDatabaseUtility.ConvertDatabaseDefaultValueToDbColumnDefaultValue(columnName,
									typeof(TfLongIntegerDatabaseColumn), defaultValue);

								bool isAutoDefaultValue = defaultValue?.Trim() == TfConstants.DB_LONG_INT_COLUMN_AUTO_DEFAULT_VALUE;

								var columnBuider = columnCollectionBuilder
									.AddLongIntegerColumnBuilder(meta.Id, columnName)
									.WithDefaultValue(columnDefaultValue);

								if (isNullable)
									columnBuider.Nullable();
								else
									columnBuider.NotNullable();

								if (isAutoDefaultValue)
									columnBuider.WithAutoDefaultValue();
								else
									columnBuider.WithoutAutoDefaultValue();

								columnBuider.WithLastCommited(meta.LastCommited);
							}
						}
						break;
					default:
						throw new TfDatabaseException($"Not supported dbType for column '{columnName}'");
				}
				;
			}

			#endregion

			#region <--- Constraints --->

			Dictionary<string, string> constraintTableDict = new Dictionary<string, string>();
			Dictionary<string, List<string>> constraintColumnsDict = new Dictionary<string, List<string>>();
			Dictionary<string, List<string>> constraintForeignColumnsDict = new Dictionary<string, List<string>>();
			Dictionary<string, string> constraintForeignTableDict = new Dictionary<string, string>();
			Dictionary<string, char> constraintTypeDict = new Dictionary<string, char>();

			DataTable dtConstraints = _dbService.ExecuteSqlQueryCommand(TfDatabaseSqlProvider.GetConstraintsMetaSql());
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
			DataTable dtIndexes = _dbService.ExecuteSqlQueryCommand(TfDatabaseSqlProvider.GetIndexesMetaSql());
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

	public TfDatabaseUpdateResult CloneTableForSynch(string tableToClone)
	{
		var databaseBuilder = GetDatabaseBuilder();

		const string suffix = "sync";

		var cloneTableName = $"{tableToClone}_{suffix}";

		using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
		{
			#region <---tables--->

			TfDatabaseTableBuilder newTableBuilder = null;

			DataTable dtTables = _dbService.ExecuteSqlQueryCommand(TfDatabaseSqlProvider.GetTablesMetaSql());
			foreach (DataRow row in dtTables.Rows)
			{
				TfDatabaseTableMeta meta = null;
				//ignore tables with comments cant deserialize to meta object 
				try { meta = JsonSerializer.Deserialize<TfDatabaseTableMeta>((string)row["meta"]); } catch { continue; }

				string name = (string)row["table_name"];

				if (name == tableToClone)
				{
					newTableBuilder = databaseBuilder
						.NewTableBuilder(Guid.NewGuid(), cloneTableName)
						.WithApplicationId(meta.ApplicationId)
						.WithDataProviderId(meta.DataProviderId);
				}
			}

			#endregion

			if (newTableBuilder == null)
			{
				throw new Exception("Table to clone was not found in tefter database structures");
			}

			#region <--- columns --->

			DataTable dtColumns = _dbService.ExecuteSqlQueryCommand(TfDatabaseSqlProvider.GetColumnsMetaSql());
			foreach (DataRow row in dtColumns.Rows)
			{

				var tableName = (string)row["table_name"];

				if (tableName != tableToClone)
					continue;

				TfDatabaseColumnMeta meta = null;
				//ignore columns with comments cant deserialize to meta object 
				try { meta = JsonSerializer.Deserialize<TfDatabaseColumnMeta>((string)row["meta"]); } catch { continue; }

				var columnName = (string)row["column_name"];
				var defaultValue = row["column_default"] == DBNull.Value ? null : ((string)row["column_default"]);
				var isNullable = ((string)row["is_nullable"]).ToLower() == "yes";
				var dbType = (string)row["data_type"];
				var isGenerated = (string)row["is_generated"] == "ALWAYS";
				var generationExpression = row["generation_expression"] == DBNull.Value ? null : (string)row["generation_expression"];

				var columnCollectionBuilder = newTableBuilder.WithColumnsBuilder();
				meta!.Id = Guid.NewGuid();

				switch (meta.ColumnType)
				{
					case TfDatabaseColumnType.Guid:
						{
							if (isGenerated && !string.IsNullOrEmpty(generationExpression))
							{
								columnCollectionBuilder
									.AddGuidColumnBuilder(meta.Id, columnName)
									.AsExpression(generationExpression)
									.WithLastCommited(meta.LastCommited);
							}
							else
							{
								Guid? guidDefaultValue = (Guid?)TfDatabaseUtility.ConvertDatabaseDefaultValueToDbColumnDefaultValue(columnName, typeof(TfGuidDatabaseColumn), defaultValue);

								bool isAutoDefaultValue = defaultValue?.Trim() == TfConstants.DB_GUID_COLUMN_AUTO_DEFAULT_VALUE;

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
					case TfDatabaseColumnType.Boolean:
						{
							if (isGenerated && !string.IsNullOrEmpty(generationExpression))
							{
								columnCollectionBuilder
									.AddBooleanColumnBuilder(meta.Id, columnName)
									.AsExpression(generationExpression)
									.WithLastCommited(meta.LastCommited);
							}
							else
							{
								bool? columnDefaultValue = (bool?)TfDatabaseUtility.ConvertDatabaseDefaultValueToDbColumnDefaultValue(columnName, typeof(TfBooleanDatabaseColumn), defaultValue);

								var columnBuider = columnCollectionBuilder
									.AddBooleanColumnBuilder(meta.Id, columnName)
									.WithDefaultValue(columnDefaultValue)
									.WithLastCommited(meta.LastCommited);

								if (isNullable)
									columnBuider.Nullable();
								else
									columnBuider.NotNullable();
							}
						}
						break;
					case TfDatabaseColumnType.Number:
						{
							if (isGenerated && !string.IsNullOrEmpty(generationExpression))
							{
								columnCollectionBuilder
									.AddNumberColumnBuilder(meta.Id, columnName)
									.AsExpression(generationExpression)
									.WithLastCommited(meta.LastCommited);
							}
							else
							{
								decimal? columnDefaultValue = (decimal?)TfDatabaseUtility.ConvertDatabaseDefaultValueToDbColumnDefaultValue(columnName, typeof(TfNumberDatabaseColumn), defaultValue);

								var columnBuider = columnCollectionBuilder
									.AddNumberColumnBuilder(meta.Id, columnName)
									.WithDefaultValue(columnDefaultValue);

								if (isNullable)
									columnBuider.Nullable();
								else
									columnBuider.NotNullable();

								columnBuider.WithLastCommited(meta.LastCommited);
							}
						}
						break;
					case TfDatabaseColumnType.DateOnly:
						{
							if (isGenerated && !string.IsNullOrEmpty(generationExpression))
							{
								columnCollectionBuilder
									.AddDateColumnBuilder(meta.Id, columnName)
									.AsExpression(generationExpression)
									.WithLastCommited(meta.LastCommited);
							}
							else
							{
								DateOnly? columnDefaultValue = (DateOnly?)TfDatabaseUtility.ConvertDatabaseDefaultValueToDbColumnDefaultValue(columnName, typeof(TfDateDatabaseColumn), defaultValue);

								bool isAutoDefaultValue = defaultValue?.Trim() == TfConstants.DB_DATETIME_COLUMN_AUTO_DEFAULT_VALUE;

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
						}
						break;
					case TfDatabaseColumnType.DateTime:
						{
							if (isGenerated && !string.IsNullOrEmpty(generationExpression))
							{
								columnCollectionBuilder
									.AddDateTimeColumnBuilder(meta.Id, columnName)
									.AsExpression(generationExpression)
									.WithLastCommited(meta.LastCommited);
							}
							else
							{
								DateTime? columnDefaultValue = (DateTime?)TfDatabaseUtility.ConvertDatabaseDefaultValueToDbColumnDefaultValue(columnName, typeof(TfDateTimeDatabaseColumn), defaultValue);

								bool isAutoDefaultValue = defaultValue?.Trim() == TfConstants.DB_DATETIME_COLUMN_AUTO_DEFAULT_VALUE;

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
						}
						break;
					case TfDatabaseColumnType.Text:
						{
							if (isGenerated && !string.IsNullOrEmpty(generationExpression))
							{
								columnCollectionBuilder
									.AddTextColumnBuilder(meta.Id, columnName)
									.AsExpression(generationExpression)
									.WithLastCommited(meta.LastCommited);
							}
							else
							{
								string columnDefaultValue = (string)TfDatabaseUtility
								.ConvertDatabaseDefaultValueToDbColumnDefaultValue(columnName, typeof(TfTextDatabaseColumn), defaultValue);

								var columnBuider = columnCollectionBuilder
									.AddTextColumnBuilder(meta.Id, columnName)
									.WithDefaultValue(columnDefaultValue)
									.WithLastCommited(meta.LastCommited);

								if (isNullable)
									columnBuider.Nullable();
								else
									columnBuider.NotNullable();
							}
						}
						break;
					case TfDatabaseColumnType.ShortText:
						{
							if (isGenerated && !string.IsNullOrEmpty(generationExpression))
							{
								columnCollectionBuilder
									.AddShortTextColumnBuilder(meta.Id, columnName)
									.AsExpression(generationExpression)
									.WithLastCommited(meta.LastCommited);
							}
							else
							{
								string columnDefaultValue = (string)TfDatabaseUtility
									.ConvertDatabaseDefaultValueToDbColumnDefaultValue(columnName, typeof(TfShortTextDatabaseColumn), defaultValue);

								bool isAutoDefaultValue = defaultValue?.Trim() == TfConstants.DB_SHORT_TEXT_COLUMN_AUTO_SHA1_DEFAULT_VALUE;

								var columnBuider = columnCollectionBuilder
									.AddShortTextColumnBuilder(meta.Id, columnName)
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
						}
						break;
					case TfDatabaseColumnType.ShortInteger:
						{
							if (isGenerated && !string.IsNullOrEmpty(generationExpression))
							{
								columnCollectionBuilder
									.AddShortIntegerColumnBuilder(meta.Id, columnName)
									.AsExpression(generationExpression)
									.WithLastCommited(meta.LastCommited);
							}
							else
							{
								short? columnDefaultValue = (short?)TfDatabaseUtility.ConvertDatabaseDefaultValueToDbColumnDefaultValue(columnName,
									typeof(TfShortIntegerDatabaseColumn), defaultValue);

								bool isAutoDefaultValue = defaultValue?.Trim() == TfConstants.DB_SHORT_INT_COLUMN_AUTO_DEFAULT_VALUE;

								var columnBuider = columnCollectionBuilder
									.AddShortIntegerColumnBuilder(meta.Id, columnName)
									.WithDefaultValue(columnDefaultValue);

								if (isNullable)
									columnBuider.Nullable();
								else
									columnBuider.NotNullable();

								if (isAutoDefaultValue)
									columnBuider.WithAutoDefaultValue();
								else
									columnBuider.WithoutAutoDefaultValue();

								columnBuider.WithLastCommited(meta.LastCommited);
							}
						}
						break;
					case TfDatabaseColumnType.Integer:
						{
							if (isGenerated && !string.IsNullOrEmpty(generationExpression))
							{
								columnCollectionBuilder
									.AddIntegerColumnBuilder(meta.Id, columnName)
									.AsExpression(generationExpression)
									.WithLastCommited(meta.LastCommited);
							}
							else
							{
								int? columnDefaultValue = (int?)TfDatabaseUtility.ConvertDatabaseDefaultValueToDbColumnDefaultValue(columnName,
									typeof(TfIntegerDatabaseColumn), defaultValue);

								bool isAutoDefaultValue = defaultValue?.Trim() == TfConstants.DB_INT_COLUMN_AUTO_DEFAULT_VALUE;

								var columnBuider = columnCollectionBuilder
									.AddIntegerColumnBuilder(meta.Id, columnName)
									.WithDefaultValue(columnDefaultValue);

								if (isNullable)
									columnBuider.Nullable();
								else
									columnBuider.NotNullable();

								if (isAutoDefaultValue)
									columnBuider.WithAutoDefaultValue();
								else
									columnBuider.WithoutAutoDefaultValue();

								columnBuider.WithLastCommited(meta.LastCommited);
							}
						}
						break;
					case TfDatabaseColumnType.LongInteger:
						{
							if (isGenerated && !string.IsNullOrEmpty(generationExpression))
							{
								columnCollectionBuilder
									.AddLongIntegerColumnBuilder(meta.Id, columnName)
									.AsExpression(generationExpression)
									.WithLastCommited(meta.LastCommited);
							}
							else
							{
								long? columnDefaultValue = (long?)TfDatabaseUtility.ConvertDatabaseDefaultValueToDbColumnDefaultValue(columnName,
									typeof(TfLongIntegerDatabaseColumn), defaultValue);

								bool isAutoDefaultValue = defaultValue?.Trim() == TfConstants.DB_LONG_INT_COLUMN_AUTO_DEFAULT_VALUE;

								var columnBuider = columnCollectionBuilder
									.AddLongIntegerColumnBuilder(meta.Id, columnName)
									.WithDefaultValue(columnDefaultValue);

								if (isNullable)
									columnBuider.Nullable();
								else
									columnBuider.NotNullable();

								if (isAutoDefaultValue)
									columnBuider.WithAutoDefaultValue();
								else
									columnBuider.WithoutAutoDefaultValue();

								columnBuider.WithLastCommited(meta.LastCommited);
							}
						}
						break;
					default:
						throw new TfDatabaseException($"Not supported dbType for column '{columnName}'");
				}
				;
			}

			#endregion

			#region <--- Constraints --->

			Dictionary<string, string> constraintTableDict = new Dictionary<string, string>();
			Dictionary<string, List<string>> constraintColumnsDict = new Dictionary<string, List<string>>();
			Dictionary<string, List<string>> constraintForeignColumnsDict = new Dictionary<string, List<string>>();
			Dictionary<string, string> constraintForeignTableDict = new Dictionary<string, string>();
			Dictionary<string, char> constraintTypeDict = new Dictionary<string, char>();

			DataTable dtConstraints = _dbService.ExecuteSqlQueryCommand(TfDatabaseSqlProvider.GetConstraintsMetaSql());
			foreach (DataRow row in dtConstraints.Rows)
			{
				if ((string)row["table_name"] != tableToClone)
					continue;

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
				var tableName = constraintTableDict[constraintName];

				if (tableName != tableToClone)
					continue;

				var constraintsBuilder = newTableBuilder.WithConstraintsBuilder();

				var newConstraintName = $"{constraintName}_{suffix}";

				switch (constraintTypeDict[constraintName])
				{
					case 'p':
						{
							constraintsBuilder
								.AddPrimaryKeyConstraintBuilder(newConstraintName)
								.WithColumns(constraintColumnsDict[constraintName].ToArray());
						}
						break;
					case 'u':
						{
							constraintsBuilder
								.AddUniqueKeyConstraintBuilder(newConstraintName)
								.WithColumns(constraintColumnsDict[constraintName].ToArray());
						}
						break;
					case 'f':
						{
							constraintsBuilder
								.AddForeignKeyConstraintBuilder(newConstraintName)
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
			DataTable dtIndexes = _dbService.ExecuteSqlQueryCommand(TfDatabaseSqlProvider.GetIndexesMetaSql());
			foreach (DataRow row in dtIndexes.Rows)
			{
				if ((string)row["table_name"] != tableToClone)
					continue;

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
				var tableName = indexTableDict[indexName];

				if (tableName != tableToClone)
					continue;

				var newIndexName = $"{indexName}_{suffix}";

				var indexesBuilder = newTableBuilder.WithIndexesBuilder();

				switch (indexTypeDict[indexName])
				{
					case "btree":
						{
							indexesBuilder
								.AddBTreeIndexBuilder(newIndexName)
								.WithColumns(indexColumnsDict[indexName].ToArray());
						}
						break;
					case "gin":
						{
							indexesBuilder
								.AddGinIndexBuilder(newIndexName)
								.WithColumns(indexColumnsDict[indexName].ToArray());

						}
						break;
					case "gist":
						{
							indexesBuilder
								.AddGistIndexBuilder(newIndexName)
								.WithColumns(indexColumnsDict[indexName].ToArray());
						}
						break;
					case "hash":
						{
							indexesBuilder
								.AddHashIndexBuilder(newIndexName)
								.WithColumn(indexColumnsDict[indexName][0]);
						}
						break;
					default:
						continue;
				}
			}

			#endregion


			var result = SaveChanges(databaseBuilder);

			if (result.IsSuccess)
				scope.Complete();

			return result;
		}
	}

	public TfDatabaseUpdateResult SaveChanges(TfDatabaseBuilder databaseBuilder)
	{
		if (databaseBuilder == null)
			throw new ArgumentNullException(nameof(databaseBuilder));

		var updatedDatabase = databaseBuilder.Build();
		var currentDatabase = GetDatabaseBuilder().Build();

		var differences = TfDatabaseComparer.Compare(currentDatabase, updatedDatabase);
		var updateScript = TfDatabaseSqlProvider.GenerateUpdateScript(differences);
		var cleanupScript = TfDatabaseSqlProvider.GetUpdateCleanupScript();

		List<TfDatabaseUpdateLogRecord> log = new List<TfDatabaseUpdateLogRecord>();

		if (differences.Count == 0)
			return new TfDatabaseUpdateResult(log);

		using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
		{
			var dtLogs = _dbService.ExecuteSqlQueryCommand(updateScript);

			foreach (DataRow row in dtLogs.Rows)
			{
				var createdOn = (DateTime)row["created_on"];
				var statement = (string)row["statement"];
				var success = (bool)row["success"];
				var sqlErr = row["sql_error"] == DBNull.Value ? string.Empty : (string)row["sql_error"];

				log.Add(new TfDatabaseUpdateLogRecord
				{
					CreatedOn = createdOn,
					Statement = statement,
					Success = success,
					SqlError = sqlErr
				});
			}

			_dbService.ExecuteSqlNonQueryCommand(cleanupScript);

			var result = new TfDatabaseUpdateResult(log);

			if (result.IsSuccess)
				scope.Complete();
			else
				throw new TfDatabaseUpdateException(result);

			return result;
		}
	}
}
