using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.Web.Models;

public record TucFileForm
{
	public Guid? Id { get; set; }
	[Required]
	public Guid? CreatedBy { get; set; }
	public string Name { get; set; }
	public string FilePath { get; set; }
	[Required]
	public string LocalFilePath { get; set; }
	
}
