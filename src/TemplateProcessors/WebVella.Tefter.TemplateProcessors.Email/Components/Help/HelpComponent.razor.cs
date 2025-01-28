using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.TemplateProcessors.Email.Components;

[LocalizationResource("WebVella.Tefter.TemplateProcessors.Email.Components.Help.HelpComponent", "WebVella.Tefter.TemplateProcessors.Email")]
public partial class HelpComponent : TfBaseComponent,
	ITfRegionComponent<TfTemplateProcessorHelpComponentContext>
{
	public Guid Id { get; init; } = new Guid("e1cc2761-3526-4672-b1f7-aa1092f8fb1d");
	public int PositionRank { get; init; } = 1000;
	public string Name { get; init; } = "Email Template Help";
	public string Description { get; init; } = "";
	public string FluentIconName { get; init; } = "PuzzlePiece";
	public List<TfRegionComponentScope> Scopes { get; init; } = new List<TfRegionComponentScope>(){ 
		new TfRegionComponentScope(typeof(EmailTemplateProcessor),null)
	};
	[Parameter] public TfTemplateProcessorHelpComponentContext Context { get; init; }
}