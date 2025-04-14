namespace WebVella.Tefter.Jobs;

internal class TfDataProviderSynchronizeJob : BackgroundService
{
	private readonly ITfService _tfService;
	private readonly ILogger<TfDataProviderSynchronizeJob> _logger;

	public TfDataProviderSynchronizeJob(
		ITfService tfService,
		ILogger<TfDataProviderSynchronizeJob> logger)
	{
		_tfService = tfService;
		_logger = logger;
	}

	protected override async Task ExecuteAsync(
		CancellationToken stoppingToken)
	{
		_logger.LogInformation($"{GetType().Name} is running.");

#if DEBUG
		//initial 0 sec wait
		await Task.Delay(10 * 1000);
#else
		//initial 300 sec wait
		await Task.Delay(300 * 1000);
#endif


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
					var inProgresTasks = _tfService.GetSynchronizationTasks(
										status: TfSynchronizationStatus.InProgress);

					foreach (var taskInProgress in inProgresTasks)
					{
						_tfService.UpdateSychronizationTask(
							taskInProgress.Id,
							TfSynchronizationStatus.Failed,
							taskInProgress.SynchronizationLog,
							completedOn: DateTime.Now);

						_tfService.CreateSynchronizationResultInfo(
							syncTaskId: taskInProgress.Id,
							tfRowIndex: null,
							tfId: null,
							warning: null,
							error: "Application was stopped during synchronization process");
					}
				}

				var pendingTasks = _tfService.GetSynchronizationTasks(
									status: TfSynchronizationStatus.Pending);

				if (pendingTasks.Count() == 0)
				{
					await Task.Delay(10 * 1000); //check every 10 seconds
					continue;
				}

				var task = pendingTasks.OrderBy(x => x.CreatedOn).FirstOrDefault();

				try
				{
					_tfService.UpdateSychronizationTask(
						task.Id,
						TfSynchronizationStatus.InProgress,
						task.SynchronizationLog,
						startedOn: DateTime.Now);
										
					await _tfService.BulkSynchronize(task);

					_tfService.UpdateSychronizationTask(
						task.Id,
						TfSynchronizationStatus.Completed,
						task.SynchronizationLog,
						completedOn: DateTime.Now);

					_tfService.CreateSynchronizationResultInfo(
						syncTaskId: task.Id,
						tfRowIndex: null,
						tfId: null);
				}
				catch (OperationCanceledException ex)
				{
					_tfService.UpdateSychronizationTask(
						task.Id,
						TfSynchronizationStatus.Failed,
						task.SynchronizationLog,
						completedOn: DateTime.Now);

					_tfService.CreateSynchronizationResultInfo(
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
					_tfService.UpdateSychronizationTask(
						task.Id,
						TfSynchronizationStatus.Failed,
						task.SynchronizationLog,
						completedOn: DateTime.Now);

					_tfService.CreateSynchronizationResultInfo(
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
		var inProgresTasks = _tfService.GetSynchronizationTasks(
			status: TfSynchronizationStatus.InProgress);

		foreach (var taskInProgress in inProgresTasks)
		{
			_tfService.UpdateSychronizationTask(
				taskInProgress.Id,
				TfSynchronizationStatus.Failed,
				taskInProgress.SynchronizationLog,
				completedOn: DateTime.Now);

			_tfService.CreateSynchronizationResultInfo(
				syncTaskId: taskInProgress.Id,
				tfRowIndex: null,
				tfId: null,
				warning: null,
				error: "Application was stopped during synchronization process");
		}


		var pendingTasks = _tfService.GetSynchronizationTasks(
				status: TfSynchronizationStatus.Pending);

		if (pendingTasks.Count() == 0)
			return;

		var task = pendingTasks.OrderBy(x => x.CreatedOn).FirstOrDefault();

		try
		{
			_tfService.UpdateSychronizationTask(
				task.Id,
				TfSynchronizationStatus.InProgress,
				task.SynchronizationLog,
				startedOn: DateTime.Now);

			await _tfService.BulkSynchronize(task);

			_tfService.UpdateSychronizationTask(
				task.Id,
				TfSynchronizationStatus.Completed,
				task.SynchronizationLog,
				completedOn: DateTime.Now);
		}
		catch (OperationCanceledException ex)
		{
			_tfService.UpdateSychronizationTask(
				task.Id,
				TfSynchronizationStatus.Failed,
				task.SynchronizationLog,
				completedOn: DateTime.Now);

			_tfService.CreateSynchronizationResultInfo(
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
			_tfService.UpdateSychronizationTask(
				task.Id,
				TfSynchronizationStatus.Failed,
				task.SynchronizationLog,
				completedOn: DateTime.Now);

			_tfService.CreateSynchronizationResultInfo(
				syncTaskId: task.Id,
				tfRowIndex: null,
				tfId: null,
				warning: null,
				error: ex.Message);
		}
	}
}
