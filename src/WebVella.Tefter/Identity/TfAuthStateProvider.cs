using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Org.BouncyCastle.Asn1.Ocsp;

namespace WebVella.Tefter.Identity;

public class TfBrowserStorageAuthStateProvider : AuthenticationStateProvider
{
	private readonly IServiceProvider _serviceProvider;
	private readonly ClaimsPrincipal _anonymous;

	public TfBrowserStorageAuthStateProvider(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
		_anonymous = new ClaimsPrincipal(new ClaimsIdentity());
	}

	public override async Task<AuthenticationState> GetAuthenticationStateAsync()
	{
		try
		{
			//var userSessionStorageResult = await _sessionStorage.GetAsync<User>("UserSession");
			//var user = userSessionStorageResult.Success ? userSessionStorageResult.Value : null;

			var httpAccessor = _serviceProvider.GetRequiredService<IHttpContextAccessor>();



			//User user = ;

			if (httpAccessor.HttpContext.User == null)
				return await Task.FromResult(new AuthenticationState(_anonymous));

			//var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, user.Id.ToString()) };
			//claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role.Name)));

			//var claimsIdentity = new ClaimsIdentity(claims, "Tefter Authentication");
			//var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
			return await Task.FromResult(new AuthenticationState(httpAccessor.HttpContext.User));
		}
		catch
		{
			return await Task.FromResult(new AuthenticationState(_anonymous));
		}
	}

	public async Task UpdateAuthenticationStateAsync(User user)
	{
		ClaimsPrincipal claimsPrincipal;
		if (user is not null)
		{
			var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, user.Id.ToString()) };
			claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role.Name)));

			var claimsIdentity = new ClaimsIdentity(claims, "Tefter Authentication");
			claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
		}
		else
		{
			claimsPrincipal = _anonymous;
		}

		NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
	}
}
