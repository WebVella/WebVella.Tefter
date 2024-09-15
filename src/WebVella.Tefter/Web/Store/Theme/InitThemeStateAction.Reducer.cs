namespace WebVella.Tefter.Web.Store;

public static partial class ThemeStateReducers
{
	[ReducerMethod()]
	public static TfState InitThemeStateActionReducer(TfState state, InitThemeStateAction action)
		=> state with { ThemeMode = action.ThemeMode, ThemeColor = action.ThemeColor };
}
