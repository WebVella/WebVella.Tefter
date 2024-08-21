namespace WebVella.Tefter.Web.Store.Base;

public partial class StateEffect
{
	[EffectMethod]
	public Task SetFastAccessStateActionEffect(SetFastAccessStateAction action, IDispatcher dispatcher)
	{
		dispatcher.Dispatch(new FastAccessStateChangedAction(action.IsBusy));//, action.Provider));
		return Task.CompletedTask;
	}
}