namespace WebVella.Tefter.TemplateProcessors.TextContent.Models;

public class TextContentTemplateResult : ITfTemplateResult
{
	public List<string> Content { get; set; } = new();
	public bool IsHtml { get; set; }
	public List<ValidationError> Errors { get; set; } = new();
}