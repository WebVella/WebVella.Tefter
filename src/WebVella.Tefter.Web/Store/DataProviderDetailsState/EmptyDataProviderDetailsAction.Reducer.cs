namespace WebVella.Tefter.Web.Store.DataProviderDetailsState;

public static partial class DataProviderDetailsStateReducers
{
	[ReducerMethod()]
	public static DataProviderDetailsState EmptyDataProviderDetailsActionReducer(DataProviderDetailsState state, EmptyDataProviderDetailsAction action) 
		=> state with { User = null};
}
