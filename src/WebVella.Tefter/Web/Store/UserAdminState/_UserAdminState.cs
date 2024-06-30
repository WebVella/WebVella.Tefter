namespace WebVella.Tefter.Web.Store.UserAdminState;

[FeatureState]
public record UserAdminState
{
	internal StateUseCase UseCase { get; }
	public bool IsBusy { get; init; }
	public TucUser User { get; init; }
}
