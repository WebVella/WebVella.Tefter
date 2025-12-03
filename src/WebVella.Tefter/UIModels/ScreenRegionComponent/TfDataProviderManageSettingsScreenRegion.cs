namespace WebVella.Tefter.Models;

/// <summary>
/// Context need to be included in the ScanAndRegisterRegionComponents method as a case in order to be discovered
/// </summary>

public class TfDataProviderManageSettingsScreenRegion : TfBaseScreenRegion
{
	public string SettingsJson { get; set; } = "{}";
	public EventCallback<string> SettingsJsonChanged { get; init; }
	public Func<List<ValidationError>> Validate { get; private set; } = null!;
	public void SetValidate(Func<List<ValidationError>> validateFunc) => Validate = validateFunc;
}