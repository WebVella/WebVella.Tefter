﻿using Microsoft.AspNetCore.Components.Forms;
using System.Globalization;

namespace WebVella.Tefter.Templates.Components;

public partial class EmailContentProcessorSettingsComponent : TfFormBaseComponent, ITfDataProviderSettings
{
	//For this component only ReadOnly and Form will be supported
	[Parameter] public TfComponentMode DisplayMode { get; set; } = TfComponentMode.Read;

	[Parameter]
#pragma warning disable BL0007 // Component parameters should be auto properties
	public string Value
#pragma warning restore BL0007 // Component parameters should be auto properties
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
				_form = JsonSerializer.Deserialize<EmailTemplateProcessorSettings>(value);
			}
		}
	}



	private EmailTemplateProcessorSettings _form = new();

	protected override void OnInitialized()
	{
		base.OnInitialized();
		base.InitForm(_form);

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



}