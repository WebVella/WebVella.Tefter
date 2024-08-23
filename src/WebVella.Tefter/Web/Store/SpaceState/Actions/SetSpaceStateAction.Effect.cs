namespace WebVella.Tefter.Web.Store.SpaceState;

public partial class SpaceStateEffects
{
	[EffectMethod]
	public Task HandleSetSpaceAction(SetSpaceStateAction action, IDispatcher dispatcher)
	{
		dispatcher.Dispatch(new SpaceStateChangedAction());
		return Task.CompletedTask;
	}

}

