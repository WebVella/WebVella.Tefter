namespace WebVella.Tefter.Web.Store.DashboardState;

public record DashboardStateChangedAction
{
	public bool IsBusy { get; }
	//public TucDataProvider Provider { get;}

	public DashboardStateChangedAction(
		bool isBusy//,
		//TucDataProvider provider
		)
	{
		IsBusy = isBusy;
		//Provider = provider;
	}
}
