using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.TemplateProcessors.TextFile.Components;

[LocalizationResource("WebVella.Tefter.TemplateProcessors.TextFile.Components.Help.HelpComponent", "WebVella.Tefter.TemplateProcessors.TextFile")]
public partial class HelpComponent : TfBaseComponent, ITfDynamicComponent<TfTemplateProcessorHelpComponentContext>
{
	public Guid Id { get; set; } = new Guid("34ec50ca-eeb8-41fe-b876-25946f079a88");
	[Parameter] public TfComponentMode DisplayMode { get; set; } = TfComponentMode.Read;
	[Parameter] public TfTemplateProcessorHelpComponentContext Context { get; set; }

}