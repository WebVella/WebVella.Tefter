﻿namespace WebVella.Tefter.Jobs;

internal class TfDataProviderSynchScheduleJob : BackgroundService
{
	private readonly ITfService _tfService;
	private readonly ILogger<TfDataProviderSynchScheduleJob> _logger;

	public TfDataProviderSynchScheduleJob(
		ITfService tfService,
		ILogger<TfDataProviderSynchScheduleJob> logger)
	{
		_tfService = tfService;
		_logger = logger;
	}

	protected override async Task ExecuteAsync(
		CancellationToken stoppingToken)
	{
		_logger.LogInformation($"{GetType().Name} is running.");

		//initial 60 sec wait
		await Task.Delay(60 * 1000);

		SynchronizationContext.SetSynchronizationContext(new TfHostedServiceSynchContext());
		await Task.Run(async () =>
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				try
				{
					await _tfService.CheckScheduleSynchronizationTasksAsync(stoppingToken);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, $"{GetType().Name} exception");
				}

				await Task.Delay(5 * 60 * 1000); //5min
			}

		}, stoppingToken);
	}

	public override async Task StopAsync(
		CancellationToken stoppingToken)
	{
		_logger.LogInformation($"{GetType().Name} is stopping.");
		await base.StopAsync(stoppingToken);
	}

}
