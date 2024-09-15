namespace WebVella.Tefter.Web.Store;

public static partial class FastAccessStateReducers
{
	[ReducerMethod()]
	public static TfState SetFastAccessStateActionReducer(TfState state, 
		SetFastAccessStateAction action) 
		=> state with {  }; //, Provider = action.Provider};
}
