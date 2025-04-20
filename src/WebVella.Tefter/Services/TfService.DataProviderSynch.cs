using NpgsqlTypes;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace WebVella.Tefter.Services;

public partial interface ITfService
{
	internal Task BulkSynchronize(
		TfDataProviderSynchronizeTask task);

	internal TfDataProviderSynchronizeTask GetSynchronizationTask(
		Guid taskId);

	internal List<TfDataProviderSynchronizeTask> GetSynchronizationTasks(
		Guid? providerId = null,
		TfSynchronizationStatus? status = null);

	internal Guid CreateSynchronizationTask(
		Guid providerId,
		TfSynchronizationPolicy synchPolicy);

	internal void UpdateSychronizationTask(
		Guid taskId,
		TfSynchronizationStatus status,
		ITfDataProviderSychronizationLog log,
		DateTime? startedOn = null,
		DateTime? completedOn = null);

	internal TfDataProviderSourceSchemaInfo GetDataProviderSourceSchemaInfo(
		Guid providerId);

	internal Task CheckScheduleSynchronizationTasksAsync(
		CancellationToken stoppingToken);

	public DateTime? GetDataProviderNextSynchronizationTime(Guid id);
}

public partial class TfService : ITfService
{
	#region <--- Synchronization Tasks --->

	public TfDataProviderSynchronizeTask GetSynchronizationTask(
		Guid taskId)
	{
		var dbo = _dboManager.Get<TfDataProviderSynchronizeTaskDbo>(taskId);

		if (dbo == null)
			return null;

		return new TfDataProviderSynchronizeTask
		{
			Id = dbo.Id,
			DataProviderId = dbo.DataProviderId,
			CompletedOn = dbo.CompletedOn,
			CreatedOn = dbo.CreatedOn,
			Policy = JsonSerializer.Deserialize<TfSynchronizationPolicy>(dbo.PolicyJson),
			StartedOn = dbo.StartedOn,
			Status = dbo.Status,
		};
	}

	public List<TfDataProviderSynchronizeTask> GetSynchronizationTasks(
		Guid? providerId = null,
		TfSynchronizationStatus? status = null)
	{
		var orderSettings = new TfOrderSettings(
		nameof(TfDataProviderSynchronizeTaskDbo.CreatedOn),
		OrderDirection.ASC);

		List<TfDataProviderSynchronizeTaskDbo> dbos = null;
		if (providerId is not null && status is not null)
		{
			dbos = _dboManager.GetList<TfDataProviderSynchronizeTaskDbo>(
				"WHERE data_provider_id = @data_provider_id AND status = @status",
				orderSettings,
				new NpgsqlParameter("@data_provider_id", providerId.Value),
				new NpgsqlParameter("@status", (short)status.Value));

		}
		else if (providerId is not null)
		{
			dbos = _dboManager.GetList<TfDataProviderSynchronizeTaskDbo>(
				"WHERE data_provider_id = @data_provider_id ",
				orderSettings,
				new NpgsqlParameter("@data_provider_id", providerId.Value));

		}
		else if (status is not null)
		{
			dbos = _dboManager.GetList<TfDataProviderSynchronizeTaskDbo>(
				"WHERE status = @status",
				orderSettings,
				new NpgsqlParameter("@status", (short)status.Value));
		}
		else
		{
			dbos = _dboManager.GetList<TfDataProviderSynchronizeTaskDbo>(order: orderSettings);
		}

		var result = new List<TfDataProviderSynchronizeTask>();

		foreach (var dbo in dbos)
		{
			var logEntries = JsonSerializer.Deserialize<List<TfDataProviderSychronizationLogEntry>>(dbo.SynchLogJson ?? "[]");
			var task = new TfDataProviderSynchronizeTask
			{
				Id = dbo.Id,
				DataProviderId = dbo.DataProviderId,
				CompletedOn = dbo.CompletedOn,
				CreatedOn = dbo.CreatedOn,
				Policy = JsonSerializer.Deserialize<TfSynchronizationPolicy>(dbo.PolicyJson),
				StartedOn = dbo.StartedOn,
				Status = dbo.Status,
				SynchronizationLog = new TfDataProviderSychronizationLog(logEntries)
			};
			result.Add(task);
		}

		return result;
	}

	public TfDataProviderSynchronizeTask GetLastSynchronizationTask(
		Guid? providerId = null)
	{
		var orderSettings = new TfOrderSettings(
		nameof(TfDataProviderSynchronizeTaskDbo.CreatedOn),
		OrderDirection.ASC);

		var dbo = _dboManager.GetBySql<TfDataProviderSynchronizeTaskDbo>(
				"SELECT * FROM tf_data_provider_synchronize_task WHERE data_provider_id = @data_provider_id ORDER BY created_on DESC LIMIT 1 ",
				new NpgsqlParameter("@data_provider_id", providerId.Value));


		if (dbo == null)
			return null;

		var logEntries = JsonSerializer.Deserialize<List<TfDataProviderSychronizationLogEntry>>(dbo.SynchLogJson ?? "[]");
		return new TfDataProviderSynchronizeTask
		{
			Id = dbo.Id,
			DataProviderId = dbo.DataProviderId,
			CompletedOn = dbo.CompletedOn,
			CreatedOn = dbo.CreatedOn,
			Policy = JsonSerializer.Deserialize<TfSynchronizationPolicy>(dbo.PolicyJson),
			StartedOn = dbo.StartedOn,
			Status = dbo.Status,
			SynchronizationLog = new TfDataProviderSychronizationLog(logEntries)
		};
	}

