namespace WebVella.Tefter.TemplateProcessors.ExcelFile.Models;

public interface ExcelFileTemplateResult : ITfTemplateResult
{
	public List<ExcelFileTemplateResultItem> Items { get; }
}