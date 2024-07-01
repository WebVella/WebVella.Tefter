namespace WebVella.Tefter.Web.Store.DataProviderAdminState;

public record EmptyDataProviderAdminAction
{
	public bool IsBusy { get; }
	public TucDataProvider Provider { get; }

	public EmptyDataProviderAdminAction()
	{
		IsBusy = false;
		Provider = null;
	}
}
