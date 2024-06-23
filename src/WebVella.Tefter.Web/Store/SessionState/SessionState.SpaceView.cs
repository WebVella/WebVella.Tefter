namespace WebVella.Tefter.Web.Store.SessionState;

public partial record SessionState
{
	public bool IsDataLoading { get; init; }
	public string DataHashId { get; init; }
	public Space Space { get; init; }
	public SpaceData SpaceData { get; init; }
	public SpaceView SpaceView { get; init; }
	public IDictionary<Guid, SpaceData> SpaceDataDict { get; init; }
	public IDictionary<Guid, SpaceView> SpaceViewDict { get; init; }
	public List<MenuItem> SpaceNav { get; init; }
	public string SpaceColor
	{
		get
		{
			if (RouteSpaceId is null || Space is null) return ThemeColor.ToAttributeValue();

			return Space.Color.ToAttributeValue();
		}
	}
	public List<Guid> SelectedDataRows { get; init; } = new();
}
