namespace WebVella.Tefter.Web.Store.SessionState;

[FeatureState]
public partial record SessionState
{
	public Guid UserId { get; init; }
	public bool IsBusy { get; init; } = true;

}
