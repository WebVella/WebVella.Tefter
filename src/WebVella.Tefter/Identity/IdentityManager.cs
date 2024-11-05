namespace WebVella.Tefter.Identity;

public partial interface IIdentityManager
{
}

public partial class IdentityManager : IIdentityManager
{
	private readonly IServiceProvider _serviceProvider;
	private readonly ITfDatabaseService _dbService;
	private readonly ITfDboManager _dboManager;
	private readonly UserValidator _userValidator;
	private readonly RoleValidator _roleValidator;

	public IdentityManager(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
		_dbService = serviceProvider.GetService<ITfDatabaseService>();
		_dboManager = serviceProvider.GetService<ITfDboManager>();

		_userValidator = new UserValidator(_dboManager, this);
		_roleValidator = new RoleValidator(_dboManager, this);
	}
}
