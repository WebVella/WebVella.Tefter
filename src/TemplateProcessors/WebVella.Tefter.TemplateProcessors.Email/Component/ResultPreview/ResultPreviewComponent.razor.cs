using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.TemplateProcessors.Email.Components;

[LocalizationResource("WebVella.Tefter.TemplateProcessors.Email.Components.ResultPreview.ResultPreviewComponent", "WebVella.Tefter.TemplateProcessors.Email")]
public partial class ResultPreviewComponent : TfFormBaseComponent, ITfDynamicComponent<TfTemplateProcessorResultPreviewComponentContext>
{
	//For this component only ReadOnly and Form will be supported
	[Parameter] public TfComponentMode DisplayMode { get; set; } = TfComponentMode.Read;
	[Parameter] public TfTemplateProcessorResultPreviewComponentContext Context { get; set; }
	private EmailTemplateProcessSettings _form = new();


	protected override void OnInitialized()
	{
		base.OnInitialized();
		if (Context is null) throw new Exception("Context is not defined");

		Context.Validate = _validate;
		base.InitForm(_form);
	}



	private List<ValidationError> _validate()
	{
		MessageStore.Clear();
		var errors = new List<ValidationError>();


		var isValid = EditContext.Validate();
		StateHasChanged();
		return errors;
	}


}