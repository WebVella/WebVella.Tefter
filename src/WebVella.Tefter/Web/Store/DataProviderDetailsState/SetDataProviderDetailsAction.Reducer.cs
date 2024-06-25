namespace WebVella.Tefter.Web.Store.DataProviderDetailsState;

public static partial class DataProviderDetailsStateReducers
{
	[ReducerMethod()]
	public static DataProviderDetailsState SetDataProviderDetailsActionReducer(DataProviderDetailsState state, 
		SetDataProviderDetailsAction action) 
		=> state with { IsBusy = false, User = action.User};
}
