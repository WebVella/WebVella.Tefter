namespace WebVella.Tefter.Web.Store.SessionState;

public static partial class SessionStateReducers
{
	[ReducerMethod()]
	public static SessionState SessionActiveSpaceDataChangeReducer(SessionState state, SessionActiveSpaceDataChangeAction action)
	{
		return state with
		{
			RouteSpaceId = action.SpaceRouteId,
			RouteSpaceDataId = action.SpaceDataRouteId,
			RouteSpaceViewId = action.SpaceViewRouteId,
			Space = action.Space,
			SpaceData = action.SpaceData,
			SpaceView = action.SpaceView
		};
	}
}
