//namespace WebVella.Tefter.Assets.Services;

//internal class AssetBackgroundJob : BackgroundService
//{
//	private readonly ILogger<AssetBackgroundJob> _logger;
//	private readonly IAssetsService _assetsService;

//    public AssetBackgroundJob(
//        IAssetsService assetsService,
//        ILogger<AssetBackgroundJob> logger)
//	{
//		_assetsService = assetsService;
//        _logger = logger;
//	}

//	protected override async Task ExecuteAsync(
//		CancellationToken stoppingToken)
//	{
//		_logger.LogInformation($"{this.GetType().Name} is running.");

//		//initial 10 sec wait
//		await Task.Delay(10 * 1000);

//		while (!stoppingToken.IsCancellationRequested)
//		{
//            var now = DateTime.Now;

//            //in 2:00 every day recalculate all folders shared column count
//            if (now.Minute == 0 || now.Hour == 2 )
//            {
//                try
//                {
//                    //clear queue because we will recalculate all folders anyway
//                    //we do it before recalculation because during recalculation new folders can be added to the queue
//                    _assetsService.SharedColumnAssetsProcessQueue.Clear();

//                    _assetsService.UpdateFoldersSharedColumnCount();
//                }
//                catch (Exception ex)
//                {
//                    _logger.LogError(ex, $"Error while processing all folder for shared column count recalculation.");
//                }

//                await Task.Delay(60 * 1000, stoppingToken); //delay 1 min to avoid multiple runs in the same hour
//            }

//            await Task.Delay(1000);

//            //otherwise if there are assets in the queue , we check and process one every second
//            if (_assetsService.SharedColumnAssetsProcessQueue.Count > 0)
//			{
//				Asset asset = _assetsService.SharedColumnAssetsProcessQueue.Dequeue();
//				try
//				{
//					_assetsService.ModifyAssetSharedColumnCount(asset);
//				}
//				catch (Exception ex)
//				{
//					_logger.LogError(ex, $"Error while processing asset {asset.Id} for shared column count recalculation.");
//				}
//            }
           
//		}
//	}

//	public override async Task StopAsync(
//		CancellationToken stoppingToken)
//	{
//		_logger.LogInformation($"{this.GetType().Name} is stopping.");
//		await base.StopAsync(stoppingToken);
//	}

//}
