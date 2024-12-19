namespace WebVella.Tefter.TemplateProcessors.Email.Models;

public class EmailTemplateSettings
{
	public string Sender { get; set; }
	public string Recipients { get; set; }
	public string CcRecipients { get; set; }
	public string BccRecipients { get; set; }
	public string Subject { get; set; }
	public string HtmlContent { get; set; }
	public string TextContent { get; set; }
	public List<string> GroupBy { get; set; } = new();
	public List<EmailTemplateSettingsAttachmentItem> AttachmentItems { get; set; } = new();
}