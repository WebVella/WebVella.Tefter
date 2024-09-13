namespace WebVella.Tefter.Web.Store.SpaceState;

public partial class SpaceStateEffects
{
	[EffectMethod]
	public Task ToggleSpaceViewItemSelectionActionEffect(ToggleSpaceViewItemSelectionAction action, IDispatcher dispatcher)
	{
		dispatcher.Dispatch(new SpaceViewSelectionChangedAction());
		return Task.CompletedTask;
	}

}

