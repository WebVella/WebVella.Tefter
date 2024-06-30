namespace WebVella.Tefter.Web.Store.UserState;

[FeatureState]
public record UserState
{
	[JsonIgnore]
	internal StateUseCase UseCase { get; init; }
	public bool Loading { get; init; } = true;
	public TucUser User { get; init; }

}
