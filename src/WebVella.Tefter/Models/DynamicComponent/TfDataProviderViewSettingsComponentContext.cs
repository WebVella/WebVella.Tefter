namespace WebVella.Tefter.Models;

/// <summary>
/// Context need to be included in the ScanAndRegisterDynamicComponents method as a case in order to be discovered
/// </summary>
public class TfDataProviderViewSettingsComponentContext : TfBaseDynamicScopedComponentContext<ITfDataProviderType>
{
	public string SettingsJson { get; set; } = "{}";
}