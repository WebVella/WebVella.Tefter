namespace WebVella.Tefter.UI.Components;

public partial class TucTileCardHeader : TfBaseComponent
{
	[Parameter] public string Title { get; set; } = "Title";
	[Parameter] public string Subtitle { get; set; } = "subTitle";
}