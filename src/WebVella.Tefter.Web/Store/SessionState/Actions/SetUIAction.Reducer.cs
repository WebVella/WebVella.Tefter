namespace WebVella.Tefter.Web.Store.SessionState;

public static partial class SessionStateReducers
{
	[ReducerMethod()]
	public static SessionState SetThemeReducer(SessionState state, SetUIAction action)
	{
		return state with { 
			ThemeMode = action.ThemeMode, ThemeColor = action.ThemeColor, 
			SidebarExpanded = action.SidebarExpanded };
	}
}