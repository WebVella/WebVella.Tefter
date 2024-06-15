namespace WebVella.Tefter.Web.Store.SessionState;

public static partial class SessionStateReducers
{
	[ReducerMethod()]
	public static SessionState InitSessionResultReducer(SessionState state, InitSessionAction action)
	{
		return state with
		{
			ThemeColor = action.UserSession.ThemeColor,
			ThemeMode = action.UserSession.ThemeMode,
			SpaceRouteId = action.RouteSpaceId,
			SpaceDataRouteId = action.RouteSpaceDataId,
			SpaceViewRouteId = action.RouteSpaceViewId,
			SidebarExpanded = action.UserSession.SidebarExpanded,
			Space = action.UserSession.Space,
			SpaceData = action.UserSession.SpaceData,
			SpaceView = action.UserSession.SpaceView,
			IsLoading = false,
			DataHashId = action.UserSession.DataHashId,
			IsDataLoading = false,
			SpaceDataDict = action.UserSession.SpaceDataDict,
			SpaceViewDict = action.UserSession.SpaceViewDict,
			SpaceNav = action.UserSession.SpaceNav,
		};
	}
}
