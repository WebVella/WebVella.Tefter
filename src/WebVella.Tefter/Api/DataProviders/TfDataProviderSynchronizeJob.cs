namespace WebVella.Tefter;

internal class TfDataProviderSynchronizeJob : BackgroundService
{

	private readonly ITfDataProviderSynchronizeTaskList _taskList;
	private readonly ILogger<TfDataProviderSynchronizeJob> _logger;

	public TfDataProviderSynchronizeJob(
		ITfDataProviderSynchronizeTaskList taskList,
		ILogger<TfDataProviderSynchronizeJob> logger)
	{
		_logger = logger;
		_taskList = taskList;
	}
	
	protected override Task ExecuteAsync(
		CancellationToken stoppingToken)
	{
		_logger.LogInformation($"{this.GetType().Name} is running.");
		return ProcessTasks(stoppingToken);
	}

	private async Task ProcessTasks(
		CancellationToken stoppingToken)
	{
		try
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				TfDataProviderSynchronizeTask task = _taskList.GetPendingTask();

				if (task is null)
				{
					await Task.Delay(1000);
					continue;
				}

				try
				{
					task.StartedOn = DateTime.Now;
					var data = task.DataProvider.GetSourceData();

					//TODO synchronization

					task.Result = new TfSynchronizationResult();
					task.Status = TfSynchronizationStatus.Completed;
				}
				catch (OperationCanceledException ex)
				{
					task.Result = new TfSynchronizationResult { Error = "Synchronization task was canceled." };
					task.Status = TfSynchronizationStatus.Failed;
					// Prevent throwing if stoppingToken was signaled
					_logger.LogError($"Task canceled {task.GetType().Name}", ex);
				}
				catch (Exception ex)
				{
					task.Status = TfSynchronizationStatus.Failed;
					task.Result = new TfSynchronizationResult { Error = $"Synchronization task ended with exception: {ex.Message}" };
				}
				finally
				{
					task.CompletedOn = DateTime.Now;
					if (task.Status == TfSynchronizationStatus.Pending)
						task.Status = TfSynchronizationStatus.Completed;
				}
			}
		}
		catch (OperationCanceledException)
		{
			// Prevent throwing if stoppingToken was signaled
		}
		catch (Exception ex)
		{
			_logger.LogError($"Exception in {this.GetType().Name} while processing task queue", ex);
		}
	}

	public override async Task StopAsync(
		CancellationToken stoppingToken)
	{
		_logger.LogInformation($"{this.GetType().Name} is stopping.");
		await base.StopAsync(stoppingToken);
	}
}
