namespace WebVella.Tefter.Web.Store.DataProviderAdminState;

public record SetDataProviderAdminAction
{
	public TucDataProvider Provider { get; }

	public SetDataProviderAdminAction(TucDataProvider provider)
	{
		Provider = provider;
	}
}
