namespace WebVella.Tefter.Talk.Services;

public partial interface ITalkService
{
}

internal partial class TalkService : ITalkService
{
	public readonly IDatabaseService _dbService;

	public TalkService(IDatabaseService dbService)
	{
		_dbService = dbService;
	}
}
