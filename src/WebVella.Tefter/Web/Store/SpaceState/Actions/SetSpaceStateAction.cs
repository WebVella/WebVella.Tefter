namespace WebVella.Tefter.Web.Store.SpaceState;

public record SetSpaceStateAction
{
	public bool IsBusy { get; }
	public TucSpace Space { get; }
	public TucSpaceData SpaceData { get; }
	public TucSpaceView SpaceView { get; }
	public Guid? RouteSpaceId { get; }
	public Guid? RouteSpaceDataId { get; }
	public Guid? RouteSpaceViewId { get; }

	internal SetSpaceStateAction(
		bool isBusy,
		TucSpace space,
		TucSpaceData spaceData,
		TucSpaceView spaceView,
		Guid? routeSpaceId,
		Guid? routeSpaceDataId,
		Guid? routeSpaceViewId
		)
	{
		IsBusy = isBusy;
		Space = space;
		SpaceData = spaceData;
		SpaceView = spaceView;
		RouteSpaceId = routeSpaceId;
		RouteSpaceDataId = routeSpaceDataId;
		RouteSpaceViewId = routeSpaceViewId;
	}
}
