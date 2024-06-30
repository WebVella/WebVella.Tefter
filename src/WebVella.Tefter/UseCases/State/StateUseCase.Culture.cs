namespace WebVella.Tefter.UseCases.State;

internal partial class StateUseCase
{
	internal async void SetUserCulture(Guid userId, string cultureCode)
	{
		User user = await GetUserWithChecks(userId);

		var userBld = _identityManager.CreateUserBuilder(user);
		userBld.WithCultureCode(cultureCode);

		var saveResult = await _identityManager.SaveUserAsync(userBld.Build());
		if (saveResult.IsFailed) throw new Exception("SaveUserAsync failed");
	}
}
