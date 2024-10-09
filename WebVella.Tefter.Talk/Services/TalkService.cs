namespace WebVella.Tefter.Talk.Services;

public partial interface ITalkService
{
}

internal partial class TalkService : ITalkService
{
	public readonly ITfDataProviderManager _dataProviderManager;
	public readonly IDataManager _dataManager;
	public readonly IDatabaseService _dbService;
	public readonly IIdentityManager _identityManager;

	public TalkService(
		IDatabaseService dbService,
		IIdentityManager identityManager,
		IDataManager dataManager,
		ITfDataProviderManager dataProviderManager)
	{
		_dbService = dbService;
		_identityManager = identityManager;
		_dataManager = dataManager;
		_dataProviderManager = dataProviderManager;	
	}
}