	public Guid CreateSynchronizationTask(
		Guid providerId,
		TfSynchronizationPolicy synchPolicy)
	{
		var task = new TfDataProviderSynchronizeTaskDbo
		{
			Id = Guid.NewGuid(),
			DataProviderId = providerId,
			PolicyJson = JsonSerializer.Serialize(synchPolicy),
			Status = TfSynchronizationStatus.Pending,
			CreatedOn = DateTime.Now,
			CompletedOn = null,
			StartedOn = DateTime.Now,
			SynchLogJson = JsonSerializer.Serialize(new List<TfDataProviderSychronizationLogEntry>()),

		};

		var success = _dboManager.Insert<TfDataProviderSynchronizeTaskDbo>(task);
		if (!success)
			throw new TfDatabaseException("Failed to insert synchronization task.");

		return task.Id;
	}

	public void UpdateSychronizationTask(
		Guid taskId,
		TfSynchronizationStatus status,
		ITfDataProviderSychronizationLog log,
		DateTime? startedOn = null,
		DateTime? completedOn = null)
	{
		var dbo = _dboManager.Get<TfDataProviderSynchronizeTaskDbo>(taskId);
		if (dbo == null)
			throw new Exception("Synchronization task was not found.");

		dbo.Status = status;
		if (startedOn is not null)
			dbo.StartedOn = startedOn;
		if (completedOn is not null)
			dbo.CompletedOn = completedOn;

		dbo.SynchLogJson = JsonSerializer.Serialize(log.GetEntries());

		var success = _dboManager.Update<TfDataProviderSynchronizeTaskDbo>(dbo);
		if (!success)
			throw new TfDatabaseException("Failed to update synchronization task in database.");
	}

	#endregion

