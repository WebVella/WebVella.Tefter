namespace WebVella.Tefter.Web.Store.Base;

public partial class StateEffect
{
	[EffectMethod]
	public Task EmptySpaceStateActionEffect(EmptySpaceStateAction action, IDispatcher dispatcher)
	{
		dispatcher.Dispatch(new SpaceStateChangedAction(action.IsBusy));//,action.Provider));
		return Task.CompletedTask;
	}
}