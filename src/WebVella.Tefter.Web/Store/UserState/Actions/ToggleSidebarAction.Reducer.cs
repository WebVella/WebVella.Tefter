namespace WebVella.Tefter.Web.Store.UserState;

public static partial class UserStateReducers
{
	[ReducerMethod()]
	public static UserState SetThemeReducer(UserState state, ToggleSidebarAction action)
	{
		return state with { User = state.User with { SidebarExpanded = !state.User.SidebarExpanded } };
	}
}