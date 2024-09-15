namespace WebVella.Tefter.Web.Store;

public static partial class ThemeStateReducers
{
	[ReducerMethod()]
	public static TfState SetThemeActionReducer(TfState state, SetThemeAction action)
		=> state with { ThemeMode = action.ThemeMode, ThemeColor = action.ThemeColor };
}
