namespace WebVella.Tefter.UI.Components;

public partial class TucLayoutPageContent : TfBaseComponent
{
	[Parameter] public string? Title { get; set; } = null;
	[Parameter] public RenderFragment? Tabs { get; set; }
	[Parameter] public RenderFragment? Toolbar { get; set; }
	[Parameter] public RenderFragment? ChildContent { get; set; }

}