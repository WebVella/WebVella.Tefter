namespace WebVella.Tefter.UseCases.State;

internal partial class StateUseCase
{
	private readonly IIdentityManager _identityManager;
	public StateUseCase(IIdentityManager identityManager)
	{
		_identityManager = identityManager;
	}

	internal async Task<User> GetUserWithChecks(Guid userId)
	{
		Result<User> userResult = await _identityManager.GetUserAsync(userId);
		if(userResult.IsFailed) throw new Exception("GetUserAsync failed");
		if(userResult.Value is null ) throw new Exception("User not found failed");
		return userResult.Value;
	}
}
