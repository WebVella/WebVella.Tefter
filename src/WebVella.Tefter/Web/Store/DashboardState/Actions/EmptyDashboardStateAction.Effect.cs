namespace WebVella.Tefter.Web.Store.Base;

public partial class StateEffect
{
	[EffectMethod]
	public Task EmptyDashboardStateActionEffect(EmptyDashboardStateAction action, IDispatcher dispatcher)
	{
		dispatcher.Dispatch(new DashboardStateChangedAction(action.IsBusy));//,action.Provider));
		return Task.CompletedTask;
	}
}