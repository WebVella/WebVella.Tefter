namespace WebVella.Tefter.Web.Store;

public record SetDashboardStateAction
{
	public bool IsBusy { get; }
	//public TucDataProvider Provider { get; }

	public SetDashboardStateAction(
		bool isBusy//,
		//TucDataProvider provider
		)
	{
		IsBusy = isBusy;
		//Provider = provider;
	}
}
