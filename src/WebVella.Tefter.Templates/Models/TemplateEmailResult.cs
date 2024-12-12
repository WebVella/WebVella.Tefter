namespace WebVella.Tefter.Templates.Models;

public interface TemplateEmailResult : ITemplateResult
{
	public List<TemplateEmailResultItem> Items { get; }
}

public class TemplateEmailResultItem
{
	public string Sender { get; set; }
	public List<string> Recipients { get; set; } = new();
	public List<string> CcRecipients { get; set; } = new();
	public List<string> BccRecipients { get; set; } = new();
	public string Subject { get; set; }
	public string HtmlContent { get; set; }
	public string TextContent { get; set; }
	public List<TemplateEmailResultItemAttachment> Attachments { get; set; } = new();
}

public class TemplateEmailResultItemAttachment
{
	public string FileName { get; set; }
	public byte[] Content { get; set; }
}

public class TemplateEmailSettings
{
	public string Sender { get; set; }
	public string Recipients { get; set; }
	public string CcRecipients { get; set; }
	public string BccRecipients { get; set; }
	public string Subject { get; set; }
	public string HtmlContent { get; set; }
	public string TextContent { get; set; }
	public string GroupBy { get; set; }
	public List<TemplateEmailSettingsAttachmentItem> AttachmentItems { get; set; } = new();
}

public class TemplateEmailSettingsAttachmentItem
{
	public Guid TemplateId { get; set; }
	public bool SendWithNoContent { get; set; }
}

