namespace WebVella.Tefter.Identity;

public partial interface IIdentityManager
{
}

public partial class IdentityManager : IIdentityManager
{
	private readonly IDatabaseService _dbService;
	private readonly IDboManager _dboManager;

	public IdentityManager(IServiceProvider serviceProvider)
	{
		_dbService = serviceProvider.GetService<IDatabaseService>();
		_dboManager = serviceProvider.GetService<IDboManager>();
	}
}
