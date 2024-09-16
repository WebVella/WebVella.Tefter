namespace WebVella.Tefter.Web.Store;

public record InitStateAction : TfBaseAction
{
	public TfState State { get; }
	internal InitStateAction(
		TfBaseComponent component,
		TfState state)
	{
		Component = component;
		State = state;
	}
}

public static partial class StateReducers
{
	[ReducerMethod()]
	public static TfState InitStateActionReducer(TfState state, InitStateAction action){ 
		state = action.State with { ThemeColor = action.State.ThemeColor}; //if no prop provided will not copy object
		return state;
	}
}