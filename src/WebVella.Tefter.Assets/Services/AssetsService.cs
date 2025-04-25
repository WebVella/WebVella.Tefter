namespace WebVella.Tefter.Assets.Services;

public partial interface IAssetsService
{
	Task<List<string>> GetAllJoinKeysAsync();
}

internal partial class AssetsService : IAssetsService
{
	public readonly ITfDatabaseService _dbService;
	public readonly ITfService _tfService;

	public AssetsService(
		ITfDatabaseService dbService,
		ITfService tfService)
	{
		_dbService = dbService;
		_tfService = tfService;
	}

	public Task<List<string>> GetAllJoinKeysAsync()
	{
		try
		{
			return Task.FromResult(_tfService.GetAllJoinKeyNames());
		}
		catch (Exception)
		{
			return Task.FromResult(new List<string>());
		}
	}
}
