namespace WebVella.Tefter.Web.Store;
using SystemColor = System.Drawing.Color;

public partial record TfAppState
{
	public List<TucSpaceData> SpaceDataList { get; init; } = new();
	public TucSpaceData SpaceData { get; init; }



}
