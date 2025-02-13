namespace WebVella.Tefter.Jobs;

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
		_logger.LogInformation($"{GetType().Name} is running.");

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
					var inProgresTasks = _providerManager.GetSynchronizationTasks(
										status: TfSynchronizationStatus.InProgress);

					foreach (var taskInProgress in inProgresTasks)
					{
						_providerManager.UpdateSychronizationTask(
							taskInProgress.Id,
							TfSynchronizationStatus.Failed,
							completedOn: DateTime.Now);

						_providerManager.CreateSynchronizationResultInfo(
							syncTaskId: taskInProgress.Id,
							tfRowIndex: null,
							tfId: null,
							warning: null,
							error: "Application was stopped during synchronization process");
					}
				}

				var pendingTasks = _providerManager.GetSynchronizationTasks(
									status: TfSynchronizationStatus.Pending);

				if (pendingTasks.Count() == 0)
				{
					await Task.Delay(10 * 1000); //check every 10 seconds
					continue;
				}

				var task = pendingTasks.OrderBy(x => x.CreatedOn).FirstOrDefault();

				try
				{
					_providerManager.UpdateSychronizationTask(
						task.Id,
						TfSynchronizationStatus.InProgress,
						startedOn: DateTime.Now);
										
					await _providerManager.BulkSynchronize(task);

					_providerManager.UpdateSychronizationTask(
						task.Id,
						TfSynchronizationStatus.Completed,
						completedOn: DateTime.Now);
				}
				catch (OperationCanceledException ex)
				{
					_providerManager.UpdateSychronizationTask(
						task.Id,
						TfSynchronizationStatus.Failed,
						completedOn: DateTime.Now);

					_providerManager.CreateSynchronizationResultInfo(
						syncTaskId: task.Id,
						tfRowIndex: null,
						tfId: null,
						warning: null,
						error: ex.Message);

					// Prevent throwing if stoppingToken was signaled
					_logger.LogError($"Task canceled {task.GetType().Name}", ex);
				}
				catch (Exception ex)
				{
					_providerManager.UpdateSychronizationTask(
						task.Id,
						TfSynchronizationStatus.Failed,
						completedOn: DateTime.Now);

					_providerManager.CreateSynchronizationResultInfo(
						syncTaskId: task.Id,
						tfRowIndex: null,
						tfId: null,
						warning: null,
						error: ex.Message);
				}
			}
		}
		catch (OperationCanceledException)
		{
			// Prevent throwing if stoppingToken was signaled
		}
		catch (Exception ex)
		{
			_logger.LogError($"Exception in {GetType().Name} while processing synchronization task", ex);
			await Task.Delay(10 * 60 * 1000); //10 minutes
		}
	}

	public override async Task StopAsync(
		CancellationToken stoppingToken)
	{
		_logger.LogInformation($"{GetType().Name} is stopping.");
		await base.StopAsync(stoppingToken);
	}


	//this method is used to fill data in unit tests
	internal async Task StartManualProcessTasks()
	{
		var inProgresTasks = _providerManager.GetSynchronizationTasks(
			status: TfSynchronizationStatus.InProgress);

		foreach (var taskInProgress in inProgresTasks)
		{
			_providerManager.UpdateSychronizationTask(
				taskInProgress.Id,
				TfSynchronizationStatus.Failed,
				completedOn: DateTime.Now);

			_providerManager.CreateSynchronizationResultInfo(
				syncTaskId: taskInProgress.Id,
				tfRowIndex: null,
				tfId: null,
				warning: null,
				error: "Application was stopped during synchronization process");
		}


		var pendingTasks = _providerManager.GetSynchronizationTasks(
				status: TfSynchronizationStatus.Pending);

		if (pendingTasks.Count() == 0)
			return;

		var task = pendingTasks.OrderBy(x => x.CreatedOn).FirstOrDefault();

		try
		{
			_providerManager.UpdateSychronizationTask(
				task.Id,
				TfSynchronizationStatus.InProgress,
				startedOn: DateTime.Now);

			await _providerManager.BulkSynchronize(task);

			_providerManager.UpdateSychronizationTask(
				task.Id,
				TfSynchronizationStatus.Completed,
				completedOn: DateTime.Now);
		}
		catch (OperationCanceledException ex)
		{
			_providerManager.UpdateSychronizationTask(
				task.Id,
				TfSynchronizationStatus.Failed,
				completedOn: DateTime.Now);

			_providerManager.CreateSynchronizationResultInfo(
				syncTaskId: task.Id,
				tfRowIndex: null,
				tfId: null,
				warning: null,
				error: ex.Message);

			// Prevent throwing if stoppingToken was signaled
			_logger.LogError($"Task canceled {task.GetType().Name}", ex);
		}
		catch (Exception ex)
		{
			_providerManager.UpdateSychronizationTask(
				task.Id,
				TfSynchronizationStatus.Failed,
				completedOn: DateTime.Now);

			_providerManager.CreateSynchronizationResultInfo(
				syncTaskId: task.Id,
				tfRowIndex: null,
				tfId: null,
				warning: null,
				error: ex.Message);
		}
	}
}
