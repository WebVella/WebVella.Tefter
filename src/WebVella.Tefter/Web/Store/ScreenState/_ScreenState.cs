namespace WebVella.Tefter.Web.Store.ScreenState;

[FeatureState]
public record ScreenState
{
	public bool SidebarExpanded { get; init; } = true;
}
