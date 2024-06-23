namespace WebVella.Tefter.Web.Store.Base;

public partial class StateEffect
{
	private readonly ITfService TefterService;
	public StateEffect(ITfService tfService)
	{
		this.TefterService = tfService;
	}
}
