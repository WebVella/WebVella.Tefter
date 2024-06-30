namespace WebVella.Tefter.Web.Store.Base;

public partial class StateEffect
{
	[EffectMethod]
	public async Task SetThemeActionEffect(SetThemeAction action, IDispatcher dispatcher)
	{
		if(!action.Persist){ 
			dispatcher.Dispatch(new SetThemeActionResult());
			return;
		}
		throw new NotImplementedException();
		//var setResult = await UseCase.SetUserTheme(
		//	userId: action.UserId,
		//	themeMode: action.ThemeMode,
		//	themeColor: action.ThemeColor
		//);

		//if (setResult.IsSuccess && setResult.Value)
		//{
		//	await TefterService.RemoveUnprotectedLocalStorage(TfConstants.UIThemeLocalKey);
		//	if (setResult.Value)
		//	{
		//		var themeSetting = new ThemeSettings { ThemeMode = action.ThemeMode, ThemeColor = action.ThemeColor };
		//		await TefterService.SetUnprotectedLocalStorage(TfConstants.UIThemeLocalKey, JsonSerializer.Serialize(themeSetting));
		//	}
		//}
		//else{
		//	Console.WriteLine($"Persisting SetCultureAction failed");
		//}
		dispatcher.Dispatch(new SetThemeActionResult());
	}
}