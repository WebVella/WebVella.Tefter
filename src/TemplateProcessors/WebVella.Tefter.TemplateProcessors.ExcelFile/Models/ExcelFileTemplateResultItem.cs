using WebVella.Tefter.Exceptions;

namespace WebVella.Tefter.TemplateProcessors.ExcelFile.Models;

public class ExcelFileTemplateResultItem
{
	public string FileName { get; set; }
	public Guid? BlobId { get; set; } = null;
	public string DownloadUrl { get; set; }
	public int NumberOfRows { get; set; }
	public List<ValidationError> Errors { get; set; } = new();
}