namespace WebVella.Tefter.Assets.Services;

internal class AssetBackgroundJob : BackgroundService
{
	private readonly ILogger<AssetBackgroundJob> _logger;

	public AssetBackgroundJob(
		ILogger<AssetBackgroundJob> logger)
	{
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
			//do something every minute
			await Task.Delay(60 * 1000);
		}
	}

	public override async Task StopAsync(
		CancellationToken stoppingToken)
	{
		_logger.LogInformation($"{this.GetType().Name} is stopping.");
		await base.StopAsync(stoppingToken);
	}

}
