namespace WebVella.Tefter.Web.Store.Base;

public partial class StateEffect
{
	[EffectMethod]
	public Task EmptyFastAccessStateActionEffect(EmptyFastAccessStateAction action, IDispatcher dispatcher)
	{
		dispatcher.Dispatch(new FastAccessStateChangedAction(action.IsBusy));//,action.Provider));
		return Task.CompletedTask;
	}
}