namespace WebVella.Tefter.Web.Store.SpaceState;

public record SpaceStateChangedAction
{
	public bool IsBusy { get; }
	//public TucDataProvider Provider { get;}

	public SpaceStateChangedAction(
		bool isBusy//,
		//TucDataProvider provider
		)
	{
		IsBusy = isBusy;
		//Provider = provider;
	}
}
