namespace WebVella.Tefter.TemplateProcessors.TextContent.Components;

[LocalizationResource("WebVella.Tefter.TemplateProcessors.TextContent.Components.Help.HelpComponent", "WebVella.Tefter.TemplateProcessors.TextContent")]
public partial class HelpComponent : TfBaseComponent, ITfDynamicComponent<TfTemplateProcessorHelpComponentContext>
{
	public Guid Id { get; set; } = new Guid("cf4e06c3-f96f-4806-9406-e50548244bd0");
	[Parameter] public TfComponentMode DisplayMode { get; set; } = TfComponentMode.Read;
	[Parameter] public TfTemplateProcessorHelpComponentContext Context { get; set; }


}