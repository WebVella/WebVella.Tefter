namespace WebVella.Tefter.Web.Store.FastAccessState;

public record FastAccessStateChangedAction
{
	public bool IsBusy { get; }
	//public TucDataProvider Provider { get;}

	public FastAccessStateChangedAction(
		bool isBusy//,
		//TucDataProvider provider
		)
	{
		IsBusy = isBusy;
		//Provider = provider;
	}
}
