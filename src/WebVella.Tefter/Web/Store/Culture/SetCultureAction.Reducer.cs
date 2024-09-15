namespace WebVella.Tefter.Web.Store;

public static partial class StateReducers
{
	[ReducerMethod()]
	public static TfState SetCultureActionReducer(TfState state, SetCultureAction action) 
		=> state with {Culture = action.Culture};
}
