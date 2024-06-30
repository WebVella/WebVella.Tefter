namespace WebVella.Tefter.UseCases.StateEffects;

public partial class StateEffectsUseCase
{
	public async Task<Result<bool>> SetUserTheme(Guid userId,
		DesignThemeModes themeMode, OfficeColor themeColor)
	{
		var user = await GetUserWithChecks(userId);
		var userBld = _identityManager.CreateUserBuilder(user);
		userBld
		.WithThemeMode(themeMode)
		.WithThemeColor(themeColor);
		var saveResult = await _identityManager.SaveUserAsync(userBld.Build());
		if (saveResult.IsFailed)
			return Result.Fail(new Error("SaveUserAsync failed").CausedBy(saveResult.Errors));

		await RemoveUnprotectedLocalStorage(TfConstants.UIThemeLocalKey);
		var themeSetting = new TucThemeSettings { ThemeMode = themeMode, ThemeColor = themeColor };
		await SetUnprotectedLocalStorage(TfConstants.UIThemeLocalKey, JsonSerializer.Serialize(themeSetting));

		return Result.Ok(true);
	}
}
