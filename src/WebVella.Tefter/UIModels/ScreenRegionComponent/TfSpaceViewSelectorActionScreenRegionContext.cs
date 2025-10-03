namespace WebVella.Tefter.Models;

/// <summary>
/// Context need to be included in the ScanAndRegisterRegionComponents method as a case in order to be discovered
/// </summary>
public class TfSpaceViewSelectorActionScreenRegionContext : TfBaseScreenRegionContext
{
	public List<Guid> SelectedDataRows { get; set; } = new();
	public TfDataset SpaceData { get; set; } = null!;
	public TfUser CurrentUser { get; set; } = null!;
}
