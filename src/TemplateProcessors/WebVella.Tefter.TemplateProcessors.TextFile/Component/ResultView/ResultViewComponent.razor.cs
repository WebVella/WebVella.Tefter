﻿using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.TemplateProcessors.TextFile.Components;

[LocalizationResource("WebVella.Tefter.TemplateProcessors.TextFile.Components.Help.ResultViewComponent", "WebVella.Tefter.TemplateProcessors.TextFile")]
public partial class ResultViewComponent : TfFormBaseComponent, ITfCustomComponent
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
				_form = JsonSerializer.Deserialize<TextFileTemplateSettings>(value);
			}
		}
	}
	[Parameter] public EventCallback<string> ValueChanged { get; set; }
	[Parameter] public object Context { get; set; }

	private TextFileTemplateSettings _form = new();


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