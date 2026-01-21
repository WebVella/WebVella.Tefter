namespace WebVella.Tefter.TemplateProcessors.FileGroup.Models;

public class FileGroupTemplateSettings
{
	public List<string> GroupBy { get; set; } = new();
	public List<FileGroupTemplateSettingsAttachmentItem> AttachmentItems { get; set; } = new();
}