using Microsoft.AspNetCore.Http;

namespace WebVella.Tefter.Authorization;

public class TfAuthStateProvider : AuthenticationStateProvider
{
	private readonly ITfCryptoService _cryptoService;
	private readonly ITfService _tfService;
	private readonly IHttpContextAccessor _contextAccessor;
	private readonly IServiceProvider _serviceProvider;
	private readonly ClaimsPrincipal _anonymous;

	public TfAuthStateProvider(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;

		_cryptoService = _serviceProvider.GetRequiredService<ITfCryptoService>();

		_contextAccessor = _serviceProvider.GetRequiredService<IHttpContextAccessor>();

		_tfService = _serviceProvider.GetService<ITfService>();

		_anonymous = new TfPrincipal(new TfIdentity());
	}

	public override async Task<AuthenticationState> GetAuthenticationStateAsync()
	{
		try
		{
			if (_contextAccessor is null || _contextAccessor.HttpContext is null)
				return await Task.FromResult(new AuthenticationState(_anonymous));

			var cookieEncryptedText = _contextAccessor.HttpContext.Request.Cookies[Constants.TEFTER_AUTH_COOKIE_NAME];
			if (!string.IsNullOrWhiteSpace(cookieEncryptedText))
			{
				var cookieDecryptedText = _cryptoService.Decrypt(cookieEncryptedText);
				Guid currentUserId;
				if (!Guid.TryParse(cookieDecryptedText, out currentUserId))
					return await Task.FromResult(new AuthenticationState(_anonymous));

				var user = await _tfService.GetUserAsync(currentUserId);
				if (user is null)
					return await Task.FromResult(new AuthenticationState(_anonymous));

				var claims = new List<Claim> {
					new(ClaimTypes.NameIdentifier, user.Id.ToString()),
					new(ClaimTypes.Email, user.Email.ToString()) };
				claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role.Name)));

				var tfIdentity = new TfIdentity(user, claims);
				var tfPrincipal = new TfPrincipal(tfIdentity);

				_contextAccessor.HttpContext.User = tfPrincipal;
				return await Task.FromResult(new AuthenticationState(tfPrincipal));
			}
		}
		catch { }

		return await Task.FromResult(new AuthenticationState(_anonymous));
	}
}
