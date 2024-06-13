namespace WebVella.Tefter.Demo.Store;

public static partial class UserStoreReducers
{
	[ReducerMethod()]
	public static UserStore GetUserReducer(UserStore state, GetUserAction action)
	{
		return state with { IsLoading = true };
	}
}
