namespace WebVella.Tefter.DataProviders.Csv.Components;

public partial class CsvDataProviderSettingsComponent : ComponentBase, ITfDataProviderSettings
{
	public string Value
	{
		get => _value;
		set
		{
			_value = value;
			if (String.IsNullOrEmpty(_value))
			{
				_settings = new();
			}
			else
			{
				_settings = JsonSerializer.Deserialize<CsvDataProviderSettings>(_value);
			}
		}
	}

	private string _value = string.Empty;
	private CsvDataProviderSettings _settings;

	public List<ValidationError> Validate()
	{
		return new List<ValidationError>();
	}
}