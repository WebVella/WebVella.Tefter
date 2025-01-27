namespace WebVella.Tefter.Models;

/// <summary>
/// Context need to be included in the ScanAndRegisterDynamicComponents method as a case in order to be discovered
/// </summary>
public class TfTemplateProcessorManageSettingsComponentContext : TfBaseRegionScopedComponentContext<ITfTemplateProcessor>
{
	public TucTemplate Template { get; set; }
	public EventCallback<string> SettingsJsonChanged { get; set; }
	public Func<List<ValidationError>> Validate { get; set; }
}
