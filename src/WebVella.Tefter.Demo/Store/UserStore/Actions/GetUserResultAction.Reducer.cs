namespace WebVella.Tefter.Demo.Store;

public static partial class UserStoreReducers
{
	[ReducerMethod]
	public static UserStore GetUserResultReducer(UserStore state, GetUserResultAction action)
	{
		return state with { IsLoading = action.IsLoading };
	}
}
