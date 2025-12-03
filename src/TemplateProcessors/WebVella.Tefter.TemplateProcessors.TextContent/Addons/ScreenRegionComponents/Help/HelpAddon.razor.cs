namespace WebVella.Tefter.TemplateProcessors.TextContent.Addons;

public partial class HelpAddon : TfBaseComponent, 
	ITfScreenRegionAddon<TfTemplateProcessorHelpScreenRegion>
{
	public const string ID = "68407341-381c-4d66-8f27-7db46c223d74";
	public const string NAME = "Text Content Template Help";
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
	[Parameter] public TfTemplateProcessorHelpScreenRegion RegionContext { get; set; }
}