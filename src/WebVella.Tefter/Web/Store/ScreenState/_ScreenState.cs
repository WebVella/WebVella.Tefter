namespace WebVella.Tefter.Web.Store.ScreenState;

[FeatureState]
public record ScreenState
{
	[JsonIgnore]
	internal StateUseCase UseCase { get; init; }
	public bool SidebarExpanded { get; init; } = true;
}
