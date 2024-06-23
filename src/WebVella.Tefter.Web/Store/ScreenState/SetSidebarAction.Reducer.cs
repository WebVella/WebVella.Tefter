namespace WebVella.Tefter.Web.Store.ScreenState;

public static partial class ScreenStateReducers
{
	[ReducerMethod()]
	public static ScreenState SetSidebarActionReducer(ScreenState state, SetSidebarAction action) 
		=> state with {SidebarExpanded = action.SidebarExpanded};
}
