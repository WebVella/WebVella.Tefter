namespace WebVella.Tefter.Email.Models;

public class EmailMessage
{
	public Guid Id { get; internal set; }
	public EmailAddress Sender { get; internal set; }
	public List<EmailAddress> Recipients { get; internal set; }
	public List<EmailAddress> RecipientsCc { get; internal set; }
	public List<EmailAddress> RecipientsBcc { get; internal set; }
	public string ReplyToEmail { get; internal set; }
	public string Subject { get; internal set; }
	public string ContentText { get; internal set; }
	public string ContentHtml { get; internal set; }
	public DateTime CreatedOn { get; internal set; }
	public DateTime? SentOn { get; internal set; }
	public EmailStatus Status { get; internal set; }
	public EmailPriority Priority { get; internal set; }
	public string ServerError { get; internal set; }
	public DateTime? ScheduledOn { get; internal set; }
	public int RetriesCount { get; internal set; }
	public List<EmailAttachment> Attachments { get; internal set; } = new();
	public string XSearch { get; internal set; }
}

public class CreateEmailMessageModel
{
	public EmailAddress Sender { get; set; }
	public string Subject { get; set; }
	public string TextBody { get; set; }
	public string HtmlBody { get; set; }
	public List<EmailAddress> Recipients { get; set; } = new();
	public List<EmailAddress> RecipientsCc { get; set; } = new();
	public List<EmailAddress> RecipientsBcc { get; set; } = new();
	public string ReplyTo { get; set; } = null;
	public EmailPriority Priority { get; set; } = EmailPriority.Normal;
	public List<CreateEmailAttachmentModel> Attachments { get; set; } = new();
}