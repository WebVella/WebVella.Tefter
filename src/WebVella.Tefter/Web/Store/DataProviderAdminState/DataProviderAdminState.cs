namespace WebVella.Tefter.Web.Store.DataProviderAdminState;

[FeatureState]
public record DataProviderAdminState
{
	public bool IsBusy { get; init; }
	public TucDataProvider Provider { get; init; }
}
