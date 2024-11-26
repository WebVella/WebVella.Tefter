namespace WebVella.Tefter;

public partial interface ITfDataProviderManager { }

public partial class TfDataProviderManager : ITfDataProviderManager
{
	private readonly IServiceProvider _serviceProvider;
	private readonly ITfDataManager _dataManager;
	private readonly ITfDboManager _dboManager;
	private readonly ITfDatabaseService _dbService;
	private readonly ITfDatabaseManager _dbManager;
	private readonly ITfSharedColumnsManager _sharedColumnManager;

	public TfDataProviderManager(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
		_dataManager = serviceProvider.GetService<ITfDataManager>();
		_dboManager = serviceProvider.GetService<ITfDboManager>();
		_dbService = serviceProvider.GetService<ITfDatabaseService>();
		_dbManager = serviceProvider.GetService<ITfDatabaseManager>();
		_sharedColumnManager = serviceProvider.GetService<ITfSharedColumnsManager>();
	}
}
