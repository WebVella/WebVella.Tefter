using Microsoft.AspNetCore.Components.Forms;
using System.Globalization;

namespace WebVella.Tefter.Templates.Components;

public partial class EmailContentProcessorSettingsComponent : TfFormBaseComponent//, ITfCustomComponent
{
	//For this component only ReadOnly and Form will be supported
	[Parameter] public TfComponentMode DisplayMode { get; set; } = TfComponentMode.Read;
	[Parameter] public string Value { get; set; }
	[Parameter] public EventCallback<string> ValueChanged { get; set; }
	[Parameter] public object Context { get; set; }

	private EmailTemplateProcessorSettings _form = new();

	protected override void OnInitialized()
	{
		base.OnInitialized();
		_form = String.IsNullOrWhiteSpace(Value) ? new() : JsonSerializer.Deserialize<EmailTemplateProcessorSettings>(Value);
		base.InitForm(_form);

	}

	protected override void OnParametersSet()
	{
		base.OnParametersSet();
		if (Value != JsonSerializer.Serialize(_form))
		{
			_form = String.IsNullOrWhiteSpace(Value) ? new() : JsonSerializer.Deserialize<EmailTemplateProcessorSettings>(Value);
			base.InitForm(_form);
		}
	}

	public List<ValidationError> Validate()
	{
		MessageStore.Clear();
		var errors = new List<ValidationError>();

		//if (String.IsNullOrWhiteSpace(_form.Filepath))
		//{
		//	errors.Add(new ValidationError(nameof(CsvDataProviderSettings.Filepath), LOC("required")));
		//}
		//else
		//{
		//	string extension = Path.GetExtension(_form.Filepath);
		//	if (extension != ".csv")
		//	{
		//		errors.Add(new ValidationError(nameof(CsvDataProviderSettings.Filepath), LOC("'csv' file extension is required")));
		//	}
		//}

		//if (!String.IsNullOrWhiteSpace(_form.CultureName))
		//{
		//	CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures & ~CultureTypes.NeutralCultures);
		//	var culture = cultures.FirstOrDefault(c => c.Name.Equals(_form.CultureName, StringComparison.OrdinalIgnoreCase));
		//	if (culture == null)
		//		errors.Add(new ValidationError(nameof(CsvDataProviderSettings.CultureName), LOC("invalid. format like 'en-US'")));
		//}

		//foreach (var item in errors)
		//{
		//	MessageStore.Add(EditContext.Field(item.PropertyName), item.Reason);
		//}
		var isValid = EditContext.Validate();
		StateHasChanged();
		return errors;
	}

	private async Task _valueChanged()
	{
		await ValueChanged.InvokeAsync(JsonSerializer.Serialize(_form));
	}

}