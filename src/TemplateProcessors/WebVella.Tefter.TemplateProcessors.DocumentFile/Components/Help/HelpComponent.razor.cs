using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.TemplateProcessors.DocumentFile.Components;

[LocalizationResource("WebVella.Tefter.TemplateProcessors.DocumentFile.Components.Help.HelpComponent", "WebVella.Tefter.TemplateProcessors.DocumentFile")]
public partial class HelpComponent : TfBaseComponent, 
	ITfRegionComponent<TfTemplateProcessorHelpComponentContext>
{
	public Guid Id { get; init; } = new Guid("c1090557-af1e-48c9-844d-51c90170df24");
	public int PositionRank { get; init; } = 1000;
	public string Name { get; init; } = "Document Template Help";
	public string Description { get; init; } = "";
	public string FluentIconName { get; init; } = "PuzzlePiece";
	public List<TfRegionComponentScope> Scopes { get; init; } = new List<TfRegionComponentScope>(){ 
		new TfRegionComponentScope(typeof(DocumentFileTemplateProcessor),null)
	};
	[Parameter] public TfTemplateProcessorHelpComponentContext Context { get; init; }

}