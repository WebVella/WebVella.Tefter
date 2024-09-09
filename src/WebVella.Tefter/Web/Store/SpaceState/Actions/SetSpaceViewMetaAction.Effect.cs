namespace WebVella.Tefter.Web.Store.SpaceState;

public partial class SpaceStateEffects
{
	[EffectMethod]
	public Task HandleSetSpaceMetaViewAction(SetSpaceViewMetaAction action, IDispatcher dispatcher)
	{
		dispatcher.Dispatch(new SpaceViewMetaChangedAction());
		return Task.CompletedTask;
	}

}

