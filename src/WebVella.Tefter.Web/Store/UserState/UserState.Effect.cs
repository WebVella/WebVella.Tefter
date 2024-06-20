using WebVella.Tefter.Identity;

namespace WebVella.Tefter.Web.Store.UserState;

public partial class UserStateEffects
{
	private readonly ITfService tfService;

	public UserStateEffects(ITfService tfService,
	IIdentityManager identityManager)
	{
		this.tfService = tfService;
	}
}

