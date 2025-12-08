namespace WebVella.Tefter.UI.Components;

public partial class TucTileCardActions : TfBaseComponent
{
	[Parameter] public List<TfMenuItem> Actions { get; set; } = new();
}