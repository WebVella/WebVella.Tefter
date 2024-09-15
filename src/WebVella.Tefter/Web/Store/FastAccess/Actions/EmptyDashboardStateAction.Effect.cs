namespace WebVella.Tefter.Web.Store;

public partial class TfStateEffect
{
	[EffectMethod]
	public Task EmptyFastAccessStateActionEffect(EmptyFastAccessStateAction action, IDispatcher dispatcher)
	{
		dispatcher.Dispatch(new FastAccessStateChangedAction(action.IsBusy));//,action.Provider));
		return Task.CompletedTask;
	}
}