namespace WebVella.Tefter.UseCases.Login;

public class LoginUseCase
{
	private readonly IIdentityManager _identityManager;
	public LoginUseCase(IIdentityManager identityManager)
	{
		_identityManager = identityManager;
	}
	internal void OnInitialized()
	{
		Form = new TucLoginForm();
	}

	internal TucLoginForm Form { get; set; }
	internal bool IsSubmitting { get; set; } = false;

	internal async Task<Result<bool>> AuthenticateAsync(IJSRuntime jsRuntime)
	{
		Result result = await _identityManager.AuthenticateAsync(
			jsRuntime: jsRuntime,
			email: Form.Email,
			password: Form.Password,
			rememberMe: Form.RememberMe);
		return Result.Ok(result.IsSuccess);
	}

}
