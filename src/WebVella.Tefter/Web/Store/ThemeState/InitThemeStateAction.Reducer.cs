namespace WebVella.Tefter.Web.Store.ThemeState;

public static partial class ThemeStateReducers
{
	[ReducerMethod()]
	public static ThemeState InitThemeStateActionReducer(ThemeState state, InitThemeStateAction action)
		=> state with { ThemeMode = action.ThemeMode, ThemeColor = action.ThemeColor };
}
