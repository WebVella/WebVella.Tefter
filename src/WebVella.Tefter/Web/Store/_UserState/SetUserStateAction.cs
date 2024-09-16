namespace WebVella.Tefter.Web.Store;

public record SetUserStateAction : TfBaseAction
{
	public TfUserState State { get; }
	internal SetUserStateAction(
		FluxorComponent component,
		TfUserState state
		)
	{
		StateComponent = component;
		State = state;
	}
}

public static partial class UserStateReducers
{
	[ReducerMethod()]
	public static TfUserState SetUserStateReducer(TfUserState state, SetUserStateAction action)
		=> action.State with { Culture = action.State.Culture };
}