namespace WebVella.Tefter.Assets.Services;

public partial interface IAssetsService
{
}

internal partial class AssetsService : IAssetsService
{
	public readonly ITfDataProviderManager _dataProviderManager;
	public readonly IDataManager _dataManager;
	public readonly IDatabaseService _dbService;
	public readonly IIdentityManager _identityManager;

	public AssetsService(
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
