namespace WebVella.Tefter.Web.Store.DataProviderDetailsState;

public record DataProviderDetailsChangedAction
{
	public TfDataProvider Provider { get;}

	public DataProviderDetailsChangedAction(TfDataProvider provider)
	{
		Provider = provider;
	}
}
