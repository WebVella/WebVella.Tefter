namespace WebVella.Tefter.Web.Store.DataProviderAdminState;

public static partial class DataProviderAdminStateReducers
{
	[ReducerMethod()]
	public static DataProviderAdminState SetDataProviderAdminActionReducer(DataProviderAdminState state, 
		SetDataProviderAdminAction action) 
		=> state with { IsBusy = false, Provider = action.Provider};
}
