namespace WebVella.Tefter.Web.Store.SpaceState;

public record EmptySpaceStateAction
{
	public bool IsBusy { get; }
	//public TucDataProvider Provider { get; }

	public EmptySpaceStateAction()
	{
		IsBusy = false;
		//Provider = null;
	}
}
