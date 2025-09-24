namespace WebVella.Tefter.Models;

/// <summary>
/// Context need to be included in the ScanAndRegisterRegionComponents method as a case in order to be discovered
/// </summary>
public class TfTemplateProcessorResultScreenRegionContext : TfBaseScreenRegionContext
{
	public TfTemplate Template { get; set; } = default!;
	public List<Guid> SelectedRowIds { get; set; } = new();
	public TfDataSet SpaceData { get; set; } = default!;
	public TfUser User { get; set; } = default!;
	public string? CustomSettingsJson { get; set; } = null;
	public ITfTemplatePreviewResult Preview { get; set; } = default!;
}
