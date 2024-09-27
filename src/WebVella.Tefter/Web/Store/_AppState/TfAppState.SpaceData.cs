namespace WebVella.Tefter.Web.Store;
using SystemColor = System.Drawing.Color;

public partial record TfAppState
{
	public List<TucSpaceData> SpaceDataList { get; init; } = new();
	public TucSpaceData SpaceData { get; init; }
	internal List<TucDataProvider> AllDataProviders { get; set; } = new();

	public TfDataTable SpaceDataData { get; init; }
	public string SpaceDataSearch { get; init; } = null;
	public int SpaceDataPage { get; init; } = 1;
	public int SpaceDataPageSize { get; init; } = TfConstants.PageSize;

}
