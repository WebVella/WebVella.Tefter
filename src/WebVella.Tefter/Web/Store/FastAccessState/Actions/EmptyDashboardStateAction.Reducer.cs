namespace WebVella.Tefter.Web.Store.FastAccessState;

public static partial class FastAccessStateReducers
{
	[ReducerMethod()]
	public static FastAccessState EmptyFastAccessStateActionReducer(FastAccessState state, EmptyFastAccessStateAction action) 
		=> state with {IsBusy = action.IsBusy};//, Provider = action.Provider};
}
