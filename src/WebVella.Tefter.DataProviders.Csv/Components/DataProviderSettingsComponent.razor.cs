using Microsoft.AspNetCore.Components.Forms;
using WebVella.Tefter.Web.Models;

namespace WebVella.Tefter.DataProviders.Csv.Components;

public partial class DataProviderSettingsComponent : TfFormBaseComponent, ITfDataProviderSettings
{
	//For this component only ReadOnly and Form will be supported
	[Parameter] public ComponentDisplayMode DisplayMode { get; set; } = ComponentDisplayMode.ReadOnly;

	[Parameter]
	public string Value
	{
		get => JsonSerializer.Serialize(_form);
		set
		{
			if (String.IsNullOrEmpty(value))
			{
				_form = new();
			}
			else
			{
				_form = JsonSerializer.Deserialize<CsvDataProviderSettings>(value);
			}
		}
	}

	private CsvDataProviderSettings _form = new();

	protected override void OnInitialized()
	{
		base.OnInitialized();
		base.InitForm(_form);
	}

	public List<ValidationError> Validate()
	{
		MessageStore.Clear();
		var errors = new List<ValidationError>();

		if(String.IsNullOrWhiteSpace(_form.Filepath)){ 
			errors.Add(new ValidationError(nameof(CsvDataProviderSettings.Filepath), LOC("required")));
		}
		else{
			string extension = Path.GetExtension(_form.Filepath);
			if(extension != ".csv"){
				errors.Add(new ValidationError(nameof(CsvDataProviderSettings.Filepath), LOC("'csv' file extension is required")));
			}
		}

		foreach (var item in errors)
		{
			MessageStore.Add(EditContext.Field(item.PropertyName), item.Reason);
		}
		var isValid = EditContext.Validate();
		StateHasChanged();
		return errors;
	}
}