	public TfDataProviderSourceSchemaInfo GetDataProviderSourceSchemaInfo(
		Guid providerId)
	{
		try
		{
			var result = new TfDataProviderSourceSchemaInfo();
			var provider = GetDataProvider(providerId);
			if (provider is null)
				throw new TfException("GetProvider failed");

			return provider.ProviderType.GetDataProviderSourceSchema(provider);
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public Task BulkSynchronize(
		TfDataProviderSynchronizeTask task)
	{
		try
		{
			task.SynchronizationLog.Log("synchronization task started");

			var provider = GetDataProvider(task.DataProviderId);

			if (provider is null)
			{
				var ex = new TfException("Unable to get provider.");
				task.SynchronizationLog.Log($"data provider ({task.DataProviderId}) for task not found.", ex);
				throw ex;
			}

			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_SYNC_OPERATION_LOCK_KEY))
			{
				Dictionary<string, DataRow> existingData = null;
				try
				{
					task.SynchronizationLog.Log($"start loading existing system data information needed for synchronization");
					existingData = GetExistingData(provider);
					task.SynchronizationLog.Log($"complete loading existing system data information needed for synchronization");
				}
				catch (Exception ex)
				{
					task.SynchronizationLog.Log($"failed to load existing system data information needed for synchronization", ex);
					throw new TfException("Failed to load existing system data information needed for synchronization.", ex);
				}

				ReadOnlyCollection<TfDataProviderDataRow> newData = null;
				try
				{
					task.SynchronizationLog.Log($"start loading data from provider");
					newData = GetNewData(provider, task.SynchronizationLog);
					task.SynchronizationLog.Log($"complete loading data from provider");
				}
				catch (Exception ex)
				{
					task.SynchronizationLog.Log($"failed to load data from provider", ex);
					throw new TfException("Failed to load data from provider.", ex);
				}

				if (newData.Count() == 0)
				{
					task.SynchronizationLog.Log($"data provider returned empty data set");
					task.SynchronizationLog.Log($"delete all rows from data provider data table");

					DeleteAllProviderRows(provider);

					task.SynchronizationLog.Log($"all rows deleted successfully");

					scope.Complete();

					task.SynchronizationLog.Log($"task completed successfully");
					return Task.CompletedTask;
				}

				var tableName = $"dp{provider.Index}";

				Dictionary<string, Guid> preparedJoinKeyIds = null;
				List<string> columnList = null;
				Dictionary<string, NpgsqlParameter> paramsDict = null;
				Dictionary<string, Guid> newTfIds = null;

				try
				{
					task.SynchronizationLog.Log($"start prepare join keys informations needed for synchronization");
					preparedJoinKeyIds = BulkPrepareJoinKeyValueIds(provider, newData);
					task.SynchronizationLog.Log($"complete prepare join keys informations needed for synchronization");
				}
				catch (Exception ex)
				{
					task.SynchronizationLog.Log($"failed prepare join keys informations needed for synchronization", ex);
					throw new TfException("Failed to prepare join key value ids.", ex);
				}

				try
				{
					task.SynchronizationLog.Log($"start processing new rows system identifiers");

					(columnList, paramsDict, newTfIds) = PrepareQueryArrayParameters(provider, preparedJoinKeyIds, existingData, newData);

					BulkPrepareAndUpdateTfIds(newTfIds, paramsDict["tf_id"]);

					task.SynchronizationLog.Log($"complete processing new rows system identifiers");
				}
				catch (Exception ex)
				{
					task.SynchronizationLog.Log($"failed to process new rows system identifiers", ex);
					throw new TfException("Failed to process new rows system identifiers.", ex);
				}
				try
				{
					task.SynchronizationLog.Log($"start creating temporary database structures for syncronization");

					//drop cloned table if exists
					_dbService.ExecuteSqlNonQueryCommand($"DROP TABLE IF EXISTS {tableName}_sync CASCADE;");

					//clone table
					var result = _dbManager.CloneTableForSynch(tableName);

					if (!result.IsSuccess)
					{
						throw new Exception("Failed to create duplicate structure of provider database table.");
					}

					task.SynchronizationLog.Log($"complete creating temporary database structures for syncronization");
				}
				catch (Exception ex)
				{
					task.SynchronizationLog.Log($"failed to create temporary database structures for syncronization", ex);
					throw new TfException("Failed to create temporary database structures for syncronization.", ex);
				}

				try
				{
					task.SynchronizationLog.Log($"start generating SQL code for synchronization");

					StringBuilder sql = new StringBuilder();
					sql.Append($"INSERT INTO {tableName}_sync ( ");
					sql.Append(string.Join(", ", columnList.Select(x => $"\"{x}\"").ToArray()));
					sql.Append(" ) SELECT * FROM UNNEST ( ");
					sql.Append(string.Join(", ", columnList.Select(x => $"@{x}").ToArray()));
					sql.Append(" ); ");
					sql.AppendLine();

					sql.AppendLine($"DROP TABLE IF EXISTS {tableName} CASCADE;");
					sql.AppendLine($"ALTER TABLE {tableName}_sync RENAME TO {tableName};");

					DataTable dtConstraints = _dbService.ExecuteSqlQueryCommand(TfDatabaseSqlProvider.GetConstraintsMetaSql());
					foreach (DataRow row in dtConstraints.Rows)
					{
						var constraintName = (string)row["constraint_name"];

						if (constraintName.EndsWith("_sync") && (string)row["table_name"] == $"{tableName}_sync")
						{
							var constraintNewName = constraintName.Substring(0, constraintName.Length - 5);
							sql.AppendLine($"ALTER TABLE {tableName} RENAME CONSTRAINT {constraintName} TO {constraintNewName};");
						}
					}

					DataTable dtIndexes = _dbService.ExecuteSqlQueryCommand(TfDatabaseSqlProvider.GetIndexesMetaSql());
					foreach (DataRow row in dtIndexes.Rows)
					{
						var indexName = (string)row["index_name"];
						if (indexName.EndsWith("_sync") && (string)row["table_name"] == $"{tableName}_sync")
						{
							var indexNewName = indexName.Substring(0, indexName.Length - 5);
							sql.AppendLine($"ALTER INDEX {indexName} RENAME TO {indexNewName};");
						}
					}

					List<NpgsqlParameter> paramList = new List<NpgsqlParameter>();
					foreach (var column in columnList)
					{
						paramList.Add(paramsDict[column]);
					}

					task.SynchronizationLog.Log($"complete generating SQL code for synchronization");

					task.SynchronizationLog.Log($"start database update");

					_dbService.ExecuteSqlNonQueryCommand(sql.ToString(), paramList);

					task.SynchronizationLog.Log($"completed database update");
				}
				catch (Exception ex)
				{
					task.SynchronizationLog.Log($"failed to update database", ex);
					throw new TfException("Failed to update database.", ex);
				}

				scope.Complete();

				task.SynchronizationLog.Log($"synchronization task completed successfully");

				return Task.CompletedTask;
			}
		}
		catch (Exception ex)
		{
			task.SynchronizationLog.Log($"synchronization task failed",
							TfDataProviderSychronizationLogEntryType.Error);
			throw ProcessException(ex);
		}
	}

