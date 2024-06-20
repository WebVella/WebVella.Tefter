using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace WebVella.Tefter.Identity;

public partial interface IIdentityManager
{
	Task<Result<User>> AuthenticateAsync(string email, string password, bool rememberMe);
}

public partial class IdentityManager : IIdentityManager
{
	public async Task<Result<User>> AuthenticateAsync(string email, string password, bool rememberMe)
	{
		if (_httpContextAccessor == null)
			return Result.Fail("IHttpContextAccessor is not available.Call services.AddHttpContextAccessor(); to register it in DI.");

		if (_httpContextAccessor.HttpContext == null)
			return Result.Fail("HttpContext is not available");

		var user = (await GetUserAsync(email, password)).Value;

		if (user == null)
			return Result.Fail(new ValidationError(null, "Invalid email or password."));

		if (user is { Enabled: false })
			return Result.Fail(new ValidationError(null, "User access to site is denied."));

		var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, user.Id.ToString()) };
		claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role.Name)));

		var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
		var authProperties = new AuthenticationProperties
		{
			AllowRefresh = true,
			ExpiresUtc = rememberMe ? DateTimeOffset.UtcNow.AddDays(30) : DateTimeOffset.UtcNow.AddDays(1),
			IsPersistent = rememberMe,
			IssuedUtc = DateTimeOffset.UtcNow
		};

		await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
			new ClaimsPrincipal(claimsIdentity), authProperties);

		//TODO if we need activity log - here is the place

		return Result.Ok(user);
	}

}
