namespace WebVella.Tefter.TemplateProcessors.Email.Models;

public interface EmailTemplateResult : ITfTemplateResult
{
	public List<EmailTemplateResultItem> Items { get; }
}






