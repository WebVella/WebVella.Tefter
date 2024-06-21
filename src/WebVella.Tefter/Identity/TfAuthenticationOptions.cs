using Microsoft.AspNetCore.Authentication;

namespace WebVella.Tefter.Identity;

internal class TfAuthenticationOptions : AuthenticationSchemeOptions
{
	public static string DefaultScheme { get; set; } = "TfAuthenticationScheme";
	public string COOKIE_NAME { get; set; } = Constants.TEFTER_AUTH_COOKIE_NAME;
}
