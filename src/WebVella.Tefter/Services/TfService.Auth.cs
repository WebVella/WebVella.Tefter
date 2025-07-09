namespace WebVella.Tefter.Services;

public partial interface ITfService
{
	internal Task<TfUser> AuthenticateAsync(
		IJSRuntime jsRuntime,
		string email,
		string password,
		bool rememberMe);

	internal Task LogoutAsync(IJSRuntime jsRuntime);
}

public partial class TfService : ITfService
{
	public async Task<TfUser> AuthenticateAsync(
		IJSRuntime jsRuntime,
		string email,
		string password,
		bool rememberMe)
	{
		try
		{
			var valEx = new TfValidationException();

			if (string.IsNullOrWhiteSpace(email))
				valEx.AddValidationError(nameof(TfUser.Email), "The email is required.");

			if (string.IsNullOrWhiteSpace(password))
				valEx.AddValidationError(nameof(TfUser.Password), "The password is required.");

			valEx.ThrowIfContainsErrors();

			var user = (await GetUserAsync(email, password));

			if (user == null)
				valEx.AddValidationError(nameof(TfUser.Password), "Invalid email or password.");

			if (user is { Enabled: false })
				valEx.AddValidationError(nameof(TfUser.Email), "User access to site is denied.");

			valEx.ThrowIfContainsErrors();

			var cryptoService = _serviceProvider.GetRequiredService<ITfCryptoService>();
			if (jsRuntime == null)
				throw new TfException("Unable to instantiate JSRuntime.");

			//Set auth cookie
			await new CookieService(jsRuntime).SetAsync(
					key: TfConstants.TEFTER_AUTH_COOKIE_NAME,
					value: cryptoService.Encrypt(user.Id.ToString()),
					expiration: rememberMe ? DateTimeOffset.Now.AddDays(30) : null);

			return user;
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}



	public async Task LogoutAsync(IJSRuntime jsRuntime)
	{
		try
		{
			//remove auth cookie
			await new CookieService(jsRuntime).RemoveAsync(TfConstants.TEFTER_AUTH_COOKIE_NAME);
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}
}
