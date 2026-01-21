namespace WebVella.Tefter.TemplateProcessors.FileGroup.Models;

public class FileGroupTemplateResult : ITfTemplateResult
{
	public int ItemsWithErrorsCount { get => Items.Where(x=> x.Errors.Count > 0).Count(); }
	public List<FileGroupTemplateResultItem> Items { get; set; } = new();
	public List<ValidationError> Errors { get; set; } = new();
}






