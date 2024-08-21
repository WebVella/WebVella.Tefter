namespace WebVella.Tefter.Web.Store.FastAccessState;

public static partial class FastAccessStateReducers
{
	[ReducerMethod()]
	public static FastAccessState SetFastAccessStateActionReducer(FastAccessState state, 
		SetFastAccessStateAction action) 
		=> state with { IsBusy = action.IsBusy }; //, Provider = action.Provider};
}
