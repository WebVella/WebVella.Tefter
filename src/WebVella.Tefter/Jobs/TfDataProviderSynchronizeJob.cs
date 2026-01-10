using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace WebVella.Tefter.Jobs;

internal class TfDataProviderSynchronizeJob : BackgroundService
{
	private readonly IServiceScopeFactory _scopeFactory;
	private readonly ITfService _tfService;
	private readonly ILogger<TfDataProviderSynchronizeJob> _logger;


	public TfDataProviderSynchronizeJob(
		IServiceScopeFactory serviceScopeFactory,
		ITfService tfService,
		ILogger<TfDataProviderSynchronizeJob> logger)
	{
		_scopeFactory = serviceScopeFactory;
		_tfService = tfService;
		_logger = logger;
	}

	protected override async Task ExecuteAsync(
		CancellationToken stoppingToken)
	{
		_logger.LogInformation($"{GetType().Name} is running.");

#if DEBUG
		//initial 10 sec wait
		await Task.Delay(10 * 1000);

#else
		//initial 120 sec wait
		await Task.Delay(120 * 1000);
#endif

		SynchronizationContext.SetSynchronizationContext(new TfHostedServiceSynchContext());
		await Task.Run(async () =>
		{
			await ProcessTasks(stoppingToken).ConfigureAwait(false);

		}, stoppingToken );
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
						taskInProgress.SynchronizationLog.Log(
							"Application was stopped during synchronization process", 
							TfDataProviderSychronizationLogEntryType.Error);

						_tfService.UpdateSychronizationTask(
							taskInProgress.Id,
							TfSynchronizationStatus.Failed,
							taskInProgress.SynchronizationLog,
							completedOn: DateTime.Now);
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
				}
				catch (OperationCanceledException ex)
				{
					_tfService.UpdateSychronizationTask(
						task.Id,
						TfSynchronizationStatus.Failed,
						task.SynchronizationLog,
						completedOn: DateTime.Now);

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
					
					_logger.LogError($"Task failed {task.GetType().Name}", ex);
				}
				finally
				{
					await Task.Delay(1000, stoppingToken); //add 1 sec delay to prevent tight loop
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
			taskInProgress.SynchronizationLog.Log(
							"Application was stopped during synchronization process",
							TfDataProviderSychronizationLogEntryType.Error);

			_tfService.UpdateSychronizationTask(
				taskInProgress.Id,
				TfSynchronizationStatus.Failed,
				taskInProgress.SynchronizationLog,
				completedOn: DateTime.Now);
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

			// Prevent throwing if stoppingToken was signaled
			_logger.LogError($"Task canceled {task.GetType().Name}", ex);
		}
		catch(Exception ex)
		{
			_tfService.UpdateSychronizationTask(
				task.Id,
				TfSynchronizationStatus.Failed,
				task.SynchronizationLog,
				completedOn: DateTime.Now);

			_logger.LogError($"Task failed {task.GetType().Name}", ex);
		}
	}
}
