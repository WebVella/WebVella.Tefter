namespace WebVella.Tefter.Web.Store;

public static partial class DashboardStateReducers
{
	[ReducerMethod()]
	public static TfState SetDashboardStateActionReducer(TfState state, 
		SetDashboardStateAction action) 
		=> state with {  }; //, Provider = action.Provider};
}
