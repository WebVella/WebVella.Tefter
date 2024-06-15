namespace WebVella.Tefter.Web.Store.SessionState;

public static partial class SessionStateReducers
{
    [ReducerMethod()]
    public static SessionState SetThemeReducer(SessionState state, SetThemeAction action)
    {
        return state with { ThemeMode = action.ThemeMode, ThemeColor = action.ThemeColor};
    }
}