namespace WebVella.Tefter.Web.Store;

public record SetAuxDataStateAction : TfBaseAction
{
	public TfAuxDataState State { get; }
	public SetAuxDataStateAction(
		FluxorComponent component,
		TfAuxDataState state
		)
	{
		StateComponent = component;
		State = state;
	}
}

public static partial class AuxDataStateReducers
{
	[ReducerMethod()]
	public static TfAuxDataState SetAuxDataStateActionReducer(TfAuxDataState state, SetAuxDataStateAction action)
		=> action.State with { Hash = Guid.NewGuid() };
}