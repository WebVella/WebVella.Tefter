namespace WebVella.Tefter.Web.Store;

public record SetAppStateAction : TfBaseAction
{
	public TfAppState State { get; }
	internal SetAppStateAction(
		FluxorComponent component,
		TfAppState state
		)
	{
		StateComponent = component;
		State = state;
	}
}

public static partial class AppStateReducers
{
	[ReducerMethod()]
	public static TfAppState SetAppStateActionReducer(TfAppState state, SetAppStateAction action)
	{
		return action.State with { Space = action.State.Space };

	}
}