namespace WebVella.Tefter.Web.Store.SpaceState;

public record SetSpaceStateAction
{
	public bool IsBusy { get; }
	public TucSpace Space { get; }
	public TucSpaceView SpaceView { get; }
	public List<TucSpaceView> SpaceViewList { get; }
	public Guid? RouteSpaceId { get; }
	public Guid? RouteSpaceViewId { get; }

	internal SetSpaceStateAction(
		bool isBusy,
		TucSpace space,
		TucSpaceView spaceView,
		List<TucSpaceView> spaceViewList,
		Guid? routeSpaceId,
		Guid? routeSpaceViewId
		)
	{
		IsBusy = isBusy;
		Space = space;
		SpaceView = spaceView;
		SpaceViewList = spaceViewList;
		RouteSpaceId = routeSpaceId;
		RouteSpaceViewId = routeSpaceViewId;
	}
}
