namespace WebVella.Tefter.Web.Store.UserState;

public static partial class UserStateReducers
{
	[ReducerMethod()]
	public static UserState InitUserResultReducer(UserState state, InitUserAction action)
	{
		return state with { User = action.User, Loading = false };
	}
}
