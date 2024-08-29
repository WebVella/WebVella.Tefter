namespace WebVella.Tefter.Web.Store.SpaceState;

public record SetSpaceDataAction
{
	public TucSpaceData SpaceData { get; }
	public List<TucSpaceData> SpaceDataList { get; }

	internal SetSpaceDataAction(
		TucSpaceData spaceData,
		List<TucSpaceData> spaceDataList
		)
	{
		SpaceData = spaceData;
		SpaceDataList = spaceDataList;
	}
}
