using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;

namespace WebVella.Tefter.Identity;

public class TfAuthStateProvider : AuthenticationStateProvider
{
	private readonly ICryptoService _cryptoService;
	private readonly IIdentityManager _identityManager;
	private readonly IHttpContextAccessor _contextAccessor;
	private readonly IServiceProvider _serviceProvider;
	private readonly ClaimsPrincipal _anonymous;

	public TfAuthStateProvider(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;

		_contextAccessor = _serviceProvider.GetRequiredService<IHttpContextAccessor>();

		_identityManager = _serviceProvider.GetService<IIdentityManager>();

		_anonymous = new ClaimsPrincipal(new ClaimsIdentity());
	}

	public override async Task<AuthenticationState> GetAuthenticationStateAsync()
	{
		try
		{
			var cookieEncryptedText = _contextAccessor.HttpContext.Request.Cookies[Constants.TEFTER_AUTH_COOKIE_NAME];
			if (!string.IsNullOrWhiteSpace(cookieEncryptedText))
			{
				var cookieDecryptedText = _cryptoService.Decrypt(cookieEncryptedText);
				Guid currentUserId;
				if (!Guid.TryParse(cookieDecryptedText, out currentUserId))
					return await Task.FromResult(new AuthenticationState(_anonymous));

				var userResult = await _identityManager.GetUserAsync(currentUserId);
				if (!userResult.IsSuccess || userResult.Value is null)
					return await Task.FromResult(new AuthenticationState(_anonymous));

				var user = userResult.Value;

				var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, user.Id.ToString()) };
				claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role.Name)));

				var claimsIdentity = new ClaimsIdentity(claims);
				var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

				_contextAccessor.HttpContext.User = claimsPrincipal;
				return await Task.FromResult(new AuthenticationState(_anonymous));
			}
		}
		catch { }

		return await Task.FromResult(new AuthenticationState(_anonymous));
	}
}
