using WebVella.Tefter.Exceptions;

namespace WebVella.Tefter.TemplateProcessors.DocumentFile.Models;

public class DocumentFileTemplateResult : ITfTemplateResult
{
	public int ItemsWithErrorsCount { get => Items.Where(x=> x.Errors.Count > 0).Count(); }
	public List<DocumentFileTemplateResultItem> Items { get; set; } = new();
	public List<ValidationError> Errors { get; set; } = new();

	public string? ZipFilename { get; set; } = null;
	public Guid? ZipBlobId { get; set; } = null;
	public string? ZipDownloadUrl { get; set; } = null;
}