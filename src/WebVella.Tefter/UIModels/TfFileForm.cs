using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.Models;

public record TfFileForm
{
	public Guid? Id { get; set; }
	[Required]
	public Guid? CreatedBy { get; set; }
	[Required]
	public string? Filename { get; set; }
	public string? FileExtension { get => String.IsNullOrWhiteSpace(Filename) ? null : Path.GetExtension(Filename);}
	public string? LocalFilePath { get; set; }
	
}
