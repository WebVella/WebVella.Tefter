using WebVella.Tefter.Api;

namespace WebVella.Tefter;

public partial interface ITfDataProviderManager { }

public partial class TfDataProviderManager : ITfDataProviderManager
{
	private readonly IServiceProvider _serviceProvider;
	private readonly IDataManager _dataManager;
	private readonly ITfDboManager _dboManager;
	private readonly ITfDatabaseService _dbService;
	private readonly IDatabaseManager _dbManager;
	private readonly ITfSharedColumnsManager _sharedColumnManager;

	public TfDataProviderManager(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
		_dataManager = serviceProvider.GetService<IDataManager>();
		_dboManager = serviceProvider.GetService<ITfDboManager>();
		_dbService = serviceProvider.GetService<ITfDatabaseService>();
		_dbManager = serviceProvider.GetService<IDatabaseManager>();
		_sharedColumnManager = serviceProvider.GetService<ITfSharedColumnsManager>();
	}
}
