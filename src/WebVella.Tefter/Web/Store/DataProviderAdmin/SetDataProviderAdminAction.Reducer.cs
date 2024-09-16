namespace WebVella.Tefter.Web.Store;

public static partial class DataProviderAdminStateReducers
{
	[ReducerMethod()]
	public static TfAppState SetDataProviderAdminActionReducer(TfAppState state, 
		SetDataProviderAdminAction action) 
		=> state with { Provider = action.Provider};
}
