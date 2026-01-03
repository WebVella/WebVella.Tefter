using Nito.AsyncEx.Synchronous;
using NpgsqlTypes;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace WebVella.Tefter.Services;

public partial interface ITfService
{
	internal Task BulkSynchronize(
		TfDataProviderSynchronizeTask task);

	internal List<TfDataProviderSynchronizeTask> GetSynchronizationTasks(
		Guid? providerId = null,
		TfSynchronizationStatus? status = null);

	internal Guid CreateSynchronizationTask(
		Guid providerId,
		TfSynchronizationPolicy synchPolicy);

	internal void TriggerSynchronization(Guid providerId);
	internal void UpdateDataProviderSunchronization(Guid providerId, short syncScheduleMinutes, bool syncScheduleEnabled);
	internal void UpdateDataProviderSynchPrimaryKeyColumns(Guid providerId, List<string> columns);

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

	List<TfDataProviderSynchronizeTask> GetDataProviderSynchronizationTasks(Guid providerId, int? page = null, int? pageSize = null,
		TfSynchronizationStatus? status = null);
}

public partial class TfService : ITfService
{
	#region <--- Synchronization Tasks --->

	public List<TfDataProviderSynchronizeTask> GetSynchronizationTasks(
		Guid? providerId = null,
		TfSynchronizationStatus? status = null)
	{
		var orderSettings = new TfOrderSettings(
		nameof(TfDataProviderSynchronizeTaskDbo.CreatedOn),
		OrderDirection.ASC);

		List<TfDataProviderSynchronizeTaskDbo> dbos;
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
				Policy = JsonSerializer.Deserialize<TfSynchronizationPolicy>(dbo.PolicyJson) ?? new(),
				StartedOn = dbo.StartedOn,
				Status = dbo.Status,
				SynchronizationLog = new TfDataProviderSychronizationLog(logEntries!)
			};
			result.Add(task);
		}

		return result;
	}

	public TfDataProviderSynchronizeTask? GetLastSynchronizationTask(
		Guid providerId)
	{
		var orderSettings = new TfOrderSettings(
		nameof(TfDataProviderSynchronizeTaskDbo.CreatedOn),
		OrderDirection.ASC);

		var dbo = _dboManager.GetBySql<TfDataProviderSynchronizeTaskDbo>(
				"SELECT * FROM tf_data_provider_synchronize_task WHERE data_provider_id = @data_provider_id ORDER BY created_on DESC LIMIT 1 ",
				new NpgsqlParameter("@data_provider_id", providerId));


		if (dbo == null)
			return null;

		var logEntries = JsonSerializer.Deserialize<List<TfDataProviderSychronizationLogEntry>>(dbo.SynchLogJson ?? "[]");
		return new TfDataProviderSynchronizeTask
		{
			Id = dbo.Id,
			DataProviderId = dbo.DataProviderId,
			CompletedOn = dbo.CompletedOn,
			CreatedOn = dbo.CreatedOn,
			Policy = JsonSerializer.Deserialize<TfSynchronizationPolicy>(dbo.PolicyJson) ?? new(),
			StartedOn = dbo.StartedOn,
			Status = dbo.Status,
			SynchronizationLog = new TfDataProviderSychronizationLog(logEntries!)
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

	public void TriggerSynchronization(Guid providerId)
	{
		CreateSynchronizationTask(providerId, new TfSynchronizationPolicy());
	}
	public void UpdateDataProviderSunchronization(Guid providerId, short syncScheduleMinutes, bool syncScheduleEnabled)
	{
		var provider = GetDataProvider(providerId);
		if (provider is null)
			throw new TfException("Provider not found");
		UpdateDataProvider(new TfUpdateDataProvider
		{
			Id = provider.Id,
			Name = provider.Name,
			SettingsJson = provider.SettingsJson,
			SynchPrimaryKeyColumns = provider.SynchPrimaryKeyColumns.ToList(),
			SynchScheduleEnabled = syncScheduleEnabled,
			SynchScheduleMinutes = syncScheduleMinutes
		});
	}
	public void UpdateDataProviderSynchPrimaryKeyColumns(Guid providerId, List<string> columns)
	{
		var provider = GetDataProvider(providerId);
		if (provider is null)
			throw new TfException("Provider not found");
		UpdateDataProvider(new TfUpdateDataProvider
		{
			Id = provider.Id,
			Name = provider.Name,
			SettingsJson = provider.SettingsJson,
			SynchPrimaryKeyColumns = columns,
			SynchScheduleEnabled = provider.SynchScheduleEnabled,
			SynchScheduleMinutes = provider.SynchScheduleMinutes
		});
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

	private Dictionary<string, DataRow> GetExistingData(
		TfDataProvider provider)
	{
		List<string> columnsToSelect = new List<string>();

		foreach (var column in provider.SystemColumns)
		{
			columnsToSelect.Add(column.DbName!);
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
			columnsToSelect.Add(column.DbName!);
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
		Dictionary<string, object> uniqueValuesDict,
		ITfDataProviderSychronizationLog synchLog)
	{
		ReadOnlyCollection<TfDataProviderDataRow> newData = new List<TfDataProviderDataRow>().AsReadOnly();
		try
		{
			newData = provider.GetRows(synchLog);
		}
		catch (Exception ex)
		{
			synchLog.Log(ex.Message, ex);
			throw new TfException("An error occurred while getting data from data provider.", ex);
		}

		if (newData.Count == 0)
		{
			return newData;
		}

		var requiredColumns = provider.Columns.Where(x => x.SourceName is not null && x.IsNullable == false);

		foreach (var requiredColum in requiredColumns)
		{
			var found = newData[0].ColumnNames.Contains(requiredColum.DbName!);
			if (!found)
			{
				throw new Exception($"Required column '{requiredColum.DbName!}'(Source Name='{requiredColum.SourceName})' " +
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

				if (!column.IsNullable && row[column.DbName!] == null)
				{
					row[column.DbName!] = GetColumnDefaultOrUniqueValue(column, uniqueValuesDict);
				}

				if (column.IsUnique)
				{
					if (!uniqueValidationDict.ContainsKey(column.DbName!))
					{
						uniqueValidationDict[column.DbName!] = new HashSet<object>();
					}

					if (uniqueValidationDict[column.DbName!].Contains(row[column.DbName!]!))
					{
						throw new Exception($"The column '{column.DbName!}'(Source Name='{column.SourceName})' ]" +
												$" is specified as unique, but provider data contains records" +
												$" with duplicate value for this column");
					}

					uniqueValidationDict[column.DbName!].Add(row[column.DbName!]!);
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
				columnKeyValues.Add($"{value}");
			}

			return string.Join(TfConstants.SHARED_KEY_SEPARATOR, columnKeyValues) ?? string.Empty;
		}

		return $"{dr["tf_row_index"]}";
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
				columnKeyValues.Add( $"{dr[column]}");
			}

			return string.Join(TfConstants.SHARED_KEY_SEPARATOR, columnKeyValues) ?? string.Empty;
		}
		else
		{
			return rowIndex.ToString();
		}
	}

	private object? GetColumnDefaultOrUniqueValue(
		TfDataProviderColumn column,
		Dictionary<string, object> uniqueValuesDict)
	{
		if (column.IsUnique)
		{
			switch (column.DbType)
			{
				//for boolean we can have only 2 unique values - true or false,
				//so we cannot generate unique value and use default value 
				case TfDatabaseColumnType.Boolean:
					return Convert.ToBoolean(column.DefaultValue);
				case TfDatabaseColumnType.Text:
				case TfDatabaseColumnType.ShortText:
					{
						if (!uniqueValuesDict.ContainsKey(column.DbName!))
							uniqueValuesDict[column.DbName!] = new HashSet<string>();
						do
						{
							var newValue = Guid.NewGuid().ToSha1();
							if (!((HashSet<string>)uniqueValuesDict[column.DbName!]).Contains(newValue))
							{
								((HashSet<string>)uniqueValuesDict[column.DbName!]).Add(newValue);
								return newValue;
							}
						} while (true);
					}
				case TfDatabaseColumnType.Guid:
					{
						return Guid.NewGuid();
					}
				case TfDatabaseColumnType.DateOnly:
					{
						if (!uniqueValuesDict.ContainsKey(column.DbName!))
							uniqueValuesDict[column.DbName!] = new HashSet<long>();

						var newValue = DateOnly.FromDateTime(DateTime.Now);
						do
						{
							long seconds = (long)(newValue.ToDateTime() - DateTime.UnixEpoch).TotalSeconds;
							if (!((HashSet<long>)uniqueValuesDict[column.DbName!]).Contains(seconds))
							{
								((HashSet<long>)uniqueValuesDict[column.DbName!]).Add(seconds);
								return newValue.ToDateTime();
							}
							newValue = newValue.AddDays(1);
						} while (true);
					}

				case TfDatabaseColumnType.DateTime:
					{
						if (!uniqueValuesDict.ContainsKey(column.DbName!))
							uniqueValuesDict[column.DbName!] = new HashSet<long>();

						var newValue = DateTime.Now;
						do
						{
							long seconds = (long)(newValue - DateTime.UnixEpoch).TotalSeconds;
							if (!((HashSet<long>)uniqueValuesDict[column.DbName!]).Contains(seconds))
							{
								((HashSet<long>)uniqueValuesDict[column.DbName!]).Add(seconds);
								return newValue;
							}
							newValue = newValue.AddSeconds(1);
						} while (true);
					}
				case TfDatabaseColumnType.Number:
					{
						if (!uniqueValuesDict.ContainsKey(column.DbName!))
							uniqueValuesDict[column.DbName!] = new HashSet<decimal>();

						decimal newValue = 1;
						do
						{
							if (!((HashSet<decimal>)uniqueValuesDict[column.DbName!]).Contains(newValue))
							{
								((HashSet<decimal>)uniqueValuesDict[column.DbName!]).Add(newValue);
								return newValue;
							}
							newValue++;
						} while (true);
					}
				case TfDatabaseColumnType.ShortInteger:
					{
						if (!uniqueValuesDict.ContainsKey(column.DbName!))
							uniqueValuesDict[column.DbName!] = new HashSet<short>();

						short newValue = 1;
						do
						{
							if (!((HashSet<short>)uniqueValuesDict[column.DbName!]).Contains(newValue))
							{
								((HashSet<short>)uniqueValuesDict[column.DbName!]).Add(newValue);
								return newValue;
							}
							newValue++;
						} while (true);
					}
				case TfDatabaseColumnType.Integer:
					{
						if (!uniqueValuesDict.ContainsKey(column.DbName!))
							uniqueValuesDict[column.DbName!] = new HashSet<int>();

						int newValue = 1;
						do
						{
							if (!((HashSet<int>)uniqueValuesDict[column.DbName!]).Contains(newValue))
							{
								((HashSet<int>)uniqueValuesDict[column.DbName!]).Add(newValue);
								return newValue;
							}
							newValue++;
						} while (true);
					}
				case TfDatabaseColumnType.LongInteger:
					{
						if (!uniqueValuesDict.ContainsKey(column.DbName!))
							uniqueValuesDict[column.DbName!] = new HashSet<long>();

						long newValue = 1;
						do
						{
							if (!((HashSet<long>)uniqueValuesDict[column.DbName!]).Contains(newValue))
							{
								((HashSet<long>)uniqueValuesDict[column.DbName!]).Add(newValue);
								return newValue;
							}
							newValue++;
						} while (true);
					}
				default:
					throw new Exception("Not supported database column type while validate default value.");
			}
		}

		if (!column.IsNullable && column.DefaultValue is null)
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
					if (column.AutoDefaultValue)
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

			if (lastSynchTask is null)
			{
				CreateSynchronizationTask(provider.Id, new TfSynchronizationPolicy());
				continue;
			}

			if (lastSynchTask.Status == TfSynchronizationStatus.Pending)
				continue;

			if (lastSynchTask.CompletedOn is null)
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

		return lastSynchTask.CompletedOn!.Value.AddMinutes(provider.SynchScheduleMinutes);
	}

	public List<TfDataProviderSynchronizeTask> GetDataProviderSynchronizationTasks(Guid providerId, int? page = null, int? pageSize = null,
		TfSynchronizationStatus? status = null)
	{
		var tasks = GetSynchronizationTasks(providerId, status: status);
		tasks = tasks.OrderByDescending(x => x.CreatedOn).ToList();
		if (page is null || pageSize is null)
			return tasks;

		return tasks.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value).ToList();
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
				#region <--- loads existing data --->

				Dictionary<string, object> uniqueValuesDict = new Dictionary<string, object>();
				Dictionary<string, DataRow> existingData;
				try
				{
					task.SynchronizationLog.Log($"start loading existing system data information needed for synchronization");
					existingData = GetExistingData(provider);
					task.SynchronizationLog.Log($"complete loading existing system data information needed for synchronization");
				}
				catch (Exception ex)
				{
					task.SynchronizationLog.Log(ex.Message, ex);
					task.SynchronizationLog.Log($"failed to load existing system data information needed for synchronization");
					throw new TfException("Failed to load existing system data information needed for synchronization.", ex);
				}

				#endregion

				#region <--- loads new data --->

				ReadOnlyCollection<TfDataProviderDataRow> newData;
				try
				{
					task.SynchronizationLog.Log($"start loading data from provider");
					newData = GetNewData(provider, uniqueValuesDict, task.SynchronizationLog);
					task.SynchronizationLog.Log($"complete loading data from provider");
				}
				catch (Exception ex)
				{
					task.SynchronizationLog.Log(ex.Message, ex);
					task.SynchronizationLog.Log($"failed to load data from provider");
					throw new TfException("Failed to load data from provider.", ex);
				}

				#endregion

				#region <--- delete all records if new data is empty and exists --->

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

				#endregion

				#region <--- process and prepare new data for database insert --->

				var tableName = $"dp{provider.Index}";
				List<string> columnList;
				List<NpgsqlParameter> paramList;

				try
				{
					task.SynchronizationLog.Log($"start prepare new data for database insert");

					PrepareQueryArrayParametersNew(provider, existingData, newData, 
						uniqueValuesDict, out columnList, out paramList);

					task.SynchronizationLog.Log($"complete prepare new data for database insert");
				}
				catch (Exception ex)
				{
					task.SynchronizationLog.Log($"failed to prepare new data for database insert", ex);
					throw new TfException("Failed to prepare new data for database insert.", ex);
				}

				#endregion

				#region <--- creating temporary database structures --->

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

				#endregion

				#region <--- generate update sql script --->

				StringBuilder sql = new StringBuilder();

				try
				{
					task.SynchronizationLog.Log($"start generating SQL code for synchronization");


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

					task.SynchronizationLog.Log($"complete generating SQL code for synchronization");
				}
				catch (Exception ex)
				{
					task.SynchronizationLog.Log($"failed to update database", ex);
					throw new TfException("Failed to update database.", ex);
				}

				#endregion

				#region <--- execute update sql script --->

				try
				{
					task.SynchronizationLog.Log($"start database update");

					_dbService.ExecuteSqlNonQueryCommand(sql.ToString(), paramList);

					task.SynchronizationLog.Log($"completed database update");
				}
				catch (Exception ex)
				{
					task.SynchronizationLog.Log($"failed to update database", ex);
					throw new TfException("Failed to update database.", ex);
				}

				#endregion

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

	private void PrepareQueryArrayParametersNew(
		TfDataProvider provider,
		Dictionary<string, DataRow> existingDataDict,
		ReadOnlyCollection<TfDataProviderDataRow> newData,
		Dictionary<string, object> uniqueValuesDict,
		out List<string> columnNames,
		out List<NpgsqlParameter> paramList)
	{

		columnNames = new List<string>();
		Dictionary<string, NpgsqlParameter> paramsDict = new Dictionary<string, NpgsqlParameter>();

		#region <--- prepare columns --->

		foreach (var column in provider.Columns)
		{
			//do not process expression columns
			if (!string.IsNullOrWhiteSpace( column.Expression ) )
				continue;

			columnNames.Add(column.DbName!);

			switch (column.DbType)
			{
				case TfDatabaseColumnType.Boolean:
					{
						var parameter = new NpgsqlParameter($"@{column.DbName!}", NpgsqlDbType.Array | NpgsqlDbType.Boolean);
						parameter.Value = new List<bool?>();
						paramsDict.Add(column.DbName!, parameter);
					}
					break;
				case TfDatabaseColumnType.Guid:
					{
						var parameter = new NpgsqlParameter($"@{column.DbName!}", NpgsqlDbType.Array | NpgsqlDbType.Uuid);
						parameter.Value = new List<Guid?>();
						paramsDict.Add(column.DbName!, parameter);
					}
					break;
				case TfDatabaseColumnType.Text:
					{
						var parameter = new NpgsqlParameter($"@{column.DbName!}", NpgsqlDbType.Array | NpgsqlDbType.Text);
						parameter.Value = new List<string>();
						paramsDict.Add(column.DbName!, parameter);
					}
					break;
				case TfDatabaseColumnType.ShortText:
					{
						var parameter = new NpgsqlParameter($"@{column.DbName!}", NpgsqlDbType.Array | NpgsqlDbType.Varchar);
						parameter.Value = new List<string>();
						paramsDict.Add(column.DbName!, parameter);
					}
					break;
				case TfDatabaseColumnType.DateOnly:
				case TfDatabaseColumnType.DateTime:
					{
						var parameter = new NpgsqlParameter($"@{column.DbName!}", NpgsqlDbType.Array | NpgsqlDbType.Timestamp );
						parameter.Value = new List<DateTime?>();
						paramsDict.Add(column.DbName!, parameter);
					}
					break;
				case TfDatabaseColumnType.ShortInteger:
					{
						var parameter = new NpgsqlParameter($"@{column.DbName!}", NpgsqlDbType.Array | NpgsqlDbType.Smallint);
						parameter.Value = new List<short?>();
						paramsDict.Add(column.DbName!, parameter);
					}
					break;
				case TfDatabaseColumnType.Integer:
					{
						var parameter = new NpgsqlParameter($"@{column.DbName!}", NpgsqlDbType.Array | NpgsqlDbType.Integer);
						parameter.Value = new List<int?>();
						paramsDict.Add(column.DbName!, parameter);
					}
					break;
				case TfDatabaseColumnType.LongInteger:
					{
						var parameter = new NpgsqlParameter($"@{column.DbName!}", NpgsqlDbType.Array | NpgsqlDbType.Bigint);
						parameter.Value = new List<long?>();
						paramsDict.Add(column.DbName!, parameter);
					}
					break;
				case TfDatabaseColumnType.Number:
					{
						var parameter = new NpgsqlParameter($"@{column.DbName!}", NpgsqlDbType.Array | NpgsqlDbType.Numeric);
						parameter.Value = new List<decimal?>();
						paramsDict.Add(column.DbName!, parameter);
					}
					break;
				default:
					throw new Exception("Not supported database type");
			}
		}

		//add system columns
		{
			columnNames.Add($"tf_id");
			columnNames.Add($"tf_created_on");
			columnNames.Add($"tf_updated_on");
			columnNames.Add($"tf_row_index");
			columnNames.Add($"tf_search");

			var parameter = new NpgsqlParameter($"@tf_id", NpgsqlDbType.Array | NpgsqlDbType.Uuid);
			parameter.Value = new List<Guid>();
			paramsDict.Add("tf_id", parameter);

			parameter = new NpgsqlParameter($"@tf_created_on", NpgsqlDbType.Array | NpgsqlDbType.Date);
			parameter.Value = new List<DateTime>();
			paramsDict.Add("tf_created_on", parameter);

			parameter = new NpgsqlParameter($"@tf_updated_on", NpgsqlDbType.Array | NpgsqlDbType.Date);
			parameter.Value = new List<DateTime>();
			paramsDict.Add("tf_updated_on", parameter);

			parameter = new NpgsqlParameter($"@tf_row_index", NpgsqlDbType.Array | NpgsqlDbType.Integer);
			parameter.Value = new List<int>();
			paramsDict.Add("tf_row_index", parameter);

			parameter = new NpgsqlParameter($"@tf_search", NpgsqlDbType.Array | NpgsqlDbType.Text);
			parameter.Value = new List<string>();
			paramsDict.Add("tf_search", parameter);
		}

		#endregion

		var columnsWithSource = provider.Columns.Where(x => !string.IsNullOrWhiteSpace(x.SourceName)).ToList();
		var columnsWithoutSource = provider.Columns.Where(x => string.IsNullOrWhiteSpace(x.SourceName)).ToList();

		int currentRowIndex = 0;
		foreach (var row in newData)
		{
			currentRowIndex++;
			var key = GetDataRowPrimaryKeyValueAsString(provider, row, currentRowIndex);
			var searchSb = new StringBuilder();

			DataRow? existingDataRow = null;
			if (existingDataDict.ContainsKey(key))
			{
				existingDataRow = existingDataDict[key];
			}

			ProcessColumnsWithoutSource(columnsWithoutSource, paramsDict, searchSb,
				existingDataRow, uniqueValuesDict);

			ProcessColumnsWithSource(columnsWithSource, paramsDict, searchSb, 
				row, uniqueValuesDict);

			#region <--- processs system columns data --->

			if (existingDataDict.ContainsKey(key))
			{
				//if row already exists (found by specified in data provider key)
				//we use existing system data
				((List<Guid>)paramsDict["tf_id"].Value!).Add((Guid)existingDataDict[key]["tf_id"]);
				((List<DateTime>)paramsDict["tf_created_on"].Value!).Add((DateTime)existingDataDict[key]["tf_created_on"]);
				((List<DateTime>)paramsDict["tf_updated_on"].Value!).Add((DateTime)DateTime.Now);
				((List<int>)paramsDict["tf_row_index"].Value!).Add(currentRowIndex);
				((List<string>)paramsDict["tf_search"].Value!).Add(searchSb.ToString());
			}
			else
			{
				//if it is a new row, we init system data
				((List<Guid>)paramsDict["tf_id"].Value!).Add((Guid)Guid.NewGuid());
				((List<DateTime>)paramsDict["tf_created_on"].Value!).Add((DateTime)DateTime.Now);
				((List<DateTime>)paramsDict["tf_updated_on"].Value!).Add((DateTime)DateTime.Now);
				((List<int>)paramsDict["tf_row_index"].Value!).Add(currentRowIndex);
				((List<string>)paramsDict["tf_search"].Value!).Add(searchSb.ToString());
			}

			#endregion
		}

		paramList = new List<NpgsqlParameter>();
		foreach (var column in columnNames)
		{
			paramList.Add(paramsDict[column]);
		}
	}

	private void ProcessColumnsWithoutSource(
		List<TfDataProviderColumn> columns,
		Dictionary<string, NpgsqlParameter> paramsDict,
		StringBuilder searchSb,
		DataRow? existingDataRow,
		Dictionary<string, object> uniqueValuesDict)
	{
		if (existingDataRow is not null)
		{
			foreach (var column in columns)
			{
				//do not process expression columns
				if (!string.IsNullOrWhiteSpace(column.Expression))
					continue;

				if (column.IncludeInTableSearch)
				{
					object value = existingDataRow[column.DbName!];
					if (value is not null)
						searchSb.Append($" {value}");
				}

				switch (column.DbType)
				{
					case TfDatabaseColumnType.Boolean:
						{
							((List<bool?>)paramsDict[column.DbName!].Value!).Add((bool?)existingDataRow[column.DbName!]);
							//we do not process unique for boolean type, because it can have only 2 values - true or false
						}
						break;
					case TfDatabaseColumnType.Guid:
						{
							((List<Guid?>)paramsDict[column.DbName!].Value!).Add((Guid?)existingDataRow[column.DbName!]);
							
							if(column.IsUnique)
							{
								if (!uniqueValuesDict.ContainsKey(column.DbName!))
									uniqueValuesDict[column.DbName!] = new HashSet<Guid>();

								((HashSet<Guid>)uniqueValuesDict[column.DbName!]).Add((Guid)existingDataRow[column.DbName!]);
							}
						}
						break;
					case TfDatabaseColumnType.Text:
					case TfDatabaseColumnType.ShortText:
						{
							((List<string>)paramsDict[column.DbName!].Value!).Add((string)existingDataRow[column.DbName!]);

							if (column.IsUnique)
							{
								if (!uniqueValuesDict.ContainsKey(column.DbName!))
									uniqueValuesDict[column.DbName!] = new HashSet<string>();

								((HashSet<string>)uniqueValuesDict[column.DbName!]).Add((string)existingDataRow[column.DbName!]);
							}
						}
						break;
					case TfDatabaseColumnType.DateOnly:
					case TfDatabaseColumnType.DateTime:
						{
							DateTime? value = null;
							if (existingDataRow[column.DbName!] is DateOnly)
							{
								value = ((DateOnly)existingDataRow[column.DbName!]).ToDateTime();
							}
							else if (existingDataRow[column.DbName!] is DateOnly?)
							{
								if (existingDataRow[column.DbName!] == null)
								{
									value = null;
								}
								else
								{
									value = ((DateOnly)existingDataRow[column.DbName!]).ToDateTime();
								}
							}
							else if (existingDataRow[column.DbName!] is DateTime)
							{
								value = (DateTime)existingDataRow[column.DbName!];
							}
							else if (existingDataRow[column.DbName!] is DateTime?)
							{
								if (existingDataRow[column.DbName!] == null)
								{
									value = null;
								}
								else
								{
									value = (DateTime)existingDataRow[column.DbName!];
								}
							}
							else if (existingDataRow[column.DbName!] == null)
							{
								value = null;
							}
							else
							{
								throw new Exception($"Some source rows contains non DateTime or DateOnly objects for column '{column.DbName!}' of type Date\\DateTime.");
							}

							((List<DateTime?>)paramsDict[column.DbName!].Value!).Add(value);

							if (column.IsUnique)
							{
								if (!uniqueValuesDict.ContainsKey(column.DbName!))
									uniqueValuesDict[column.DbName!] = new HashSet<DateTime>();

								((HashSet<DateTime>)uniqueValuesDict[column.DbName!]).Add((DateTime)existingDataRow[column.DbName!]);
							}
						}
						break;
					case TfDatabaseColumnType.ShortInteger:
						{
							((List<short?>)paramsDict[column.DbName!].Value!).Add((short?)existingDataRow[column.DbName!]);

							if (column.IsUnique)
							{
								if (!uniqueValuesDict.ContainsKey(column.DbName!))
									uniqueValuesDict[column.DbName!] = new HashSet<short>();

								((HashSet<short>)uniqueValuesDict[column.DbName!]).Add((short)existingDataRow[column.DbName!]);
							}
						}
						break;
					case TfDatabaseColumnType.Integer:
						{
							((List<int?>)paramsDict[column.DbName!].Value!).Add((int?)existingDataRow[column.DbName!]);

							if (column.IsUnique)
							{
								if (!uniqueValuesDict.ContainsKey(column.DbName!))
									uniqueValuesDict[column.DbName!] = new HashSet<int>();

								((HashSet<int>)uniqueValuesDict[column.DbName!]).Add((int)existingDataRow[column.DbName!]);
							}
						}
						break;
					case TfDatabaseColumnType.LongInteger:
						{
							((List<long?>)paramsDict[column.DbName!].Value!).Add((long?)existingDataRow[column.DbName!]);
							
							if (column.IsUnique)
							{
								if (!uniqueValuesDict.ContainsKey(column.DbName!))
									uniqueValuesDict[column.DbName!] = new HashSet<long>();

								((HashSet<long>)uniqueValuesDict[column.DbName!]).Add((long)existingDataRow[column.DbName!]);
							}
						}
						break;
					case TfDatabaseColumnType.Number:
						{
							((List<decimal?>)paramsDict[column.DbName!].Value!).Add((decimal?)existingDataRow[column.DbName!]);
							
							if (column.IsUnique)
							{
								if (!uniqueValuesDict.ContainsKey(column.DbName!))
									uniqueValuesDict[column.DbName!] = new HashSet<decimal>();

								((HashSet<decimal>)uniqueValuesDict[column.DbName!]).Add((decimal)existingDataRow[column.DbName!]);
							}
						}
						break;
					default:
						throw new Exception("Not supported database type");
				}
			}
		}
		else
		{
			foreach (var column in columns)
			{
				//do not process expression columns
				if (!string.IsNullOrWhiteSpace(column.Expression))
					continue;

				object? defaultValue = null;

				if (!column.IsNullable || column.IsUnique)
				{
					defaultValue = GetColumnDefaultOrUniqueValue(column, uniqueValuesDict);
				}

				if (column.IncludeInTableSearch)
				{
					if (!column.IsNullable && defaultValue is not null)
					{
						searchSb.Append($" {defaultValue}");
					}
				}

				switch (column.DbType)
				{
					case TfDatabaseColumnType.Boolean:
						((List<bool?>)paramsDict[column.DbName!].Value!).Add((bool?)defaultValue);
						break;
					case TfDatabaseColumnType.Guid:
						((List<Guid?>)paramsDict[column.DbName!].Value!).Add((Guid?)defaultValue);
						break;
					case TfDatabaseColumnType.Text:
					case TfDatabaseColumnType.ShortText:
						((List<string>)paramsDict[column.DbName!].Value!).Add((string)defaultValue);
						break;
					case TfDatabaseColumnType.DateOnly:
					case TfDatabaseColumnType.DateTime:
						((List<DateTime?>)paramsDict[column.DbName!].Value!).Add((DateTime?)defaultValue);
						break;
					case TfDatabaseColumnType.ShortInteger:
						((List<short?>)paramsDict[column.DbName!].Value!).Add((short?)defaultValue);
						break;
					case TfDatabaseColumnType.Integer:
						((List<int?>)paramsDict[column.DbName!].Value!).Add((int?)defaultValue);
						break;
					case TfDatabaseColumnType.LongInteger:
						((List<long?>)paramsDict[column.DbName!].Value!).Add((long?)defaultValue);
						break;
					case TfDatabaseColumnType.Number:
						((List<decimal?>)paramsDict[column.DbName!].Value!).Add((decimal?)defaultValue);
						break;
					default:
						throw new Exception("Not supported database type");
				}
			}
		}
	}

	private void ProcessColumnsWithSource(
		List<TfDataProviderColumn> columns,
		Dictionary<string, NpgsqlParameter> paramsDict,
		StringBuilder searchSb,
		TfDataProviderDataRow newDataRow,
		Dictionary<string, object> uniqueValuesDict)
	{
		foreach (var column in columns)
		{
			//do not process expression columns
			if (!string.IsNullOrWhiteSpace(column.Expression))
				continue;

			//for unique columns with null values we generate new unique value
			if (column.IsUnique && newDataRow[column.DbName!] == null)
			{
				newDataRow[column.DbName!] = GetColumnDefaultOrUniqueValue(column, uniqueValuesDict);
			}

			if (column.IncludeInTableSearch)
			{
				object? value = newDataRow[column.DbName!];
				if (value is not null)
					searchSb.Append($" {value}");
			}

			switch (column.DbType)
			{
				case TfDatabaseColumnType.Boolean:
					{
						((List<bool?>)paramsDict[column.DbName!].Value!).Add((bool?)newDataRow[column.DbName!]);
					}
					break;
				case TfDatabaseColumnType.Guid:
					{
						((List<Guid?>)paramsDict[column.DbName!].Value!).Add((Guid?)newDataRow[column.DbName!]);
					}
					break;
				case TfDatabaseColumnType.Text:
				case TfDatabaseColumnType.ShortText:
					{
						((List<string>)paramsDict[column.DbName!].Value!).Add((string)newDataRow[column.DbName!]);
					}
					break;
				case TfDatabaseColumnType.DateOnly:
				case TfDatabaseColumnType.DateTime:
					{
						DateTime? value = null;
						if (newDataRow[column.DbName!] is DateOnly)
						{
							value = ((DateOnly)newDataRow[column.DbName!]!).ToDateTime();
						}
						else if (newDataRow[column.DbName!] is DateOnly?)
						{
							if (newDataRow[column.DbName!] == null)
							{
								value = null;
							}
							else
							{
								value = ((DateOnly)newDataRow[column.DbName!]!).ToDateTime();
							}
						}
						else if (newDataRow[column.DbName!] is DateTime)
						{
							value = (DateTime)newDataRow[column.DbName!]!;
						}
						else if (newDataRow[column.DbName!] is DateTime?)
						{
							if (newDataRow[column.DbName!] == null)
							{
								value = null;
							}
							else
							{
								value = (DateTime)newDataRow[column.DbName!]!;
							}
						}
						else if (newDataRow[column.DbName!] == null)
						{
							value = null;
						}
						else
						{
							throw new Exception($"Some source rows contains non DateTime or DateOnly objects for column '{column.DbName!}' of type Date\\DateTime.");
						}

						((List<DateTime?>)paramsDict[column.DbName!].Value!).Add(value);
					}
					break;
				case TfDatabaseColumnType.ShortInteger:
					{
						((List<short?>)paramsDict[column.DbName!].Value!).Add((short?)newDataRow[column.DbName!]);
					}
					break;
				case TfDatabaseColumnType.Integer:
					{
						((List<int?>)paramsDict[column.DbName!].Value!).Add((int?)newDataRow[column.DbName!]);

					}
					break;
				case TfDatabaseColumnType.LongInteger:
					{
						((List<long?>)paramsDict[column.DbName!].Value!).Add((long?)newDataRow[column.DbName!]);
					}
					break;
				case TfDatabaseColumnType.Number:
					{
						((List<decimal?>)paramsDict[column.DbName!].Value!).Add((decimal?)newDataRow[column.DbName!]);
					}
					break;
				default:
					throw new Exception("Not supported database type");
			}
		}
	}

}