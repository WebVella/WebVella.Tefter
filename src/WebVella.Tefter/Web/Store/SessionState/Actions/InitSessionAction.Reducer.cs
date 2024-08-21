namespace WebVella.Tefter.Web.Store.SessionState;

public static partial class SessionStateReducers
{
	[ReducerMethod()]
	public static SessionState InitSessionResultReducer(SessionState state, InitSessionAction action)
	{
		return state with
		{
			UserId = action.UserSession.UserId,
			RouteSpaceId = action.RouteSpaceId,
			RouteSpaceDataId = action.RouteSpaceDataId,
			RouteSpaceViewId = action.RouteSpaceViewId,
			Space = action.UserSession.Space,
			SpaceData = action.UserSession.SpaceData,
			SpaceView = action.UserSession.SpaceView,
			IsBusy = false,
			DataHashId = action.UserSession.DataHashId,
			IsDataLoading = false,
			SpaceDataDict = action.UserSession.SpaceDataDict,
			SpaceViewDict = action.UserSession.SpaceViewDict,
			SpaceNav = action.UserSession.SpaceNav
		};
	}
}
