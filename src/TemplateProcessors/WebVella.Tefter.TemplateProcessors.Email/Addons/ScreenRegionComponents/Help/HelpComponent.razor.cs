using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.TemplateProcessors.Email.Addons;

[LocalizationResource("WebVella.Tefter.TemplateProcessors.Email.Addons.ScreenRegionComponents.Help.HelpComponent", "WebVella.Tefter.TemplateProcessors.Email")]
public partial class HelpComponent : TfBaseComponent,
	ITfRegionComponent<TfTemplateProcessorHelpScreenRegionContext>
{
	public const string ID = "e1cc2761-3526-4672-b1f7-aa1092f8fb1d";
	public const string NAME = "Email Template Help";
	public const string DESCRIPTION = "";
	public const string FLUENT_ICON_NAME = "PuzzlePiece";
	public const int POSITION_RANK = 1000;
	public Guid Id { get; init; } = new Guid(ID);
	public string Name { get; init; } = NAME;
	public string Description { get; init; } = DESCRIPTION;
	public string FluentIconName { get; init; } = FLUENT_ICON_NAME;
	public int PositionRank { get; init; } = POSITION_RANK;
	public List<TfScreenRegionScope> Scopes { get; init; } = new List<TfScreenRegionScope>(){ 
		new TfScreenRegionScope(typeof(EmailTemplateProcessor),null)
	};
	[Parameter] public TfTemplateProcessorHelpScreenRegionContext RegionContext { get; init; }
}