namespace WebVella.Tefter.Web.Store.UserState;

public static partial class UserStateReducers
{
    [ReducerMethod()]
    public static UserState SetThemeReducer(UserState state, SetThemeAction action)
    {
        return state with { User = state.User with { ThemeMode = action.ThemeMode, ThemeColor = action.ThemeColor} };
    }
}