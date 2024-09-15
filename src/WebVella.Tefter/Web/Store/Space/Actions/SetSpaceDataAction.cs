namespace WebVella.Tefter.Web.Store;

public record SetSpaceDataAction : TfBaseAction
{
	public TucSpaceData SpaceData { get; }
	public List<TucSpaceData> SpaceDataList { get; }

	internal SetSpaceDataAction(
		TfBaseComponent component,
		TucSpaceData spaceData,
		List<TucSpaceData> spaceDataList
		)
	{
		Component = component;
		SpaceData = spaceData;
		SpaceDataList = spaceDataList;
	}
}
