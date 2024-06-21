namespace WebVella.Tefter.Identity;

public class TfIdentity : ClaimsIdentity
{
	public User User { get; init; }

	public TfIdentity(User user, IEnumerable<Claim>? claims)
		:base (claims, "Tefter Cookie Authentication")
	{
		User = user;
	}
}
