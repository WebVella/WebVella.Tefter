namespace WebVella.Tefter.Identity;

public class TfIdentity : ClaimsIdentity
{
	public User User { get; init; }

	public TfIdentity(User user = null, IEnumerable<Claim> claims = null)
		:base (claims, "Tefter Cookie Authentication")
	{
		User = user;
	}
}
