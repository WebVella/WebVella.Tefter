namespace WebVella.Tefter.Web.Store.DataProviderAdminState;

public static partial class DataProviderAdminStateReducers
{
	[ReducerMethod()]
	public static DataProviderAdminState GetDataProviderAdminActionReducer(DataProviderAdminState state, GetDataProviderAdminAction action) 
		=> state with { IsBusy = true};
}
