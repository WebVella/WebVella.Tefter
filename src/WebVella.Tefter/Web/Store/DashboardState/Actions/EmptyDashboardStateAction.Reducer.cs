namespace WebVella.Tefter.Web.Store.DashboardState;

public static partial class DashboardStateReducers
{
	[ReducerMethod()]
	public static DashboardState EmptyDashboardStateActionReducer(DashboardState state, EmptyDashboardStateAction action) 
		=> state with {IsBusy = action.IsBusy};//, Provider = action.Provider};
}
