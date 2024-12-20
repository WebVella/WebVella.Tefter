namespace WebVella.Tefter.TemplateProcessors.ExcelFile.Models;

public class ExcelFileTemplateSettings
{
	public string FileName { get; set; }
	public Guid? TemplateFileBlobId { get; set; }
	public List<string> GroupBy { get; set; } = new();
}