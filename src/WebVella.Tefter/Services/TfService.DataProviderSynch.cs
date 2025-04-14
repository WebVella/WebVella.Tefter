using NpgsqlTypes;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace WebVella.Tefter.Services;

public partial interface ITfService
{
	internal Task BulkSynchronize(
		TfDataProviderSynchronizeTask task);

	internal TfDataProviderSynchronizeTask GetSynchronizationTask(
		Guid taskId);

	internal TfDataProviderSynchronizeTaskExtended GetSynchronizationTaskExtended(
		Guid taskId);

	internal List<TfDataProviderSynchronizeTaskExtended> GetSynchronizationTasksExtended(
		Guid? providerId = null,
		TfSynchronizationStatus? status = null);

	internal List<TfDataProviderSynchronizeTask> GetSynchronizationTasks(
		Guid? providerId = null,
		TfSynchronizationStatus? status = null);

	internal Guid CreateSynchronizationTask(
		Guid providerId,
		TfSynchronizationPolicy synchPolicy);

	internal void UpdateSychronizationTask(
		Guid taskId,
		TfSynchronizationStatus status,
		DateTime? startedOn = null,
		DateTime? completedOn = null);

	internal List<TfDataProviderSynchronizeResultInfo> GetSynchronizationTaskResultInfos(
		Guid taskId);

	internal void CreateSynchronizationResultInfo(
		Guid syncTaskId,
		int? tfRowIndex,
		Guid? tfId,
		string info = null,
		string warning = null,
		string error = null);

