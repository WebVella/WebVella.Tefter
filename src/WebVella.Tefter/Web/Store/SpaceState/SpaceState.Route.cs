namespace WebVella.Tefter.Web.Store.SpaceState;

public partial record SpaceState
{
	public Guid? RouteSpaceId { get; init; }
	public Guid? RouteSpaceDataId { get; init; }
	public Guid? RouteSpaceViewId { get; init; }
	public Guid? RouteUserId { get; init; }

	public string RouteHashId
	{
		get
		{
			var sb = new StringBuilder();
			sb.Append(RouteSpaceId.HasValue ? RouteSpaceId.Value.ToString() : "null");
			sb.Append(RouteSpaceDataId.HasValue ? RouteSpaceDataId.Value.ToString() : "null");
			sb.Append(RouteSpaceViewId.HasValue ? RouteSpaceViewId.Value.ToString() : "null");
			sb.Append(RouteUserId.HasValue ? RouteUserId.Value.ToString() : "null");
			return sb.ToString();
		}
	}

}
