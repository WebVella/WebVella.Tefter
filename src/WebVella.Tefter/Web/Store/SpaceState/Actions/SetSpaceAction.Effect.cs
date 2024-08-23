namespace WebVella.Tefter.Web.Store.SpaceState;

public partial class SpaceStateEffects
{
	[EffectMethod]
	public Task HandleSetSpaceAction(SetSpaceAction action, IDispatcher dispatcher)
	{
		dispatcher.Dispatch(new SpaceChangedAction());
		return Task.CompletedTask;
	}

}

