namespace WebVella.Tefter.Jobs;

internal class TfBlobMaintenanceJob : BackgroundService
{
	private readonly ITfService _tfService;
	private readonly ILogger<TfBlobMaintenanceJob> _logger;

	public TfBlobMaintenanceJob(
		ITfService tfService,
		ILogger<TfBlobMaintenanceJob> logger)
	{
		_tfService = tfService;
		_logger = logger;
	}

	protected override async Task ExecuteAsync(
		CancellationToken stoppingToken)
	{
		_logger.LogInformation($"{GetType().Name} is running.");

		//initial 120 sec wait
		await Task.Delay(120 * 1000);

		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				await _tfService.CleanupEmptyFoldersAndExpiredTemporaryFilesAsync(stoppingToken);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"{GetType().Name} exception");
			}

			await Task.Delay(4320000); //12 hour
		}
	}

	public override async Task StopAsync(
		CancellationToken stoppingToken)
	{
		_logger.LogInformation($"{GetType().Name} is stopping.");
		await base.StopAsync(stoppingToken);
	}

}
