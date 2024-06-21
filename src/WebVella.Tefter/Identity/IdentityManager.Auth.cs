using System.Globalization;

namespace WebVella.Tefter.Identity;

public partial interface IIdentityManager
{
	Task<Result> AuthenticateAsync(IJSRuntime jsRuntime,
		string email, string password, bool rememberMe);

	Task<Result> LogoutAsync(IJSRuntime jsRuntime);
}

public partial class IdentityManager : IIdentityManager
{
	public async Task<Result> AuthenticateAsync(IJSRuntime jsRuntime,
		string email, string password, bool rememberMe)
	{
		Result validationResult = new Result();
		if (string.IsNullOrWhiteSpace(email))
			validationResult.WithError(new ValidationError(nameof(User.Email), "The email is required."));

		if (string.IsNullOrWhiteSpace(password))
			validationResult.WithError(new ValidationError(nameof(User.Email), "The password is required."));

		if (validationResult.IsFailed)
			return validationResult;

		var userResult = (await GetUserAsync(email, password));
		if (userResult.IsFailed)
			return Result.Fail(new Error("Unable to get user.").CausedBy(userResult.Errors));

		var user = userResult.Value;

		if (user == null)
			return Result.Fail(new ValidationError(nameof(User.Password), "Invalid email or password."));

		if (user is { Enabled: false })
			return Result.Fail(new ValidationError(nameof(User.Email), "User access to site is denied."));

		try
		{
			var cryptoService = _serviceProvider.GetRequiredService<ICryptoService>();
			if (jsRuntime == null)
				return Result.Fail("Unable to instantiate JSRuntime.");

			//Set auth cookie
			await new CookieService(jsRuntime).SetAsync(
					key: Constants.TEFTER_AUTH_COOKIE_NAME,
					value: cryptoService.Encrypt(user.Id.ToString()),
					expiration: rememberMe ? DateTimeOffset.Now.AddDays(30) : null);

			return Result.Ok();
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("User authentication failed.").CausedBy(ex));
		}
	}

	public async Task<Result> LogoutAsync(IJSRuntime jsRuntime )
	{
		try
		{
			//remove auth cookie
			await new CookieService(jsRuntime).RemoveAsync(Constants.TEFTER_AUTH_COOKIE_NAME);
			return Result.Ok();
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("User logout failed.").CausedBy(ex));
		}
	}

}
