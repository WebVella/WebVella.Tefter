namespace WebVella.Tefter.Web.Store.Base;

public partial class StateEffect
{
	[EffectMethod]
	public Task SetDataProviderDetailsActionEffect(SetDataProviderDetailsAction action, IDispatcher dispatcher)
	{
		dispatcher.Dispatch(new DataProviderDetailsChangedAction(action.Provider));
		return Task.CompletedTask;
	}
}