namespace WebVella.Tefter.Web.Store;

public static partial class FastAccessStateReducers
{
	[ReducerMethod()]
	public static TfState EmptyFastAccessStateActionReducer(TfState state, EmptyFastAccessStateAction action) 
		=> state with {};//, Provider = action.Provider};
}
