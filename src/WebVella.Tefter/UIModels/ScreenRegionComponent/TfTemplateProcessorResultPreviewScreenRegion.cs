namespace WebVella.Tefter.Models;

/// <summary>
/// Context need to be included in the ScanAndRegisterRegionComponents method as a case in order to be discovered
/// </summary>
public class TfTemplateProcessorResultPreviewScreenRegion : TfBaseScreenRegion
{
	public TfTemplate Template { get; set; } = null!;
	public List<Guid> SelectedRowIds { get; set; } = new();
	public List<Guid> RelatedDatasetIds { get; set; } = new();
	public List<Guid> RelatedSpaceIds { get; set; } = new();
	public TfDataset SpaceData { get; set; } = null!;
	public Guid SessionId { get; set; }
	public TfUser User { get; set; } = null!;
	public string? CustomSettingsJson { get; set; } = null;
	public EventCallback<string> CustomSettingsJsonChanged { get; set; }
	public EventCallback<ITfTemplatePreviewResult> PreviewResultChanged { get; set; }
	public Func<List<ValidationError>> ValidatePreviewResult { get; set; } = null!;
}
