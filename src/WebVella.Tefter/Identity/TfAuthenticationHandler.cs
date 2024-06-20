//using Microsoft.AspNetCore.Authentication.Cookies;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Options;
//using System.Net;
//using System.Text.Encodings.Web;

//namespace WebVella.Tefter.Identity;


//internal class TfAuthenticationHandler : AuthenticationHandler<TfAuthenticationOptions>
//{
//	private readonly AuthenticationStateProvider _authStateProvider;
//	private readonly IIdentityManager _identityManager;
//	private readonly TfTempStorageSessionStoreService _store;

//	public TfAuthenticationHandler(
//		AuthenticationStateProvider authStateProvider,
//		TfTempStorageSessionStoreService store,
//		IIdentityManager identityManager,
//		IOptionsMonitor<TfAuthenticationOptions> options,
//		ILoggerFactory logger, UrlEncoder encoder)
//		: base(options, logger, encoder)
//	{
//		_store = store;
//		_identityManager = identityManager;
//		_authStateProvider = authStateProvider;
//	}

//	protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
//	{
//		Guid? currentUserId = null;
//		bool cookieExists = false;

//		if (Request.Path == "/login")
//		{
//			var token = Request.Query[Options.AUTH_TOKEN_NAME];
//			if (!string.IsNullOrWhiteSpace(token))
//				currentUserId = _store.Get(token);
//		}

//		if (currentUserId == null)
//		{

//			var sessionIdentifier = Request.Cookies[Options.COOKIE_NAME];
//			if (!string.IsNullOrWhiteSpace(sessionIdentifier))
//			{
//				Guid id;
//				if (Guid.TryParse(sessionIdentifier, out id))
//				{
//					cookieExists = true;
//					currentUserId = id;
//				}
//			}
//		}

//		if (currentUserId != null)
//		{
//			var userResult = await _identityManager.GetUserAsync(currentUserId.Value);
//			if (!userResult.IsSuccess || userResult.Value is null)
//			{
//				if (cookieExists)
//					Context.Response.Cookies.Delete(Options.COOKIE_NAME);

//				return AuthenticateResult.Fail($"Authentication failed: wrong user identifier.");
//			}

//			var user = userResult.Value;

//			var claimsIdentity = new ClaimsIdentity(
//				new List<Claim> { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) },
//				CookieAuthenticationDefaults.AuthenticationScheme);

//			var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

//			if (!cookieExists)
//			{
//				Context.Response.Cookies.Append(
//					Options.COOKIE_NAME,
//					currentUserId.ToString(),
//					new CookieOptions
//					{
//						Expires = DateTime.Now.AddDays(30)
//					});
//			}

//			Context.User = claimsPrincipal;


//			await ((TfBrowserStorageAuthStateProvider)_authStateProvider).UpdateAuthenticationStateAsync(user);
//			return AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, this.Scheme.Name));
//		}
		

//		return AuthenticateResult.Fail($"Authentication failed: user identifier not provided.");
//	}
//}

