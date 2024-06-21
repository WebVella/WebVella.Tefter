using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using System.Globalization;

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

		_cryptoService = _serviceProvider.GetRequiredService<ICryptoService>();

		_contextAccessor = _serviceProvider.GetRequiredService<IHttpContextAccessor>();

		_identityManager = _serviceProvider.GetService<IIdentityManager>();

		_anonymous = new TfPrincipal(new TfIdentity());
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


				CultureInfo cultureInfo = CultureInfo.GetCultureInfo(user.Settings.CultureCode);
				Thread.CurrentThread.CurrentCulture = cultureInfo;
				Thread.CurrentThread.CurrentUICulture = cultureInfo;


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
