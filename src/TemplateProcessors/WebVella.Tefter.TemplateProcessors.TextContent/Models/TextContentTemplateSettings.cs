using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.TemplateProcessors.TextContent.Models;

public class TextContentTemplateSettings
{
	[Required]
	public string Content { get; set; }

	[Required]
	public bool IsHtml { get; set; } = false;
	public List<string> GroupBy { get; set; } = new();
}