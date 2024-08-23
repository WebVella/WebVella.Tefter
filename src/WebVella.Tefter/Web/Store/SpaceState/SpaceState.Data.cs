namespace WebVella.Tefter.Web.Store.SpaceState;

public partial record SpaceState
{
	internal TucSpaceData SpaceData { get; init; }
	internal List<TucSpaceData> SpaceDataList { get; init; } = new();
}
