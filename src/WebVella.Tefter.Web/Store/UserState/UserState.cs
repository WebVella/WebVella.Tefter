namespace WebVella.Tefter.Web.Store.UserState;

[FeatureState]
public record UserState
{
	public bool Loading { get; init; } = true;
	public User User { get; init; }
}
