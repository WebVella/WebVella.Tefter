namespace WebVella.Tefter.EmailSender;

internal class SmtpBackgroundJob : BackgroundService
{
	private IServiceScopeFactory _serviceScopeFactory;
	private ISmtpService _smtpService;
	private ISmtpConfigurationService _smtpConfigurationService;
	private ILogger<SmtpBackgroundJob> _logger;

	public SmtpBackgroundJob(
		IServiceScopeFactory serviceScopeFactory)
	{
		_serviceScopeFactory = serviceScopeFactory;
	}

	protected override async Task ExecuteAsync(
		CancellationToken stoppingToken)
	{
		using (var scope = _serviceScopeFactory.CreateScope())
		{
			_smtpService = scope.ServiceProvider.GetRequiredService<ISmtpService>();

			_smtpConfigurationService = scope.ServiceProvider.GetRequiredService<ISmtpConfigurationService>();

			_logger = scope.ServiceProvider.GetRequiredService<ILogger<SmtpBackgroundJob>>();

			var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

			if (!_smtpConfigurationService.Enabled)
			{
				_logger.LogInformation($"{this.GetType().Name} will not run. Stopped in configuration.");
				return;
			}

			_logger.LogInformation($"{this.GetType().Name} is running.");

			if (!env.IsDevelopment())
			{
				await Task.Delay(30000, stoppingToken); //wait 30 sec before start
			}

			while (!stoppingToken.IsCancellationRequested)
			{
				try
				{
					var now = DateTime.Now;
					if (!env.IsDevelopment())
					{
						if (now.Minute % 5 != 0 || now.Second != 0) //every 5 mins
						{
							//check every sec
							await Task.Delay(1000, stoppingToken);
							continue;
						}
					}
					else
					{
						if (now.Second % 10 != 0) //every 10 sec in dev mode
						{
							//check every sec
							await Task.Delay(100, stoppingToken);
							continue;
						}
					}

					ExceptionDispatchInfo capturedException = null;

					try
					{
						await _smtpService.ProcessSmtpQueue();
					}
					catch (Exception ex)
					{
						capturedException = ExceptionDispatchInfo.Capture(ex);
					}

					if (capturedException != null)
					{
						_logger.LogError(capturedException.SourceException, capturedException.SourceException.Message);
						await Task.Delay(1000, stoppingToken); //1min wait
					}
				}
				catch (OperationCanceledException)
				{
					// Prevent throwing if stoppingToken was signaled
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, ex.Message);
					_logger.LogInformation("Exiting SMTP background service cycle");
					//on exception wait 1 min
					await Task.Delay(60 * 1000, stoppingToken); //1min wait
				}
			}
		}
	}

	public override async Task StopAsync(
		CancellationToken stoppingToken)
	{
		_logger.LogInformation($"{this.GetType().Name} is stopping.");
		await base.StopAsync(stoppingToken);
	}

}
