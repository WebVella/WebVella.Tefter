namespace WebVella.Tefter.Web.Store;

public static partial class StateReducers
{
	[ReducerMethod()]
	public static TfState InitStateActionReducer(TfState state, InitStateAction action)
		=> state with
		{
			CurrentUser = action.User,
			CurrentUserSpaces = action.UserSpaces,
			Culture = action.Culture,
			ThemeMode = action.ThemeMode,
			ThemeColor = action.ThemeColor,
			SidebarExpanded = action.SidebarExpanded,
		};
}
