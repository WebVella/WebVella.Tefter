namespace WebVella.Tefter.TemplateProcessors.FileGroup.Models;

public class FileGroupTemplateResultItemAttachment
{
	public Guid TemplateId { get; set; }
	public string FileName { get; set; }
	public Guid? BlobId { get; set; } = null;
	public string DownloadUrl { get; set; }	
	public List<ValidationError> Errors { get; set; } = new();
}