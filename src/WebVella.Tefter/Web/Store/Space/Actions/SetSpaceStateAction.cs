namespace WebVella.Tefter.Web.Store;

public record SetSpaceStateAction : TfBaseAction
{
	public bool IsBusy { get; }
	public TucSpace Space { get; }
	public TucSpaceView SpaceView { get; }
	public List<TucSpaceView> SpaceViewList { get; }
	public TucSpaceData SpaceData { get; }
	public List<TucSpaceData> SpaceDataList { get; }
	public Guid? RouteSpaceId { get; }
	public Guid? RouteSpaceViewId { get; }
	public Guid? RouteSpaceDataId { get; }

	internal SetSpaceStateAction(
		TfBaseComponent component,
		bool isBusy,
		TucSpace space,
		TucSpaceView spaceView,
		List<TucSpaceView> spaceViewList,
		TucSpaceData spaceData,
		List<TucSpaceData> spaceDataList,
		Guid? routeSpaceId,
		Guid? routeSpaceViewId,
		Guid? routeSpaceDataId
		)
	{
		Component = component;
		IsBusy = isBusy;
		Space = space;
		SpaceView = spaceView;
		SpaceViewList = spaceViewList;
		SpaceData = spaceData;
		SpaceDataList = spaceDataList;
		RouteSpaceId = routeSpaceId;
		RouteSpaceViewId = routeSpaceViewId;
		RouteSpaceDataId = routeSpaceDataId;
	}
}
