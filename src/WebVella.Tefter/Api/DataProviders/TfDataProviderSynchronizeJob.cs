namespace WebVella.Tefter;

internal class TfDataProviderSynchronizeJob : BackgroundService
{
	private readonly ITfDataProviderManager _providerManager;
	private readonly ILogger<TfDataProviderSynchronizeJob> _logger;

	public TfDataProviderSynchronizeJob(
		ITfDataProviderManager providerManager,
		ILogger<TfDataProviderSynchronizeJob> logger)
	{
		_providerManager = providerManager;
		_logger = logger;
	}

	protected override async Task ExecuteAsync(
		CancellationToken stoppingToken)
	{
		_logger.LogInformation($"{this.GetType().Name} is running.");

		//initial 10 sec wait
		await Task.Delay(10 * 1000);

		await ProcessTasks(stoppingToken);
	}

	private async Task ProcessTasks(
		CancellationToken stoppingToken)
	{
		try
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				//update already started, but not finished task as Failed
				{
					var inprogresTasksResult = _providerManager.GetSynchronizationTasks(
						status: TfSynchronizationStatus.InProgress);

					if (!inprogresTasksResult.IsSuccess)
						throw new Exception("Unable to get in progress synchronization tasks");

					foreach (var taskInProgress in inprogresTasksResult.Value)
					{
						var result = _providerManager.UpdateSychronizationTask(
							taskInProgress.Id,
							TfSynchronizationStatus.Failed,
							completedOn: DateTime.Now);

						if (!result.IsSuccess)
							throw new Exception("Unable to update synchronization tasks");
					}
				}

				var pendingTasksResult = _providerManager.GetSynchronizationTasks(
				status: TfSynchronizationStatus.Pending);

				if (!pendingTasksResult.IsSuccess)
					throw new Exception("Unable to get pending synchronization tasks");

				if (pendingTasksResult.Value.Count() == 0)
				{
					await Task.Delay(10 * 1000); //check every 10 seconds
					continue;
				}

				var task = pendingTasksResult.Value
					.OrderBy(x => x.CreatedOn).FirstOrDefault();

				try
				{
					var result = _providerManager.UpdateSychronizationTask(
						task.Id,
						TfSynchronizationStatus.InProgress,
						startedOn: DateTime.Now);

					if (!result.IsSuccess)
						throw new Exception("Unable to update synchronization tasks");

					_providerManager.Synchronize(task);

					result = _providerManager.UpdateSychronizationTask(
						task.Id,
						TfSynchronizationStatus.Completed,
						completedOn: DateTime.Now);

					if (!result.IsSuccess)
						throw new Exception("Unable to update synchronization tasks");
				}
				catch (OperationCanceledException ex)
				{
					var result = _providerManager.UpdateSychronizationTask(
						task.Id,
						TfSynchronizationStatus.Failed,
						completedOn: DateTime.Now);

					if (!result.IsSuccess)
						throw new Exception("Unable to update synchronization tasks");

					result = _providerManager.CreateSynchronizationResultInfo(
						syncTaskId: task.Id,
						tfRowIndex: null,
						tfId: null,
						warning: null,
						error: ex.Message);

					if (!result.IsSuccess)
						throw new Exception("Unable to write synchronization result info.");

					// Prevent throwing if stoppingToken was signaled
					_logger.LogError($"Task canceled {task.GetType().Name}", ex);
				}
				catch (Exception ex)
				{
					var result = _providerManager.UpdateSychronizationTask(
						task.Id,
						TfSynchronizationStatus.Failed,
						completedOn: DateTime.Now);

					if (!result.IsSuccess)
						throw new Exception("Unable to update synchronization tasks");

					result = _providerManager.CreateSynchronizationResultInfo(
						syncTaskId: task.Id,
						tfRowIndex: null,
						tfId: null,
						warning: null,
						error: ex.Message);

					if (!result.IsSuccess)
						throw new Exception("Unable to write synchronization result info.");
				}
			}
		}
		catch (OperationCanceledException)
		{
			// Prevent throwing if stoppingToken was signaled
		}
		catch (Exception ex)
		{
			_logger.LogError($"Exception in {this.GetType().Name} while processing synchronization task", ex);
			await Task.Delay(10 * 60 * 1000); //10 minutes
		}
	}

	public override async Task StopAsync(
		CancellationToken stoppingToken)
	{
		_logger.LogInformation($"{this.GetType().Name} is stopping.");
		await base.StopAsync(stoppingToken);
	}
}
