using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.TemplateProcessors.TextFile.Components;

[LocalizationResource("WebVella.Tefter.TemplateProcessors.TextFile.Components.Help.HelpComponent", "WebVella.Tefter.TemplateProcessors.TextFile")]
public partial class HelpComponent : TfBaseComponent, ITfDynamicComponent<TfTemplateProcessorHelpComponentContext>
{
	//For this component only ReadOnly and Form will be supported
	[Parameter] public TfComponentMode DisplayMode { get; set; } = TfComponentMode.Read;
	[Parameter] public TfTemplateProcessorHelpComponentContext Context { get; set; }

}