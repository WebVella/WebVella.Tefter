namespace WebVella.Tefter.Identity;

public partial interface IIdentityManager
{
	Task<Result> AuthenticateAsync(string email, string password, bool rememberMe);
}

public partial class IdentityManager : IIdentityManager
{
	public async Task<Result> AuthenticateAsync(string email, string password, bool rememberMe)
	{
		var user = (await GetUserAsync(email, password)).Value;

		if (user == null)
			return Result.Fail(new ValidationError(null, "Invalid email or password."));

		if (user is { Enabled: false })
			return Result.Fail(new ValidationError(null, "User access to site is denied."));

		try
		{
			IServiceScopeFactory serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
			using (var scope = serviceScopeFactory.CreateScope())
			{
				var jsRuntime = scope.ServiceProvider.GetRequiredService<IJSRuntime>();
				var cryptoService = scope.ServiceProvider.GetRequiredService<ICryptoService>();
				if (jsRuntime == null)
					return Result.Fail("Unable to instantiate JSRuntime.");

				//Set auth cookie
				await new CookieService(jsRuntime).SetAsync(
						key: Constants.TEFTER_AUTH_COOKIE_NAME,
						value: cryptoService.Encrypt(user.Id.ToString()),
						expiration: rememberMe ? DateTimeOffset.Now.AddDays(30) : null);


				return Result.Ok();
			}
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("User authentication failed.").CausedBy(ex));
		}
	}
}
