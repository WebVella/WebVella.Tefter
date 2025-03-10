using WebVella.Tefter.Exceptions;

namespace WebVella.Tefter.TemplateProcessors.Email.Models;

public class EmailTemplateResultItemAttachment
{
	public string FileName { get; set; }
	public Guid? BlobId { get; set; } = null;
	public string DownloadUrl { get; set; }
	public List<ValidationError> Errors { get; set; } = new();
}