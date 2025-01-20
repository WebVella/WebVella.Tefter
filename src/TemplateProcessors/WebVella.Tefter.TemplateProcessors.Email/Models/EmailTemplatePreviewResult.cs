namespace WebVella.Tefter.TemplateProcessors.Email.Models;

public class EmailTemplatePreviewResult : ITfTemplatePreviewResult
{
	public int ItemWithErrorsCount { get => Items.Where(x=> x.Errors.Count > 0).Count(); }
	public List<EmailTemplateResultItem> Items { get; set; } = new();
	public List<ValidationError> Errors { get; set; } = new();
}
