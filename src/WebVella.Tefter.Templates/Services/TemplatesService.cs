namespace WebVella.Tefter.Templates.Services;

public partial interface ITemplatesService
{
}

internal partial class TemplatesService : ITemplatesService
{
	public readonly ITfDataProviderManager _dataProviderManager;
	public readonly ITfDataManager _dataManager;
	public readonly ITfDatabaseService _dbService;
	public readonly IIdentityManager _identityManager;
	public readonly ITfBlobManager _blobManager;

	public TemplatesService(
		ITfDatabaseService dbService,
		IIdentityManager identityManager,
		ITfDataManager dataManager,
		ITfBlobManager blobManager,
		ITfDataProviderManager dataProviderManager)
	{
		_dbService = dbService;
		_identityManager = identityManager;
		_dataManager = dataManager;
		_dataProviderManager = dataProviderManager;
		_blobManager = blobManager;
	}
}