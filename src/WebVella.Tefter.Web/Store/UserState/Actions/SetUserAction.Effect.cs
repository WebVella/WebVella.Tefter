namespace WebVella.Tefter.Web.Store.UserState;

public partial class UserStateEffects
{
	[EffectMethod]
	public async Task HandleSetUserAction(SetUserAction action, IDispatcher dispatcher)
	{
		if (action is null) return;
		await TfService.RemoveUnprotectedLocalStorage(TfConstants.UIThemeLocalKey);
		if (action.User is not null)
		{
			var themeSetting = new ThemeSettings{ ThemeMode = action.User.Settings.ThemeMode, ThemeColor = action.User.Settings.ThemeColor};
			await TfService.SetUnprotectedLocalStorage(TfConstants.UIThemeLocalKey,JsonSerializer.Serialize(themeSetting));
		}



	}

}

