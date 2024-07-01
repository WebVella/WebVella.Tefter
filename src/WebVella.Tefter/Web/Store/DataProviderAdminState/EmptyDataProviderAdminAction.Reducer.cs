namespace WebVella.Tefter.Web.Store.DataProviderAdminState;

public static partial class DataProviderAdminStateReducers
{
	[ReducerMethod()]
	public static DataProviderAdminState EmptyDataProviderAdminActionReducer(DataProviderAdminState state, EmptyDataProviderAdminAction action) 
		=> state with {IsBusy = action.IsBusy, Provider = action.Provider};
}
