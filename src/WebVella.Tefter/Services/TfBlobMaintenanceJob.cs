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
			try
			{
				await _blobManager.CleanupEmptyFoldersAndExpiredTemporaryFilesAsync(stoppingToken);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"{this.GetType().Name} exception");
			}

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
