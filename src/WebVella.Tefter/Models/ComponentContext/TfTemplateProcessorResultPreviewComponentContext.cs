namespace WebVella.Tefter.Models;

public class TfTemplateProcessorResultPreviewComponentContext
{
	public TucTemplate Template { get; set; }
	public Guid? TemplateId { get; set; }
	public List<Guid> SelectedRowIds { get; set; }
	public TucSpaceData SpaceData { get; set; }
	public TucUser User { get; set; }
	public EventCallback<ITfTemplatePreviewResult> PreviewResultChanged { get; set; }
	public EventCallback<string> SettingsJsonChanged { get; set; }
	public Func<List<ValidationError>> Validate { get; set; }
}
