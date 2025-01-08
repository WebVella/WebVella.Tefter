namespace WebVella.Tefter.TemplateProcessors.ExcelFile.Models;

public class ExcelFileTemplateResult : ITfTemplateResult
{
	public List<ExcelFileTemplateResultItem> Items { get; set; } = new();
	public List<ValidationError> Errors { get; set; } = new();

	public string ZipDownloadUrl { get; set; } = null;
}