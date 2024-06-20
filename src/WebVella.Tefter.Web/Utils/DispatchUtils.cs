namespace WebVella.Tefter.Web.Utils;

public static class DispatchUtils
{
	public static void DispatchKeyDown(
		IDispatcher dispatcher,
		IState<SessionState> sessionState,
		IState<UserState> userState,
		FluentKeyCodeEventArgs args)
	{

		if (args.CtrlKey)
		{
			if (args.Key == KeyCode.Function11)
			{
				dispatcher.Dispatch(new SetUIAction(
				userId: userState.Value.User.Id,
				spaceId: sessionState.Value.SpaceRouteId,
				spaceDataId: sessionState.Value.SpaceDataRouteId,
				spaceViewId: sessionState.Value.SpaceViewRouteId,
				mode: sessionState.Value.ThemeMode,
				color: sessionState.Value.ThemeColor,
				sidebarExpanded: !sessionState.Value.SidebarExpanded,
				cultureOption: sessionState.Value.CultureOption
				));
			}
		}
	}
}
