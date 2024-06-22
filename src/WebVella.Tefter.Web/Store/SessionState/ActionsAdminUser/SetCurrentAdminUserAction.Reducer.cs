namespace WebVella.Tefter.Web.Store.SessionState;

public static partial class SessionStateReducers
{
	[ReducerMethod()]
	public static SessionState SetCurrentAdminUserReducer(SessionState state, SetCurrentAdminUser action)
	{
		return state with
		{
			CurrentAdminUser = action.User
		};
	}
}