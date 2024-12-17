namespace WebVella.Tefter.TemplateProcessors.TextFile.Models;

public class TextFileTemplateSettings
{
	public string FileName { get; set; }
	public Guid TemplateFileBlobId { get; set; }
	public string GroupBy { get; set; }
}