﻿namespace WebVella.Tefter.Identity;

public class TfPrincipal : ClaimsPrincipal
{
	public TfPrincipal(TfIdentity identity) : base(identity)
	{
	}
}
