namespace WebVella.Tefter.UseCases.UserAdmin;
public partial class UserAdminUseCase
{
	internal async Task<Result<List<TucRole>>> GetUserRolesAsync(){ 
		var userResult = await _identityManager.GetRolesAsync();
		if(userResult.IsFailed) return Result.Fail(new Error("GetRolesAsync failed").CausedBy(userResult.Errors));
		if(userResult.Value is null) return Result.Ok(new List<TucRole>());
		return userResult.Value.Select(x=> new TucRole(x)).ToList();
	}


}
