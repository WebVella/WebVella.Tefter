namespace WebVella.Tefter.Models;

public class TfTemplateProcessorResultComponentContext : TfBaseComponentContext
{
	public TucTemplate Template { get; set; }
	public List<Guid> SelectedRowIds { get; set; }
	public TucSpaceData SpaceData { get; set; }
	public TucUser User { get; set; }
	public string CustomSettingsJson { get; set; } = null;
	public ITfTemplatePreviewResult Preview { get; set; }
}
