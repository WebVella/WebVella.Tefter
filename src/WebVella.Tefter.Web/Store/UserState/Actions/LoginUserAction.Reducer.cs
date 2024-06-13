namespace WebVella.Tefter.Web.Store.UserState;

public static partial class UserStateReducers
{
	[ReducerMethod()]
	public static UserState LoginUserReducer(UserState state, LoginUserAction action)
	{
		return state with {IsLoading = true};
	}
}
