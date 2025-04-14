namespace WebVella.Tefter.TemplateProcessors.TextContent.Addons;

[LocalizationResource("WebVella.Tefter.TemplateProcessors.TextContent.Addons.ScreenRegionComponents.Help.HelpComponent", "WebVella.Tefter.TemplateProcessors.TextContent")]
public partial class HelpComponent : TfBaseComponent, 
	ITfScreenRegionComponent<TfTemplateProcessorHelpScreenRegionContext>
{
	public const string ID = "68407341-381c-4d66-8f27-7db46c223d74";
	public const string NAME = "Text Content Template Help";
	public const string DESCRIPTION = "";
	public const string FLUENT_ICON_NAME = "PuzzlePiece";
	public const int POSITION_RANK = 1000;
	public Guid Id { get; init; } = new Guid(ID);
	public string Name { get; init; } = NAME;
	public string Description { get; init; } = DESCRIPTION;
	public string FluentIconName { get; init; } = FLUENT_ICON_NAME;
	public int PositionRank { get; init; } = POSITION_RANK;
	public List<TfScreenRegionScope> Scopes { get; init; } = new List<TfScreenRegionScope>(){ 
		new TfScreenRegionScope(typeof(TextContentTemplateProcessor),null)
	};
	[Parameter] public TfTemplateProcessorHelpScreenRegionContext RegionContext { get; init; }
}