namespace WebVella.Tefter.Web.Store.ScreenState;

[FeatureState]
public record ScreenState
{
	public long SpaceViewActionsRegionTimestamp { get; init;} = 0;
	public List<ScreenRegionComponent> SpaceViewActionsRegion { get; init; } = new();

	public long SpaceViewMenuItemsRegionTimestamp { get; init; } = 0;
	public List<ScreenRegionComponent> SpaceViewMenuItemsRegion { get; init; } = new();
}
