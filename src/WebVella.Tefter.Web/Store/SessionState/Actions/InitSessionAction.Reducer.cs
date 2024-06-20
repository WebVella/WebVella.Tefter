namespace WebVella.Tefter.Web.Store.SessionState;

public static partial class SessionStateReducers
{
	[ReducerMethod()]
	public static SessionState InitSessionResultReducer(SessionState state, InitSessionAction action)
	{
		var culture = String.IsNullOrWhiteSpace(action.UserSession.CultureCode) ? null : CultureInfo.GetCultureInfo(action.UserSession.CultureCode);
		CultureInfo.CurrentCulture = culture;
		CultureInfo.CurrentUICulture = culture;
		CultureOption cultureOption = TfConstants.CultureOptions.FirstOrDefault(x=> x.CultureCode ==  action.UserSession.CultureCode);
		if(cultureOption is null) cultureOption = TfConstants.CultureOptions[0];
		return state with
		{
			UserId = action.UserSession.UserId,
			ThemeColor = action.UserSession.ThemeColor,
			ThemeMode = action.UserSession.ThemeMode,
			SpaceRouteId = action.RouteSpaceId,
			SpaceDataRouteId = action.RouteSpaceDataId,
			SpaceViewRouteId = action.RouteSpaceViewId,
			SidebarExpanded = action.UserSession.SidebarExpanded,
			Space = action.UserSession.Space,
			SpaceData = action.UserSession.SpaceData,
			SpaceView = action.UserSession.SpaceView,
			IsLoading = false,
			DataHashId = action.UserSession.DataHashId,
			IsDataLoading = false,
			SpaceDataDict = action.UserSession.SpaceDataDict,
			SpaceViewDict = action.UserSession.SpaceViewDict,
			SpaceNav = action.UserSession.SpaceNav,
			CultureOption = cultureOption
		};
	}
}
