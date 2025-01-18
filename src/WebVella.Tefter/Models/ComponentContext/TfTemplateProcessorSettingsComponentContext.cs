namespace WebVella.Tefter.Models;

public class TfTemplateProcessorSettingsComponentContext
{
	public TucTemplate Template { get; set; }
	public EventCallback<string> SettingsJsonChanged { get; set; }
	public Func<List<ValidationError>> Validate { get; set; }
}
