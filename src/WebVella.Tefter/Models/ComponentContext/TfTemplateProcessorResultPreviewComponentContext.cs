namespace WebVella.Tefter.Models;

public class TfTemplateProcessorResultPreviewComponentContext
{
	public TucTemplate Template { get; set; }
	public TucUseTemplateContext Data { get; set; }
	public string SettingsJson { get; set; } = "{}";
	public Func<List<ValidationError>> Validate { get; set; }
}
