namespace WebVella.Tefter.Web.Store.ScreenState;

public class RemoveComponentToRegionAction
{
	public Guid ScreenComponentId { get; }
	public ScreenRegionType Region { get; } = ScreenRegionType.SpaceViewActions;

	public RemoveComponentToRegionAction(Guid componentId, ScreenRegionType region)
	{
        ScreenComponentId = componentId;
        Region = region;
	}
}
