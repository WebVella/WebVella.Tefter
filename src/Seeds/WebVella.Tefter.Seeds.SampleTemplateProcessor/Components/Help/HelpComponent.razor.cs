namespace WebVella.Tefter.Seeds.SampleTemplateProcessor.Components;

public partial class HelpComponent : TfBaseComponent, 
	ITfRegionComponent<TfTemplateProcessorHelpComponentContext>
{
	public Guid Id { get; init; } = new Guid("2a9fd6da-2827-425b-9841-7b995e114ab2");
	public int PositionRank { get; init; } = 1000;
	public string Name { get; init; } = "Sample Template Help";
	public string Description { get; init; } = "";
	public string FluentIconName { get; init; } = "PuzzlePiece";
	public List<TfRegionComponentScope> Scopes { get; init; } = new List<TfRegionComponentScope>(){ 
		new TfRegionComponentScope(typeof(SampleTemplateProcessor),null)
	};
	[Parameter] public TfTemplateProcessorHelpComponentContext Context { get; init; }
}