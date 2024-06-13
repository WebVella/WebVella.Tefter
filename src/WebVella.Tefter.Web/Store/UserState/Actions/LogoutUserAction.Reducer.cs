namespace WebVella.Tefter.Web.Store.UserState;

public static partial class UserStateReducers
{
	[ReducerMethod()]
	public static UserState LogountUserReducer(UserState state, LogoutUserAction action)
	{
		return state;
	}
}
