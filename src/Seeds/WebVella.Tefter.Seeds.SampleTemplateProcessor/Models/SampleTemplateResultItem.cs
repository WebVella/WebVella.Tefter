namespace WebVella.Tefter.Seeds.SampleTemplateProcessor.Models;

public class SampleTemplateResultItem
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public string Content { get; set; }
	public int NumberOfRows { get; set; }
	public List<ValidationError> Errors { get; set; } = new();
}