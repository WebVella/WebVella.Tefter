namespace WebVella.Tefter.TemplateProcessors.TextFile.Models;

public interface TextFileTemplateResult : ITfTemplateResult
{
	public List<TextFileTemplateResultItem> Items { get; }
}