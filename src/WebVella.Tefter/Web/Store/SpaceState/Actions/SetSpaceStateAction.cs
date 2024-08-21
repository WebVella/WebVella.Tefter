namespace WebVella.Tefter.Web.Store.SpaceState;

public record SetSpaceStateAction
{
	public bool IsBusy { get; }
	//public TucDataProvider Provider { get; }

	public SetSpaceStateAction(
		bool isBusy//,
		//TucDataProvider provider
		)
	{
		IsBusy = isBusy;
		//Provider = provider;
	}
}
