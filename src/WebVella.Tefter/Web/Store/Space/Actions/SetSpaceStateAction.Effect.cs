namespace WebVella.Tefter.Web.Store;

public partial class SpaceStateEffects
{
	[EffectMethod]
	public Task HandleSetSpaceAction(SetSpaceStateAction action, IDispatcher dispatcher)
	{
		dispatcher.Dispatch(new SpaceStateChangedAction(component: action.Component));
		return Task.CompletedTask;
	}

}

