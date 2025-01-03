namespace WebVella.Tefter.TemplateProcessors.TextFile.Models;

public class TextFileTemplateResultItem
{
	public string FileName { get; set; }
	public byte[] Content { get; set; }

	public List<ValidationError> Errors { get; set; } = new();
}