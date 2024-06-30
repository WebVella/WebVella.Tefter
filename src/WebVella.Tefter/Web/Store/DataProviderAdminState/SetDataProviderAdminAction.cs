namespace WebVella.Tefter.Web.Store.DataProviderAdminState;

public record SetDataProviderAdminAction
{
	public TfDataProvider Provider { get; }

	public SetDataProviderAdminAction(TfDataProvider provider)
	{
		Provider = provider;
	}
}
