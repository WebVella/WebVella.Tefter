namespace WebVella.Tefter.Web.Store;

public static partial class DataProviderAdminStateReducers
{
	[ReducerMethod()]
	public static TfState EmptyDataProviderAdminActionReducer(TfState state, EmptyDataProviderAdminAction action) 
		=> state with {Provider = action.Provider};
}
