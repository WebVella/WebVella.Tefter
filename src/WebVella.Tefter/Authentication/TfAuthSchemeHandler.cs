using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;

namespace WebVella.Tefter.Authentication;

internal class TfAuthSchemeOptions : AuthenticationSchemeOptions
{
	public const string DefaultScheme = "WebVella.Tefter";
	public const string DefaultAuthenticationType = "WebVella.Tefter";
}

internal class TfAuthSchemeHandler : AuthenticationHandler<TfAuthSchemeOptions>
{
	private readonly TfAuthStateProvider _tfAuthStateProvider;

	public TfAuthSchemeHandler(
	   IOptionsMonitor<TfAuthSchemeOptions> options,
	   ILoggerFactory logger,
	   UrlEncoder encoder,
	   AuthenticationStateProvider authStateProvider)
		: base(options, logger, encoder)
	{
		_tfAuthStateProvider = (TfAuthStateProvider)authStateProvider;
	}

	protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
	{
		var authState = await _tfAuthStateProvider.GetAuthenticationStateAsync();

		if (authState is not null && authState.User is not null && authState.User.Identities?.Count() > 0 )
		{
			var ticket = new AuthenticationTicket(authState.User, this.Scheme.Name);
			return AuthenticateResult.Success(ticket);
		}
		return AuthenticateResult.Fail("Authentication failed");
	}
}
