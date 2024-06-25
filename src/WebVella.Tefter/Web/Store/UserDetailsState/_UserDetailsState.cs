namespace WebVella.Tefter.Web.Store.UserDetailsState;

[FeatureState]
public record UserDetailsState
{
	public bool IsBusy { get; init; }
	public User User { get; init; }
}
