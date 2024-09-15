namespace WebVella.Tefter.Web.Store;

public static partial class ThemeStateReducers
{
	[ReducerMethod()]
	public static TfState SetThemeActionReducer(TfState state, SetThemeAction action)
		=> state with { CurrentUser = action.CurrentUser, ThemeMode = action.ThemeMode, ThemeColor = action.ThemeColor };
}
