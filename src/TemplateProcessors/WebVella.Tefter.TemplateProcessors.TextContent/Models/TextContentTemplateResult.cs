using WebVella.Tefter.Exceptions;

namespace WebVella.Tefter.TemplateProcessors.TextContent.Models;

public class TextContentTemplateResult : ITfTemplateResult
{
	public int ItemWithErrorsCount { get => Items.Where(x => x.Errors.Count > 0).Count(); }
	public List<TextContentTemplateResultItem> Items { get; set; } = new();
	public bool IsHtml { get; set; }
	public List<ValidationError> Errors { get; set; } = new();
}