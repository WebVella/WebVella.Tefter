namespace WebVella.Tefter.DataProviders.Csv.Components;

public class CsvDataProviderSettingsComponent : ComponentBase, ITfDataProviderSettings
{
	public string Value { get; set; }

	public List<ValidationError> Validate()
	{
		return new List<ValidationError>();
	}
}