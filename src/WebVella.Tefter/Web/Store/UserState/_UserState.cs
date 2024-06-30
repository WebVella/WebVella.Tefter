namespace WebVella.Tefter.Web.Store.UserState;

[FeatureState]
public record UserState
{
	public bool Loading { get; init; } = true;
	public TucUser User { get; init; }

}
