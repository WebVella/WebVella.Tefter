using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.TemplateProcessors.Email.Components;

[LocalizationResource("WebVella.Tefter.TemplateProcessors.Email.Components.Help.HelpComponent", "WebVella.Tefter.TemplateProcessors.Email")]
public partial class HelpComponent : TfFormBaseComponent, ITfDynamicComponent<TfTemplateProcessorHelpComponentContext>
{
	//For this component only ReadOnly and Form will be supported
	[Parameter] public TfComponentMode DisplayMode { get; set; } = TfComponentMode.Read;
	[Parameter] public TfTemplateProcessorHelpComponentContext Context { get; set; }

	private EmailTemplateSettings _form = new();


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