using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.Seeds.SampleTemplateProcessor.Components;

public partial class ManageSettingsComponent : TfFormBaseComponent,
	ITfRegionComponent<TfTemplateProcessorManageSettingsScreenRegion>
{
	public Guid Id { get; init; } = new Guid("459ce24e-37af-48eb-99fe-abf32d0b83b4");
	public int PositionRank { get; init; } = 1000;
	public string Name { get; init; } = "Sample Template Manage Settings";
	public string Description { get; init; } = "";
	public string FluentIconName { get; init; } = "PuzzlePiece";
	public List<TfScreenRegionScope> Scopes { get; init; } = new List<TfScreenRegionScope>(){
		new TfScreenRegionScope(typeof(SampleTemplateProcessor),null)
	};
	[Parameter] public TfTemplateProcessorManageSettingsScreenRegion Context { get; init; }

	private SampleTemplateSettings _form = new();

	protected override void OnInitialized()
	{
		base.OnInitialized();
		if (Context is null || Context.Template is null) throw new Exception("Context is not defined");
		_form = String.IsNullOrWhiteSpace(Context.Template.SettingsJson) ? new() : JsonSerializer.Deserialize<SampleTemplateSettings>(Context.Template.SettingsJson);
		Context.Validate = _validate;
		base.InitForm(_form);
	}
	protected override void OnParametersSet()
	{
		base.OnParametersSet();
		if (Context.Template.SettingsJson != JsonSerializer.Serialize(_form))
		{
			_form = String.IsNullOrWhiteSpace(Context.Template.SettingsJson) ? new() : JsonSerializer.Deserialize<SampleTemplateSettings>(Context.Template.SettingsJson);
			base.InitForm(_form);
		}
	}

	private List<ValidationError> _validate()
	{
		MessageStore.Clear();
		var errors = new List<ValidationError>();
		if (String.IsNullOrWhiteSpace(_form.Content))
			errors.Add(new ValidationError(nameof(SampleTemplateSettings.Content), LOC("required")));

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