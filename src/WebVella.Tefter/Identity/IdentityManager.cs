namespace WebVella.Tefter.Identity;

public partial interface IIdentityManager
{
}

public partial class IdentityManager : IIdentityManager
{
	private readonly IDatabaseService _dbService;
	private readonly IDboManager _dboManager;
	private readonly UserValidator _userValidator;
	private readonly RoleValidator _roleValidator;

	public IdentityManager(IServiceProvider serviceProvider)
	{
		_dbService = serviceProvider.GetService<IDatabaseService>();
		_dboManager = serviceProvider.GetService<IDboManager>();

		_userValidator = new UserValidator(_dboManager,this);
		_roleValidator = new RoleValidator(_dboManager,this);
	}
}
