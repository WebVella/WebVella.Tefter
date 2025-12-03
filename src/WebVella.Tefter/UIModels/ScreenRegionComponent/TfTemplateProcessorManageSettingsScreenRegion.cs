namespace WebVella.Tefter.Models;

/// <summary>
/// Context need to be included in the ScanAndRegisterRegionComponents method as a case in order to be discovered
/// </summary>
public class TfTemplateProcessorManageSettingsScreenRegion : TfBaseScreenRegion
{
	public TfTemplate Template { get; set; } = null!;
	public EventCallback<string> SettingsJsonChanged { get; set; }
	public Func<List<ValidationError>> Validate { get; set; } = null!;
}
