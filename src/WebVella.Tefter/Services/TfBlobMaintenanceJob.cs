namespace WebVella.Tefter.Services;

internal class TfBlobMaintenanceJob : BackgroundService
{
	private readonly ITfBlobManager _blobManager;
	private readonly ILogger<TfBlobMaintenanceJob> _logger;

	public TfBlobMaintenanceJob(
		ITfBlobManager blobManager,
		ILogger<TfBlobMaintenanceJob> logger)
	{
		_blobManager = blobManager;	
		_logger = logger;
	}

	protected override async Task ExecuteAsync(
		CancellationToken stoppingToken)
	{
		_logger.LogInformation($"{this.GetType().Name} is running.");

		//initial 10 sec wait
		await Task.Delay(10 * 1000);

		while (!stoppingToken.IsCancellationRequested)
		{
			await _blobManager.CleanupEmptyFoldersAndExpiredTemporaryFilesAsync(stoppingToken);
			
			await Task.Delay(4320000); //12 hour
		}
	}

	public override async Task StopAsync(
		CancellationToken stoppingToken)
	{
		_logger.LogInformation($"{this.GetType().Name} is stopping.");
		await base.StopAsync(stoppingToken);
	}

}
