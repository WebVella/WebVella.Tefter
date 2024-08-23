namespace WebVella.Tefter.Web.Store.SpaceState;

public record SetSpaceAction
{
	public bool IsBusy { get; }
	public TucSpace Space { get; }
	public Guid? RouteSpaceId { get; }
	public Guid? RouteSpaceDataId { get; }
	public Guid? RouteSpaceViewId { get; }

	internal SetSpaceAction(
		bool isBusy,
		TucSpace space,
		Guid? routeSpaceId,
		Guid? routeSpaceDataId,
		Guid? routeSpaceViewId
		)
	{
		IsBusy = isBusy;
		Space = space;
		RouteSpaceId = routeSpaceId;
		RouteSpaceDataId = routeSpaceDataId;
		RouteSpaceViewId = routeSpaceViewId;
	}
}
