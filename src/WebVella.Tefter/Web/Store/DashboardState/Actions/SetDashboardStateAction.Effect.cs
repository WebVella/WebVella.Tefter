namespace WebVella.Tefter.Web.Store.Base;

public partial class StateEffect
{
	[EffectMethod]
	public Task SetDashboardStateActionEffect(SetDashboardStateAction action, IDispatcher dispatcher)
	{
		dispatcher.Dispatch(new DashboardStateChangedAction(action.IsBusy));//, action.Provider));
		return Task.CompletedTask;
	}
}