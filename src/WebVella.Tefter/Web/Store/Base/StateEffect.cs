namespace WebVella.Tefter.Web.Store.Base;

public partial class StateEffect
{
	private readonly ITfService TefterService;
	private readonly IIdentityManager IdentityManager;
	private readonly ITfDataProviderManager DataPrividerManager;
	public StateEffect(ITfService tfService, 
		IIdentityManager identityManager,
		ITfDataProviderManager dataPrividerManager)
	{
		this.TefterService = tfService;
		IdentityManager = identityManager;
		DataPrividerManager = dataPrividerManager;
	}
}
