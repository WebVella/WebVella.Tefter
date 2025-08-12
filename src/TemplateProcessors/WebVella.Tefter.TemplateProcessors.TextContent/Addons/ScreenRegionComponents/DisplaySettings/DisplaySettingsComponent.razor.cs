namespace WebVella.Tefter.TemplateProcessors.TextContent.Addons;

[LocalizationResource("WebVella.Tefter.TemplateProcessors.TextContent.Addons.ScreenRegionComponents.DisplaySettings.DisplaySettingsComponent", "WebVella.Tefter.TemplateProcessors.TextContent")]
public partial class DisplaySettingsComponent : TfFormBaseComponent, 
	ITfScreenRegionComponent<TfTemplateProcessorDisplaySettingsScreenRegionContext>
{
	public const string ID = "d05ea639-b6d2-4f8b-8cc7-307961cf0502";
	public const string NAME = "Text content View Settings";
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
	[Parameter] public TfTemplateProcessorDisplaySettingsScreenRegionContext RegionContext { get; set; }

	private TextContentTemplateSettings _form = new();

	protected override void OnInitialized()
	{
		base.OnInitialized();
		if (RegionContext is null || RegionContext.Template is null) throw new Exception("Context is not defined");
		_form = String.IsNullOrWhiteSpace(RegionContext.Template.SettingsJson) ? new() : JsonSerializer.Deserialize<TextContentTemplateSettings>(RegionContext.Template.SettingsJson);
	}
	protected override void OnParametersSet()
	{
		base.OnParametersSet();
		if (RegionContext.Template.SettingsJson != JsonSerializer.Serialize(_form))
		{
			_form = String.IsNullOrWhiteSpace(RegionContext.Template.SettingsJson) ? new() : JsonSerializer.Deserialize<TextContentTemplateSettings>(RegionContext.Template.SettingsJson);
		}
	}


}