namespace WebVella.Tefter.TemplateProcessors.Email.Models;

public class EmailTemplateSettingsAttachmentItem
{
	public Guid TemplateId { get; set; }
}


public class EmailTemplateSettingsAttachmentItemDisplay
{
	public Guid TemplateId { get; set; }
	public TfTemplate Template { get; set; }
}
