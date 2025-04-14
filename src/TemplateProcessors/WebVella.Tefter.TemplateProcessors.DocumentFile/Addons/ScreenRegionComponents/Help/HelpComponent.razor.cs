using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.TemplateProcessors.DocumentFile.Addons;

[LocalizationResource("WebVella.Tefter.TemplateProcessors.DocumentFile.Addons.ScreenRegionComponents.Help.HelpComponent", "WebVella.Tefter.TemplateProcessors.DocumentFile")]
public partial class HelpComponent : TfBaseComponent, 
	ITfScreenRegionComponent<TfTemplateProcessorHelpScreenRegionContext>
{
	public const string ID = "c1090557-af1e-48c9-844d-51c90170df24";
	public const string NAME = "Document Template Help";
	public const string DESCRIPTION = "";
	public const string FLUENT_ICON_NAME = "PuzzlePiece";
	public const int POSITION_RANK = 1000;
	public Guid Id { get; init; } = new Guid(ID);
	public string Name { get; init; } = NAME;
	public string Description { get; init; } = DESCRIPTION;
	public string FluentIconName { get; init; } = FLUENT_ICON_NAME;
	public int PositionRank { get; init; } = POSITION_RANK;
	public List<TfScreenRegionScope> Scopes { get; init; } = new List<TfScreenRegionScope>(){ 
		new TfScreenRegionScope(typeof(DocumentFileTemplateProcessor),null)
	};
	[Parameter] public TfTemplateProcessorHelpScreenRegionContext RegionContext { get; init; }

}