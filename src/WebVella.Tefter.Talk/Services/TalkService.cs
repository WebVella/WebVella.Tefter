namespace WebVella.Tefter.Talk.Services;

public partial interface ITalkService
{
	Task<List<string>> GetAllJoinKeysAsync();
}

internal partial class TalkService : ITalkService
{
	public readonly ITfDatabaseService _dbService;
	public readonly ITfService _tfService;

	public TalkService(
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
