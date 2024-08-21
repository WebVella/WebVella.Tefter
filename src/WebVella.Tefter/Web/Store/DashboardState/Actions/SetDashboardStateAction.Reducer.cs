namespace WebVella.Tefter.Web.Store.DashboardState;

public static partial class DashboardStateReducers
{
	[ReducerMethod()]
	public static DashboardState SetDashboardStateActionReducer(DashboardState state, 
		SetDashboardStateAction action) 
		=> state with { IsBusy = action.IsBusy }; //, Provider = action.Provider};
}
