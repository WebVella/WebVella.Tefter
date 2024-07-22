namespace WebVella.Tefter;

public partial interface ITfDataProviderManager { }

public partial class TfDataProviderManager : ITfDataProviderManager
{
	private readonly IDataManager _dataManager;
	private readonly IDboManager _dboManager;
	private readonly IDatabaseService _dbService;
	private readonly IDatabaseManager _dbManager;

	public TfDataProviderManager(IServiceProvider serviceProvider)
	{
		_dataManager = serviceProvider.GetService<IDataManager>();
		_dboManager = serviceProvider.GetService<IDboManager>();
		_dbService = serviceProvider.GetService<IDatabaseService>();
		_dbManager = serviceProvider.GetService<IDatabaseManager>();
	}
}
