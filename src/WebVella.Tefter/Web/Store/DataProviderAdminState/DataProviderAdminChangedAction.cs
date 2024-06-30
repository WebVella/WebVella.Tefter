namespace WebVella.Tefter.Web.Store.DataProviderAdminState;

public record DataProviderAdminChangedAction
{
	public TfDataProvider Provider { get;}

	public DataProviderAdminChangedAction(TfDataProvider provider)
	{
		Provider = provider;
	}
}
