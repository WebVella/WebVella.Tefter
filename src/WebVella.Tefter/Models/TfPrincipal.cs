namespace WebVella.Tefter.Models;

public class TfPrincipal : ClaimsPrincipal
{
	public TfPrincipal() : base()
	{
	}

	public TfPrincipal(TfIdentity identity) : base(identity)
	{
	}
}
