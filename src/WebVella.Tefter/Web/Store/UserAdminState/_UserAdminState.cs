namespace WebVella.Tefter.Web.Store.UserAdminState;

[FeatureState]
public record UserAdminState
{
	public bool IsBusy { get; init; }
	public TucUser User { get; init; }
}
