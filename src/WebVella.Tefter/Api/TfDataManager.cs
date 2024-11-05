namespace WebVella.Tefter;

public partial interface ITfDataManager
{
}

public partial class TfDataManager : ITfDataManager
{
	private readonly Lazy<ITfDatabaseService> _dbServiceLazy;
	private readonly Lazy<ITfSpaceManager> _spaceManagerLazy;
	private readonly Lazy<ITfDataProviderManager> _providerManagerLazy;

	private ITfDatabaseService _dbService { get { return _dbServiceLazy.Value; } }
	private ITfSpaceManager _spaceManager { get { return _spaceManagerLazy.Value; } }
	private ITfDataProviderManager _providerManager { get { return _providerManagerLazy.Value; } }

	public TfDataManager(
		Lazy<ITfDatabaseService> dbService,
		Lazy<ITfSpaceManager> spaceManager,
		Lazy<ITfDataProviderManager> providerManager)
	{
		_dbServiceLazy = dbService;
		_spaceManagerLazy = spaceManager;
		_providerManagerLazy = providerManager;
	}
}