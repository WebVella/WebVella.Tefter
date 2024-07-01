namespace WebVella.Tefter.UseCases.UserAdmin;
public partial class UserAdminUseCase
{
	internal async Task<Result<TucUser>> GetUserAsync(Guid userId)
	{
		var userResult = await _identityManager.GetUserAsync(userId);
		if (userResult.IsFailed) return Result.Fail(new Error("GetUserAsync failed").CausedBy(userResult.Errors));
		if (userResult.Value is null) return Result.Ok((TucUser)null);
		return new TucUser(userResult.Value);
	}


}
