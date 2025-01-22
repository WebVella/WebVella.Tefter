namespace WebVella.Tefter.TemplateProcessors.TextContent.Models;

public class TextContentTemplateResultItem
{
	public string Content { get; set; }
	public int NumberOfRows { get; set; }
	public List<ValidationError> Errors { get; set; } = new();
}