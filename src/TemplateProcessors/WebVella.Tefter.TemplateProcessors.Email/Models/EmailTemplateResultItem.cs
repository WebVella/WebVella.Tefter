namespace WebVella.Tefter.TemplateProcessors.Email.Models;

public class EmailTemplateResultItem
{
	public string Sender { get; set; }
	public List<string> Recipients { get; set; } = new();
	public List<string> CcRecipients { get; set; } = new();
	public List<string> BccRecipients { get; set; } = new();
	public string Subject { get; set; }
	public string HtmlContent { get; set; }
	public string TextContent { get; set; }
	public List<EmailTemplateResultItemAttachment> Attachments { get; set; } = new();
}
