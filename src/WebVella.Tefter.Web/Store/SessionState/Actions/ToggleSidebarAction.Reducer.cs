namespace WebVella.Tefter.Web.Store.SessionState;

public static partial class SessionStateReducers
{
	[ReducerMethod()]
	public static SessionState SetThemeReducer(SessionState state, ToggleSidebarAction action)
	{
		return state with { IsSidebarExpanded = !state.IsSidebarExpanded };
	}
}