namespace WebVella.Tefter.Web.Store.Base;

public partial class StateEffect
{
	[EffectMethod]
	public Task DataProviderDetailsActionEffect(EmptyDataProviderDetailsAction action, IDispatcher dispatcher)
	{
		dispatcher.Dispatch(new DataProviderDetailsChangedAction(null));
		return Task.CompletedTask;
	}
}