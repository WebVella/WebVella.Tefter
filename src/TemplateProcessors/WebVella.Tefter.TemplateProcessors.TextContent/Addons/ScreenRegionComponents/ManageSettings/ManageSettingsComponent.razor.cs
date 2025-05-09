using Microsoft.AspNetCore.Components.Forms;
using WebVella.Tefter.Exceptions;

namespace WebVella.Tefter.TemplateProcessors.TextContent.Addons;

[LocalizationResource("WebVella.Tefter.TemplateProcessors.TextContent.Addons.ScreenRegionComponents.ManageSettings.ManageSettingsComponent", "WebVella.Tefter.TemplateProcessors.TextContent")]
public partial class ManageSettingsComponent : TfFormBaseComponent, 
	ITfScreenRegionComponent<TfTemplateProcessorManageSettingsScreenRegionContext>
{
	public const string ID = "459ce24e-37af-48eb-99fe-abf32d0b83b4";
	public const string NAME = "Text Content Template Manage Settings";
	public const string DESCRIPTION = "";
	public const string FLUENT_ICON_NAME = "PuzzlePiece";
	public const int POSITION_RANK = 1000;
	public Guid AddonId { get; init; } = new Guid(ID);
	public string AddonName { get; init; } = NAME;
	public string AddonDescription { get; init; } = DESCRIPTION;
	public string AddonFluentIconName { get; init; } = FLUENT_ICON_NAME;
	public int PositionRank { get; init; } = POSITION_RANK;
	public List<TfScreenRegionScope> Scopes { get; init; } = new List<TfScreenRegionScope>(){ 
		new TfScreenRegionScope(typeof(TextContentTemplateProcessor),null)
	};
	[Parameter] public TfTemplateProcessorManageSettingsScreenRegionContext RegionContext { get; init; }

	private TextContentTemplateSettings _form = new();

	protected override void OnInitialized()
	{
		base.OnInitialized();
		if (RegionContext is null || RegionContext.Template is null) throw new Exception("Context is not defined");
		_form = String.IsNullOrWhiteSpace(RegionContext.Template.SettingsJson) ? new() : JsonSerializer.Deserialize<TextContentTemplateSettings>(RegionContext.Template.SettingsJson);
		RegionContext.Validate = _validate;
		base.InitForm(_form);
	}
	protected override void OnParametersSet()
	{
		base.OnParametersSet();
		if (RegionContext.Template.SettingsJson != JsonSerializer.Serialize(_form))
		{
			_form = String.IsNullOrWhiteSpace(RegionContext.Template.SettingsJson) ? new() : JsonSerializer.Deserialize<TextContentTemplateSettings>(RegionContext.Template.SettingsJson);
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
		RegionContext.Template.SettingsJson = JsonSerializer.Serialize(_form);
		await RegionContext.SettingsJsonChanged.InvokeAsync(RegionContext.Template.SettingsJson);
	}

}