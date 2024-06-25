namespace WebVella.Tefter.Web.Store.Base;

public partial class StateEffect
{
	private readonly ITfService TefterService;
	private readonly IIdentityManager IdentityManager;
	public StateEffect(ITfService tfService, IIdentityManager identityManager)
	{
		this.TefterService = tfService;
		IdentityManager = identityManager;
	}
}
