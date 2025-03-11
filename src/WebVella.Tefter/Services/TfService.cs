using Microsoft.Extensions.DependencyInjection;

namespace WebVella.Tefter.Services;

public partial interface ITfService
{
}

public partial class TfService : ITfService
{
	private static AsyncLock _lock = new AsyncLock();
	
	private readonly ITfConfigurationService _config;
	private readonly IServiceProvider _serviceProvider;
	private readonly ITfDboManager _dboManager;
	private readonly ITfDatabaseManager _dbManager;
	private readonly ITfMetaService _metaService;
	private readonly ITfDatabaseService _dbService;

	public TfService(
		IServiceProvider serviceProvider,
		ITfConfigurationService config,
		ITfMetaService metaService,
		ITfDatabaseService dbService)
	{
		_serviceProvider = serviceProvider;
		_dbService = dbService;
		_config = config;
		_metaService = metaService;
		_dboManager = serviceProvider.GetService<ITfDboManager>();
		
		InitBlobStorageFolder(_config.BlobStoragePath);
	}
}
