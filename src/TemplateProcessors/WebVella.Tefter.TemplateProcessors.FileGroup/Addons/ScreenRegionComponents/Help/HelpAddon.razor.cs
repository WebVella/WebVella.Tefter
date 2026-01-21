namespace WebVella.Tefter.TemplateProcessors.FileGroup.Addons;

public partial class HelpAddon : TfBaseComponent,
	ITfScreenRegionAddon<TfTemplateProcessorHelpScreenRegion>
{
	public const string ID = "B53BF128-B639-407D-9A06-07AAD36BB02C";
	public const string NAME = "FileGroup Template Help";
	public const string DESCRIPTION = "";
	public const string FLUENT_ICON_NAME = "PuzzlePiece";
	public const int POSITION_RANK = 1000;
	public Guid AddonId { get; init; } = new Guid(ID);
	public string AddonName { get; init; } = NAME;
	public string AddonDescription { get; init; } = DESCRIPTION;
	public string AddonFluentIconName { get; init; } = FLUENT_ICON_NAME;
	public int PositionRank { get; init; } = POSITION_RANK;
	public List<TfScreenRegionScope> Scopes { get; init; } = new List<TfScreenRegionScope>(){ 
		new TfScreenRegionScope(typeof(FileGroupTemplateProcessor),null)
	};
	[Parameter] public TfTemplateProcessorHelpScreenRegion RegionContext { get; set; }
}