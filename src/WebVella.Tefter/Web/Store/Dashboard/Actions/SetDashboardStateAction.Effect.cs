namespace WebVella.Tefter.Web.Store;

public partial class TfStateEffect
{
	[EffectMethod]
	public Task SetDashboardStateActionEffect(SetDashboardStateAction action, IDispatcher dispatcher)
	{
		dispatcher.Dispatch(new DashboardStateChangedAction(action.IsBusy));//, action.Provider));
		return Task.CompletedTask;
	}
}