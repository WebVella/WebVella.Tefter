namespace WebVella.Tefter.UI.Components;

public partial class TucTileCardContent : TfBaseComponent
{
	[Parameter] public string? Title { get; set; }
	[Parameter] public string? Subtitle { get; set; }
	[Parameter] public string? Description { get; set; }
}