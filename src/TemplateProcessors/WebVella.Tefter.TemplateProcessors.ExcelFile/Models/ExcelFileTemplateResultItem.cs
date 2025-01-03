namespace WebVella.Tefter.TemplateProcessors.ExcelFile.Models;

public class ExcelFileTemplateResultItem
{
	public string FileName { get; set; }
	public byte[] Content { get; set; }
	public List<ValidationError> Errors { get; set; } = new();
}