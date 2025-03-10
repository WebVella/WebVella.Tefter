using WebVella.Tefter.Exceptions;

namespace WebVella.Tefter.TemplateProcessors.TextContent.Models;

public class TextContentTemplateResultItem
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public string Content { get; set; }
	public int NumberOfRows { get; set; }
	public List<ValidationError> Errors { get; set; } = new();
}