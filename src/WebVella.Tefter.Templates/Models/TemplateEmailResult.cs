namespace WebVella.Tefter.Templates.Models;

public interface TemplateEmailResult : TemplateResult
{
	public List<TfTemplateEmailResultItem> Items { get; }
}

public class TfTemplateEmailResultItem
{
	public string Sender { get; set; }
	public List<string> Recipients { get; set; } = new();
	public List<string> CcRecipients { get; set; } = new();
	public List<string> BccRecipients { get; set; } = new();
	public string Subject { get; set; }
	public string HtmlContent { get; set; }
	public string TextContent { get; set; }
	public List<TfTemplateEmailResultItemAttachment> Attachments { get; set; } = new();
}

public class TfTemplateEmailResultItemAttachment
{
	public string FileName { get; set; }
	public byte[] Content { get; set; }
}