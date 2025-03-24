namespace WebVella.Tefter.Jobs;

internal class TfSharedKeysUpdateJob : BackgroundService
{
	private readonly ITfService _tfService;
	private readonly ILogger<TfSharedKeysUpdateJob> _logger;

	public TfSharedKeysUpdateJob(
		ITfService tfService,
		ILogger<TfSharedKeysUpdateJob> logger)
	{
		_tfService = tfService;
		_logger = logger;
	}

	protected override async Task ExecuteAsync(
		CancellationToken stoppingToken)
	{
		_logger.LogInformation($"{GetType().Name} is running.");

		//initial 600 sec wait
		await Task.Delay(600 * 1000);

		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				await _tfService.UpdateSharedKeysVersionAsync(stoppingToken);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"{GetType().Name} exception");
			}

			await Task.Delay(5 * 60 * 1000); //5mins
		}
	}

	public override async Task StopAsync(
		CancellationToken stoppingToken)
	{
		_logger.LogInformation($"{GetType().Name} is stopping.");
		await base.StopAsync(stoppingToken);
	}

}
