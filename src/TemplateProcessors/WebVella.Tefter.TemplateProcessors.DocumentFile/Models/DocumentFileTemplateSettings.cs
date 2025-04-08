namespace WebVella.Tefter.TemplateProcessors.DocumentFile.Models;

public class DocumentFileTemplateSettings
{
	public string? FileName { get; set; }
	public Guid? TemplateFileBlobId { get; set; }
	public List<string> GroupBy { get; set; } = new();
}