namespace WebVella.Tefter.TemplateProcessors.Email.Models;

public class EmailTemplateResult : ITfTemplateResult
{
	public int ItemsWithErrorsCount { get => Items.Where(x=> x.Errors.Count > 0).Count(); }
	public List<EmailTemplateResultItem> Items { get; set; } = new();
	public List<ValidationError> Errors { get; set; } = new();
}






