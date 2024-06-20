//using Microsoft.AspNetCore.Authentication.Cookies;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.DependencyInjection;

//namespace WebVella.Tefter.Identity;

//public partial interface IIdentityManager
//{
//	Task<Result<User>> AuthenticateAsync(string email, string password);
//}

//public partial class IdentityManager : IIdentityManager
//{
//	public async Task<Result<User>> AuthenticateAsync(string email, string password)
//	{
//		var user = (await GetUserAsync(email, password)).Value;

//		if (user == null)
//			return Result.Fail(new ValidationError(null, "Invalid email or password."));

//		if (user is { Enabled: false })
//			return Result.Fail(new ValidationError(null, "User access to site is denied."));

//		try
//		{
//			IServiceScopeFactory serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
//			using (var scope = serviceScopeFactory.CreateScope())
//			{
//				//var authStateProvider = (TfAuthStateProvider)scope.ServiceProvider.GetRequiredService<AuthenticationStateProvider>();
//				//await authStateProvider.UpdateAuthenticationStateAsync(user);
//				//return Result.Ok(user);

//				var claimsIdentity = new ClaimsIdentity(
//					new List<Claim> { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) },
//					CookieAuthenticationDefaults.AuthenticationScheme);

//				var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

//				var authProperties = new AuthenticationProperties
//				{
//					AllowRefresh = true,
//					ExpiresUtc = DateTimeOffset.UtcNow.AddYears(100),
//					IsPersistent = false,
//					IssuedUtc = DateTimeOffset.UtcNow,
//				};

//				var httpContextAccesor = _serviceProvider.GetRequiredService<IHttpContextAccessor>();
//				await httpContextAccesor.HttpContext.SignInAsync(
//					CookieAuthenticationDefaults.AuthenticationScheme, 
//					claimsPrincipal, authProperties);
				
//				return Result.Ok(user);
//			}
//		}
//		catch (Exception ex)
//		{
//			return Result.Fail(new Error("User authentication failed.").CausedBy(ex));
//		}
//	}
//}
