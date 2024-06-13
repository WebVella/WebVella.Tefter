namespace WebVella.Tefter.Web.Store.UserState;

[FeatureState]
public record UserState
{
	public bool IsLoading { get; init; } = true;
	public User User { get; init; }
}
