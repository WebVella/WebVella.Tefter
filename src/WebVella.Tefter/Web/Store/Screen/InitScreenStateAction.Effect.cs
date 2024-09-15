namespace WebVella.Tefter.Web.Store;

public partial class TfStateEffect
{
	[EffectMethod]
	public Task InitScreenStateActionEffect(InitScreenStateAction action, IDispatcher dispatcher)
	{
		dispatcher.Dispatch(new SetSidebarActionResult());
		return Task.CompletedTask;
	}
}