using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.Templates.Models;

public class EmailContentProcessorSettings
{
	[Required]
	public string Subject { get; set; }
}
