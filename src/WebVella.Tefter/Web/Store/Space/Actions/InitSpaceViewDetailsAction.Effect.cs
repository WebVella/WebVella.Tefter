namespace WebVella.Tefter.Web.Store;

public partial class SpaceStateEffects
{
	[EffectMethod]
	public Task HandleSetSpaceDataViewAction(InitSpaceViewDetailsAction action, IDispatcher dispatcher)
	{
		dispatcher.Dispatch(new SpaceStateChangedAction(component:action.Component));
		return Task.CompletedTask;
	}

}