using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.TemplateProcessors.Email.Components;

[LocalizationResource("WebVella.Tefter.TemplateProcessors.Email.Components.Settings.SettingsComponent", "WebVella.Tefter.TemplateProcessors.Email")]
public partial class SettingsComponent : TfFormBaseComponent, ITfDataProviderSettings
{
	[Inject] public ITfBlobManager BlobManager { get; set; }

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
				_form = JsonSerializer.Deserialize<EmailTemplateSettings>(value);
			}
			if(_form is null) _form = new();
		}
	}

	[Parameter]
	public EventCallback<string> ValueChanged { get; set; }
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