using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.TemplateProcessors.TextContent.Components;

[LocalizationResource("WebVella.Tefter.TemplateProcessors.TextContent.Components.ManageSettings.ManageSettingsComponent", "WebVella.Tefter.TemplateProcessors.TextContent")]
public partial class ManageSettingsComponent : TfFormBaseComponent, 
	ITfDynamicComponent<TfTemplateProcessorManageSettingsComponentContext>,
	ITfComponentScope<TextContentTemplateProcessor>
{
	public Guid Id { get; init; } = new Guid("459ce24e-37af-48eb-99fe-abf32d0b83b4");
	public int PositionRank { get; init; } = 0;
	public string Name { get; init; } = "Text Content Template Manage Settings";
	public string Description { get; init; } = "";
	public string FluentIconName { get; init; } = "PuzzlePiece";
	[Parameter] public TfTemplateProcessorManageSettingsComponentContext Context { get; init; }

	private TextContentTemplateSettings _form = new();

	protected override void OnInitialized()
	{
		base.OnInitialized();
		if (Context is null || Context.Template is null) throw new Exception("Context is not defined");
		_form = String.IsNullOrWhiteSpace(Context.Template.SettingsJson) ? new() : JsonSerializer.Deserialize<TextContentTemplateSettings>(Context.Template.SettingsJson);
		Context.Validate = _validate;
		base.InitForm(_form);
	}
	protected override void OnParametersSet()
	{
		base.OnParametersSet();
		if (Context.Template.SettingsJson != JsonSerializer.Serialize(_form))
		{
			_form = String.IsNullOrWhiteSpace(Context.Template.SettingsJson) ? new() : JsonSerializer.Deserialize<TextContentTemplateSettings>(Context.Template.SettingsJson);
			base.InitForm(_form);
		}
	}

	private List<ValidationError> _validate()
	{
		MessageStore.Clear();
		var errors = new List<ValidationError>();
		if (String.IsNullOrWhiteSpace(_form.Content))
			errors.Add(new ValidationError(nameof(TextContentTemplateSettings.Content), LOC("required")));

		foreach (var item in errors)
		{
			MessageStore.Add(EditContext.Field(item.PropertyName), item.Message);
		}
		var isValid = EditContext.Validate();
		StateHasChanged();
		return errors;
	}

	private async Task _valueChanged()
	{
		Context.Template.SettingsJson = JsonSerializer.Serialize(_form);
		await Context.SettingsJsonChanged.InvokeAsync(Context.Template.SettingsJson);
	}

}