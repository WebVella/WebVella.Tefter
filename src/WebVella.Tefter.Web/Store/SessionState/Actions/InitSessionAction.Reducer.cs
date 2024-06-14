namespace WebVella.Tefter.Web.Store.SessionState;

public static partial class SessionStateReducers
{
	[ReducerMethod()]
	public static SessionState InitUserResultReducer(SessionState state, InitSessionAction action)
	{
		return state with
		{
			Space = action.UserSession.Space,
			SpaceData = action.UserSession.SpaceData,
			SpaceView = action.UserSession.SpaceView,
			IsLoading = false
		};
	}
}
