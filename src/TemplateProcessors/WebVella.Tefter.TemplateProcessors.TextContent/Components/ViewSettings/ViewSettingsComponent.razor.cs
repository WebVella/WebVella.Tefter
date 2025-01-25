namespace WebVella.Tefter.TemplateProcessors.TextContent.Components;

[LocalizationResource("WebVella.Tefter.TemplateProcessors.TextContent.Components.ViewSettings.ViewSettingsComponent", "WebVella.Tefter.TemplateProcessors.TextContent")]
public partial class ViewSettingsComponent : TfFormBaseComponent, ITfDynamicComponent<TfTemplateProcessorViewSettingsComponentContext>
{
	public Guid Id { get; init; } = new Guid("d05ea639-b6d2-4f8b-8cc7-307961cf0502");
	public int PositionRank { get; init; } = 0;
	public string Name { get; init; } = "Text content View Settings";
	public string Description { get; init; } = "";
	public string FluentIconName { get; init; } = "PuzzlePiece";
	[Parameter] public TfTemplateProcessorViewSettingsComponentContext Context { get; init; }

	private TextContentTemplateSettings _form = new();

	protected override void OnInitialized()
	{
		base.OnInitialized();
		if (Context is null || Context.Template is null) throw new Exception("Context is not defined");
		_form = String.IsNullOrWhiteSpace(Context.Template.SettingsJson) ? new() : JsonSerializer.Deserialize<TextContentTemplateSettings>(Context.Template.SettingsJson);
	}
	protected override void OnParametersSet()
	{
		base.OnParametersSet();
		if (Context.Template.SettingsJson != JsonSerializer.Serialize(_form))
		{
			_form = String.IsNullOrWhiteSpace(Context.Template.SettingsJson) ? new() : JsonSerializer.Deserialize<TextContentTemplateSettings>(Context.Template.SettingsJson);
		}
	}


}