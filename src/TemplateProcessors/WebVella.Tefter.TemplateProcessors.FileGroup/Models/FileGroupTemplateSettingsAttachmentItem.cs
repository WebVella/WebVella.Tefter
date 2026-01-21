namespace WebVella.Tefter.TemplateProcessors.FileGroup.Models;

public class FileGroupTemplateSettingsAttachmentItem
{
	public Guid TemplateId { get; set; }
}


public class FileGroupTemplateSettingsAttachmentItemDisplay
{
	public Guid TemplateId { get; set; }
	public TfTemplate Template { get; set; }
}
