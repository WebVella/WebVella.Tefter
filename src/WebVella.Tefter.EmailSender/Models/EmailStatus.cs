namespace WebVella.Tefter.EmailSender.Models;

public enum EmailStatus
{
	[Description("pending")]
	Pending = 0,
	[Description("sent")]
	Sent = 1,
	[Description("aborted")]
	Aborted = 2
}