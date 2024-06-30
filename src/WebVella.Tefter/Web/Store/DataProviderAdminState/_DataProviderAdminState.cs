namespace WebVella.Tefter.Web.Store.DataProviderAdminState;

[FeatureState]
public record DataProviderAdminState
{
	internal StateUseCase UseCase { get; }
	public bool IsBusy { get; init; }
	public TfDataProvider Provider { get; init; }
}
