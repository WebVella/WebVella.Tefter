namespace WebVella.Tefter.Web.Store.SessionState;

public static partial class SessionStateReducers
{
	[ReducerMethod()]
	public static SessionState SessionActiveSpaceDataChangeReducer(SessionState state, SessionActiveSpaceDataChangeAction action)
	{
		return state with
		{
			SpaceRouteId = action.SpaceRouteId,
			SpaceDataRouteId = action.SpaceDataRouteId,
			SpaceViewRouteId = action.SpaceViewRouteId,
			Space = action.Space,
			SpaceData = action.SpaceData,
			SpaceView = action.SpaceView
		};
	}
}
