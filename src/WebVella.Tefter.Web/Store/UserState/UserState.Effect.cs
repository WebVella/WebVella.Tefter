using WebVella.Tefter.Identity;

namespace WebVella.Tefter.Web.Store.UserState;

public partial class UserStateEffects
{
	private readonly IIdentityManager IdentityManager;

	public UserStateEffects(IIdentityManager identityManager)
	{
		this.IdentityManager = identityManager;
	}
}

