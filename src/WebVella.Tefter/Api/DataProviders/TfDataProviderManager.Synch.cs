namespace WebVella.Tefter;

public partial interface ITfDataProviderManager
{
	internal Result<TfDataProviderSynchronizeTask> GetSynchronizationTask(
		Guid taskId);

	internal Result<List<TfDataProviderSynchronizeTask>> GetSynchronizationTasks(
		Guid? providerId = null,
		TfSynchronizationStatus? status = null);

	internal Result CreateSynchronizationTask(
		Guid providerId,
		TfSynchronizationPolicy synchPolicy);

	internal Result UpdateSychronizationTask(
		Guid taskId,
		TfSynchronizationStatus status,
		DateTime? startedOn = null,
		DateTime? completedOn = null);

	internal Result CreateSynchronizationResultInfo(
		Guid syncTaskId,
		int? tfRowIndex,
		Guid? tfId,
		string info = null,
		string warning = null,
		string error = null);

	public void Synchronize(
		TfDataProviderSynchronizeTask task);
}

public partial class TfDataProviderManager : ITfDataProviderManager
{
	#region <--- Synchronization Tasks --->

	public Result<TfDataProviderSynchronizeTask> GetSynchronizationTask(
		Guid taskId)
	{
		try
		{
			var dbo = _dboManager.Get<TfDataProviderSynchronizeTaskDbo>(taskId);

			if (dbo == null)
				return Result.Ok();

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
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to get data providers").CausedBy(ex));
		}
	}

	public Result<List<TfDataProviderSynchronizeTask>> GetSynchronizationTasks(
		Guid? providerId = null,
		TfSynchronizationStatus? status = null)
	{
		try
		{
			var orderSettings = new OrderSettings(
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

			return Result.Ok(result);
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to get list of synchronization tasks").CausedBy(ex));
		}
	}

	public Result CreateSynchronizationTask(
		Guid providerId,
		TfSynchronizationPolicy synchPolicy)
	{
		try
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
				throw new DatabaseException("Failed to insert synchronization task.");

			return Result.Ok();
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to insert synchronization task.").CausedBy(ex));
		}
	}

