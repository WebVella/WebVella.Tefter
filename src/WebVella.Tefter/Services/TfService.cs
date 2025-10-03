using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.Extensions.DependencyInjection;

namespace WebVella.Tefter.Services;

public partial interface ITfService
{
}

public partial class TfService : ITfService
{
	private static AsyncLock _lock = new AsyncLock();
	private static readonly Lock _lockObject = new();
	private readonly ITfConfigurationService _config;
	private readonly IServiceProvider _serviceProvider;
	private readonly ITfDboManager _dboManager;
	private readonly ITfDatabaseManager _dbManager;
	private readonly ITfMetaService _metaService;
	private readonly ITfDatabaseService _dbService;
	private readonly ILogger<TfService> _logger;
	private readonly TfGlobalEventProvider _eventProvider;
	private readonly IMemoryCache _cache;
	private readonly IStringLocalizer<TfService> LOC;
	
	public TfService(
		IServiceProvider serviceProvider,
		ITfConfigurationService config,
		ITfMetaService metaService,
		ITfDatabaseService dbService,
		ITfDatabaseManager dbManager,
		ILogger<TfService> logger,
		TfGlobalEventProvider globalEventProvider)
	{
		_serviceProvider = serviceProvider;
		_dbService = dbService;
		_config = config;
		_metaService = metaService;
		_dbManager = dbManager;
		_logger = logger;
		_dboManager = serviceProvider.GetService<ITfDboManager>();

		var cacheOptions = new MemoryCacheOptions();
		cacheOptions.ExpirationScanFrequency = TimeSpan.FromHours(1);
		_cache = new MemoryCache(cacheOptions);

		var tefterInstanceId = GetSetting(TfConstants.TEFTER_INSTANCE_SETTING_KEY).Value;
		var blobStoragePath = Path.Combine( _config.BlobStoragePath, tefterInstanceId);

		_eventProvider = globalEventProvider;
		LOC = serviceProvider.GetService<IStringLocalizer<TfService>>() ?? null!;		
		InitBlobStorageFolder(blobStoragePath);
	}
}
