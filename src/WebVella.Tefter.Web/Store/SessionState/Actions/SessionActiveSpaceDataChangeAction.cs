namespace WebVella.Tefter.Web.Store.SessionState;

public record SessionActiveSpaceDataChangeAction
{
	public Guid? SpaceRouteId { get; init; }
	public Guid? SpaceDataRouteId { get; init; }
	public Guid? SpaceViewRouteId { get; init; }
	public Space Space { get; }
	public SpaceData SpaceData { get; }
	public SpaceView SpaceView { get; }

	public SessionActiveSpaceDataChangeAction(Guid? spaceId, Guid? spaceDataId, Guid? spaceViewId,
		Space space, SpaceData spaceData, SpaceView spaceView)
	{
		SpaceRouteId = spaceId;
		SpaceDataRouteId = spaceDataId;
		SpaceViewRouteId = spaceViewId;
		Space = space;
		SpaceData = spaceData;
		SpaceView = spaceView;
	}
}
