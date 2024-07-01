namespace WebVella.Tefter.Web.Store.DataProviderAdminState;

public record DataProviderAdminChangedAction
{
	public TucDataProvider Provider { get;}

	public DataProviderAdminChangedAction(TucDataProvider provider)
	{
		Provider = provider;
	}
}
