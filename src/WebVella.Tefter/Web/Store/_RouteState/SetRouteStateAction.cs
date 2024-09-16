namespace WebVella.Tefter.Web.Store;

public record SetRouteStateAction : TfBaseAction
{
	public TfRouteState State { get; }
	internal SetRouteStateAction(
		FluxorComponent component,
		TfRouteState state
		)
	{
		StateComponent = component;
		State = state;
	}
}

public static partial class UserStateReducers
{
	[ReducerMethod()]
	public static TfRouteState SetRouteStateActionReducer(TfRouteState state, SetRouteStateAction action)
		=> action.State with { FirstNode = action.State.FirstNode };
}