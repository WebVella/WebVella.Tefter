﻿namespace WebVella.Tefter.Identity;

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
		var user = (await GetUserAsync(email, password)).Value;

		if (user == null)
			return Result.Fail(new ValidationError(null, "Invalid email or password."));

		if (user is { Enabled: false })
			return Result.Fail(new ValidationError(null, "User access to site is denied."));

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
