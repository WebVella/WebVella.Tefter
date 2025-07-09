namespace WebVella.Tefter.UI.Components;
public partial class TucGridToolbar : TfBaseComponent
{
	[Parameter] public RenderFragment ToolbarSearch { get; set; } = default!;
	[Parameter] public RenderFragment ToolbarLeft { get; set; } = default!;
	[Parameter] public RenderFragment ToolbarRight { get; set; } = default!;
	[Parameter] public string? Style { get; set; } = null;
}