namespace WebVella.Tefter.Models;

public class TfDataProviderSettingsComponentContext
{
	public string SettingsJson { get; set; } = "{}";
	public EventCallback<string> SettingsJsonChanged { get; set; }
	public Func<List<ValidationError>> Validate { get; set; }
}
