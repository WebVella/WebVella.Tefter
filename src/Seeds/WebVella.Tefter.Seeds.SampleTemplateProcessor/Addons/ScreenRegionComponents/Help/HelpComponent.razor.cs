namespace WebVella.Tefter.Seeds.SampleTemplateProcessor.Addons;

public partial class HelpComponent : TfBaseComponent, 
	ITfRegionComponent<TfTemplateProcessorHelpScreenRegionContext>
{
	public const string ID = "2a9fd6da-2827-425b-9841-7b995e114ab2";
	public const string NAME = "Sample Template Help";
	public const string DESCRIPTION = "";
	public const string FLUENT_ICON_NAME = "PuzzlePiece";
	public const int POSITION_RANK = 1000;
	public Guid Id { get; init; } = new Guid(ID);
	public string Name { get; init; } = NAME;
	public string Description { get; init; } = DESCRIPTION;
	public string FluentIconName { get; init; } = FLUENT_ICON_NAME;
	public int PositionRank { get; init; } = POSITION_RANK;
	public List<TfScreenRegionScope> Scopes { get; init; } = new List<TfScreenRegionScope>(){ 
		new TfScreenRegionScope(typeof(SampleTemplateProcessor),null)
	};
	[Parameter] public TfTemplateProcessorHelpScreenRegionContext RegionContext { get; init; }
}