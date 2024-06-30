namespace WebVella.Tefter.Web.Store.ScreenState;

public static partial class ScreenStateReducers
{
	[ReducerMethod()]
	public static ScreenState InitScreenStateActionReducer(ScreenState state, InitScreenStateAction action)
	{
		return state with {UseCase = action.UseCase, SidebarExpanded = action.SidebarExpanded};
	}
}
