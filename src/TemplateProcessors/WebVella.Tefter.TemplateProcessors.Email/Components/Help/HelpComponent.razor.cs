using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.TemplateProcessors.Email.Components;

[LocalizationResource("WebVella.Tefter.TemplateProcessors.Email.Components.Help.HelpComponent", "WebVella.Tefter.TemplateProcessors.Email")]
public partial class HelpComponent : TfBaseComponent, ITfDynamicComponent<TfTemplateProcessorHelpComponentContext>
{
	public Guid Id { get; set; } = new Guid("1164d432-c025-40c2-98f4-703470e49948");
	[Parameter] public TfComponentMode DisplayMode { get; set; } = TfComponentMode.Read;
	[Parameter] public TfTemplateProcessorHelpComponentContext Context { get; set; }


}