	private (List<string> columnNames, Dictionary<string, NpgsqlParameter>, Dictionary<string, Guid>) PrepareQueryArrayParameters(
		TfDataProvider provider,
		Dictionary<string, Guid> joinKeyBulkIdDict,
		Dictionary<string, DataRow> existingData,
		ReadOnlyCollection<TfDataProviderDataRow> newData)
	{

		List<string> columnNames = new List<string>();

		Dictionary<string, NpgsqlParameter> paramsDict =
			new Dictionary<string, NpgsqlParameter>();

		Dictionary<string, Guid> newTfIds = new Dictionary<string, Guid>();

		//data column names and parameters
		foreach (var column in provider.Columns)
		{
			columnNames.Add(column.DbName);

			switch (column.DbType)
			{
				case TfDatabaseColumnType.Boolean:
					{
						var parameter = new NpgsqlParameter($"@{column.DbName}", NpgsqlDbType.Array | NpgsqlDbType.Boolean);
						parameter.Value = new List<bool?>();
						paramsDict.Add(column.DbName, parameter);
					}
					break;
				case TfDatabaseColumnType.Guid:
					{
						var parameter = new NpgsqlParameter($"@{column.DbName}", NpgsqlDbType.Array | NpgsqlDbType.Uuid);
						parameter.Value = new List<Guid?>();
						paramsDict.Add(column.DbName, parameter);
					}
					break;
				case TfDatabaseColumnType.Text:
					{
						var parameter = new NpgsqlParameter($"@{column.DbName}", NpgsqlDbType.Array | NpgsqlDbType.Text);
						parameter.Value = new List<string>();
						paramsDict.Add(column.DbName, parameter);
					}
					break;
				case TfDatabaseColumnType.ShortText:
					{
						var parameter = new NpgsqlParameter($"@{column.DbName}", NpgsqlDbType.Array | NpgsqlDbType.Varchar);
						parameter.Value = new List<string>();
						paramsDict.Add(column.DbName, parameter);
					}
					break;
				case TfDatabaseColumnType.DateOnly:
				case TfDatabaseColumnType.DateTime:
					{
						var parameter = new NpgsqlParameter($"@{column.DbName}", NpgsqlDbType.Array | NpgsqlDbType.Date);
						parameter.Value = new List<DateTime?>();
						paramsDict.Add(column.DbName, parameter);
					}
					break;
				case TfDatabaseColumnType.ShortInteger:
					{
						var parameter = new NpgsqlParameter($"@{column.DbName}", NpgsqlDbType.Array | NpgsqlDbType.Smallint);
						parameter.Value = new List<short?>();
						paramsDict.Add(column.DbName, parameter);
					}
					break;
				case TfDatabaseColumnType.Integer:
					{
						var parameter = new NpgsqlParameter($"@{column.DbName}", NpgsqlDbType.Array | NpgsqlDbType.Integer);
						parameter.Value = new List<int?>();
						paramsDict.Add(column.DbName, parameter);
					}
					break;
				case TfDatabaseColumnType.LongInteger:
					{
						var parameter = new NpgsqlParameter($"@{column.DbName}", NpgsqlDbType.Array | NpgsqlDbType.Bigint);
						parameter.Value = new List<long?>();
						paramsDict.Add(column.DbName, parameter);
					}
					break;
				case TfDatabaseColumnType.Number:
					{
						var parameter = new NpgsqlParameter($"@{column.DbName}", NpgsqlDbType.Array | NpgsqlDbType.Numeric);
						parameter.Value = new List<decimal?>();
						paramsDict.Add(column.DbName, parameter);
					}
					break;
				default:
					throw new Exception("Not supported database type");
			}
		}

		//join keys column names and parameters
		foreach (var joinKey in provider.JoinKeys)
		{
			columnNames.Add($"tf_jk_{joinKey.DbName}_id");
			columnNames.Add($"tf_jk_{joinKey.DbName}_version");

			{
				var parameter = new NpgsqlParameter($"tf_jk_{joinKey.DbName}_id", NpgsqlDbType.Array | NpgsqlDbType.Uuid);
				parameter.Value = new List<Guid>();
				paramsDict.Add($"tf_jk_{joinKey.DbName}_id", parameter);
			}

			{
				var parameter = new NpgsqlParameter($"tf_jk_{joinKey.DbName}_version", NpgsqlDbType.Array | NpgsqlDbType.Smallint);
				parameter.Value = new List<short>();
				paramsDict.Add($"tf_jk_{joinKey.DbName}_version", parameter);
			}
		}


		//add system columns names and parameters
		{
			columnNames.Add($"tf_id");
			columnNames.Add($"tf_created_on");
			columnNames.Add($"tf_updated_on");
			columnNames.Add($"tf_row_index");

			{
				var parameter = new NpgsqlParameter($"@tf_id", NpgsqlDbType.Array | NpgsqlDbType.Uuid);
				parameter.Value = new List<Guid>();
				paramsDict.Add("tf_id", parameter);
			}

			{
				var parameter = new NpgsqlParameter($"@tf_created_on", NpgsqlDbType.Array | NpgsqlDbType.Date);
				parameter.Value = new List<DateTime>();
				paramsDict.Add("tf_created_on", parameter);
			}

			{
				var parameter = new NpgsqlParameter($"@tf_updated_on", NpgsqlDbType.Array | NpgsqlDbType.Date);
				parameter.Value = new List<DateTime>();
				paramsDict.Add("tf_updated_on", parameter);
			}

			{
				var parameter = new NpgsqlParameter($"@tf_row_index", NpgsqlDbType.Array | NpgsqlDbType.Integer);
				parameter.Value = new List<int>();
				paramsDict.Add("tf_row_index", parameter);
			}
		}


		int currentRowIndex = 0;
		foreach (var row in newData)
		{
			currentRowIndex++;

			var key = GetDataRowPrimaryKeyValueAsString(provider, row, currentRowIndex);

			var columnsWithoutSource = provider.Columns.Where(x => string.IsNullOrWhiteSpace(x.SourceName));

			if (existingData.ContainsKey(key))
			{
				((List<Guid>)paramsDict["tf_id"].Value).Add((Guid)existingData[key]["tf_id"]);
				((List<DateTime>)paramsDict["tf_created_on"].Value).Add((DateTime)existingData[key]["tf_created_on"]);
				((List<DateTime>)paramsDict["tf_updated_on"].Value).Add((DateTime)DateTime.Now);
				((List<int>)paramsDict["tf_row_index"].Value).Add(currentRowIndex);
			
				foreach (var column in columnsWithoutSource)
				{
					switch (column.DbType)
					{
						case TfDatabaseColumnType.Boolean:
							{
								((List<bool?>)paramsDict[column.DbName].Value).Add((bool?)existingData[key][column.DbName]);
							}
							break;
						case TfDatabaseColumnType.Guid:
							{
								((List<Guid?>)paramsDict[column.DbName].Value).Add((Guid?)existingData[key][column.DbName]);
							}
							break;
						case TfDatabaseColumnType.Text:
						case TfDatabaseColumnType.ShortText:
							{
								((List<string>)paramsDict[column.DbName].Value).Add((string)existingData[key][column.DbName]);
							}
							break;
						case TfDatabaseColumnType.DateOnly:
						case TfDatabaseColumnType.DateTime:
							{
								DateTime? value = null;
								if (existingData[key][column.DbName] is DateOnly)
								{
									value = ((DateOnly)existingData[key][column.DbName]).ToDateTime();
								}
								else if (existingData[key][column.DbName] is DateOnly?)
								{
									if (existingData[key][column.DbName] == null)
									{
										value = null;
									}
									else
									{
										value = ((DateOnly)existingData[key][column.DbName]).ToDateTime();
									}
								}
								else if (existingData[key][column.DbName] is DateTime)
								{
									value = (DateTime)existingData[key][column.DbName];
								}
								else if (existingData[key][column.DbName] is DateTime?)
								{
									if (existingData[key][column.DbName] == null)
									{
										value = null;
									}
									else
									{
										value = (DateTime)row[column.DbName];
									}
								}
								else if (existingData[key][column.DbName] == null)
								{
									value = null;
								}
								else
								{
									throw new Exception($"Some source rows contains non DateTime or DateOnly objects for column '{column.DbName}' of type Date\\DateTime.");
								}

								((List<DateTime?>)paramsDict[column.DbName].Value).Add(value);
							}
							break;
						case TfDatabaseColumnType.ShortInteger:
							{
								((List<short?>)paramsDict[column.DbName].Value).Add((short?)existingData[key][column.DbName]);
							}
							break;
						case TfDatabaseColumnType.Integer:
							{
								((List<int?>)paramsDict[column.DbName].Value).Add((int?)existingData[key][column.DbName]);

							}
							break;
						case TfDatabaseColumnType.LongInteger:
							{
								((List<long?>)paramsDict[column.DbName].Value).Add((long?)existingData[key][column.DbName]);
							}
							break;
						case TfDatabaseColumnType.Number:
							{
								((List<decimal?>)paramsDict[column.DbName].Value).Add((decimal?)existingData[key][column.DbName]);
							}
							break;
						default:
							throw new Exception("Not supported database type");
					}
				}
			}
			else
			{
				var tfId = Guid.NewGuid();
				newTfIds[tfId.ToString()] = tfId;
				((List<Guid>)paramsDict["tf_id"].Value).Add((Guid)tfId);
				((List<DateTime>)paramsDict["tf_created_on"].Value).Add((DateTime)DateTime.Now);
				((List<DateTime>)paramsDict["tf_updated_on"].Value).Add((DateTime)DateTime.Now);
				((List<int>)paramsDict["tf_row_index"].Value).Add(currentRowIndex);

				foreach (var column in columnsWithoutSource)
				{

					switch (column.DbType)
					{
						case TfDatabaseColumnType.Boolean:
							{
								if(!column.IsNullable)
									((List<bool?>)paramsDict[column.DbName].Value).Add((bool?)GetColumnDefaultValue(column));
								else
									((List<bool?>)paramsDict[column.DbName].Value).Add(null);
							}
							break;
						case TfDatabaseColumnType.Guid:
							{
								if (!column.IsNullable)
									((List<Guid?>)paramsDict[column.DbName].Value).Add((Guid?)GetColumnDefaultValue(column));
								else
									((List<Guid?>)paramsDict[column.DbName].Value).Add(null);

							}
							break;
						case TfDatabaseColumnType.Text:
						case TfDatabaseColumnType.ShortText:
							{
								if(!column.IsNullable)
									((List<string>)paramsDict[column.DbName].Value).Add((string)GetColumnDefaultValue(column));
								else
									((List<string>)paramsDict[column.DbName].Value).Add(null);
							}
							break;
						case TfDatabaseColumnType.DateOnly:
						case TfDatabaseColumnType.DateTime:
							{
								if(!column.IsNullable)
									((List<DateTime?>)paramsDict[column.DbName].Value).Add((DateTime?)GetColumnDefaultValue(column));
								else
									((List<DateTime?>)paramsDict[column.DbName].Value).Add(null);
							}
							break;
						case TfDatabaseColumnType.ShortInteger:
							{
								if(!column.IsNullable)
									((List<short?>)paramsDict[column.DbName].Value).Add((short?)GetColumnDefaultValue(column));
								else
									((List<short?>)paramsDict[column.DbName].Value).Add(null);
							}
							break;
						case TfDatabaseColumnType.Integer:
							{
								if(!column.IsNullable)
									((List<int?>)paramsDict[column.DbName].Value).Add((int?)GetColumnDefaultValue(column));
								else
									((List<int?>)paramsDict[column.DbName].Value).Add(null);

							}
							break;
						case TfDatabaseColumnType.LongInteger:
							{
								if(!column.IsNullable)
									((List<long?>)paramsDict[column.DbName].Value).Add((long?)GetColumnDefaultValue(column));
								else
									((List<long?>)paramsDict[column.DbName].Value).Add(null);
							}
							break;
						case TfDatabaseColumnType.Number:
							{
								if(!column.IsNullable)
									((List<decimal?>)paramsDict[column.DbName].Value).Add((decimal?)GetColumnDefaultValue(column));
								else
									((List<decimal?>)paramsDict[column.DbName].Value).Add(null);
							}
							break;
						default:
							throw new Exception("Not supported database type");
					}
				}
			}


			var columnsWithSource = provider.Columns.Where(x=> !string.IsNullOrWhiteSpace(x.SourceName));
			foreach (var column in columnsWithSource)
			{
				switch (column.DbType)
				{
					case TfDatabaseColumnType.Boolean:
						{
							((List<bool?>)paramsDict[column.DbName].Value).Add((bool?)row[column.DbName]);
						}
						break;
					case TfDatabaseColumnType.Guid:
						{
							((List<Guid?>)paramsDict[column.DbName].Value).Add((Guid?)row[column.DbName]);
						}
						break;
					case TfDatabaseColumnType.Text:
					case TfDatabaseColumnType.ShortText:
						{
							((List<string>)paramsDict[column.DbName].Value).Add((string)row[column.DbName]);
						}
						break;
					case TfDatabaseColumnType.DateOnly:
					case TfDatabaseColumnType.DateTime:
						{
							DateTime? value = null;
							if (row[column.DbName] is DateOnly)
							{
								value = ((DateOnly)row[column.DbName]).ToDateTime();
							}
							else if (row[column.DbName] is DateOnly?)
							{
								if (row[column.DbName] == null)
								{
									value = null;
								}
								else
								{
									value = ((DateOnly)row[column.DbName]).ToDateTime();
								}
							}
							else if (row[column.DbName] is DateTime)
							{
								value = (DateTime)row[column.DbName];
							}
							else if (row[column.DbName] is DateTime?)
							{
								if (row[column.DbName] == null)
								{
									value = null;
								}
								else
								{
									value = (DateTime)row[column.DbName];
								}
							}
							else if (row[column.DbName] == null)
							{
								value = null;
							}
							else
							{
								throw new Exception($"Some source rows contains non DateTime or DateOnly objects for column '{column.DbName}' of type Date\\DateTime.");
							}

							((List<DateTime?>)paramsDict[column.DbName].Value).Add(value);
						}
						break;
					case TfDatabaseColumnType.ShortInteger:
						{
							((List<short?>)paramsDict[column.DbName].Value).Add((short?)row[column.DbName]);
						}
						break;
					case TfDatabaseColumnType.Integer:
						{
							((List<int?>)paramsDict[column.DbName].Value).Add((int?)row[column.DbName]);

						}
						break;
					case TfDatabaseColumnType.LongInteger:
						{
							((List<long?>)paramsDict[column.DbName].Value).Add((long?)row[column.DbName]);
						}
						break;
					case TfDatabaseColumnType.Number:
						{
							((List<decimal?>)paramsDict[column.DbName].Value).Add((decimal?)row[column.DbName]);
						}
						break;
					default:
						throw new Exception("Not supported database type");
				}
			}

			foreach (var joinKey in provider.JoinKeys)
			{
				List<string> keys = new List<string>();
				foreach (var column in joinKey.Columns)
					keys.Add(row[column.DbName]?.ToString());

				var combinedKey = CombineKey(keys);

				var skIdValue = joinKeyBulkIdDict[combinedKey];

				((List<Guid>)paramsDict[$"tf_jk_{joinKey.DbName}_id"].Value)
					.Add(skIdValue);

				((List<short>)paramsDict[$"tf_jk_{joinKey.DbName}_version"].Value)
					.Add(joinKey.Version);
			}
		}

		return (columnNames, paramsDict, newTfIds);
	}

