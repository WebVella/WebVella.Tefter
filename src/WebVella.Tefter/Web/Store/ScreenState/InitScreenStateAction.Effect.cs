namespace WebVella.Tefter.Web.Store.Base;

public partial class StateEffect
{
	[EffectMethod]
	public Task InitScreenStateActionEffect(InitScreenStateAction action, IDispatcher dispatcher)
	{
		dispatcher.Dispatch(new SetSidebarActionResult());
		return Task.CompletedTask;
	}
}