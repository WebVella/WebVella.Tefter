namespace WebVella.Tefter.Models;

/// <summary>
/// Context need to be included in the ScanAndRegisterRegionComponents method as a case in order to be discovered
/// </summary>
public class TfTemplateProcessorResultPreviewScreenRegionContext : TfBaseScreenRegionContext
{
	public TfTemplate Template { get; set; } = default!;
	public List<Guid> SelectedRowIds { get; set; } = new();
	public TfDataSet SpaceData { get; set; } = default!;
	public TfUser User { get; set; } = default!;
	public string? CustomSettingsJson { get; set; } = null;
	public EventCallback<string> CustomSettingsJsonChanged { get; set; }
	public EventCallback<ITfTemplatePreviewResult> PreviewResultChanged { get; set; }
	public Func<List<ValidationError>> ValidatePreviewResult { get; set; } = default!;
}
