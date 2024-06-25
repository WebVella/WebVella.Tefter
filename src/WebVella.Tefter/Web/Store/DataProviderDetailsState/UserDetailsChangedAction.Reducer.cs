namespace WebVella.Tefter.Web.Store.DataProviderDetailsState;

public static partial class DataProviderDetailsStateReducers
{
	[ReducerMethod()]
	public static DataProviderDetailsState GetDataProviderDetailsActionResultReducer(DataProviderDetailsState state, 
		DataProviderDetailsChangedAction action)
		=> state with { IsBusy = false, User = action.User };
}
