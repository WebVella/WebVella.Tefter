namespace WebVella.Tefter.UseCases.StateEffects;

public partial class StateEffectsUseCase
{
	public async Task<Result<bool>> SetSidebarState(Guid userId,bool sidebarExpanded)
	{
		User user = await GetUserWithChecks(userId);
		var userBld = _identityManager.CreateUserBuilder(user).WithOpenSidebar(sidebarExpanded);
		var saveResult = await _identityManager.SaveUserAsync(userBld.Build());
		if (saveResult.IsFailed)
			return Result.Fail(new Error("SaveUserAsync failed").CausedBy(saveResult.Errors));
		user = await GetUserWithChecks(userId);
		return Result.Ok(user.Settings?.IsSidebarOpen ?? true);
	}
}
