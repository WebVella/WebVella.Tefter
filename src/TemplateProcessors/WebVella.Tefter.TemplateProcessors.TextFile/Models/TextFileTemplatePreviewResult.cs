namespace WebVella.Tefter.TemplateProcessors.TextFile.Models;

public class TextFileTemplatePreviewResult : ITfTemplatePreviewResult
{
	public int ItemWithErrorsCount { get => Items.Where(x => x.Errors.Count > 0).Count(); }
	public List<TextFileTemplateResultItem> Items { get; set; } = new();
	public List<ValidationError> Errors { get; set; } = new();
}