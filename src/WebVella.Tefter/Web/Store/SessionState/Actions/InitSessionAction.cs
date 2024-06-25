namespace WebVella.Tefter.Web.Store.SessionState;

public record InitSessionAction
{

	public Guid? RouteSpaceId { get; init; }
	public Guid? RouteSpaceDataId { get; init; }
	public Guid? RouteSpaceViewId { get; init; }
	public UserSession UserSession { get; }

	public InitSessionAction(Guid? spaceId, Guid? spaceDataId, Guid? spaceViewId,
		UserSession userSession)
	{
		RouteSpaceId = spaceId;
		RouteSpaceDataId = spaceDataId;
		RouteSpaceViewId = spaceViewId;
		UserSession = userSession;
	}
}
