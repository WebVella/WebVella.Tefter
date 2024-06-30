namespace WebVella.Tefter.Web.Store.Base;

public partial class StateEffect
{
	[EffectMethod]
	public Task InitCultureActionEffect(InitCultureStateAction action, IDispatcher dispatcher)
	{
		dispatcher.Dispatch(new SetCultureActionResult());
		return Task.CompletedTask;
	}
}