namespace WebVella.Tefter.Web.Store;

public partial class TfStateEffect
{
	[EffectMethod]
	public Task SetFastAccessStateActionEffect(SetFastAccessStateAction action, IDispatcher dispatcher)
	{
		dispatcher.Dispatch(new FastAccessStateChangedAction(action.IsBusy));//, action.Provider));
		return Task.CompletedTask;
	}
}