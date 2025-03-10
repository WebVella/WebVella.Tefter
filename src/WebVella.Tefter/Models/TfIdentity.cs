namespace WebVella.Tefter.Models;

public class TfIdentity : ClaimsIdentity
{
	public TfUser User { get; init; }

	public TfIdentity(TfUser user = null, IEnumerable<Claim> claims = null)
		: base(claims, "Tefter Cookie Authentication")
	{
		User = user;
	}
}
