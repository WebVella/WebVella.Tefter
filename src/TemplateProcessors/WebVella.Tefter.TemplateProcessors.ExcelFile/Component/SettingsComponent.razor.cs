﻿using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.TemplateProcessors.ExcelFile.Components;

[LocalizationResource("WebVella.Tefter.TemplateProcessors.ExcelFile.Components.TfUserManageDialog", "WebVella.Tefter.TemplateProcessors.ExcelFile")]
public partial class SettingsComponent : TfFormBaseComponent, ITfDataProviderSettings
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
				_form = JsonSerializer.Deserialize<ExcelFileTemplateSettings>(value);
			}
		}
	}

	private ExcelFileTemplateSettings _form = new();

	protected override void OnInitialized()
	{
		base.OnInitialized();
		base.InitForm(_form);
	}

	public List<ValidationError> Validate()
	{
		MessageStore.Clear();
		var errors = new List<ValidationError>();


		var isValid = EditContext.Validate();
		StateHasChanged();
		return errors;
	}


}