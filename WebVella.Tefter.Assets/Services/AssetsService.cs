namespace WebVella.Tefter.Assets.Services;

public partial interface IAssetsService
{
}

internal partial class AssetsService : IAssetsService
{
	public readonly ITfDataProviderManager _dataProviderManager;
	public readonly ITfDataManager _dataManager;
	public readonly ITfDatabaseService _dbService;
	public readonly IIdentityManager _identityManager;
	public readonly ITfFileManager _fileManager;

	public AssetsService(
		ITfDatabaseService dbService,
		IIdentityManager identityManager,
		ITfDataManager dataManager,
		ITfFileManager fileManager,
		ITfDataProviderManager dataProviderManager)
	{
		_dbService = dbService;
		_identityManager = identityManager;
		_dataManager = dataManager;
		_dataProviderManager = dataProviderManager;	
		_fileManager = fileManager;
	}
}
