using WebVella.Tefter.Identity;

namespace WebVella.Tefter.Web.Store.UserState;

public partial class UserStateEffects
{
	private readonly IIdentityManager IdentityManager;
	private readonly ITfService TfService;

	public UserStateEffects(IIdentityManager identityManager,
		ITfService tfService)
	{
		this.IdentityManager = identityManager;
		this.TfService = tfService;
	}
}

