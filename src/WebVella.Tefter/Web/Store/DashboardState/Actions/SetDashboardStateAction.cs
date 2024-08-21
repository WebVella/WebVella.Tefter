namespace WebVella.Tefter.Web.Store.DashboardState;

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
