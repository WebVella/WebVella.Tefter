namespace WebVella.Tefter.Web.Store.UserState;

public partial class UserStateEffects
{
	private readonly ITfService tfService;

	public UserStateEffects(ITfService tfService)
	{
		this.tfService = tfService;
	}
}

