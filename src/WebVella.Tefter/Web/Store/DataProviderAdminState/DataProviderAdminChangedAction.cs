﻿namespace WebVella.Tefter.Web.Store.DataProviderAdminState;

public record DataProviderAdminChangedAction
{
	public bool IsBusy { get; }
	public TucDataProvider Provider { get;}

	public DataProviderAdminChangedAction(
		bool isBusy,
		TucDataProvider provider)
	{
		IsBusy = isBusy;
		Provider = provider;
	}
}