	private Dictionary<string, Guid> BulkPrepareJoinKeyValueIds(
		TfDataProvider provider,
		ReadOnlyCollection<TfDataProviderDataRow> newData)
	{
		Dictionary<string, Guid> joinKeyBulkIdDict = new();

		foreach (var row in newData)
		{
			foreach (var joinKey in provider.JoinKeys)
			{
				List<string> keys = new List<string>();
				foreach (var column in joinKey.Columns)
					keys.Add(row[column.DbName]?.ToString());

				var key = CombineKey(keys);

				if (joinKeyBulkIdDict.ContainsKey(key))
					continue;

				joinKeyBulkIdDict[key] = Guid.Empty;
			}
		}

		BulkFillIds(joinKeyBulkIdDict);

		return joinKeyBulkIdDict;
	}

	private void BulkPrepareAndUpdateTfIds(
		Dictionary<string, Guid> newTfIds,
		NpgsqlParameter tfIdsParam)
	{
		BulkFillIds(newTfIds);

		var tfIdList = (List<Guid>)tfIdsParam.Value;
		List<Guid> newtfIdList = new List<Guid>();
		foreach (var id in tfIdList)
		{
			if (newTfIds.ContainsKey(id.ToString()))
			{
				newtfIdList.Add(newTfIds[id.ToString()]);
			}
			else
			{
				newtfIdList.Add(id);
			}
		}
		tfIdsParam.Value = newtfIdList;
	}

