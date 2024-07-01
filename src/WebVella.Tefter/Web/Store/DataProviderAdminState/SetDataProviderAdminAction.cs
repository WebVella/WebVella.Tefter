namespace WebVella.Tefter.Web.Store.DataProviderAdminState;

public record SetDataProviderAdminAction
{
	public bool IsBusy { get; }
	public TucDataProvider Provider { get; }

	public SetDataProviderAdminAction(
		bool isBusy,
		TucDataProvider provider
		)
	{
		IsBusy = isBusy;
		Provider = provider;
	}
}
