using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.TemplateProcessors.DocumentFile.Addons;

[LocalizationResource("WebVella.Tefter.TemplateProcessors.DocumentFile.Addons.ScreenRegionComponents.Help.HelpComponent", "WebVella.Tefter.TemplateProcessors.DocumentFile")]
public partial class HelpComponent : TfBaseComponent, 
	ITfScreenRegionComponent<TfTemplateProcessorHelpScreenRegionContext>
{
	public const string ID = "fd399174-a079-4743-ad46-a0a4d66fa35e";
	public const string NAME = "Document Template Help";
	public const string DESCRIPTION = "";
	public const string FLUENT_ICON_NAME = "PuzzlePiece";
	public const int POSITION_RANK = 1000;
	public Guid AddonId { get; init; } = new Guid(ID);
	public string AddonName { get; init; } = NAME;
	public string AddonDescription { get; init; } = DESCRIPTION;
	public string AddonFluentIconName { get; init; } = FLUENT_ICON_NAME;
	public int PositionRank { get; init; } = POSITION_RANK;
	public List<TfScreenRegionScope> Scopes { get; init; } = new List<TfScreenRegionScope>(){ 
		new TfScreenRegionScope(typeof(DocumentFileTemplateProcessor),null)
	};
	[Parameter] public TfTemplateProcessorHelpScreenRegionContext? RegionContext { get; init; }

}