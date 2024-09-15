namespace WebVella.Tefter.UseCases.StateEffects;

public partial class StateEffectsUseCase
{
	public async Task<Result<TucUser>> SetUserCulture(Guid userId, string cultureCode)
	{
		User user = await GetUserWithChecks(userId);

		var userBld = _identityManager.CreateUserBuilder(user);
		userBld.WithCultureCode(cultureCode);

		var saveResult = await _identityManager.SaveUserAsync(userBld.Build());
		if (saveResult.IsFailed)
			return Result.Fail(new Error("SaveUserAsync failed").CausedBy(saveResult.Errors));

		user = await GetUserWithChecks(userId);
		return Result.Ok(new TucUser(user));
	}
}
