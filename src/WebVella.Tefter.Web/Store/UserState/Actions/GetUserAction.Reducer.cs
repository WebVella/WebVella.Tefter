namespace WebVella.Tefter.Web.Store.UserState;

public static partial class UserStateReducers
{
	[ReducerMethod()]
	public static UserState InitUserReducer(UserState state, GetUserAction action)
	{
		return state with {Loading = true};
	}
}
