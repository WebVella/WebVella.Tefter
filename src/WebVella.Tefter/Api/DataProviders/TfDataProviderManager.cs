namespace WebVella.Tefter;

public partial interface ITfDataProviderManager { }

public partial class TfDataProviderManager : ITfDataProviderManager
{
	private readonly IDboManager _dboManager;
	private readonly IDatabaseService _dbService;
	private readonly IDatabaseManager _dbManager;
	private readonly ITfDataProviderSynchronizeTaskList _syncTaskList;

	public TfDataProviderManager(IServiceProvider serviceProvider)
	{
		_dboManager = serviceProvider.GetService<IDboManager>();
		_dbService = serviceProvider.GetService<IDatabaseService>();
		_dbManager = serviceProvider.GetService<IDatabaseManager>();
		_syncTaskList = serviceProvider.GetService<ITfDataProviderSynchronizeTaskList>();
	}
}
