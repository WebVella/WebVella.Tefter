namespace WebVella.Tefter.Web.Store.ThemeState;

public static partial class ThemeStateReducers
{
	[ReducerMethod()]
	public static ThemeState SetThemeActionReducer(ThemeState state, SetThemeAction action)
		=> state with { ThemeMode = action.ThemeMode, ThemeColor = action.ThemeColor };
}
