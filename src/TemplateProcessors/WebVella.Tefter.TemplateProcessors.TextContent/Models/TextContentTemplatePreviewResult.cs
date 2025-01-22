namespace WebVella.Tefter.TemplateProcessors.TextContent.Models;

public class TextContentTemplatePreviewResult : ITfTemplatePreviewResult
{
	public List<string> Content { get; set; } = new();
	public bool IsHtml { get; set; }
	public List<ValidationError> Errors { get; set; } = new();
}