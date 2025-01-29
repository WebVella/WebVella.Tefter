namespace WebVella.Tefter.TemplateProcessors.TextFile.Components;

[LocalizationResource("WebVella.Tefter.TemplateProcessors.TextFile.Components.Help.HelpComponent", "WebVella.Tefter.TemplateProcessors.TextFile")]
public partial class HelpComponent : TfBaseComponent, 
	ITfRegionComponent<TfTemplateProcessorHelpComponentContext>
{
	public Guid Id { get; init; } = new Guid("12d4f78b-daf2-48f0-ab84-3108703e7d7b");
	public int PositionRank { get; init; } = 1000;
	public string Name { get; init; } = "Text File Template Help";
	public string Description { get; init; } = "";
	public string FluentIconName { get; init; } = "PuzzlePiece";
	public List<TfRegionComponentScope> Scopes { get; init; } = new List<TfRegionComponentScope>(){ 
		new TfRegionComponentScope(typeof(TextFileTemplateProcessor),null)
	};
	[Parameter] public TfTemplateProcessorHelpComponentContext Context { get; init; }

}