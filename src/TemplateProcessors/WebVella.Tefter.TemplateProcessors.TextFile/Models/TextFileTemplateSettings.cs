namespace WebVella.Tefter.TemplateProcessors.TextFile.Models;

public class TextFileTemplateSettings
{
	public string FileName { get; set; }
	public Guid? TemplateFileBlobId { get; set; }
	public List<string> GroupBy { get; set; } = new();
}