using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.TemplateProcessors.Email.Models;

public class EmailTemplateSettings
{
	[Required]
	public string Sender { get; set; }
	[Required]
	public string Recipients { get; set; }
	public string CcRecipients { get; set; }
	public string BccRecipients { get; set; }
	[Required]
	public string Subject { get; set; }
	[Required]
	public string HtmlContent { get; set; }
	//[Required]
	public string TextContent { get; set; }
	public List<string> GroupBy { get; set; } = new();
	public List<EmailTemplateSettingsAttachmentItem> AttachmentItems { get; set; } = new();
}