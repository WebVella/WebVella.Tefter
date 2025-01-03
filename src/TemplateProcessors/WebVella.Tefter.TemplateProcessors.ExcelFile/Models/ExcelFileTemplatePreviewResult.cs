namespace WebVella.Tefter.TemplateProcessors.ExcelFile.Models;

public class ExcelFileTemplatePreviewResult : ITfTemplatePreviewResult
{
	public List<ExcelFileTemplateResultItem> Items { get; set; } = new();
	public List<ValidationError> Errors { get; set; } = new();
}