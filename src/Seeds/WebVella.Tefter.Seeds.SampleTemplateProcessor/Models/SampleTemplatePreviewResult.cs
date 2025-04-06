namespace WebVella.Tefter.Seeds.SampleTemplateProcessor.Models;

public class SampleTemplatePreviewResult : ITfTemplatePreviewResult
{
	public int ItemWithErrorsCount { get => Items.Where(x => x.Errors.Count > 0).Count(); }
	public List<SampleTemplateResultItem> Items { get; set; } = new();
	public bool IsHtml { get; set; }
	public List<ValidationError> Errors { get; set; } = new();
}