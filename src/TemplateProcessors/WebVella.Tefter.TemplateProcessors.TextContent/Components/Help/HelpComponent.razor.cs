namespace WebVella.Tefter.TemplateProcessors.TextContent.Components;

[LocalizationResource("WebVella.Tefter.TemplateProcessors.TextContent.Components.Help.HelpComponent", "WebVella.Tefter.TemplateProcessors.TextContent")]
public partial class HelpComponent : TfBaseComponent, ITfDynamicComponent<TfTemplateProcessorHelpComponentContext>
{
	//For this component only ReadOnly and Form will be supported
	[Parameter] public TfComponentMode DisplayMode { get; set; } = TfComponentMode.Read;
	[Parameter] public TfTemplateProcessorHelpComponentContext Context { get; set; }


}