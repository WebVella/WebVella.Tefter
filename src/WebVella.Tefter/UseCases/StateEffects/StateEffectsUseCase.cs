namespace WebVella.Tefter.UseCases.StateEffects;

public partial class StateEffectsUseCase
{
	private readonly IIdentityManager _identityManager;
	private readonly IMessageService _messageService;
	private readonly IToastService _toastService;
	private readonly IJSRuntime _jsRuntime;
	public StateEffectsUseCase(
		IIdentityManager identityManager,
		IMessageService messageService,
		IToastService toastService,
		IJSRuntime jsRuntime
		)
	{
		_identityManager = identityManager;
		_messageService = messageService;
		_toastService = toastService;
		_jsRuntime = jsRuntime;
	}

	public async Task<User> GetUserWithChecks(Guid userId)
	{
		Result<User> userResult = await _identityManager.GetUserAsync(userId);
		if(userResult.IsFailed) throw new Exception("GetUserAsync failed");
		if(userResult.Value is null ) throw new Exception("User not found failed");
		return userResult.Value;
	}

	public async Task SetUnprotectedLocalStorage(string key, string value)
	{
		await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, value);
	}

	public async Task RemoveUnprotectedLocalStorage(string key)
	{
		await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
	}

	public async Task<string> GetUnprotectedLocalStorage(string key)
	{
		return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", key);
	}
}
