namespace WebVella.Tefter.Web.Store;

public record EmptyFastAccessStateAction
{
	public bool IsBusy { get; }
	//public TucDataProvider Provider { get; }

	public EmptyFastAccessStateAction()
	{
		IsBusy = false;
		//Provider = null;
	}
}
