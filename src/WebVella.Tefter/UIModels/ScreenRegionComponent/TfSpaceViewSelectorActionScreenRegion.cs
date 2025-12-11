namespace WebVella.Tefter.Models;

/// <summary>
/// Context need to be included in the ScanAndRegisterRegionComponents method as a case in order to be discovered
/// </summary>
public class TfSpaceViewSelectorActionScreenRegion : TfBaseScreenRegion
{
	public List<Guid> SelectedDataRows { get; set; } = new();
	public TfSpacePage SpacePage { get; set; } = null!;
	public TfSpaceView SpaceView { get; set; } = null!;
	public TfDataset Dataset { get; set; } = null!;
	public TfUser CurrentUser { get; set; } = null!;
	public TucSpaceViewPageContent TucSpaceViewPageContent { get; set; } = null!;
}