	private Dictionary<string, DataRow> GetExistingData(
		TfDataProvider provider)
	{
		List<string> columnsToSelect = new List<string>();

		foreach (var column in provider.SystemColumns)
		{
			columnsToSelect.Add(column.DbName);
		}

		foreach (var joinKey in provider.JoinKeys)
		{
			string joinKeyIdColumnDbName = $"tf_jk_{joinKey.DbName}_id";
			string joinKeyVersionColumnDbName = $"tf_jk_{joinKey.DbName}_version";
			columnsToSelect.Add(joinKeyIdColumnDbName);
			columnsToSelect.Add(joinKeyVersionColumnDbName);
		}

		if (provider.SynchPrimaryKeyColumns != null &&
			provider.SynchPrimaryKeyColumns.Count > 0)
		{
			foreach (var column in provider.SynchPrimaryKeyColumns)
			{
				columnsToSelect.Add(column);
			}
		}

		var noSourceColumns = provider.Columns.Where(x => string.IsNullOrWhiteSpace(x.SourceName));
		foreach (var column in noSourceColumns)
		{
			columnsToSelect.Add(column.DbName);
		}

		var columnsString = string.Join(", ", columnsToSelect.Select(x => $"\"{x}\"").ToArray());

		var dt = _dbService.ExecuteSqlQueryCommand($"SELECT {columnsString} FROM dp{provider.Index}");

		Dictionary<string, DataRow> result = new Dictionary<string, DataRow>();

		foreach (DataRow dr in dt.Rows)
		{
			var key = GetDataRowPrimaryKeyValueAsString(provider, dr);
			if (!result.ContainsKey(key))
				result.Add(key, dr);
		}

		return result;
	}