	public Result UpdateSychronizationTask(
		Guid taskId,
		TfSynchronizationStatus status,
		DateTime? startedOn = null,
		DateTime? completedOn = null)
	{
		try
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
				throw new DatabaseException("Failed to update synchronization task in database.");

			return Result.Ok();
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to update synchronization task.").CausedBy(ex));
		}
	}

	#endregion

	#region <--- Synchronization Result Info --->

	public Result CreateSynchronizationResultInfo(
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
				throw new DatabaseException("Failed to insert synchronization task result info.");

			return Result.Ok();
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to insert synchronization task.").CausedBy(ex));
		}
	}

	#endregion

	#region <--- Synchronization --->

	public void Synchronize(
		TfDataProviderSynchronizeTask task)
	{

		var providerResult = GetProvider(task.DataProviderId);
		if (!providerResult.IsSuccess)
			throw new Exception("Unable to get provider.");

		var provider = providerResult.Value;

		var rows = provider.GetRows();

		if (task.Policy.ComparisonType == TfSynchronizationPolicyComparisonType.ByRowOrder)
		{
			_dataManager.DeleteProviderRowsAfterIndex(
				provider,
				rows.Count + 1);

			int currentRowIndex = 0;
			var sourceColumns = provider.Columns.Where(x => !string.IsNullOrWhiteSpace(x.SourceName));

			foreach (var row in rows)
			{
				currentRowIndex++;

				foreach (var warning in row.Warnings)
				{
					var result = CreateSynchronizationResultInfo(
						taskId: task.Id,
						tfRowIndex: currentRowIndex,
						tfId: null,
						warning: warning);

					if (!result.IsSuccess)
						throw new Exception("Unable to write synchronization result info.");
				}

				if (row.Errors.Count > 0)
				{
					foreach (var error in row.Errors)
					{
						var result = CreateSynchronizationResultInfo(
							taskId: task.Id,
							tfRowIndex: currentRowIndex,
							tfId: null,
							error: error);

						if (!result.IsSuccess)
							throw new Exception("Unable to write synchronization result info.");
					}
					continue;
				}


				var providerRowResult = _dataManager.GetProviderRow(
					provider,
					currentRowIndex);

				if (!providerRowResult.IsSuccess)
					throw new Exception($"Unable to get current provider row with index {currentRowIndex}");

				var providerRow = providerRowResult.Value;
				if (providerRow is null)
				{
					//we init only row index here
					row["tf_row_index"] = currentRowIndex;

					_dataManager.InsertNewProviderRow(
						provider,
						row);

					if (task.Policy.WriteInfoResults)
					{
						var result = CreateSynchronizationResultInfo(
							taskId: task.Id,
							tfRowIndex: currentRowIndex,
							tfId: (Guid)providerRow["tf_id"],
							info: "The row not found in database. New row created.");

						if (!result.IsSuccess)
							throw new Exception("Unable to write synchronization result info.");
					}
				}
				else
				{
					bool foundDiff = false;
					foreach (var column in sourceColumns)
					{
						if (!row.ColumnNames.Contains(column.DbName))
						{
							string error = $"Column {column.DbName} is not found.";
							row.AddError(error);

							var result = CreateSynchronizationResultInfo(
								taskId: task.Id,
								tfRowIndex: currentRowIndex,
								tfId: null,
								error: error);

							if (!result.IsSuccess)
								throw new Exception("Unable to write synchronization result info.");

							break;
						}

						bool valuesEqual = AreColumnValuesEqual(
							column,
							row[column.DbName],
							providerRow[column.DbName]);

						if (valuesEqual == false)
						{
							providerRow[column.DbName] = row[column.DbName];
							foundDiff = true;
						}
					}

					if (row.Errors.Count > 0)
						continue;

					if (foundDiff)
					{
						if (task.Policy.WriteInfoResults)
						{
							var resultInfo = CreateSynchronizationResultInfo(
								taskId: task.Id,
								tfRowIndex: currentRowIndex,
								tfId: (Guid)providerRow["tf_id"],
								info: "Some of row values differ. The row is updated.");

							if (!resultInfo.IsSuccess)
								throw new Exception("Unable to write synchronization result info.");
						}

						//set only row index here
						providerRow["tf_row_index"] = currentRowIndex;

						var result = _dataManager.UpdateProviderRow(
							provider,
							providerRow);

						if (!result.IsSuccess)
							throw new Exception("Unable to update data provider row with new values");
					}
					else if (task.Policy.WriteInfoResults)
					{
						var result = CreateSynchronizationResultInfo(
							taskId: task.Id,
							tfRowIndex: currentRowIndex,
							tfId: (Guid)providerRow["tf_id"],
							info: "No changes found.");

						if (!result.IsSuccess)
							throw new Exception("Unable to write synchronization result info.");
					}
				}
			}
		}
		else
			throw new Exception("Policy not implemented yet.");

		//final sych result value
		{
			var result = CreateSynchronizationResultInfo(
								taskId: task.Id,
								tfRowIndex: null,
								tfId: null,
								info: "Synchronization executed successfully.");

			if (!result.IsSuccess)
				throw new Exception("Unable to write synchronization result info.");
		}
	}

	private bool AreColumnValuesEqual(
		TfDataProviderColumn column,
		object value,
		object newValue)
	{
		try
		{
			switch (column.DbType)
			{
				case DatabaseColumnType.ShortText:
				case DatabaseColumnType.Text:
					return (value as string == newValue as string);

				case DatabaseColumnType.Boolean:
					return ((bool?)value == (bool?)newValue);

				case DatabaseColumnType.Guid:
					return ((Guid?)value == (Guid?)newValue);

				case DatabaseColumnType.DateTime:
					return ((DateTime?)value == (DateTime?)newValue);

				case DatabaseColumnType.Date:
					return ((DateOnly?)value == (DateOnly?)newValue);

				case DatabaseColumnType.ShortInteger:
					return ((short?)value == (short?)newValue);

				case DatabaseColumnType.Integer:
					return ((int?)value == (int?)newValue);

				case DatabaseColumnType.LongInteger:
					return ((long?)value == (long?)newValue);

				case DatabaseColumnType.Number:
					return ((decimal?)value == (decimal?)newValue);

				default:
					throw new Exception("Not supported source type");
			}
		}
		catch
		{
			return false;
		}
	}

	#endregion
}
