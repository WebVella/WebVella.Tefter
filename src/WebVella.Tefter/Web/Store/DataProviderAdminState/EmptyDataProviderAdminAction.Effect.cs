namespace WebVella.Tefter.Web.Store.Base;

public partial class StateEffect
{
	[EffectMethod]
	public Task DataProviderAdminActionEffect(EmptyDataProviderAdminAction action, IDispatcher dispatcher)
	{
		dispatcher.Dispatch(new DataProviderAdminChangedAction(action.IsBusy,action.Provider));
		return Task.CompletedTask;
	}
}