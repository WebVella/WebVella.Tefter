namespace WebVella.Tefter.Web.Store.DataProviderDetailsState;

public record SetDataProviderDetailsAction
{
	public TfDataProvider Provider { get; }

	public SetDataProviderDetailsAction(TfDataProvider provider)
	{
		Provider = provider;
	}
}
