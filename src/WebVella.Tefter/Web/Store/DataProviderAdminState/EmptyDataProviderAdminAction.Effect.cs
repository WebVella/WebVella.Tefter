namespace WebVella.Tefter.Web.Store.Base;

public partial class StateEffect
{
	[EffectMethod]
	public Task DataProviderAdminActionEffect(EmptyDataProviderAdminAction action, IDispatcher dispatcher)
	{
		dispatcher.Dispatch(new DataProviderAdminChangedAction(null));
		return Task.CompletedTask;
	}
}