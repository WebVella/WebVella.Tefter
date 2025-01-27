using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.TemplateProcessors.ExcelFile.Components;

[LocalizationResource("WebVella.Tefter.TemplateProcessors.ExcelFile.Components.Help.HelpComponent", "WebVella.Tefter.TemplateProcessors.ExcelFile")]
public partial class HelpComponent : TfBaseComponent, 
	ITfRegionComponent<TfTemplateProcessorHelpComponentContext>,
	ITfComponentScope<ExcelFileTemplateProcessor>
{
	public Guid Id { get; init; } = new Guid("c1090557-af1e-48c9-844d-51c90170df24");
	public int PositionRank { get; init; } = 1000;
	public string Name { get; init; } = "Excel Template Help";
	public string Description { get; init; } = "";
	public string FluentIconName { get; init; } = "PuzzlePiece";
	[Parameter] public TfTemplateProcessorHelpComponentContext Context { get; init; }

}