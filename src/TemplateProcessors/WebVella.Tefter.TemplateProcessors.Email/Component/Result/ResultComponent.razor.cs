using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.TemplateProcessors.Email.Components;

[LocalizationResource("WebVella.Tefter.TemplateProcessors.Email.Components.Result.ResultComponent", "WebVella.Tefter.TemplateProcessors.Email")]
public partial class ResultComponent : TfFormBaseComponent, ITfDynamicComponent<TfTemplateProcessorResultComponentContext>
{
	//For this component only ReadOnly and Form will be supported
	[Parameter] public TfComponentMode DisplayMode { get; set; } = TfComponentMode.Read;
	[Parameter] public TfTemplateProcessorResultComponentContext Context { get; set; }

	private EmailTemplateProcessSettings _form = new();


	protected override void OnInitialized()
	{
		base.OnInitialized();
		_form = String.IsNullOrWhiteSpace(Context.SettingsJson) ? new() : JsonSerializer.Deserialize<EmailTemplateProcessSettings>(Context.SettingsJson);
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