using WebVella.Tefter.Exceptions;

namespace WebVella.Tefter.TemplateProcessors.DocumentFile.Models;

public class DocumentFileTemplatePreviewResult : ITfTemplatePreviewResult
{
	public int ItemWithErrorsCount { get => Items.Where(x=> x.Errors.Count > 0).Count(); }
	public List<DocumentFileTemplateResultItem> Items { get; set; } = new();
	public List<ValidationError> Errors { get; set; } = new();
}