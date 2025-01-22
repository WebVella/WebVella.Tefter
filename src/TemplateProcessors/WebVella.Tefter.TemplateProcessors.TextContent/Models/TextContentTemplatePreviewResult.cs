namespace WebVella.Tefter.TemplateProcessors.TextContent.Models;

public class TextContentTemplatePreviewResult : ITfTemplatePreviewResult
{
	public string Content { get; set; }
	public bool IsHtml { get; set; }
	public List<ValidationError> Errors { get; set; } = new();
}