	private ReadOnlyCollection<TfDataProviderDataRow> GetNewData(
		TfDataProvider provider,
		ITfDataProviderSychronizationLog synchLog)
	{
		ReadOnlyCollection<TfDataProviderDataRow> newData = new List<TfDataProviderDataRow>().AsReadOnly();
		try
		{
			newData = provider.GetRows(synchLog);
		}
		catch (Exception ex)
		{
			throw new TfException("An error occured while getting data from data provider.", ex);
		}

		if (newData.Count == 0)
		{
			return newData;
		}

		var requiredColumns = provider.Columns.Where(x => x.SourceName is not null && x.IsNullable == false);

		foreach (var requiredColum in requiredColumns)
		{
			var found = newData[0].ColumnNames.Contains(requiredColum.DbName);
			if (!found)
			{
				throw new Exception($"Required column '{requiredColum.DbName}'(Source Name='{requiredColum.SourceName})' " +
					$" is not found in source columns list.");
			}
		}

		var primaryKeyValidationSet = new HashSet<string>();

		int rowIndex = 0;

		foreach (var row in newData)
		{
			rowIndex++;

			var key = GetDataRowPrimaryKeyValueAsString(provider, row, rowIndex);

			if (primaryKeyValidationSet.Contains(key))
			{
				throw new Exception("Provider data contains rows with " +
					"duplicated value for specified synchronization key.");
			}

			primaryKeyValidationSet.Add(key);

			Dictionary<string, HashSet<object>> uniqueValidationDict =
				new Dictionary<string, HashSet<object>>();

			foreach (var column in provider.Columns)
			{
				if (string.IsNullOrWhiteSpace(column.SourceName))
					continue;

				if (!column.IsNullable && row[column.DbName] == null)
				{
					row[column.DbName] = GetColumnDefaultValue(column);
				}

				if (column.IsUnique)
				{
					if (!uniqueValidationDict.ContainsKey(column.DbName))
					{
						uniqueValidationDict[column.DbName] = new HashSet<object>();
					}

					if (uniqueValidationDict[column.DbName].Contains(row[column.DbName]))
					{
						throw new Exception($"The column '{column.DbName}'(Source Name='{column.SourceName})' ]" +
												$" is specified as unique, but provider data contains records" +
												$" with duplicate value for this column");
					}

					uniqueValidationDict[column.DbName].Add(row[column.DbName]);
				}
			}
		}

		return newData;
	}

