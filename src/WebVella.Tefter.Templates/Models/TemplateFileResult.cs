namespace WebVella.Tefter.Templates.Models;

public interface TemplateFileResult : ITemplateResult
{
	public List<TemplateFileResultItem> Items { get; }
}

public class TemplateFileResultItem
{
	public string FileName { get; set; }
	public byte[] Content { get; set; }
}

public class TemplateFileSettings
{
	public string FileName { get; set; }
	public Guid TemplateFileBlobId { get; set; }
	public string GroupBy { get; set; }
}