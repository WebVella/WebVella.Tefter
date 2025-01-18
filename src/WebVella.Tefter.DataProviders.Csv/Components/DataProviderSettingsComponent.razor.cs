using Microsoft.AspNetCore.Components.Forms;
using System.Globalization;
using WebVella.Tefter.Models;
using WebVella.Tefter.Web.Components;
using WebVella.Tefter.Web.Models;

namespace WebVella.Tefter.DataProviders.Csv.Components;

public partial class DataProviderSettingsComponent : TfFormBaseComponent, ITfComponentContext<TfDataProviderSettingsComponentContext>
{
	//For this component only ReadOnly and Form will be supported
	[Parameter] public TfComponentMode DisplayMode { get; set; } = TfComponentMode.Read;
	[Parameter] public TfDataProviderSettingsComponentContext Context { get; set; }

	private string _advancedSettings
	{
		get
		{
			if (_form.AdvancedSetting is null) return JsonSerializer.Serialize(new CsvDataProviderSettingsAdvanced());
			return JsonSerializer.Serialize(_form.AdvancedSetting, new JsonSerializerOptions { WriteIndented = true });
		}
	}

	private CsvDataProviderSettings _form = new();

	protected override void OnInitialized()
	{
		base.OnInitialized();
		_form = String.IsNullOrWhiteSpace(Context.SettingsJson) ? new() : JsonSerializer.Deserialize<CsvDataProviderSettings>(Context.SettingsJson);
		Context.Validate = _validate;
		base.InitForm(_form);
	}

	protected override void OnParametersSet()
	{
		base.OnParametersSet();
		if (Context.SettingsJson != JsonSerializer.Serialize(_form))
		{
			_form = String.IsNullOrWhiteSpace(Context.SettingsJson) ? new() : JsonSerializer.Deserialize<CsvDataProviderSettings>(Context.SettingsJson);
			base.InitForm(_form);
		}
	}

	private List<ValidationError> _validate()
	{
		MessageStore.Clear();
		var errors = new List<ValidationError>();

		if (String.IsNullOrWhiteSpace(_form.Filepath))
		{
			errors.Add(new ValidationError(nameof(CsvDataProviderSettings.Filepath), LOC("required")));
		}
		else
		{
			string extension = Path.GetExtension(_form.Filepath);
			if (extension != ".csv")
			{
				errors.Add(new ValidationError(nameof(CsvDataProviderSettings.Filepath), LOC("'csv' file extension is required")));
			}
		}

		if (!String.IsNullOrWhiteSpace(_form.CultureName))
		{
			CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures & ~CultureTypes.NeutralCultures);
			var culture = cultures.FirstOrDefault(c => c.Name.Equals(_form.CultureName, StringComparison.OrdinalIgnoreCase));
			if (culture == null)
				errors.Add(new ValidationError(nameof(CsvDataProviderSettings.CultureName), LOC("invalid. format like 'en-US'")));
		}

		foreach (var item in errors)
		{
			MessageStore.Add(EditContext.Field(item.PropertyName), item.Message);
		}
		EditContext.Validate();
		StateHasChanged();
		EditContext.Validate();
		return errors;
	}

	private void _getCultureFromServer()
	{
		_form.CultureName = Thread.CurrentThread.CurrentCulture.Name;
	}

	private async Task _changeAdvancedSettings(string value)
	{
		try
		{
			_form.AdvancedSetting = JsonSerializer.Deserialize<CsvDataProviderSettingsAdvanced>(value);
			await _valueChanged();
		}
		catch (Exception ex)
		{
			ToastService.ShowError(ex.Message);
		}
	}

	private async Task _valueChanged()
	{
		await Context.SettingsJsonChanged.InvokeAsync(JsonSerializer.Serialize(_form));
	}
}