namespace WebVella.Tefter.Templates.Models;

public interface TemplateFileResult : TemplateResult
{
	public List<TfTemplateFileResultItem> Items { get; }
}

public class TfTemplateFileResultItem
{
	public string FileName { get; set; }
	public byte[] Content { get; set; }
}