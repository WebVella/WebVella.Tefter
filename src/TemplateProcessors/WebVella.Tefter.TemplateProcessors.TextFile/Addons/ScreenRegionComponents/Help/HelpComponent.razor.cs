namespace WebVella.Tefter.TemplateProcessors.TextFile.Addons;

[LocalizationResource("WebVella.Tefter.TemplateProcessors.TextFile.Addons.ScreenRegionComponents.Help.HelpComponent", "WebVella.Tefter.TemplateProcessors.TextFile")]
public partial class HelpComponent : TfBaseComponent, 
	ITfRegionComponent<TfTemplateProcessorHelpScreenRegionContext>
{
	public const string ID = "12d4f78b-daf2-48f0-ab84-3108703e7d7b";
	public const string NAME = "Text File Template Help";
	public const string DESCRIPTION = "";
	public const string FLUENT_ICON_NAME = "PuzzlePiece";
	public const int POSITION_RANK = 1000;
	public Guid Id { get; init; } = new Guid(ID);
	public string Name { get; init; } = NAME;
	public string Description { get; init; } = DESCRIPTION;
	public string FluentIconName { get; init; } = FLUENT_ICON_NAME;
	public int PositionRank { get; init; } = POSITION_RANK;
	public List<TfScreenRegionScope> Scopes { get; init; } = new List<TfScreenRegionScope>(){ 
		new TfScreenRegionScope(typeof(TextFileTemplateProcessor),null)
	};
	[Parameter] public TfTemplateProcessorHelpScreenRegionContext RegionContext { get; init; }

}