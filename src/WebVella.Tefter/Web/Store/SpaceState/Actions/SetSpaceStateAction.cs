namespace WebVella.Tefter.Web.Store.SpaceState;

public record SetSpaceStateAction
{
	public bool IsBusy { get; }
	public TucSpace Space { get; }
	public TucSpaceData SpaceData { get; }
	public List<TucSpaceData> SpaceDataList { get; }
	public TucSpaceView SpaceView { get; }
	public List<TucSpaceView> SpaceViewList { get; }
	public Guid? RouteSpaceId { get; }
	public Guid? RouteSpaceDataId { get; }
	public Guid? RouteSpaceViewId { get; }

	internal SetSpaceStateAction(
		bool isBusy,
		TucSpace space,
		TucSpaceData spaceData,
		List<TucSpaceData> spaceDataList,
		TucSpaceView spaceView,
		List<TucSpaceView> spaceViewList,
		Guid? routeSpaceId,
		Guid? routeSpaceDataId,
		Guid? routeSpaceViewId
		)
	{
		IsBusy = isBusy;
		Space = space;
		SpaceData = spaceData;
		SpaceDataList = spaceDataList;
		SpaceView = spaceView;
		SpaceViewList = spaceViewList;
		RouteSpaceId = routeSpaceId;
		RouteSpaceDataId = routeSpaceDataId;
		RouteSpaceViewId = routeSpaceViewId;
	}
}
