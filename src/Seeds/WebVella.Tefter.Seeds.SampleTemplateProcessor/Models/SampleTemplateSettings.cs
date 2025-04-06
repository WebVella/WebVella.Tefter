using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.Seeds.SampleTemplateProcessor.Models;

public class SampleTemplateSettings
{
	[Required]
	public string Content { get; set; }
}