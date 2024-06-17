namespace WebVella.Tefter.Web.Store.SessionState;

public partial class SessionStateEffects
{
	[EffectMethod]
	public async Task HandleSetThemeAction(SetUIAction action, IDispatcher dispatcher)
	{
		await tfService.SetSessionUI(
			userId: action.UserId,
			themeMode:action.ThemeMode,
			themeColor: action.ThemeColor,
			sidebarExpanded: action.SidebarExpanded
		);
	}
}