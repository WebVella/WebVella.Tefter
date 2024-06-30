﻿namespace WebVella.Tefter.Web.Store.UserAdminState;

public record SetUserAdminAction
{
	public TucUser User { get; }

	public SetUserAdminAction(TucUser user)
	{
		User = user;
	}
}
