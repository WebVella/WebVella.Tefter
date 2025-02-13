using System.Globalization;

namespace WebVella.Tefter.Identity;

public partial interface IIdentityManager
{
	Task AuthenticateAsync(IJSRuntime jsRuntime,
		string email, string password, bool rememberMe);

	Task LogoutAsync(IJSRuntime jsRuntime);
}

public partial class IdentityManager : IIdentityManager
{
	public async Task AuthenticateAsync(IJSRuntime jsRuntime,
		string email, string password, bool rememberMe)
	{
		var valEx = new TfValidationException();

		if (string.IsNullOrWhiteSpace(email))
			valEx.AddValidationError(nameof(User.Email), "The email is required.");

		if (string.IsNullOrWhiteSpace(password))
			valEx.AddValidationError(nameof(User.Password), "The password is required.");

		valEx.ThrowIfContainsErrors();

		var user = (await GetUserAsync(email, password));

		if (user == null)
			valEx.AddValidationError(nameof(User.Password), "Invalid email or password.");

		if (user is { Enabled: false })
			valEx.AddValidationError(nameof(User.Email), "User access to site is denied.");

		valEx.ThrowIfContainsErrors();

		var cryptoService = _serviceProvider.GetRequiredService<ITfCryptoService>();
		if (jsRuntime == null)
			throw new TfException("Unable to instantiate JSRuntime.");

		//Set auth cookie
		await new CookieService(jsRuntime).SetAsync(
				key: Constants.TEFTER_AUTH_COOKIE_NAME,
				value: cryptoService.Encrypt(user.Id.ToString()),
				expiration: rememberMe ? DateTimeOffset.Now.AddDays(30) : null);
	}

	public async Task LogoutAsync(IJSRuntime jsRuntime)
	{
		//remove auth cookie
		await new CookieService(jsRuntime).RemoveAsync(Constants.TEFTER_AUTH_COOKIE_NAME);
	}

}
