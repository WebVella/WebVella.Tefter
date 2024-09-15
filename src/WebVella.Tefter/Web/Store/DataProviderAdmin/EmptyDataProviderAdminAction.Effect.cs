namespace WebVella.Tefter.Web.Store;

public partial class TfStateEffect
{
	[EffectMethod]
	public Task DataProviderAdminActionEffect(EmptyDataProviderAdminAction action, IDispatcher dispatcher)
	{
		dispatcher.Dispatch(new DataProviderAdminChangedAction(action.IsBusy,action.Provider));
		return Task.CompletedTask;
	}
}