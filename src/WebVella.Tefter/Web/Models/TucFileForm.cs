using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.Web.Models;

public record TucFileForm
{
	public Guid? Id { get; set; }
	[Required]
	public Guid? CreatedBy { get; set; }
	[Required]
	public string FileName { get; set; }
	public string LocalFilePath { get; set; }
	
}
