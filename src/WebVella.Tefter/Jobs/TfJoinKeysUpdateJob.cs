namespace WebVella.Tefter.Jobs;

internal class TfJoinKeysUpdateJob : BackgroundService
{
	private readonly ITfService _tfService;
	private readonly ILogger<TfJoinKeysUpdateJob> _logger;

	public TfJoinKeysUpdateJob(
		ITfService tfService,
		ILogger<TfJoinKeysUpdateJob> logger)
	{
		_tfService = tfService;
		_logger = logger;
	}

	protected override async Task ExecuteAsync(
		CancellationToken stoppingToken)
	{
		_logger.LogInformation($"{GetType().Name} is running.");

		//This job will be removed

		////initial 600 sec wait
		//await Task.Delay(600 * 1000);

		//while (!stoppingToken.IsCancellationRequested)
		//{
		//	try
		//	{
		//		await _tfService.UpdateJoinKeysVersionAsync(stoppingToken);
		//	}
		//	catch (Exception ex)
		//	{
		//		_logger.LogError(ex, $"{GetType().Name} exception");
		//	}

		//	await Task.Delay(5 * 60 * 1000); //5mins
		//}
	}

	public override async Task StopAsync(
		CancellationToken stoppingToken)
	{
		_logger.LogInformation($"{GetType().Name} is stopping.");
		await base.StopAsync(stoppingToken);
	}

}