	private string GetDataRowPrimaryKeyValueAsString(
		TfDataProvider provider,
		DataRow dr)
	{
		if (provider.SynchPrimaryKeyColumns != null &&
					provider.SynchPrimaryKeyColumns.Count > 0)
		{
			List<string> columnKeyValues = new List<string>();
			foreach (var column in provider.SynchPrimaryKeyColumns)
			{
				var value = dr[column];
				if (value == DBNull.Value)
					value = null;
				columnKeyValues.Add(value?.ToString());
			}

			return string.Join(TfConstants.SHARED_KEY_SEPARATOR, columnKeyValues) ?? string.Empty;
		}
		else
		{
			return dr["tf_row_index"].ToString();
		}
	}

	private string GetDataRowPrimaryKeyValueAsString(
		TfDataProvider provider,
		TfDataProviderDataRow dr,
		int rowIndex)
	{
		if (provider.SynchPrimaryKeyColumns != null &&
			provider.SynchPrimaryKeyColumns.Count > 0)
		{
			List<string> columnKeyValues = new List<string>();
			foreach (var column in provider.SynchPrimaryKeyColumns)
			{
				columnKeyValues.Add(dr[column]?.ToString());
			}

			return string.Join(TfConstants.SHARED_KEY_SEPARATOR, columnKeyValues) ?? string.Empty;
		}
		else
		{
			return rowIndex.ToString();
		}
	}

	private object GetColumnDefaultValue(TfDataProviderColumn column)
	{
		if(!column.IsNullable && column.DefaultValue is null)
			throw new Exception("Column is not nullable, but default value is null.");

		if (column.DefaultValue is null)
			return null;

		switch (column.DbType)
		{
			case TfDatabaseColumnType.Boolean:
				return Convert.ToBoolean(column.DefaultValue);
			case TfDatabaseColumnType.Text:
			case TfDatabaseColumnType.ShortText:
				return column.DefaultValue;
			case TfDatabaseColumnType.Guid:
				{
					if(column.AutoDefaultValue)
					{
						return Guid.NewGuid();
					}
					return Guid.Parse(column.DefaultValue);
				}
			case TfDatabaseColumnType.DateOnly:
				{
					if (column.AutoDefaultValue == true)
					{
						return DateOnly.FromDateTime(DateTime.Now).ToDateTime();
					}
					return DateOnly.Parse(column.DefaultValue, CultureInfo.InvariantCulture).ToDateTime();
				}
			case TfDatabaseColumnType.DateTime:
				{
					if (column.AutoDefaultValue == true)
					{
						return DateTime.Now;
					}
					return DateOnly.Parse(column.DefaultValue, CultureInfo.InvariantCulture);
				}
			case TfDatabaseColumnType.Number:
				return Convert.ToDecimal(column.DefaultValue, CultureInfo.InvariantCulture);
			case TfDatabaseColumnType.ShortInteger:
				return Convert.ToInt16(column.DefaultValue);
			case TfDatabaseColumnType.Integer:
				return Convert.ToInt32(column.DefaultValue);
			case TfDatabaseColumnType.LongInteger:
				return Convert.ToInt64(column.DefaultValue);
			default:
				throw new Exception("Not supported database column type while validate default value.");
		}
	}

	public Task CheckScheduleSynchronizationTasksAsync(
		CancellationToken stoppingToken)
	{
		var providers = GetDataProviders();

		foreach (var provider in providers)
		{
			if (stoppingToken.IsCancellationRequested)
				break;

			if (!provider.SynchScheduleEnabled)
				continue;

			var lastSynchTask = GetLastSynchronizationTask(provider.Id);

			if(lastSynchTask is null)
			{
				CreateSynchronizationTask(provider.Id, new TfSynchronizationPolicy() );
				continue;
			}

			if(lastSynchTask.Status == TfSynchronizationStatus.Pending)
				continue;

			if(lastSynchTask.CompletedOn is null)
				continue;

			if (lastSynchTask.CompletedOn.Value.AddMinutes(provider.SynchScheduleMinutes) < DateTime.Now)
			{
				CreateSynchronizationTask(provider.Id, new TfSynchronizationPolicy());
				continue;
			}
		}

		return Task.CompletedTask;
	}

	public DateTime? GetDataProviderNextSynchronizationTime(Guid id)
	{
		var provider = GetDataProvider(id);

		if (provider is null)
			return null;

		if (!provider.SynchScheduleEnabled)
			return null;


		var lastSynchTask = GetLastSynchronizationTask(provider.Id);

		if (lastSynchTask is null)
			return DateTime.Now;

		return lastSynchTask.CompletedOn.Value.AddMinutes(provider.SynchScheduleMinutes);
	}
}