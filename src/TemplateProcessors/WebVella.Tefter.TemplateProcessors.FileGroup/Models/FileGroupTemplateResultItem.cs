namespace WebVella.Tefter.TemplateProcessors.FileGroup.Models;

public class FileGroupTemplateResultItem
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public string FileName { get; set; }
	public Guid? BlobId { get; set; } = null;
	public string DownloadUrl { get; set; }	
	public List<Guid> RelatedRowIds { get; set; } = new();
	public List<FileGroupTemplateResultItemAttachment> Attachments { get; set; } = new();
	public List<ValidationError> Errors { get; set; } = new();
}
