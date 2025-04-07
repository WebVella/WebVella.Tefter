namespace WebVella.Tefter.Seeds.SampleTemplateProcessor.Models;
public class SampleTemplateResult : ITfTemplateResult
{
	public int ItemWithErrorsCount { get => Items.Where(x => x.Errors.Count > 0).Count(); }
	public List<SampleTemplateResultItem> Items { get; set; } = new();
	public List<ValidationError> Errors { get; set; } = new();
}