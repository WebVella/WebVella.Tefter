namespace WebVella.Tefter;

public partial interface IDataManager
{
}

public partial class DataManager : IDataManager
{
	private readonly Lazy<IDatabaseService> _dbServiceLazy;
	private readonly Lazy<ITfSpaceManager> _spaceManagerLazy;
	private readonly Lazy<ITfDataProviderManager> _providerManagerLazy;

	private IDatabaseService _dbService { get { return _dbServiceLazy.Value; } }
	private ITfSpaceManager _spaceManager { get { return _spaceManagerLazy.Value; } }
	private ITfDataProviderManager _providerManager { get { return _providerManagerLazy.Value; } }

	public DataManager(
		Lazy<IDatabaseService> dbService,
		Lazy<ITfSpaceManager> spaceManager,
		Lazy<ITfDataProviderManager> providerManager)
	{
		_dbServiceLazy = dbService;
		_spaceManagerLazy = spaceManager;
		_providerManagerLazy = providerManager;
	}
}