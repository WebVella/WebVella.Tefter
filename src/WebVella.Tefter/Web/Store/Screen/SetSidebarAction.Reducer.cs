namespace WebVella.Tefter.Web.Store;

public static partial class ScreenStateReducers
{
	[ReducerMethod()]
	public static TfState SetSidebarActionReducer(TfState state, SetSidebarAction action)
	{
		return state with { SidebarExpanded = action.SidebarExpanded};
	}
}
