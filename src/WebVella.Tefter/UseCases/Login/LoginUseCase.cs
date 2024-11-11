﻿namespace WebVella.Tefter.UseCases.Login;

internal class LoginUseCase
{
	private readonly IIdentityManager _identityManager;
	public LoginUseCase(IServiceProvider serviceProvider)
	{
		_identityManager = serviceProvider.GetService<IIdentityManager>();
	}
	internal void OnInitialized()
	{
		Form = new TucLoginForm();
	}

	internal TucLoginForm Form { get; set; }
	internal bool IsSubmitting { get; set; } = false;

	internal virtual async Task<Result<bool>> AuthenticateAsync(IJSRuntime jsRuntime)
	{
		Result result = await _identityManager.AuthenticateAsync(
			jsRuntime: jsRuntime,
			email: Form.Email,
			password: Form.Password,
			rememberMe: Form.RememberMe);
		return Result.Ok(result.IsSuccess);
	}

}
