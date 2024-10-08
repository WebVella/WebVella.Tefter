namespace WebVella.Tefter.Talk.Services;

public partial interface ITalkService
{
}

internal partial class TalkService : ITalkService
{
	public readonly IDatabaseService _dbService;
	public readonly IdentityManager _identityManager;

	public TalkService(
		IDatabaseService dbService,
		IdentityManager identityManager)
	{
		_dbService = dbService;
		_identityManager = identityManager;
	}
}
