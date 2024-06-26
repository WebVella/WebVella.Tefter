using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.DataProviders.Csv.Components;

public partial class DataProviderSettingsComponent : TfFormBaseComponent, ITfDataProviderSettings
{
	public string Value
	{
		get => _value;
		set
		{
			_value = value;
			if (String.IsNullOrEmpty(_value))
			{
				_form = new();
			}
			else
			{
				_form = JsonSerializer.Deserialize<CsvDataProviderSettings>(_value);
			}
		}
	}

	private string _value = string.Empty;
	private CsvDataProviderSettings _form = new();

	protected override void OnInitialized()
	{
		base.OnInitialized();
		base.InitForm(_form);
	}

	protected override void OnAfterRender(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		if (firstRender)
		{
			base.InitForm(_form);
		}
	}


	public List<ValidationError> Validate()
	{
		MessageStore.Clear();
		var errors = new List<ValidationError>() {
		new ValidationError("", "required"),
		new ValidationError(nameof(CsvDataProviderSettings.Filepath), "required")
		};
		foreach (var item in errors)
		{
			MessageStore.Add(EditContext.Field(item.PropertyName), item.Reason);
		}
		var isValid = EditContext.Validate();
		StateHasChanged();
		return errors;
	}
}