namespace WebVella.Tefter.TemplateProcessors.Email.Models;

public class EmailTemplatePreviewResult : ITfTemplatePreviewResult
{
	public List<EmailTemplateResultItem> Items { get; } = new();
	public List<ValidationError> Errors { get; set; } = new();
}
