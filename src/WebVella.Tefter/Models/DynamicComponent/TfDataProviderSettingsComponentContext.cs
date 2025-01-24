namespace WebVella.Tefter.Models;

public class TfDataProviderSettingsComponentContext : TfBaseComponentContextWithRequiredScope<ITfDataProviderType>
{
	public string SettingsJson { get; set; } = "{}";
	public EventCallback<string> SettingsJsonChanged { get; init; }
	public Func<List<ValidationError>> Validate { get; private set; }
	public void SetValidate(Func<List<ValidationError>> validateFunc) => Validate = validateFunc;
}