using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.Templates.Models;

public class EmailTemplateProcessorSettings
{
	[Required]
	public string Subject { get; set; }
}
