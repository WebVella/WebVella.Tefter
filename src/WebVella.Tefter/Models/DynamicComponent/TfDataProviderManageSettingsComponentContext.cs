namespace WebVella.Tefter.Models;

/// <summary>
/// Context need to be included in the ScanAndRegisterDynamicComponents method as a case in order to be discovered
/// </summary>

public class TfDataProviderManageSettingsComponentContext : TfBaseRegionScopedComponentContext<ITfDataProviderType>
{
	public string SettingsJson { get; set; } = "{}";
	public EventCallback<string> SettingsJsonChanged { get; init; }
	public Func<List<ValidationError>> Validate { get; private set; }
	public void SetValidate(Func<List<ValidationError>> validateFunc) => Validate = validateFunc;
}