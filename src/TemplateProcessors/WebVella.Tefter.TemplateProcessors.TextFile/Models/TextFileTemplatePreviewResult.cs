namespace WebVella.Tefter.TemplateProcessors.TextFile.Models;

public class TextFileTemplatePreviewResult : ITfTemplatePreviewResult
{
	public List<TextFileTemplateResultItem> Items { get; set; } = new();
	public List<ValidationError> Errors { get; set; } = new();
}