	internal TfDataProviderSourceSchemaInfo GetDataProviderSourceSchemaInfo(
		Guid providerId);
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
			var task = new TfDataProviderSynchronizeTask
			{
				Id = dbo.Id,
				DataProviderId = dbo.DataProviderId,
				CompletedOn = dbo.CompletedOn,
				CreatedOn = dbo.CreatedOn,
				Policy = JsonSerializer.Deserialize<TfSynchronizationPolicy>(dbo.PolicyJson),
				StartedOn = dbo.StartedOn,
				Status = dbo.Status,
			};
			result.Add(task);
		}

		return result;
	}

	public TfDataProviderSynchronizeTaskExtended GetSynchronizationTaskExtended(
		Guid taskId)
	{
		TfDataProviderSynchronizeTaskExtended dbo =
			_dboManager.GetBySql<TfDataProviderSynchronizeTaskExtended>(
@"SELECT
	st.id,
	st.data_provider_id,
	st.policy_json,
	st.status,
	st.created_on,
	st.started_on,
	st.completed_on,
	(  SELECT COUNT(id) FROM tf_data_provider_synchronize_result_info sri WHERE sri.task_id = st.id AND sri.info IS NOT NULL ) AS info_count,
	(  SELECT COUNT(id) FROM tf_data_provider_synchronize_result_info sri WHERE sri.task_id = st.id AND sri.warning IS NOT NULL ) AS warning_count,
	(  SELECT COUNT(id) FROM tf_data_provider_synchronize_result_info sri WHERE sri.task_id = st.id AND sri.error IS NOT NULL ) AS error_count
FROM tf_data_provider_synchronize_task st
WHERE st.id = @task_id 
GROUP BY
	st.id,
	st.data_provider_id,
	st.policy_json,
	st.status,
	st.created_on,
	st.started_on,
	st.completed_on
ORDER BY st.created_on DESC",
				new NpgsqlParameter("@task_id", taskId));

		return dbo;
	}

	public List<TfDataProviderSynchronizeTaskExtended> GetSynchronizationTasksExtended(
		Guid? providerId = null,
		TfSynchronizationStatus? status = null)
	{
		List<TfDataProviderSynchronizeTaskExtended> dbos = null;
		if (providerId is not null && status is not null)
		{
			dbos = _dboManager.GetListBySql<TfDataProviderSynchronizeTaskExtended>(
@"SELECT
	st.id,
	st.data_provider_id,
	st.policy_json,
	st.status,
	st.created_on,
	st.started_on,
	st.completed_on,
	(  SELECT COUNT(id) FROM tf_data_provider_synchronize_result_info sri WHERE sri.task_id = st.id AND sri.info IS NOT NULL ) AS info_count,
	(  SELECT COUNT(id) FROM tf_data_provider_synchronize_result_info sri WHERE sri.task_id = st.id AND sri.warning IS NOT NULL ) AS warning_count,
	(  SELECT COUNT(id) FROM tf_data_provider_synchronize_result_info sri WHERE sri.task_id = st.id AND sri.error IS NOT NULL ) AS error_count
FROM tf_data_provider_synchronize_task st
WHERE data_provider_id = @data_provider_id AND status = @status
GROUP BY 
	st.id, 
	st.data_provider_id, 
	st.policy_json, 
	st.status, 
	st.created_on,
	st.started_on,
	st.completed_on
ORDER BY st.created_on DESC",
				new NpgsqlParameter("@data_provider_id", providerId.Value),
				new NpgsqlParameter("@status", (short)status.Value));

		}
		else if (providerId is not null)
		{
			dbos = _dboManager.GetListBySql<TfDataProviderSynchronizeTaskExtended>(
@"SELECT
	st.id,
	st.data_provider_id,
	st.policy_json,
	st.status,
	st.created_on,
	st.started_on,
	st.completed_on,
	(  SELECT COUNT(id) FROM tf_data_provider_synchronize_result_info sri WHERE sri.task_id = st.id AND sri.info IS NOT NULL ) AS info_count,
	(  SELECT COUNT(id) FROM tf_data_provider_synchronize_result_info sri WHERE sri.task_id = st.id AND sri.warning IS NOT NULL ) AS warning_count,
	(  SELECT COUNT(id) FROM tf_data_provider_synchronize_result_info sri WHERE sri.task_id = st.id AND sri.error IS NOT NULL ) AS error_count
FROM tf_data_provider_synchronize_task st
WHERE data_provider_id = @data_provider_id 
GROUP BY
	st.id,
	st.data_provider_id,
	st.policy_json,
	st.status,
	st.created_on,
	st.started_on,
	st.completed_on
ORDER BY st.created_on DESC",
				new NpgsqlParameter("@data_provider_id", providerId.Value));

		}
		else if (status is not null)
		{
			dbos = _dboManager.GetListBySql<TfDataProviderSynchronizeTaskExtended>(
@"SELECT 
	st.id, 
	st.data_provider_id, 
	st.policy_json, 
	st.status, 
	st.created_on,
	st.started_on,
	st.completed_on,
	COUNT( sri_info.id ) AS info_count,
	COUNT( sri_warning.id ) AS warning_count,
	COUNT( sri_error.id ) AS error_count
FROM tf_data_provider_synchronize_task st
	LEFT OUTER JOIN tf_data_provider_synchronize_result_info sri_info ON sri_info.task_id = st.id AND sri_info.info IS NOT NULL
	LEFT OUTER JOIN tf_data_provider_synchronize_result_info sri_warning ON sri_warning.task_id = st.id AND sri_warning.warning IS NOT NULL
	LEFT OUTER JOIN tf_data_provider_synchronize_result_info sri_error ON sri_error.task_id = st.id AND sri_error.error IS NOT NULL
WHERE status = @status
GROUP BY 
	st.id, 
	st.data_provider_id, 
	st.policy_json, 
	st.status, 
	st.created_on,
	st.started_on,
	st.completed_on
ORDER BY st.created_on DESC",
				new NpgsqlParameter("@status", (short)status.Value));
		}
		else
		{
			dbos = _dboManager.GetListBySql<TfDataProviderSynchronizeTaskExtended>(
@"SELECT
	st.id,
	st.data_provider_id,
	st.policy_json,
	st.status,
	st.created_on,
	st.started_on,
	st.completed_on,
	(  SELECT COUNT(id) FROM tf_data_provider_synchronize_result_info sri WHERE sri.task_id = st.id AND sri.info IS NOT NULL ) AS info_count,
	(  SELECT COUNT(id) FROM tf_data_provider_synchronize_result_info sri WHERE sri.task_id = st.id AND sri.warning IS NOT NULL ) AS warning_count,
	(  SELECT COUNT(id) FROM tf_data_provider_synchronize_result_info sri WHERE sri.task_id = st.id AND sri.error IS NOT NULL ) AS error_count
FROM tf_data_provider_synchronize_task st
GROUP BY 
	st.id, 
	st.data_provider_id, 
	st.policy_json, 
	st.status, 
	st.created_on,
	st.started_on,
	st.completed_on
ORDER BY st.created_on DESC");
		}

		return dbos;
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
			StartedOn = DateTime.Now
		};

		var success = _dboManager.Insert<TfDataProviderSynchronizeTaskDbo>(task);
		if (!success)
			throw new TfDatabaseException("Failed to insert synchronization task.");

		return task.Id;
	}

	public void UpdateSychronizationTask(
		Guid taskId,
		TfSynchronizationStatus status,
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

	#region <--- Synchronization Result Info --->

	public List<TfDataProviderSynchronizeResultInfo> GetSynchronizationTaskResultInfos(
		Guid taskId)
	{
		try
		{
			var orderSettings = new TfOrderSettings(
			nameof(TfDataProviderSynchronizeTaskDbo.CreatedOn),
			OrderDirection.ASC);

			var dbos = _dboManager.GetList<TfDataProviderSynchronizeResultInfoDbo>(
					"WHERE task_id = @task_id",
					orderSettings,
					new NpgsqlParameter("@task_id", taskId));

			var result = new List<TfDataProviderSynchronizeResultInfo>();

			foreach (var dbo in dbos)
			{
				var task = new TfDataProviderSynchronizeResultInfo
				{
					Id = dbo.Id,
					TaskId = dbo.TaskId,
					TfId = dbo.TfId,
					TfRowIndex = dbo.TfRowIndex,
					CreatedOn = dbo.CreatedOn,
					Info = dbo.Info,
					Error = dbo.Error,
					Warning = dbo.Warning
				};
				result.Add(task);
			}

			return result;
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public void CreateSynchronizationResultInfo(
		Guid taskId,
		int? tfRowIndex,
		Guid? tfId,
		string info = null,
		string warning = null,
		string error = null)
	{
		try
		{
			var dbo = new TfDataProviderSynchronizeResultInfoDbo
			{
				Id = Guid.NewGuid(),
				TaskId = taskId,
				CreatedOn = DateTime.Now,
				Info = info,
				Error = error,
				Warning = warning,
				TfId = tfId,
				TfRowIndex = tfRowIndex
			};

			var success = _dboManager.Insert<TfDataProviderSynchronizeResultInfoDbo>(dbo);
			if (!success)
				throw new TfDatabaseException("Failed to insert synchronization task result info.");

		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	#endregion

	public async Task BulkSynchronize(
		TfDataProviderSynchronizeTask task)
	{
		try
		{
			await Task.Delay(1);

			TfDataProviderSychronizationLog synchLog = new TfDataProviderSychronizationLog();

			var provider = GetDataProvider(task.DataProviderId);

			if (provider is null)
				throw new TfException("Unable to get provider.");

			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_SYNC_OPERATION_LOCK_KEY))
			{
				var existingData = GetExistingData(provider);
				var newData = GetNewData(provider, synchLog);

				if (newData.Count() == 0)
				{
					DeleteAllProviderRows(provider);
					scope.Complete();
					return;
				}

				var tableName = $"dp{provider.Index}";

				var preparedSharedKeyIds = BulkPrepareSharedKeyValueIds(provider, newData);

				var (columnList, paramsDict, newTfIds) = PrepareQueryArrayParameters(provider, preparedSharedKeyIds, existingData, newData);

				BulkPrepareAndUpdateTfIds(newTfIds, paramsDict["tf_id"]);

				//drop cloned table if exists
				_dbService.ExecuteSqlNonQueryCommand($"DROP TABLE IF EXISTS {tableName}_sync CASCADE;");

				//clone table
				var result = _dbManager.CloneTableForSynch(tableName);

				if (!result.IsSuccess)
				{
					throw new Exception("Failed to create duplicate structure of provider database table.");
				}

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

				_dbService.ExecuteSqlNonQueryCommand(sql.ToString(), paramList);

				scope.Complete();
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	private (List<string> columnNames, Dictionary<string, NpgsqlParameter>, Dictionary<string, Guid>) PrepareQueryArrayParameters(
		TfDataProvider provider,
		Dictionary<string, Guid> sharedKeyBulkIdDict,
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
			if (string.IsNullOrWhiteSpace(column.SourceName))
				continue;

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

		//shared keys column names and parameters
		foreach (var sharedKey in provider.SharedKeys)
		{
			columnNames.Add($"tf_sk_{sharedKey.DbName}_id");
			columnNames.Add($"tf_sk_{sharedKey.DbName}_version");

			{
				var parameter = new NpgsqlParameter($"tf_sk_{sharedKey.DbName}_id", NpgsqlDbType.Array | NpgsqlDbType.Uuid);
				parameter.Value = new List<Guid>();
				paramsDict.Add($"tf_sk_{sharedKey.DbName}_id", parameter);
			}

			{
				var parameter = new NpgsqlParameter($"tf_sk_{sharedKey.DbName}_version", NpgsqlDbType.Array | NpgsqlDbType.Smallint);
				parameter.Value = new List<short>();
				paramsDict.Add($"tf_sk_{sharedKey.DbName}_version", parameter);
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

			foreach (var column in provider.Columns)
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

			var key = GetDataRowPrimaryKeyValueAsString(provider, row, currentRowIndex);

			foreach (var sharedKey in provider.SharedKeys)
			{
				List<string> keys = new List<string>();
				foreach (var column in sharedKey.Columns)
					keys.Add(row[column.DbName]?.ToString());

				var combinedKey = CombineKey(keys);

				var skIdValue = sharedKeyBulkIdDict[combinedKey];

				((List<Guid>)paramsDict[$"tf_sk_{sharedKey.DbName}_id"].Value)
					.Add(skIdValue);

				((List<short>)paramsDict[$"tf_sk_{sharedKey.DbName}_version"].Value)
					.Add(sharedKey.Version);
			}

			if (existingData.ContainsKey(key))
			{
				((List<Guid>)paramsDict["tf_id"].Value).Add((Guid)existingData[key]["tf_id"]);
				((List<DateTime>)paramsDict["tf_created_on"].Value).Add((DateTime)existingData[key]["tf_created_on"]);
				((List<DateTime>)paramsDict["tf_updated_on"].Value).Add((DateTime)DateTime.Now);
				((List<int>)paramsDict["tf_row_index"].Value).Add(currentRowIndex);
			}
			else
			{
				var tfId = Guid.NewGuid();
				newTfIds[tfId.ToString()] = tfId;
				((List<Guid>)paramsDict["tf_id"].Value).Add((Guid)tfId);
				((List<DateTime>)paramsDict["tf_created_on"].Value).Add((DateTime)DateTime.Now);
				((List<DateTime>)paramsDict["tf_updated_on"].Value).Add((DateTime)DateTime.Now);
				((List<int>)paramsDict["tf_row_index"].Value).Add(currentRowIndex);
			}
		}

		return (columnNames, paramsDict, newTfIds);
	}

	private Dictionary<string, Guid> BulkPrepareSharedKeyValueIds(
		TfDataProvider provider,
		ReadOnlyCollection<TfDataProviderDataRow> newData)
	{
		Dictionary<string, Guid> sharedKeyBulkIdDict = new();

		foreach (var row in newData)
		{
			foreach (var sharedKey in provider.SharedKeys)
			{
				List<string> keys = new List<string>();
				foreach (var column in sharedKey.Columns)
					keys.Add(row[column.DbName]?.ToString());

				var key = CombineKey(keys);

				if (sharedKeyBulkIdDict.ContainsKey(key))
					continue;

				sharedKeyBulkIdDict[key] = Guid.Empty;
			}
		}

		BulkFillIds(sharedKeyBulkIdDict);

		return sharedKeyBulkIdDict;
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

		foreach (var sharedKey in provider.SharedKeys)
		{
			string sharedKeyIdColumnDbName = $"tf_sk_{sharedKey.DbName}_id";
			string sharedKeyVersionColumnDbName = $"tf_sk_{sharedKey.DbName}_version";
			columnsToSelect.Add(sharedKeyIdColumnDbName);
			columnsToSelect.Add(sharedKeyVersionColumnDbName);
		}

		if (provider.SynchPrimaryKeyColumns != null &&
			provider.SynchPrimaryKeyColumns.Count > 0)
		{
			foreach (var column in provider.SynchPrimaryKeyColumns)
			{
				columnsToSelect.Add(column);
			}
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
					throw new Exception($"The column '{column.DbName}'(Source Name='{column.SourceName})' " +
						$" is specified as non nullable, but provider data contains records with null for this column");
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
}