namespace WebVella.Tefter.Talk.Services;

public partial interface ITalkService
{
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
}
