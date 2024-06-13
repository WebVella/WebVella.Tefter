namespace WebVella.Tefter.Demo.Store;

public static partial class UserStoreReducers
{
	[ReducerMethod]
	public static UserStore ChangeThemeReducer(UserStore state, ChangeThemeAction action)
	{
		return state;// with { ThemeColor = action.ThemeColor, ThemeMode = action.ThemeMode};
	}
}
