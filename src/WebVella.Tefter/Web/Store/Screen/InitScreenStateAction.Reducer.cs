namespace WebVella.Tefter.Web.Store;

public static partial class ScreenStateReducers
{
	[ReducerMethod()]
	public static TfState InitScreenStateActionReducer(TfState state, InitScreenStateAction action)
	{
		return state with {SidebarExpanded = action.SidebarExpanded};
	}
}
