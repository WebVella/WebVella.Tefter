namespace WebVella.Tefter.TemplateProcessors.TextContent.Components;

[LocalizationResource("WebVella.Tefter.TemplateProcessors.TextContent.Components.Help.HelpComponent", "WebVella.Tefter.TemplateProcessors.TextContent")]
public partial class HelpComponent : TfBaseComponent, ITfDynamicComponent<TfTemplateProcessorHelpComponentContext>
{
	public Guid Id { get; init; } = new Guid("68407341-381c-4d66-8f27-7db46c223d74");
	public int PositionRank { get; init; } = 0;
	public string Name { get; init; } = "Text Content Template Help";
	public string Description { get; init; } = "";
	public string FluentIconName { get; init; } = "PuzzlePiece";
	[Parameter] public TfTemplateProcessorHelpComponentContext Context { get; init; }
}