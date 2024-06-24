namespace WebVella.Tefter.DataProviders.Csv.Components;

public class CsvDataProviderSettingsComponent : ComponentBase, IDataProviderSettings
{
	public string Value { get; set; }

	public List<ValidationError> Validate()
	{
		return new List<ValidationError>();
	}
}