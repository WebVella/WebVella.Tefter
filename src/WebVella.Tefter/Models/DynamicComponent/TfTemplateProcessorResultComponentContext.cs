namespace WebVella.Tefter.Models;

/// <summary>
/// Context need to be included in the ScanAndRegisterRegionComponents method as a case in order to be discovered
/// </summary>
public class TfTemplateProcessorResultComponentContext : TfBaseRegionScopedComponentContext<ITfTemplateProcessor>
{
	public TucTemplate Template { get; set; }
	public List<Guid> SelectedRowIds { get; set; }
	public TucSpaceData SpaceData { get; set; }
	public TucUser User { get; set; }
	public string CustomSettingsJson { get; set; } = null;
	public ITfTemplatePreviewResult Preview { get; set; }
}
