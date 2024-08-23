namespace WebVella.Tefter.Web.Store.UserState;

[FeatureState]
public record UserState
{
	public bool IsBusy { get; init; } = true;
	public TucUser User { get; init; }
	public List<TucSpace> UserSpaces { get; init; } = new();

}
