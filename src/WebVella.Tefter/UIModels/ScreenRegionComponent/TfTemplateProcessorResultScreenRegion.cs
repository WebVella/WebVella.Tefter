namespace WebVella.Tefter.Models;

/// <summary>
/// Context need to be included in the ScanAndRegisterRegionComponents method as a case in order to be discovered
/// </summary>
public class TfTemplateProcessorResultScreenRegion : TfBaseScreenRegion
{
	public TfTemplate Template { get; set; } = null!;
	public List<Guid> SelectedRowIds { get; set; } = new();
	public TfDataset SpaceData { get; set; } = null!;
	public TfUser User { get; set; } = null!;
	public string? CustomSettingsJson { get; set; } = null;
	public ITfTemplatePreviewResult Preview { get; set; } = null!;
}
