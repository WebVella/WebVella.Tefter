namespace WebVella.Tefter.Services;

public partial interface ITfService
{
}

public partial class TfService : ITfService
{
	private static AsyncLock _lock = new();
	private readonly ITfConfigurationService _config;
	private readonly IServiceProvider _serviceProvider;
	private readonly ITfDboManager _dboManager;
	private readonly ITfDatabaseManager _dbManager;
	private readonly ITfMetaService _metaService;
	private readonly ITfDatabaseService _dbService;
	private readonly ITfEventBusEx _eventBus;
	private readonly ILogger<TfService> _logger;
	private readonly IStringLocalizer<TfService> LOC;

	public TfService(
		IServiceProvider serviceProvider,
		ITfConfigurationService config,
		ITfMetaService metaService,
		ITfDatabaseService dbService,
		ITfEventBusEx eventBus,
		ITfDatabaseManager dbManager,
		ILogger<TfService> logger)
	{
		_serviceProvider = serviceProvider;
		_dbService = dbService;
		_config = config;
		_metaService = metaService;
		_dbManager = dbManager;
		_logger = logger;
		_dboManager = serviceProvider.GetService<ITfDboManager>();
		_eventBus = eventBus;
		
		var tefterInstanceId = GetSetting(TfConstants.TEFTER_INSTANCE_SETTING_KEY).Value;
		var blobStoragePath = Path.Combine(_config.BlobStoragePath, tefterInstanceId);

		LOC = serviceProvider.GetService<IStringLocalizer<TfService>>() ?? null!;
		InitBlobStorageFolder(blobStoragePath);
	}
}