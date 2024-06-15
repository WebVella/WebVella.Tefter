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
			SpaceList = action.UserSession.SpaceList,
			SpaceDict = action.UserSession.SpaceDict,
			SpaceDataDict = action.UserSession.SpaceDataDict,
			SpaceDataList = action.UserSession.SpaceDataList,
			SpaceViewDict = action.UserSession.SpaceViewDict,
			SpaceViewList = action.UserSession.SpaceViewList,
		};
	}
}
