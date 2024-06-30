namespace WebVella.Tefter.Web.Store.DataProviderAdminState;

public static partial class DataProviderAdminStateReducers
{
	[ReducerMethod()]
	public static DataProviderAdminState GetDataProviderAdminActionResultReducer(DataProviderAdminState state, 
		DataProviderAdminChangedAction action)
		=> state with { IsBusy = false, Provider = action.Provider };
}
