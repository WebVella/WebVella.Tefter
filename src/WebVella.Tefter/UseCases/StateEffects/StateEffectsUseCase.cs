namespace WebVella.Tefter.UseCases.StateEffects;

public partial class StateEffectsUseCase
{
	private readonly IIdentityManager _identityManager;
	private readonly IMessageService _messageService;
	private readonly IToastService _toastService;
	public StateEffectsUseCase(
		IIdentityManager identityManager,
		IMessageService messageService,
		IToastService toastService
		)
	{
		_identityManager = identityManager;
		_messageService = messageService;
		_toastService = toastService;
	}

	public async Task<User> GetUserWithChecks(Guid userId)
	{
		Result<User> userResult = await _identityManager.GetUserAsync(userId);
		if(userResult.IsFailed) throw new Exception("GetUserAsync failed");
		if(userResult.Value is null ) throw new Exception("User not found failed");
		return userResult.Value;
	}
}
