namespace WebVella.Tefter.Web.Store.DataProviderDetailsState;

public static partial class DataProviderDetailsStateReducers
{
	[ReducerMethod()]
	public static DataProviderDetailsState GetDataProviderDetailsActionReducer(DataProviderDetailsState state, GetDataProviderDetailsAction action) 
		=> state with { IsBusy = true};
}
