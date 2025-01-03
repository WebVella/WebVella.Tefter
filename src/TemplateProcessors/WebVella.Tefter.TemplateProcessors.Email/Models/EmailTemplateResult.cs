namespace WebVella.Tefter.TemplateProcessors.Email.Models;

public class EmailTemplateResult : ITfTemplateResult
{
	public List<EmailTemplateResultItem> Items { get; set; } = new();
	public List<ValidationError> Errors { get; set; } = new();
}






