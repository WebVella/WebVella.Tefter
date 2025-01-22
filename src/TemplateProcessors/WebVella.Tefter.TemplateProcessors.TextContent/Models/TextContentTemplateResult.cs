namespace WebVella.Tefter.TemplateProcessors.TextContent.Models;

public class TextContentTemplateResult : ITfTemplateResult
{
	public string Content { get; set; }
	public bool IsHtml { get; set; }
	public List<ValidationError> Errors { get; set; } = new();
}