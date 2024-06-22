namespace WebVella.Tefter.Web.Store.SessionState;

public partial class SessionStateEffects
{
	[EffectMethod]
	public async Task HandleSetThemeAction(SetUIAction action, IDispatcher dispatcher)
	{
		var sessionResult = await TfService.SetSessionUI(
			userId: action.UserId,
			spaceId: action.SpaceId,
			spaceDataId: action.SpaceDataId,
			spaceViewId: action.SpaceViewId,
			themeMode: action.ThemeMode,
			themeColor: action.ThemeColor,
			sidebarExpanded: action.SidebarExpanded,
			cultureCode: action.CultureOption.CultureCode
		);

		if (sessionResult.IsSuccess && sessionResult.Value is not null)
		{
			if (action is null) return;
			await TfService.RemoveUnprotectedLocalStorage(TfConstants.UIThemeLocalKey);
			if (sessionResult.Value is not null)
			{
				var themeSetting = new ThemeSettings { ThemeMode = sessionResult.Value.ThemeMode, ThemeColor = sessionResult.Value.ThemeColor };
				await TfService.SetUnprotectedLocalStorage(TfConstants.UIThemeLocalKey, JsonSerializer.Serialize(themeSetting));
			}

			dispatcher.Dispatch(new InitSessionAction(
			spaceId: sessionResult.Value.Space?.Id,
			spaceDataId: sessionResult.Value.SpaceData?.Id,
			spaceViewId: sessionResult.Value.SpaceView?.Id,
			userSession: sessionResult.Value
			));
		}
	}
}