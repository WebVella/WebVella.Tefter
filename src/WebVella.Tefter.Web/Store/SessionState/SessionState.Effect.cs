namespace WebVella.Tefter.Web.Store.SessionState;

public partial class SessionStateEffects
{
	private readonly ITfService tfService;

	public SessionStateEffects(ITfService tfService)
	{
		this.tfService = tfService;
	}
}

