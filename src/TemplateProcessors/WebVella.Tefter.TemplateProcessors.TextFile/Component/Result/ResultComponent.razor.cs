﻿using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.TemplateProcessors.TextFile.Components;

[LocalizationResource("WebVella.Tefter.TemplateProcessors.TextFile.Components.Result.ResultComponent", "WebVella.Tefter.TemplateProcessors.TextFile")]
public partial class ResultComponent : TfFormBaseComponent, ITfCustomComponent
{
	//For this component only ReadOnly and Form will be supported
	[Parameter] public TfComponentMode DisplayMode { get; set; } = TfComponentMode.Read;
	[Parameter] public string Value { get; set; }
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