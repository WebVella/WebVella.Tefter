namespace WebVella.Tefter.Web.Store;

public static partial class DataProviderAdminStateReducers
{
	[ReducerMethod()]
	public static TfState SetDataProviderAdminActionReducer(TfState state, 
		SetDataProviderAdminAction action) 
		=> state with { Provider = action.Provider};
}
