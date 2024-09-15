namespace WebVella.Tefter.Web.Store;

public static partial class DashboardStateReducers
{
	[ReducerMethod()]
	public static TfState EmptyDashboardStateActionReducer(TfState state, EmptyDashboardStateAction action) 
		=> state with {};//, Provider = action.Provider};
}
