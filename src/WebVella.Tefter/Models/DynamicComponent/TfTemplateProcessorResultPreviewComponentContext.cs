namespace WebVella.Tefter.Models;

/// <summary>
/// Context need to be included in the ScanAndRegisterDynamicComponents method as a case in order to be discovered
/// </summary>
public class TfTemplateProcessorResultPreviewComponentContext : TfBaseRegionScopedComponentContext<ITfTemplateProcessor>
{
	public TucTemplate Template { get; set; }
	public List<Guid> SelectedRowIds { get; set; }
	public TucSpaceData SpaceData { get; set; }
	public TucUser User { get; set; }
	public string CustomSettingsJson { get; set; } = null;
	public EventCallback<string> CustomSettingsJsonChanged { get; set; }
	public EventCallback<ITfTemplatePreviewResult> PreviewResultChanged { get; set; }
	public Func<List<ValidationError>> ValidatePreviewResult { get; set; }
}
