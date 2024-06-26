namespace WebVella.Tefter.Web.Store.DataProviderDetailsState;

[FeatureState]
public record DataProviderDetailsState
{
	public bool IsBusy { get; init; }
	public TfDataProvider Provider { get; init; }
}
