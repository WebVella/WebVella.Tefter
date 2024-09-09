namespace WebVella.Tefter.Web.Store.SpaceState;

public partial class SpaceStateEffects
{
	[EffectMethod]
	public Task HandleSetSpaceViewAction(SetSpaceViewAction action, IDispatcher dispatcher)
	{
		dispatcher.Dispatch(new SpaceStateChangedAction());
		dispatcher.Dispatch(new SpaceViewMetaChangedAction());
		return Task.CompletedTask;
	}

}

