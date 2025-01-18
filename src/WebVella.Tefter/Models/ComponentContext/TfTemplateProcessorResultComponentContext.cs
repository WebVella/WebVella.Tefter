namespace WebVella.Tefter.Models;

public class TfTemplateProcessorResultComponentContext
{
	public TucTemplate Template { get; set; }
	public string SettingsJson { get; set; } = "{}";
	public TucUseTemplateContext Data { get; set; }
}
