namespace WebVella.Tefter.Seeds.SampleTemplateProcessor.Components;

public partial class HelpComponent : TfBaseComponent, 
	ITfRegionComponent<TfTemplateProcessorHelpScreenRegion>
{
	public Guid Id { get; init; } = new Guid("2a9fd6da-2827-425b-9841-7b995e114ab2");
	public int PositionRank { get; init; } = 1000;
	public string Name { get; init; } = "Sample Template Help";
	public string Description { get; init; } = "";
	public string FluentIconName { get; init; } = "PuzzlePiece";
	public List<TfScreenRegionScope> Scopes { get; init; } = new List<TfScreenRegionScope>(){ 
		new TfScreenRegionScope(typeof(SampleTemplateProcessor),null)
	};
	[Parameter] public TfTemplateProcessorHelpScreenRegion Context { get; init; }
}