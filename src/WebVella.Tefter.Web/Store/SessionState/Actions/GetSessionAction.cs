namespace WebVella.Tefter.Web.Store.SessionState;

public record GetSessionAction
{
	public Guid UserId { get; }
	public Guid? SpaceId { get; }
	public Guid? SpaceDataId { get; }
	public Guid? SpaceViewId { get; }

	public GetSessionAction(Guid userId, Guid? spaceId,
	Guid? spaceDataId, Guid? spaceViewId)
	{
		UserId = userId;
		SpaceId = spaceId;
		SpaceDataId = spaceDataId;
		SpaceViewId = spaceViewId;
	}
}
