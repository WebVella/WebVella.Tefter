namespace WebVella.Tefter.Web.Store.FastAccessState;

public record SetFastAccessStateAction
{
	public bool IsBusy { get; }
	//public TucDataProvider Provider { get; }

	public SetFastAccessStateAction(
		bool isBusy//,
		//TucDataProvider provider
		)
	{
		IsBusy = isBusy;
		//Provider = provider;
	}
}
