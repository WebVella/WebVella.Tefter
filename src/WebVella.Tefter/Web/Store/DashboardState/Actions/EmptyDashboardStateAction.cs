namespace WebVella.Tefter.Web.Store.DashboardState;

public record EmptyDashboardStateAction
{
	public bool IsBusy { get; }
	//public TucDataProvider Provider { get; }

	public EmptyDashboardStateAction()
	{
		IsBusy = false;
		//Provider = null;
	}
}
