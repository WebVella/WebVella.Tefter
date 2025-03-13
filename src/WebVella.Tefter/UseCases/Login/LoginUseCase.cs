namespace WebVella.Tefter.UseCases.Login;

internal class LoginUseCase
{
	private readonly ITfService _tfService;
	public LoginUseCase(IServiceProvider serviceProvider)
	{
		_tfService = serviceProvider.GetService<ITfService>();
	}
	internal void OnInitialized()
	{
		Form = new TucLoginForm();
	}

	internal TucLoginForm Form { get; set; }
	internal bool IsSubmitting { get; set; } = false;

	internal virtual async Task<bool> AuthenticateAsync(IJSRuntime jsRuntime)
	{
		await _tfService.AuthenticateAsync(
			jsRuntime: jsRuntime,
			email: Form.Email,
			password: Form.Password,
			rememberMe: Form.RememberMe);

		return true;
	}

}
