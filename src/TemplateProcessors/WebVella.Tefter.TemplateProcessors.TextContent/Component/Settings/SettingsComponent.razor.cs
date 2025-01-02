using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.TemplateProcessors.TextContent.Components;

[LocalizationResource("WebVella.Tefter.TemplateProcessors.TextContent.Components.Settings.SettingsComponent", "WebVella.Tefter.TemplateProcessors.TextContent")]
public partial class SettingsComponent : TfFormBaseComponent, ITfCustomComponent
{
	[Inject] public ITfBlobManager BlobManager { get; set; }

	//For this component only ReadOnly and Form will be supported
	[Parameter] public TfComponentMode DisplayMode { get; set; } = TfComponentMode.Read;
	[Parameter] public string Value { get; set; }
	[Parameter] public EventCallback<string> ValueChanged { get; set; }
	[Parameter] public object Context { get; set; }

	private TextContentTemplateSettings _form = new();

	protected override void OnInitialized()
	{
		base.OnInitialized();
		_form = String.IsNullOrWhiteSpace(Value) ? new() : JsonSerializer.Deserialize<TextContentTemplateSettings>(Value);
		base.InitForm(_form);
	}
	protected override void OnParametersSet()
	{
		base.OnParametersSet();
		if (Value != JsonSerializer.Serialize(_form))
		{
			_form = String.IsNullOrWhiteSpace(Value) ? new() : JsonSerializer.Deserialize<TextContentTemplateSettings>(Value);
			base.InitForm(_form);
		}
	}

	public List<ValidationError> Validate()
	{
		MessageStore.Clear();
		var errors = new List<ValidationError>();
		if (String.IsNullOrWhiteSpace(_form.Content))
			errors.Add(new ValidationError(nameof(TextContentTemplateSettings.Content), LOC("required")));

		foreach (var item in errors)
		{
			MessageStore.Add(EditContext.Field(item.PropertyName), item.Reason);
		}
		var isValid = EditContext.Validate();
		StateHasChanged();
		return errors;
	}
	
	private async Task _valueChanged()
	{
		await ValueChanged.InvokeAsync(JsonSerializer.Serialize(_form));
	}

}