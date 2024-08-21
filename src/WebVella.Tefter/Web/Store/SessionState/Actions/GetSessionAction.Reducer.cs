namespace WebVella.Tefter.Web.Store.SessionState;

public static partial class SessionStateReducers
{
	[ReducerMethod()]
	public static SessionState GetSessionReducer(SessionState state, GetSessionAction action)
	{
		return state with {IsBusy = true, IsDataLoading = true};
	}
}
